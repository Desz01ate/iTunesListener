﻿using iTunesLib;
using Newtonsoft.Json;
using System;

namespace iTunesListener
{
    public class Music
    {
        [JsonIgnore]
        private readonly string _appFormat = "\r[{0}] |{1,-60}|{2,-20}| {3} Minutes   ";
        [JsonIgnore]
        private IITTrack Track { get; set; }
        [JsonIgnore]
        public ITPlayerState State { get; set; }
        [JsonIgnore]
        public DateTime Started { get; set; }
        [JsonIgnore]
        public string PlaylistType { get; private set; }
        public string Name { get; private set; }
        public string Album { get; private set; }
        public string Artist { get; private set; }
        public string PostFormat => Extension.RenderString(Properties.Settings.Default.FacebookFormat, this);
        public override string ToString()
        {
            //return string.Format(appFormat, DateTime.Now.ToString("HH:mm:ss"), (Name + " - " + Album).UnknownLength_Substring(60), Artist.UnknownLength_Substring(20), Time);
            return string.Format(_appFormat, Started.ToString("HH:mm:ss"), (Track.Name + " - " + Track.Album).UnknownLength_Substring(60), Track.Artist.UnknownLength_Substring(20), Track.Time);
        }
        public void Set(IITTrack track)
        {
            Started = DateTime.Now;
            PlaylistType = "Album";
            Name = track.Name;
            Album = track.Album;
            Artist = track.Artist;
            Track = track;
        }
    }
}
