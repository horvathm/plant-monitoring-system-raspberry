using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onlab_Final3.Converters
{
    /// <summary>
    /// Base class that define the ConvertValueToCategory function.
    /// After a measurement process I defined 5 category.  
    /// The category 5 very dry and 1 is very vet and 2 is still good. I tried to follow some kind of characteristic that the sensor give during the measurement.
    /// </summary>
    abstract class HumidityConverterBase
    {
        protected readonly static int LO_TRESHOLD = 0;
        protected readonly static int HI_TRESHOLD = 32768;

        public static int ConvertValueToCategory(int value)
        {
            if (value > 27500)
                return 5;
            if (value > 17500)
                return 4;
            if (value > 12000)
                return 3;
            if (value > 9000)
                return 2;
            else
                return 1;
        }
    }
}
