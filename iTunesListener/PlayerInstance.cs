using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IITTrack Track
        {
            get
            {
                return PlayerEngine.CurrentTrack;
            }
        }
        public Music Music { get; set; }
        public PlayerInstance()
        {
            PlayerEngine = new iTunesApp();
            Music = new Music();
        }
    }
}
