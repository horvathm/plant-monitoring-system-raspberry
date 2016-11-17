using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onlab_Final3.Converters
{
    class InvertBooleanConverter : IValueConverter
    {
        /*
         * Converter that create the values for the radio buttons where only 2 option possible and only one of their 
         * bindings has this converter class added es static resource.
         */
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return false;
            else
                return true;
        }
    }
}
