using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onlab_Final3.Converters
{
    /*
     * Base class for luminance converters.
     * It's readonly fields and ConvertValueToCategory function inherited witch maps the value into the [0,255] range.
     */
    abstract class LuminanceConverterBase
    {
        protected readonly static int LO_TRESHOLD = 0;
        protected readonly static int HI_TRESHOLD = 32768;

        public static int ConvertValueToCategory(int value)
        {
            if ((int)value < LO_TRESHOLD)
                return LO_TRESHOLD;
            if ((int)value > HI_TRESHOLD)
                return HI_TRESHOLD;
            return (int)(value / ((HI_TRESHOLD - LO_TRESHOLD) / 255.0));
        }
    }
}
