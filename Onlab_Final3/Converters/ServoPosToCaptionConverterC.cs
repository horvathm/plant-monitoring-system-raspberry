using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onlab_Final3.Converters
{
    class ServoPosToCaptionConverterC : IValueConverter
    {
        /*
         * Converter that converts the caption of the button
         * that changes the raspberry driven relay state in order to match the action
         */
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return "Water C";
            }
            else
            {
                return "Water D";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
