using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.Model;

namespace BuildNotifications.Converters
{
    public class BuildResultToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BuildResult? result = value == null ? null : (BuildResult?)Enum.Parse(typeof (BuildResult), value.ToString());
            switch (result)
            {
                case BuildResult.Succeeded:
                    return "Icons/tickResource.ico";
                case BuildResult.PartiallySucceeded:
                    return "Icons/warningResource.ico";
                case BuildResult.Failed:
                    return "Icons/crossResource.ico";
            }
            return "Icons/quesionResource.ico";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
