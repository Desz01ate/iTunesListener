using iTunesLib;

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
    }
}
