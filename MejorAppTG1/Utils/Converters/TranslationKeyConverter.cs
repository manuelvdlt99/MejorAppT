using MejorAppTG1.Resources.Localization;
using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    /// <summary>
    /// Converter para XAML para pasar una clave de recursos a un string traducido.  
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.IValueConverter" />
    public class TranslationKeyConverter : IValueConverter
    {
        /// <summary>
        /// Convierte un string con una clave de recursos de localización en un string traducido y lo devuelve. Si no se encuentra la clave en los recursos, devuelve la propia clave. Si la clave es null, devuelve null.
        /// </summary>
        /// <param name="value">El string con la clave del string traducido.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture">La cultura (localización) del sistema.</param>
        /// <returns>Un string localizado, la clave del string si no existe en el archivo de recursos o null si la clave es null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            // Se espera que `value` sea una clave de recurso como "str_AnsiedadRapido"
            string key = value.ToString();
            string translation = Strings.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
            return translation ?? key;
        }

        /// <summary>
        /// NO IMPLEMENTADO.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
