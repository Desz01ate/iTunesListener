using iTunesLib;
using System;

namespace iTunesListener
{
    public enum State
    {
        Playing = ITPlayerState.ITPlayerStatePlaying,
        Pause = ITPlayerState.ITPlayerStateStopped,
        Stop = ITPlayerState.ITPlayerStateStopped
    }
    class PlayerInstance
    {
        public iTunesApp PlayerEngine { get; private set; }
        public IITTrack Track => PlayerEngine.CurrentTrack;
        public Music Music { get; set; }
        public PlayerInstance()
        {
            PlayerEngine = new iTunesApp();
            Music = new Music();
        }
        public double CalculatedPosition => Math.Round(((double)PlayerEngine.PlayerPosition / Track.Duration) * 10, 2);
    }
}
