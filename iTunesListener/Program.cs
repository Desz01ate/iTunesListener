﻿using Colore;
using Colore.Data;
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
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;
using System.Windows.Forms;
using Colore.Effects.Headset;
using Colore.Effects.Mousepad;
using System.Configuration;

namespace iTunesListener
{
    class Program
    {
        private const int scale = 50;
        private const string endpoint = "http://hhcssdm.somee.com/iTunesSyncer";
        private const string mainHeader = "  START    |                        TRACK   TITLE                       |      ARTIST(S)     |      TRACK DURATION\n----------------------------------------------------------------------------------------------------------------------";
        private static string currentTrackUrl = string.Empty;
        private static ColoreColor Smoke => new ColoreColor(0x111111);
        private static ColoreColor Lemon => new ColoreColor(166, 158, 128);
        private static ColoreColor FuckingOrange => new ColoreColor(255, 40, 0);
        private static List<Key> NumpadKeys => new List<Key>() { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 };
        private static List<Key> DPadKeys => new List<Key>() { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };
        private static List<Key> FunctionKeys => new List<Key>() { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12 };
        private static List<Key> AllKeys = Enum.GetValues(typeof(Key)).Cast<Key>().ToList();
        private static List<GridLed> AllMouseLED => Enum.GetValues(typeof(GridLed)).Cast<GridLed>().ToList();
        private static List<GridLed> LeftStrip => new List<GridLed>() { GridLed.LeftSide1, GridLed.LeftSide2, GridLed.LeftSide3, GridLed.LeftSide4, GridLed.LeftSide5, GridLed.LeftSide6, GridLed.LeftSide7 };
        private static List<GridLed> RightStrip => new List<GridLed>() { GridLed.RightSide1, GridLed.RightSide2, GridLed.RightSide3, GridLed.RightSide4, GridLed.RightSide5, GridLed.RightSide6, GridLed.RightSide7 };
        private static MusicHistoryStack playedList = new MusicHistoryStack();
        private static FacebookClient fbClient;
        private static Thread mainThread;
        private static ManualResetEvent _event = new ManualResetEvent(true);
        private static EventHandler.ConsoleEventDelegate handler;
        private static PlayerInstance player;
        private static HttpClient client = new HttpClient();

        [STAThread]
        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            player = new PlayerInstance();
            handler = new EventHandler.ConsoleEventDelegate(ConsoleEventCallback);
            InitializeDiscod();
            try
            {
                FacebookHelper.RenewAccessToken();
            }
            catch
            {
                MessageBox.Show("Can't evaluate Facebook OAuth Token, please check your token in settings (S) or your network connection.", "iTunesListener", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            EventHandler.SetConsoleCtrlHandler(handler, true);
            Task.Run((Action)ActionListenerThread);
            Task.Run((Action)HeaderThread);
            Task.Run((Action)WebServiceListener);
            if (Properties.Settings.Default.ChromaSDKEnable)
                Task.Run((Action)ChromaUpdateAsync);
            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

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
            try
            {
                var presence = new DiscordRPC.RichPresence { largeImageKey = "itunes_logo_big" };
                if (player.Music.State != (ITPlayerState)State.Playing)
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseDetail, player.Music));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseState, player.Music));
                }
                else
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayDetail, player.Music));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayState, player.Music));
                    presence.startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - player.PlayerEngine.PlayerPosition;
                    presence.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + (player.PlayerEngine.CurrentTrack.Duration - player.PlayerEngine.PlayerPosition);
                }
                DiscordRPC.UpdatePresence(presence);
            }
            catch
            {

            }
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2 || eventType == 0) //2 is user perform exit, 0 is application interupt (^C)
            {
                _event.Reset();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("\n\n\nResources cleaning...");
                FacebookHelper.DeletePreviousPost(ref fbClient, post => { if (post.Message.Contains("Apple Music")) fbClient.Delete(post.Id); });
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
            Console.Clear();
            Console.WriteLine(mainHeader);
            while (!ValidateiTunesInstanceState())
            {
                Thread.Sleep(1000);
            }
            fbClient = new FacebookClient(Properties.Settings.Default.AccessToken);
            DateTime startedTime;
            while (true)
            {
                _event.WaitOne();
                try
                {
                    player.Music.State = player.PlayerEngine.PlayerState;
                    if ((player.Track.Name != player.Music.Name) || (player.Track.Album != player.Music.Album)) //the IITrack object is not the same for every call
                    {
                        startedTime = DateTime.Now;
                        Console.WriteLine();
                        player.Music.Set(player.Track);
                        currentTrackUrl = HTMLHelper.GetMusicURL(player.Track.Name, player.Track.Album, player.Track.Artist);
                        var addTrack = new Music();
                        addTrack.Set(player.Track);
                        playedList.Put(addTrack);
                        if (Properties.Settings.Default.AutoShare)
                        {
                            new Thread(NetworkWorker).Start();
                        }
                    }
                    if(Properties.Settings.Default.WebServiceListening)
                    {
                        try //must try this because it would cause the entire application to crash if the web service is down or not reachable
                        {
                            var data = new StringContent(JsonConvert.SerializeObject(player.Music), Encoding.UTF8, "application/json");
                            client.PostAsync($"{endpoint}/api/Status", data);
                        }
                        catch { }
                    }
                    if (Properties.Settings.Default.DiscordRichPresenceEnable)
                        UpdatePresence();
                    Console.Write(player.Music.ToString());
                }
                catch
                {
                    continue;
                }
                Thread.Sleep(500);
            }
        }

        private static void NetworkWorker()
        {
            FacebookHelper.DeletePreviousPost(ref fbClient, post =>
            {
                var indicator = Properties.Settings.Default.FacebookFormat.IndexOf(" ");
                if (post.Message.Contains(Properties.Settings.Default.FacebookFormat.Substring(0, indicator)))
                    fbClient.Delete(post.Id);
            });
            dynamic param = new ExpandoObject();
            param.message = player.Music.PostFormat;
            param.link = currentTrackUrl;
            fbClient.Post("me/feed", param);
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
                    case ConsoleKey.UpArrow:
                        _event.Reset();
                        ShowPlaylist();
                        break;
                    case ConsoleKey.DownArrow:
                        if (!_event.WaitOne(0))
                        {
                            Console.Clear();
                            Console.WriteLine(mainHeader);
                            for (var i = 0; i < playedList.Count - 1; i++) //-2 to compensate index 0 and index n, as n track would always show on thread running
                            {
                                Console.WriteLine();
                                Console.Write(playedList.Get(i).ToString());
                            }
                            Console.WriteLine();
                            _event.Set();
                        }
                        break;
                    case ConsoleKey.H:
                        _event.Reset();
                        ShowHelp();
                        break;
                    case ConsoleKey.R:
                        Process.Start("https://github.com/Desz01ate/iTunesListener");
                        break;
                    case ConsoleKey.O:
                        Process.Start(currentTrackUrl);
                        break;
                    case ConsoleKey.S:
                        Application.EnableVisualStyles();
                        Application.Run(new Settings());
                        break;
                    case ConsoleKey.Escape:
                        ConsoleEventCallback(2);
                        break;
                }
            }
        }
        private static void ShowPlaylist()
        {
            Console.Clear();
            var playList = player.PlayerEngine.CurrentPlaylist.Tracks;
            Console.WriteLine("COUNT |                        MUSIC   NAME                        |      ARTIST(S)     |      ALBUM");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------");
            string appFormat = "\r{0,-5} |{1,-55}|{2,-20}|{3}   ";
            for (var i = 1; i < playList.Count - 1; i++)
            {
                Console.WriteLine(string.Format(appFormat, playList[i].PlayedCount, playList[i].Name.UnknownLength_Substring(60), playList[i].Artist.UnknownLength_Substring(20), playList[i].Album.UnknownLength_Substring(40)));
            }
        }
        private static void ShowHelp()
        {
            Console.Clear();
            var fs = new FontSharp.Font();
            fs.SimpleWriter(new List<StringBuilder[]>() { fs.I, fs.T, fs.U, fs.N, fs.E, fs.S, fs.Space, fs.L, fs.I, fs.S, fs.T, fs.E, fs.N, fs.E, fs.R });
            Console.WriteLine("\nProject repository : https://github.com/Desz01ate/iTunesListener (press R to open!)");
            Console.WriteLine("\nAvailable commands : ");
            Console.WriteLine("\tUp Arrow : Show all track in your default playlist");
            Console.WriteLine("\n\tDown Arrow : Show current playing track and played history");
            Console.WriteLine("\n\tLeft/Right Arrow : Change track to previous/next respectively");
            Console.WriteLine("\n\tSpacebar/Enter : Resume/Pause music");
            Console.WriteLine("\n\t-/= : Decrease/Increase sound volume");
            Console.WriteLine("\n\tO : Open current track url");
            Console.WriteLine("\n\tS : Open settings");
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
            while (true)
            {
                try
                {
                    var result = (await (await client.GetAsync($"{endpoint}/api/Status?stat")).Content.ReadAsStringAsync()).ToLower();
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
                        var data = new StringContent(JsonConvert.SerializeObject(new { Stat = "" }), Encoding.UTF8, "application/json");
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
            AllKeys.Remove(Key.Invalid);
            var opacity = 0.5;
            var keyboardGrid = KeyboardCustom.Create();
            var mouseGrid = MouseCustom.Create();
            var chroma = await ColoreProvider.CreateNativeAsync();
            var bg_playing = Properties.Settings.Default.Background_Playing;
            var bg_pause = Properties.Settings.Default.Background_Pause;
            var pos_fore = new ColoreColor(Properties.Settings.Default.Position_Foreground.R, Properties.Settings.Default.Position_Foreground.G, Properties.Settings.Default.Position_Foreground.B);
            var pos_back = new ColoreColor(Properties.Settings.Default.Position_Background.R, Properties.Settings.Default.Position_Background.G, Properties.Settings.Default.Position_Background.B);
            var vol = new ColoreColor(Properties.Settings.Default.Volume.R, Properties.Settings.Default.Volume.G, Properties.Settings.Default.Volume.B);
            ColoreColor backgroundColor = default(ColoreColor);
            while (true)
            {
                if (Properties.Settings.Default.DynamicColorEnable)
                {
                    bg_playing = Properties.Settings.Default.Background_Playing;
                    bg_pause = Properties.Settings.Default.Background_Pause;
                    pos_fore = new ColoreColor(Properties.Settings.Default.Position_Foreground.R, Properties.Settings.Default.Position_Foreground.G, Properties.Settings.Default.Position_Foreground.B);
                    pos_back = new ColoreColor(Properties.Settings.Default.Position_Background.R, Properties.Settings.Default.Position_Background.G, Properties.Settings.Default.Position_Background.B);
                    vol = new ColoreColor(Properties.Settings.Default.Volume.R, Properties.Settings.Default.Volume.G, Properties.Settings.Default.Volume.B);
                }
                try
                {
                    var currentTime = TimeSpan.FromSeconds(player.PlayerEngine.PlayerPosition);
                    var position = Math.Round(((double)player.PlayerEngine.PlayerPosition / player.Track.Duration) * 10, 2);
                    var backgroundDetermine = player.PlayerEngine.PlayerState == ITPlayerState.ITPlayerStatePlaying ? bg_playing : bg_pause;
                    if (Properties.Settings.Default.BackgroundFadeEnable)
                    {
                        opacity = player.PlayerEngine.SoundVolume * 0.01;
                        backgroundColor = new ColoreColor((byte)((backgroundDetermine.R * opacity / 10)), (byte)((backgroundDetermine.G * opacity / 10)), (byte)((backgroundDetermine.B * opacity / 10)));
                    }
                    else
                    {
                        backgroundColor = new ColoreColor((byte)((backgroundDetermine.R / 10)), (byte)((backgroundDetermine.G / 10)), (byte)((backgroundDetermine.B / 10)));
                    }
                    keyboardGrid.Set(backgroundColor);
                    mouseGrid.Set(backgroundColor);
                    keyboardGrid[Key.Up] = ColoreColor.Pink;
                    keyboardGrid[Key.Down] = ColoreColor.Pink;
                    keyboardGrid[Key.Left] = ColoreColor.Pink;
                    keyboardGrid[Key.Right] = ColoreColor.Pink;
                    keyboardGrid[Key.OemEquals] = ColoreColor.Purple;
                    keyboardGrid[Key.OemMinus] = ColoreColor.HotPink;
                    keyboardGrid[Key.R] = ColoreColor.Green;
                    keyboardGrid[Key.O] = ColoreColor.Green;
                    keyboardGrid[Key.H] = ColoreColor.Blue;
                    keyboardGrid[Key.S] = ColoreColor.Blue;
                    SetPlayingTime(ref keyboardGrid, currentTime, ColoreColor.Red, FuckingOrange, ColoreColor.Yellow);
                    SetVolumeScale(ref mouseGrid, RightStrip, vol);
                    SetVolumeScale(ref keyboardGrid, DPadKeys, vol);
                    SetPlayingPosition(ref keyboardGrid, position, FunctionKeys, pos_fore, pos_back);
                    SetPlayingPosition(ref mouseGrid, position, LeftStrip, pos_fore, pos_back);
                }
                catch
                {
                    continue; //in case the music is not playing yet, the position is unobtainable.
                }
                finally
                {
                    await chroma.Keyboard.SetCustomAsync(keyboardGrid);
                    await chroma.Mouse.SetGridAsync(mouseGrid);
                    await chroma.Headset.SetAllAsync(backgroundColor);
                    await chroma.Mousepad.SetAllAsync(backgroundColor);
                    Thread.Sleep(500);
                }
            }
        }
        private static void SetPlayingTime(ref KeyboardCustom keyboardGrid, TimeSpan currentTime, params ColoreColor[] colors)
        {
            if (colors.Length > 3)
                return;
            int decimalDigit = currentTime.Seconds / 10;
            keyboardGrid[NumpadKeys[currentTime.Minutes]] = colors[0];
            keyboardGrid[NumpadKeys[decimalDigit]] = colors[1];
            keyboardGrid[NumpadKeys[currentTime.Seconds % 10]] = colors[2];
        }
        private static void SetVolumeScale(ref MouseCustom mouseGrid, List<GridLed> Keys, ColoreColor color)
        {
            for (var i = 0; i < (player.PlayerEngine.SoundVolume * Keys.Count) / 100; i++) //volume bar (D0-D9)
            {
                mouseGrid[RightStrip[i]] = color;
            }
        }
        private static void SetVolumeScale(ref KeyboardCustom keyboardGrid, List<Key> Keys, ColoreColor color)
        {
            for (var i = 0; i < (player.PlayerEngine.SoundVolume * Keys.Count) / 100; i++) //volume bar (D0-D9)
            {
                keyboardGrid[DPadKeys[i]] = color;
            }
        }
        private static void SetPlayingPosition(ref MouseCustom mouseGrid, double position, List<GridLed> leftStrip, ColoreColor pos, ColoreColor background)
        {
            var currentPlayPosition = (int)Math.Round(position * 0.6, 0); //can replace 1.1 with ((double)(leftStrip.Count - 1) / 10) for safe calculation
            for (var i = 0; i < currentPlayPosition + 1; i++)
            {
                mouseGrid[LeftStrip[i]] = background;
            }
            mouseGrid[LeftStrip[currentPlayPosition]] = pos;
        }
        private static void SetPlayingPosition(ref KeyboardCustom keyboardGrid, double position, List<Key> functionKeys, ColoreColor pos, ColoreColor background)
        {
            var currentPlayPosition = (int)Math.Round(position * 1.1, 0); //can replace 1.1 with ((double)(functionKeys.Count - 1) / 10) for safe calculation
            for (var i = 0; i < currentPlayPosition + 1; i++)
            {
                keyboardGrid[FunctionKeys[i]] = background;
            }
            keyboardGrid[FunctionKeys[currentPlayPosition]] = pos;
        }
        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="keyboardGrid"></param>
        /// <param name="Keys"></param>
        /// <param name="bgColor"></param>
        private static void SetGridBackground(ref KeyboardCustom keyboardGrid, List<Key> Keys, ColoreColor bgColor)
        {
            foreach (var key in Keys)
            {
                keyboardGrid[key] = bgColor;
            }
        }
        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="mouseGrid"></param>
        /// <param name="Keys"></param>
        /// <param name="bgColor"></param>
        private static void SetGridBackground(ref MouseCustom mouseGrid, List<GridLed> Keys, ColoreColor bgColor)
        {
            foreach (var key in Keys)
            {
                mouseGrid[key] = bgColor;
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
