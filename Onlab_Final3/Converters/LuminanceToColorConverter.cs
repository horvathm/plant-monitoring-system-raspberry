using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Onlab_Final3.Converters
{
    /// <summary>
    /// Converter that converts that define the color of the 'brightness' progress bar from the value. 
    /// Returns a SolidColorBrush.
    /// </summary>
    class LuminanceToColorConverter : LuminanceConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte category = (byte)ConvertValueToCategory((int)value);
            return new SolidColorBrush(Color.FromArgb(255, category, category, category));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
