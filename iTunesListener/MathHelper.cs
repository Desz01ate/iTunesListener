using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{
    class MathHelper
    {
        public static int[] FrequencyDistribution(List<Int64> dataset, Int64[] lowerRange, Int64[] upperRange)
        {
            if (lowerRange.Length != upperRange.Length)
                throw new IndexOutOfRangeException("both lower bound and upper bound must had the same dimension and length.");
            var summation = new int[lowerRange.Length];
            var result = 0;
            foreach (var data in dataset)
            {
                for (var i = 0; i < lowerRange.Length; i++)
                {
                    if (lowerRange[i] < data && data < upperRange[i])
                    {
                        summation[i]++;
                        break;
                    }
                }
            }
            return summation;
        }
    }
}
