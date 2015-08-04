using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Calendar.Converters.CustomCalendarConverter
{
    public class DateTimeToForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime val0 = (DateTime)values[0];

            if (values[1] == null)
                return new SolidColorBrush(Colors.Gray);

            DateTime val1 = (DateTime)values[1];

            if (val0.Month == val1.Month)
                return new SolidColorBrush(Colors.White);
            return new SolidColorBrush(Colors.Gray); 

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
