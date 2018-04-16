using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{
    static class Extension
    {
        public static string ToMinutes(this long elapsedMilliseconds)
        {
            var ts = TimeSpan.FromSeconds(elapsedMilliseconds);
            return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
        }
        public static string ToMinutes(this int elapsedMilliseconds)
        {
            var ts = TimeSpan.FromSeconds(elapsedMilliseconds);
            return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
        }
        public static object GetProgression(int scale, long position, long duration)
        {
            var percentage = (((double)position / duration) * (scale * 10)) / 10;
            var repeating = Math.Round(percentage, 0);
            return $"[{new string('_', (int)repeating > 0 ? (int)repeating - 1 : 0)}|{new string('_', scale - (int)repeating)}]";
        }
        public static string UnknownLength_Substring(this string s, int length)
        {
            if (s.Length <= length)
                return s;
            return s.Substring(0, length);
        }
    }
}
