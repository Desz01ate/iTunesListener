using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{
    class Music
    {
        [JsonIgnore]
        private string _postFormat = "Listening to {0} - {1} by {2} on Apple Music!";
        [JsonIgnore]
        private string _appFormat = "\r[{0}] |{1,-60}|{2,-20}| {3} Minutes   ";
        public string Name { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        [JsonIgnore]
        private iTunesLib.IITTrack Track { get; set; }
        public string GetPost()
        {
            //return String.Format(_postFormat, Name, Album, Artist);
            return String.Format(_postFormat, Track.Name, Track.Album, Track.Artist);

        }
        public string GetConsole()
        {
            //return string.Format(appFormat, DateTime.Now.ToString("HH:mm:ss"), (Name + " - " + Album).UnknownLength_Substring(60), Artist.UnknownLength_Substring(20), Time);

            return string.Format(_appFormat, DateTime.Now.ToString("HH:mm:ss"), (Track.Name + " - " + Track.Album).UnknownLength_Substring(60), Track.Artist.UnknownLength_Substring(20), Track.Time);
        }
        public void Set(iTunesLib.IITTrack track)
        {
            Name = track.Name;
            Album = track.Album;
            Artist = track.Artist;
            Track = track;
        }
    }
}
