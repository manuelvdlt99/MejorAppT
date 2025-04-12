using MejorAppTG1.Resources.Localization;
using System.Globalization;

namespace MejorAppTG1.Utils.Converters
{
    public class TranslationKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            // Se espera que `value` sea una clave de recurso como "str_AnsiedadRapido"
            string key = value.ToString();
            string translation = Strings.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
            return translation ?? key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
