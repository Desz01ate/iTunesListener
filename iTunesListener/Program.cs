using Facebook;
using iTunesLib;
using System;
using System.Dynamic;
using System.Threading;

namespace iTunesListener
{
    class Program
    {
        static FacebookClient fbClient;
        static iTunesApp itunes;
        static IITTrack track;
        static Thread actionThread;
        static Thread headerThread;
        static Post previousPost = new Post();
        static DateTime startTime;
        static string previousTrackName = string.Empty;
        const int scale = 50;
        private static Thread mainThread;
        const string mainHeader = "  START    |                        MUSIC   NAME                        |      Artist(s)     |      Remaining Time\n----------------------------------------------------------------------------------------------------------------------";
        public static void Main(string[] args)
        {
            var width = (100 + scale + 10) < 130 ? 100 + scale + 10 : 130;
            Console.SetWindowSize(width, 25);
            itunes = new iTunesApp();
            try
            { 
                FacebookHelper.RenewAccessToken();
            }
            catch (Exception e)
            {
                //MessageBox.Show("OAuth Access Token has been expired.");
                string input = Microsoft.VisualBasic.Interaction.InputBox("OAuth access token has been expired", "Please enter your renew access token here", "Access Token", -1, -1);
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.AccessToken = input;
                Properties.Settings.Default.Save();
                //Console.Write(e.Message);
                //Console.ReadLine();
            }
            finally
            {
                actionThread = new Thread(new ThreadStart(ActionListenerThread));
                actionThread.Start();
                headerThread = new Thread(new ThreadStart(HeaderThread));
                headerThread.Start();
                GenerateFacebookThread();
            }
        }
        private static void GenerateFacebookThread()
        {
            if (mainThread != null)
                if (mainThread.ThreadState == System.Threading.ThreadState.Running)
                    mainThread.Abort();
            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }
        private static bool ValidateiTunesInstanceState()
        {
            if (itunes == null)
                return false;
            if (itunes.PlayerState != ITPlayerState.ITPlayerStatePlaying)
                return false;
            return true;
        }
        private static bool TrackVerify(IITTrack previousTrack, IITTrack track)
        {
            if (previousTrack == null)
                return true;
            if (previousTrack.Name != track.Name)
                return true;
            return false;
        }
        private static void MainThread()
        {
            string postFormat = "Listening to {0} - {1} by {2} on Apple Music!";
            string appFormat = "\r[{0}] |{1,-60}|{2,-20}| {3} Minutes   ";
            Console.Clear();
            Console.WriteLine(mainHeader);
            while (ValidateiTunesInstanceState() == false)
            {
                Thread.Sleep(1000);
            }
            while (true)
            {
                try
                {
                    track = itunes.CurrentTrack;
                    if (track.Name != previousTrackName)
                    {
                        startTime = DateTime.Now;
                        Console.WriteLine();
                        previousTrackName = track.Name;
                        new Thread(new ThreadStart(delegate {
                            fbClient = new FacebookClient(Properties.Settings.Default.AccessToken);
                            FacebookHelper.DeletePreviousPost(ref fbClient,post => { if (post.message.Contains("Apple Music")) fbClient.Delete(post.id); });
                            var name = track.Name;
                            var artist = track.Artist;
                            var album = track.Album;
                            var url = HTMLHelper.GetMusicURL(name, album, artist);
                            //HTMLHelper.HTMLParser($"{name} {artist}", "<div.+?data-context-item-id=[\"'](.+?)[\"'].+?>");//HTMLHelper.HTMLAgilityPackParser($"{name} {artist}", "//div[contains(@class,'yt-lockup yt-lockup-tile yt-lockup-video vve-check clearfix')]", "data-context-item-id");
                            dynamic param = new ExpandoObject();
                            param.message = String.Format(postFormat, track.Name, track.Album, track.Artist);
                            param.link = url;
                            try
                            {
                                fbClient.Post("me/feed", param);
                            }
                            catch (FacebookOAuthException)
                            {

                            }
                            
                        })).Start();
                        //GenerateFacebookThread();
                    }
                    //Console.Write(string.Format(appFormat, startTime.ToString("HH:mm:ss"), UnknownLength_Substring(track.Name + " - " + track.Album, 60), UnknownLength_Substring(track.Artist, 20), ToMinutes(track.Duration - itunes.PlayerPosition)));
                    Console.Write(string.Format(appFormat, startTime.ToString("HH:mm:ss"), (track.Name + " - " + track.Album).UnknownLength_Substring(60), track.Artist.UnknownLength_Substring(20), track.Time));
                }
                catch (Exception e)
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
                    var track = itunes.CurrentTrack;
                    Console.Title = "[" + itunes.PlayerState + "]  " + string.Format("Listening to {0} by {1}", track.Name.UnknownLength_Substring(30), track.Artist.UnknownLength_Substring(20)) + ", " + itunes.PlayerPosition.ToMinutes() + " " + Extension.GetProgression(scale, itunes.PlayerPosition, track.Duration) + " " + Extension.ToMinutes(track.Duration - itunes.PlayerPosition);
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
                    case ConsoleKey.UpArrow:
                        mainThread.Suspend();
                        ShowPlaylist();
                        break;
                    case ConsoleKey.DownArrow:
                        if (mainThread.ThreadState == System.Threading.ThreadState.Suspended)
                        {
                            Console.Clear();
                            Console.WriteLine(mainHeader);
                            mainThread.Resume();
                        }
                        else if (mainThread.ThreadState == System.Threading.ThreadState.Aborted)
                            mainThread.Start();
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
}
