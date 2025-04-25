using CommunityToolkit.Maui.Views;
using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    /// <summary>
    /// Converter para XAML para pasar un string a una fuente de vídeo/audio para un componente MediaElement.  
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.IValueConverter" />
    public class StringToMediaSourceConverter : IValueConverter
    {
        /// <summary>
        /// Convierte una ruta dada en una fuente multimedia si se encuentra en el sistema de archivos y la devuelve. Si no se encuentra, devuelve null.
        /// </summary>
        /// <param name="value">El string con la ruta del vídeo/audio.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture">La cultura (localización) del sistema.</param>
        /// <returns>La fuente multimedia almacenada en la ruta dada o null en su defecto.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string uriString && !string.IsNullOrEmpty(uriString)) {
                return MediaSource.FromResource(uriString);
            }
            return null;
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
