using System;
using System.Windows;
using System.Windows.Data;

namespace Calendar.Converters
{
    public class DateTimeToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            if (dt.Day == 1)
            {
                return new Thickness() { Left = 1, Top = 1 };
            }
            else if (dt.Day <= 7)
            {
                return new Thickness() { Top = 1 };
            }
            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
