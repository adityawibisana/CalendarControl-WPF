using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calendar.Converters.CustomCalendarConverter
{
    public class ShowMonthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime val0 = (DateTime)values[0];
            if (values[1] == null)
                return Visibility.Visible;

            DateTime val1 = (DateTime)values[1];

            if (val0.Day == 1 || val0.Date == val1.Date)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
