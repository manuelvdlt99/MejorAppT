using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    /// <summary>
    /// Converter para XAML para pasar un string a una imagen.  
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.IValueConverter" />
    public class NullToDefaultImageConverter : IValueConverter
    {
        /// <summary>
        /// Convierte una ruta dada en una imagen si se encuentra en el sistema de archivos y la devuelve. Si no se encuentra, devuelve una imagen por defecto.
        /// </summary>
        /// <param name="value">El string con la ruta de la imagen.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture">La cultura (localización) del sistema.</param>
        /// <returns>La imagen almacenada en la ruta dada o una imagen predeterminada en su defecto.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? imagePath = value as string;
            return string.IsNullOrEmpty(imagePath) ? "no_profile_icon.png" : imagePath;
        }

        /// <summary>
        /// NO IMPLEMENTADO.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
