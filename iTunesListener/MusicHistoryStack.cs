using MoreLinq;
using System;
using System.Collections.Generic;

namespace iTunesListener
{
    public class MusicHistoryStack
    {
        private List<Music> _history = new List<Music>();
        private int _sizeLimit => Properties.Settings.Default.HistoryStackLimit * 10;
        public int Count => _history.Count;
        public List<Music> Get()
        {
            return _history;
        }
        public Music Get(int index)
        {
            if (index < 0 || (Count - 1) < index)
                throw new IndexOutOfRangeException();
            return _history[index];
        }
        public bool Put(Music track)
        {
            if (_sizeLimit == 0)
                return false;
            try
            {
                if (_history.Count < _sizeLimit)
                    _history.Add(track);
                else
                {
                    var oldestTrackIndex = _history.IndexOf(_history.MinBy(t => t.Started));
                    _history[oldestTrackIndex] = track;
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Clear()
        {
            _history.Clear();
        }


    }
}
