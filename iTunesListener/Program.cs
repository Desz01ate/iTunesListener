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
using System.Threading;
using System.Threading.Tasks;

namespace iTunesListener
{
    class Program
    {
        const int scale = 50;
        const string endpoint = "http://hhcssdm.somee.com/iTunesSyncer";
        const string mainHeader = "  START    |                        MUSIC   NAME                        |      ARTIST(S)     |      TRACK DURATION\n----------------------------------------------------------------------------------------------------------------------";
        private static string DetailsFormat = "%track - %artist";
        private static string StateFormat = "%playlist_type: %playlist_name";
        private static string PausedDetailsFormat = "%track - %artist";
        private static string PausedStateFormat = "Paused";
        private static FacebookClient fbClient;
        private static iTunesApp itunes;
        private static IITTrack track;
        private static Task actionThread;
        private static Task headerThread;
        private static Task webServiceListenerTask;
        private static Post previousPost = new Post();
        private static DateTime startTime;
        private static Thread chromaUpdater;
        private static Thread mainThread;
        private static ManualResetEvent _event = new ManualResetEvent(true);
        private static EventHandler.ConsoleEventDelegate handler;

        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            itunes = new iTunesApp();
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
        private static void UpdatePresence(Music currentPresenceTrack)
        {
            var presence = new DiscordRPC.RichPresence { largeImageKey = "itunes_logo_big" };
            if (currentPresenceTrack.State != ITPlayerState.ITPlayerStatePlaying)
            {
                presence.details = Extension.TruncateString(Extension.RenderString(PausedDetailsFormat, currentPresenceTrack));
                presence.state = Extension.TruncateString(Extension.RenderString(PausedStateFormat, currentPresenceTrack));
            }
            else
            {
                presence.details = Extension.TruncateString(Extension.RenderString(DetailsFormat, currentPresenceTrack));
                presence.state = Extension.TruncateString(Extension.RenderString(StateFormat, currentPresenceTrack));
                presence.startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - itunes.PlayerPosition;
                presence.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + (itunes.CurrentTrack.Duration - itunes.PlayerPosition);
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
            if (itunes == null)
                return false;
            if (itunes.PlayerState != ITPlayerState.ITPlayerStatePlaying)
                return false;
            return true;
        }
        private static void MainThread()
        {
            var previousTrack = new Music();
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
                    track = itunes.CurrentTrack;
                    previousTrack.State = itunes.PlayerState;
                    if ((track.Name != previousTrack.Name) || (track.Album != previousTrack.Album)) //the IITrack object is not the same for every call
                    {
                        startTime = DateTime.Now;
                        Console.WriteLine();
                        previousTrack.Set(track);
                        new Thread(new ThreadStart(delegate
                        { //using Thread instead of Task because we don't care about a callback, just let's this run in the background and hope it successfully its work :v
                            try
                            {
                                try //must try this because it would cause the entire application to crash if the web service is down or not reachable
                                {

                                    var dict = new Dictionary<string, string>();
                                    dict.Add("Name", previousTrack.Name);
                                    dict.Add("Album", previousTrack.Album);
                                    dict.Add("Artist", previousTrack.Artist);
                                    client.PostAsync($"{endpoint}/api/Status", new FormUrlEncodedContent(dict));
                                }
                                catch { }
                                FacebookHelper.DeletePreviousPost(ref fbClient, post => { if (post.message.Contains("Apple Music")) fbClient.Delete(post.id); });
                                var url = HTMLHelper.GetMusicURL(track.Name, track.Album, track.Artist);
                                dynamic param = new ExpandoObject();
                                param.message = previousTrack.GetPost();
                                param.link = url;
                                fbClient.Post("me/feed", param);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.ToString());
                            }

                        })).Start();
                    }
                    UpdatePresence(previousTrack);
                    Console.Write(previousTrack.GetConsole());
                }
                catch
                {
                    itunes = new iTunesApp();
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
                        itunes.PreviousTrack();
                        break;
                    case ConsoleKey.RightArrow:
                        itunes.NextTrack();
                        break;
                    case ConsoleKey.OemPlus:
                        itunes.SoundVolume += 10;
                        break;
                    case ConsoleKey.OemMinus:
                        itunes.SoundVolume -= 10;
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        itunes.PlayPause();
                        break;
                    case ConsoleKey.Escape:
                        itunes.Stop();
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
            var playList = itunes.LibraryPlaylist.Tracks;
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
                    var state = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying ? "▶️ Playing" : "⏸ Pause";
                    var track = itunes.CurrentTrack;
                    Console.Title = itunes.PlayerPosition.ToMinutes() + " " + Extension.GetProgression(scale, itunes.PlayerPosition, track.Duration) + " " + Extension.ToMinutes(track.Duration - itunes.PlayerPosition) + " [" + state + "]  " + string.Format("Listening to {0} by {1}", track.Name.UnknownLength_Substring(30), track.Artist.UnknownLength_Substring(30));
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
                        itunes.Play();
                        GetCommand = true;
                    }
                    else if (result.Contains("pause"))
                    {
                        itunes.Pause();
                        GetCommand = true;
                    }
                    else if (result.Contains("stop"))
                    {
                        itunes.Stop();
                        GetCommand = true;
                    }
                    else if (result.Contains("next"))
                    {
                        itunes.NextTrack();
                        GetCommand = true;
                    }
                    else if (result.Contains("previous"))
                    {
                        itunes.PreviousTrack();
                        GetCommand = true;
                    }
                    else if (result.Contains("up"))
                    {
                        itunes.SoundVolume += 10;
                        GetCommand = true;
                    }
                    else if (result.Contains("down"))
                    {
                        itunes.SoundVolume -= 10;
                        GetCommand = true;
                    }
                    if (GetCommand)
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("stat", "");
                        var content = new FormUrlEncodedContent(dict);
                        var r = await client.PostAsync($"{endpoint}/StatUpdate", content);
                        //var r = await client.PostAsync("http://localhost:54267/StatUpdate", content);
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
            var Smoke = new Colore.Data.Color(0x111111);
            var Lemon = new Colore.Data.Color(166, 158, 128);
            var FuckingOrange = new Colore.Data.Color(255, 40, 0);
            var numpadKeys = new Key[] { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 }.ToList();
            var dPadKeys = new Key[] { Key.D1, Key.D2, Key.D3, Key.D3, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 }.ToList();
            var functionKeys = new Key[] { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11 }.ToList();
            var mouseStrips = Enum.GetValues(typeof(Led)).Cast<Led>().ToList();
            var keyboardGrid = KeyboardCustom.Create();
            var mouseGrid = MouseCustom.Create();
            var chroma = await ColoreProvider.CreateNativeAsync();
            while (true)
            {
                try
                {
                    var currentTime = TimeSpan.FromSeconds(itunes.PlayerPosition);
                    var secString = currentTime.Seconds.ToString();
                    var percentage = (int)(((double)itunes.PlayerPosition / track.Duration) * 10);
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
                        keyboardGrid[key] = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying ? Colore.Data.Color.White : Smoke;
                    });
                    dPadKeys.ForEach(key =>
                    {
                        keyboardGrid[key] = Lemon;
                    });
                    numpadKeys.ForEach(key => {
                        keyboardGrid[key] = Smoke;
                    });
                    for (var i = 0; i < (itunes.SoundVolume / 10); i++)
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
                    keyboardGrid[functionKeys[percentage]] = Colore.Data.Color.Red;
                    keyboardGrid[Key.Escape] = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying ? Colore.Data.Color.Green : Colore.Data.Color.Orange;
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
