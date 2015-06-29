using System;
using System.Globalization;
using System.Windows.Data;

namespace Calendar.Converters
{
    public class DateTimeToMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime val = (DateTime)value;
            return val.ToString("MMM", CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
