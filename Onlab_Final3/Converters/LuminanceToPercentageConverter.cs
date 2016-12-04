using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onlab_Final3.Converters
{
    /// <summary>
    ///    Unused!
    /// Converter that converts value to a category number.
    /// Maps the value into the[0, 255] range.
    /// </summary>
    class LuminanceToPercentageConverter : LuminanceConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertValueToCategory((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
