using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onlab_Final3.Converters
{
    /// <summary>
    ///      Converter that convert humidity value for the progress bar.
    ///      Humidity values are decreasing.Dry ground gives bigger values than wet.
    ///      This class inverts them in order to display increasing values on the progress bar
    ///      while the values on the textblocks displays the original data.
    /// </summary>
    class HumidityValueInverter : HumidityConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value > HI_TRESHOLD)
                return 0;
            if ((int)value < LO_TRESHOLD)
                return HI_TRESHOLD;

            var temp = HI_TRESHOLD-(int)value;

            return temp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
