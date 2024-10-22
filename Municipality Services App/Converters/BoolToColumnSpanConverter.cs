using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Municipality_Services_App.Converters
{
    public class BoolToColumnSpanConverter: IValueConverter
    {
        public BoolToColumnSpanConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isExpanded = (bool)value;
            return isExpanded ? 1 : 2; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
