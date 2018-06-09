using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{
    class MathHelper
    {
        public static Int64[] FrequencyDistribution(List<Int64> dataset, Int64[] lowerRange, Int64[] upperRange)
        {
            if (lowerRange.Length != upperRange.Length)
                throw new Exception();
            var summation = new int[lowerRange.Length];
            var result = 0;
            foreach (var data in dataset)
            {
                for (var i = 0; i < lowerRange.Length; i++)
                {
                    if (lowerRange[i] < data && data < upperRange[i])
                    {
                        summation[i]++;
                        continue;
                    }
                }
            }
            for(var i = 1; i < summation.Length; i++)
            {
                if (summation[i] > summation[i - 1])
                    result = i;
            }
            return new Int64[] { lowerRange[result],upperRange[result]};
        }
    }
}
