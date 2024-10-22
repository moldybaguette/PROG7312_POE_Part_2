using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Municipality_Services_App.Converters
{
    public class BoolToRowSpanConverter: IValueConverter
    {
// this is for the filter component itself
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded && isExpanded)
            {
                return 2; // Span both rows when expanded
            }
            return 1; // Span only the first row when contracted
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
