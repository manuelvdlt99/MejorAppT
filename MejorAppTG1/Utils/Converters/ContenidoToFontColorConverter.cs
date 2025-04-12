using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    public class ContenidoToFontColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                return (Color)Application.Current.Resources["FontColor1"];
            }

            return (Color)Application.Current.Resources["FontColor2"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
