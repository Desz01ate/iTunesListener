﻿using Facebook;
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
        const string endpoint = "http://hhcssdm.somee.com/iTunesSyncer";
        private static Thread mainThread;
        static ManualResetEvent _event = new ManualResetEvent(true);
        static EventHandler.ConsoleEventDelegate handler;
        const string mainHeader = "  START    |                        MUSIC   NAME                        |      ARTIST(S)     |      TRACK DURATION\n----------------------------------------------------------------------------------------------------------------------";

        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
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
                    }else if (result.Contains("up"))
                    {
                        itunes.SoundVolume += 10;
                        GetCommand = true;
                    }else if (result.Contains("down"))
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
                HttpClient client = new HttpClient();
                try
                {
                    track = itunes.CurrentTrack;
                    if ((track.Name != previousTrack.Name) || (track.Album != previousTrack.Album)) //the IITrack object is not the same time every call
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
