using System;
using System.Globalization;
using System.Windows.Data;

namespace BuildNotifications.Converters
{
    public class DateTimeFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime) value;
            return dateTime == default(DateTime) ?
                null :
                dateTime.ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
