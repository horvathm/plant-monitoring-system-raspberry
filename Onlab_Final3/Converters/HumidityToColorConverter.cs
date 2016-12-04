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
    /// Converter that converts value into one of the 5 category then give back
    /// a SolidColorBrush with a color that match the category
    /// </summary>
    class HumidityToColorConverter : HumidityConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (ConvertValueToCategory((int)value))
            {
                case 5:
                    return new SolidColorBrush(Colors.Red);
                case 4:
                    return new SolidColorBrush(Colors.Orange);
                case 3:
                    return new SolidColorBrush(Colors.Yellow);
                case 2:
                    return new SolidColorBrush(Colors.GreenYellow);
                case 1:
                default:
                    return new SolidColorBrush(Colors.LawnGreen);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
