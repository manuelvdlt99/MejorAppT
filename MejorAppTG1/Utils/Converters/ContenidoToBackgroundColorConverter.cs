using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    public class ContenidoToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el contenido es nulo o vacío, hacer el fondo transparente.
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                return Colors.Transparent;
            }

            // Si tiene contenido, establecer el color de fondo predeterminado
            return (Color)Application.Current.Resources["SecondaryColor1"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
