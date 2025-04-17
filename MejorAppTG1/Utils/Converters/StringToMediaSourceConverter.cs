using CommunityToolkit.Maui.Views;
using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    public class StringToMediaSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string uriString && !string.IsNullOrEmpty(uriString)) {
                // Convertir la cadena en un MediaSource
                return MediaSource.FromResource(uriString);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
