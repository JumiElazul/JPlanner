using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JPlanner.Helpers
{
    internal class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                throw new ArgumentException("Exception: BooleanToVisibilityConverter parameter must be a bool");
            }

            return boolValue ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
            {
                throw new ArgumentException("Exception: BooleanToVisibilityConverter value must be a Visibility");
            }

            return visibility == Visibility.Visible;
        }
    }
}
