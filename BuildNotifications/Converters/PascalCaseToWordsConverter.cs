using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace BuildNotifications.Converters
{
    public class PascalCaseToWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value?.ToString();

            if (stringValue != null)
            {
                return string.Concat(stringValue.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}