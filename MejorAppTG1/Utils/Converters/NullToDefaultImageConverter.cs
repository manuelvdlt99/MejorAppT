using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    public class NullToDefaultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? imagePath = value as string;
            return string.IsNullOrEmpty(imagePath) ? "no_profile_icon.png" : imagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
