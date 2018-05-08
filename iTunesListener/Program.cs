using Facebook;
using iTunesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace iTunesListener
{
    class Program
    {
        static FacebookClient fbClient;
        static iTunesApp itunes;
        static IITTrack track;
        static Task actionThread;
        static Task headerThread;
        static Task webServiceListenerTask;
        static Post previousPost = new Post();
        static DateTime startTime;
        const int scale = 50;
        private static Thread mainThread;
        static ManualResetEvent _event = new ManualResetEvent(true);
        static EventHandler.ConsoleEventDelegate handler;
        const string mainHeader = "  START    |                        MUSIC   NAME                        |      ARTIST(S)     |      TRACK DURATION\n----------------------------------------------------------------------------------------------------------------------";

        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            itunes = new iTunesApp();
            handler = new EventHandler.ConsoleEventDelegate(ConsoleEventCallback);
            EventHandler.SetConsoleCtrlHandler(handler, true);
            try
            {
                FacebookHelper.RenewAccessToken();
            }
            catch
            {
                //MessageBox.Show("OAuth Access Token has been expired.");
                string input = Microsoft.VisualBasic.Interaction.InputBox("OAuth access token has been expired", "Please enter your renew access token here", "Access Token", -1, -1);
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
                mainThread = new Thread(new ThreadStart(MainThread));
                mainThread.Start();
                webServiceListenerTask = new Task(WebServiceListener);
                webServiceListenerTask.Start();
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
                    var wsCommand = await client.GetAsync("http://localhost/iTunesSyncer/api/Status?stat");
                    var result = await wsCommand.Content.ReadAsStringAsync();
                    if (result.Contains("Play"))
                    {
                        itunes.Play();
                        GetCommand = true;
                    }
                    else if (result.Contains("Pause"))
                    {
                        itunes.Pause();
                        GetCommand = true;
                    }
                    else if (result.Contains("Stop"))
                    {
                        itunes.Stop();
                        GetCommand = true;
                    }
                    else if (result.Contains("Next"))
                    {
                        itunes.NextTrack();
                        GetCommand = true;
                    }
                    else if (result.Contains("Previous"))
                    {
                        itunes.PreviousTrack();
                        GetCommand = true;
                    }
                    if (GetCommand)
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("stat", "");
                        var content = new FormUrlEncodedContent(dict);
                        var r = await client.PostAsync("http://localhost/iTunesSyncer/StatUpdate", content);
                        r.EnsureSuccessStatusCode();
                        GetCommand = false;
                    }
                    await Task.Delay(1000);
                }
                catch
                {

                }
            }
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                FacebookHelper.DeletePreviousPost(ref fbClient, post => { if (post.message.Contains("Apple Music")) fbClient.Delete(post.id); });
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
                    if ((track.Name != previousTrack.Name) || (track.Album != previousTrack.Album)) //the IITrack object is not the same time every call
                    {
                        startTime = DateTime.Now;
                        Console.WriteLine();
                        previousTrack.Set(track);
                        new Thread(new ThreadStart(delegate
                        { //using Thread instead of Task because we don't need a callback, just let's this run in the background
                            try
                            {
                                try //must try this because it would cause the entire application to crash if the web service is down or not reachable
                                {
                                    var jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject((previousTrack), Newtonsoft.Json.Formatting.Indented);
                                    using (var client = new WebClient())
                                    {
                                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                        client.UploadString(new Uri("http://localhost/iTunesSyncer/api/Status"), jsonValue);
                                    }
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
                    Console.Write(previousTrack.GetConsole());
                }
                catch
                {
                    itunes = new iTunesApp();
                }
                Thread.Sleep(500);
            }

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
    }
    class EventHandler
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        public delegate bool ConsoleEventDelegate(int eventType);

    }
}
