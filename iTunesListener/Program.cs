using Colore;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Facebook;
using iTunesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iTunesListener
{
    class Program
    {
        const int scale = 50;
        const string endpoint = "http://hhcssdm.somee.com/iTunesSyncer";
        const string mainHeader = "  START    |                        MUSIC   NAME                        |      ARTIST(S)     |      TRACK DURATION\n----------------------------------------------------------------------------------------------------------------------";
        private static readonly string DetailsFormat = "%track - %artist";
        private static readonly string StateFormat = "%playlist_type: %playlist_name";
        private static readonly string PausedDetailsFormat = "%track - %artist";
        private static readonly string PausedStateFormat = "Paused";
        private static FacebookClient fbClient;
        private static Task actionThread;
        private static Task headerThread;
        private static Task webServiceListenerTask;
        private static DateTime startTime;
        private static Thread chromaUpdater;
        private static Thread mainThread;
        private static ManualResetEvent _event = new ManualResetEvent(true);
        private static EventHandler.ConsoleEventDelegate handler;
        private static PlayerInstance player;
        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            player = new PlayerInstance();
            handler = new EventHandler.ConsoleEventDelegate(ConsoleEventCallback);
            InitializeDiscod();
            EventHandler.SetConsoleCtrlHandler(handler, true);
            try
            {
                FacebookHelper.RenewAccessToken();
            }
            catch
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Please enter your renew access token here", "OAuth access token has been expired", "Access Token", -1, -1);
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.AccessToken = input;
                Properties.Settings.Default.Save();
            }
            finally
            {

                actionThread = new Task((ActionListenerThread));
                actionThread.Start();
                headerThread = new Task((HeaderThread));
                headerThread.Start();
                webServiceListenerTask = new Task(WebServiceListener);
                webServiceListenerTask.Start();
                chromaUpdater = new Thread(new ThreadStart(ChromaUpdateAsync));
                chromaUpdater.Start();
                mainThread = new Thread(new ThreadStart(MainThread));
                mainThread.Start();
            }
        }
        private static void HandleReadyCallback() { }
        private static void HandleErrorCallback(int errorCode, string message) { }
        private static void HandleDisconnectedCallback(int errorCode, string message) { }
        private static void InitializeDiscod()
        {
            DiscordRPC.EventHandlers handlers = new DiscordRPC.EventHandlers
            {
                readyCallback = HandleReadyCallback,
                errorCallback = HandleErrorCallback,
                disconnectedCallback = HandleDisconnectedCallback
            };
            DiscordRPC.Initialize("383816327850360843", ref handlers, true, null);
        }
        private static void UpdatePresence()
        {
            var presence = new DiscordRPC.RichPresence { largeImageKey = "itunes_logo_big" };
            if (player.Music.State != (ITPlayerState)State.Playing)
            {
                presence.details = Extension.TruncateString(Extension.RenderString(PausedDetailsFormat, player.Music));
                presence.state = Extension.TruncateString(Extension.RenderString(PausedStateFormat, player.Music));
            }
            else
            {
                presence.details = Extension.TruncateString(Extension.RenderString(DetailsFormat, player.Music));
                presence.state = Extension.TruncateString(Extension.RenderString(StateFormat, player.Music));
                presence.startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - player.PlayerEngine.PlayerPosition;
                presence.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + (player.PlayerEngine.CurrentTrack.Duration - player.PlayerEngine.PlayerPosition);
            }
            DiscordRPC.UpdatePresence(presence);
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2 || eventType == 0) //2 is user perform exit, 0 is application interupt (^C)
            {
                _event.Reset();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("\n\n\nResources cleaning...");
                FacebookHelper.DeletePreviousPost(ref fbClient, post => { if (post.message.Contains("Apple Music")) fbClient.Delete(post.id); });
                Console.Clear();
                Environment.Exit(0);
            }
            return false;

        }

        private static bool ValidateiTunesInstanceState()
        {
            if (player.PlayerEngine == null)
                return false;
            if (player.PlayerEngine.PlayerState != ITPlayerState.ITPlayerStatePlaying)
                return false;
            return true;
        }
        private static void MainThread()
        {
            HttpClient client = new HttpClient();
            Console.Clear();
            Console.WriteLine(mainHeader);
            while (ValidateiTunesInstanceState() == false)
            {
                Thread.Sleep(1000);
            }
            fbClient = new FacebookClient(Properties.Settings.Default.AccessToken);
            while (true)
            {
                _event.WaitOne();
                try
                {
                    player.Music.State = player.PlayerEngine.PlayerState;
                    if ((player.Track.Name != player.Music.Name) || (player.Track.Album != player.Music.Album)) //the IITrack object is not the same for every call
                    {
                        startTime = DateTime.Now;
                        Console.WriteLine();
                        player.Music.Set(player.Track);
                        new Thread(new ThreadStart(delegate
                        { //using Thread instead of Task because we don't care about a callback, just let's this run in the background and hope it successfully its work :v
                            try
                            {
                                try //must try this because it would cause the entire application to crash if the web service is down or not reachable
                                {
                                    var data = new StringContent(JsonConvert.SerializeObject(player.Music), Encoding.UTF8, "application/json");
                                    client.PostAsync($"{endpoint}/api/Status", data);
                                }
                                catch { }
                                FacebookHelper.DeletePreviousPost(ref fbClient, post => { if (post.message.Contains("Apple Music")) fbClient.Delete(post.id); });
                                var url = HTMLHelper.GetMusicURL(player.Track.Name, player.Track.Album, player.Track.Artist);
                                dynamic param = new ExpandoObject();
                                param.message = player.Music.GetPost();
                                param.link = url;
                                fbClient.Post("me/feed", param);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.ToString());
                            }

                        })).Start();
                    }
                    UpdatePresence();
                    Console.Write(player.Music.GetConsole());
                }
                catch
                {
                    player = new PlayerInstance();
                }
                Thread.Sleep(500);
            }

        }
        private static void ActionListenerThread()
        {
            while (true)
            {
                var action = Console.ReadKey().Key;
                switch (action)
                {
                    case ConsoleKey.LeftArrow:
                        player.PlayerEngine.PreviousTrack();
                        break;
                    case ConsoleKey.RightArrow:
                        player.PlayerEngine.NextTrack();
                        break;
                    case ConsoleKey.OemPlus:
                        player.PlayerEngine.SoundVolume += 10;
                        break;
                    case ConsoleKey.OemMinus:
                        player.PlayerEngine.SoundVolume -= 10;
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        player.PlayerEngine.PlayPause();
                        break;
                    case ConsoleKey.Escape:
                        player.PlayerEngine.Stop();
                        break;
                    case ConsoleKey.UpArrow:
                        _event.Reset();
                        ShowPlaylist();
                        break;
                    case ConsoleKey.DownArrow:
                        if (!_event.WaitOne(0))
                        {
                            Console.Clear();
                            Console.WriteLine(mainHeader);
                            _event.Set();
                        }
                        break;
                    case ConsoleKey.H:
                        _event.Reset();
                        ShowHelp();
                        break;
                    case ConsoleKey.O:
                        Process.Start("https://github.com/Desz01ate/iTunesListener");
                        break;

                }
            }
        }
        private static void ShowPlaylist()
        {
            Console.Clear();
            var playList = player.PlayerEngine.LibraryPlaylist.Tracks;
            Console.WriteLine("COUNT |                        MUSIC   NAME                        |      ARTIST(S)     |      ALBUM");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------");
            string appFormat = "\r{0,-5} |{1,-60}|{2,-20}|{3}   ";
            for (var i = 1; i < playList.Count - 1; i++)
            {
                Console.WriteLine(string.Format(appFormat, playList[i].PlayedCount, playList[i].Name.UnknownLength_Substring(60), playList[i].Artist.UnknownLength_Substring(20), playList[i].Album.UnknownLength_Substring(40)));
            }
        }
        private static void ShowHelp()
        {
            Console.Clear();
            var fs = new FontSharp.Font();
            fs.SimpleWriter(new List<System.Text.StringBuilder[]>() { fs.I, fs.T, fs.U, fs.N, fs.E, fs.S, fs.Space, fs.L, fs.I, fs.S, fs.T, fs.E, fs.N, fs.E, fs.R });
            Console.WriteLine("\nProject repository : https://github.com/Desz01ate/iTunesListener (press O to open!)");
            Console.WriteLine("\nAvailable commands : ");
            Console.WriteLine("\tUp Arrow : Show all track in your default playlist");
            Console.WriteLine("\n\tDown Arrow : Show current playing track and played history");
            Console.WriteLine("\n\tLeft/Right Arrow : Change track to previous/next respectively");
            Console.WriteLine("\n\tSpacebar/Enter : Resume/Pause music");
            Console.WriteLine("\n\t-/= : Decrease/Increase sound volume");
        }
        private static void HeaderThread()
        {
            while (true)
            {
                try
                {
                    var state = player.PlayerEngine.PlayerState == ITPlayerState.ITPlayerStatePlaying ? "▶️ Playing" : "⏸ Pause";
                    var track = player.PlayerEngine.CurrentTrack;
                    Console.Title = player.PlayerEngine.PlayerPosition.ToMinutes() + " " + Extension.GetProgression(scale, player.PlayerEngine.PlayerPosition, track.Duration) + " " + Extension.ToMinutes(track.Duration - player.PlayerEngine.PlayerPosition) + " [" + state + "]  " + string.Format("Listening to {0} by {1}", track.Name.UnknownLength_Substring(30), track.Artist.UnknownLength_Substring(30));
                }
                catch
                {
                    Console.Title = "Nothing is playing at this time.";
                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
        }
        private static async void WebServiceListener()
        {
            bool GetCommand = false;
            HttpClient client = new HttpClient();
            while (true)
            {
                try
                {
                    var wsCommand = await client.GetAsync($"{endpoint}/api/Status?stat");
                    //var wsCommand = await client.GetAsync("http://localhost:54267/api/Status?stat");
                    var result = (await wsCommand.Content.ReadAsStringAsync()).ToLower();
                    if (result.Contains("play"))
                    {
                        player.PlayerEngine.Play();
                        GetCommand = true;
                    }
                    else if (result.Contains("pause"))
                    {
                        player.PlayerEngine.Pause();
                        GetCommand = true;
                    }
                    else if (result.Contains("stop"))
                    {
                        player.PlayerEngine.Stop();
                        GetCommand = true;
                    }
                    else if (result.Contains("next"))
                    {
                        player.PlayerEngine.NextTrack();
                        GetCommand = true;
                    }
                    else if (result.Contains("previous"))
                    {
                        player.PlayerEngine.PreviousTrack();
                        GetCommand = true;
                    }
                    else if (result.Contains("up"))
                    {
                        player.PlayerEngine.SoundVolume += 10;
                        GetCommand = true;
                    }
                    else if (result.Contains("down"))
                    {
                        player.PlayerEngine.SoundVolume -= 10;
                        GetCommand = true;
                    }
                    if (GetCommand)
                    {
                        var data = new StringContent(JsonConvert.SerializeObject(new { Stat="" }), Encoding.UTF8, "application/json");
                        var r = await client.PostAsync($"{endpoint}/StatUpdate", data);
                        r.EnsureSuccessStatusCode();
                        GetCommand = false;
                    }
                    await Task.Delay(500);
                }
                catch
                {

                }
            }
        }
        private static async void ChromaUpdateAsync()
        {
            var opacity = 0.5;
            var Smoke = new Colore.Data.Color(0x111111);
            var Lemon = new Colore.Data.Color(166, 158, 128);
            var FuckingOrange = new Colore.Data.Color(255, 40, 0);
            var numpadKeys = new Key[] { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 }.ToList();
            var dPadKeys = new Key[] { Key.D1, Key.D2, Key.D3, Key.D3, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 }.ToList();
            var functionKeys = new Key[] { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11,Key.F12 }.ToList();
            var allKeys = Enum.GetValues(typeof(Key)).Cast<Key>().ToList();
            var mouseStrips = Enum.GetValues(typeof(Led)).Cast<Led>().ToList();
            var keyboardGrid = KeyboardCustom.Create();
            var mouseGrid = MouseCustom.Create();
            var chroma = await ColoreProvider.CreateNativeAsync();
            while (true)
            {
                try
                {
                    var currentTime = TimeSpan.FromSeconds(player.PlayerEngine.PlayerPosition);
                    var secString = currentTime.Seconds.ToString();
                    var percentage = (int)(((double)player.PlayerEngine.PlayerPosition / player.Track.Duration) * 10);
                    opacity = player.PlayerEngine.SoundVolume * 0.01;
                    var bgColor = (byte)((255 * opacity / 10));
                    allKeys.ForEach(key =>
                    {
                        try
                        {
                            keyboardGrid[key] = new Colore.Data.Color(bgColor,bgColor,bgColor);
                        }
                        catch { }
                    });
                    keyboardGrid[Key.Up] = Colore.Data.Color.Pink;
                    keyboardGrid[Key.Down] = Colore.Data.Color.Pink;
                    keyboardGrid[Key.Left] = Colore.Data.Color.Pink;
                    keyboardGrid[Key.Right] = Colore.Data.Color.Pink;
                    keyboardGrid[Key.OemEquals] = Colore.Data.Color.Purple;
                    keyboardGrid[Key.OemMinus] = Colore.Data.Color.HotPink;
                    keyboardGrid[Key.O] = Colore.Data.Color.Green;
                    keyboardGrid[Key.H] = Colore.Data.Color.Blue;
                    functionKeys.ForEach(key =>
                    {
                        keyboardGrid[key] = player.PlayerEngine.PlayerState == (ITPlayerState)State.Playing ? Colore.Data.Color.White : Smoke;
                    });
                    dPadKeys.ForEach(key =>
                    {
                        keyboardGrid[key] = Lemon;
                    });
                    numpadKeys.ForEach(key =>
                    {
                        keyboardGrid[key] = Smoke;
                    });
                    for (var i = 0; i < (player.PlayerEngine.SoundVolume / 10); i++)
                    {
                        keyboardGrid[dPadKeys[i]] = FuckingOrange;

                    }
                    if (secString.Length == 2)
                    {
                        keyboardGrid[numpadKeys[int.Parse(secString[0].ToString())]] = FuckingOrange;
                        keyboardGrid[numpadKeys[int.Parse(secString[1].ToString())]] = Colore.Data.Color.Yellow;
                    }
                    else
                    {
                        keyboardGrid[numpadKeys[int.Parse(secString[0].ToString())]] = Colore.Data.Color.Yellow;
                    }
                    keyboardGrid[numpadKeys[currentTime.Minutes]] = Colore.Data.Color.Red;
                    keyboardGrid[functionKeys[(int)(percentage*(Convert.ToDouble(functionKeys.Count)/10))]] = Colore.Data.Color.Red;
                    keyboardGrid[Key.Escape] = player.PlayerEngine.PlayerState == ITPlayerState.ITPlayerStatePlaying ? Colore.Data.Color.Green : Colore.Data.Color.Orange;
                    await chroma.Keyboard.SetCustomAsync(keyboardGrid);
                }
                catch
                {

                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
    class EventHandler
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        public delegate bool ConsoleEventDelegate(int eventType);

    }
}
