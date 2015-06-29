using System;
using System.Windows;
using System.Windows.Data;

namespace Calendar.Converters.CustomCalendarConverter
{
    public class IsSelectedToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime val0 = (DateTime)values[0];
            DateTime val1 = (DateTime)values[1];

            if (val0.Date == val1.Date)
                return Visibility.Visible;
            return Visibility.Collapsed; 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
