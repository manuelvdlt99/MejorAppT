using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using System.Globalization;

namespace MejorAppTG1.Views;

public partial class LanguagePopup : Popup
{
    #region Variables
    private readonly List<LanguageItem> _availableLanguages;
    #endregion

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="LanguagePopup"/>.
    /// </summary>
    public LanguagePopup()
    {
        InitializeComponent();

        _availableLanguages =
        [
            new LanguageItem("Español", "es", "🇪🇸"),
            new LanguageItem("English", "en", "🇬🇧"),
            new LanguageItem("Français", "fr", "🇫🇷")
        ];

        ClvLanguages.ItemsSource = _availableLanguages;
    }
    #endregion

    #region Eventos    
    /// <summary>
    /// Maneja el evento de pulsación sobre un idioma. Si es distinto al actual, almacena el idioma seleccionado en las preferencias y refresca la app desde el menú principal.
    /// </summary>
    /// <param name="sender">El idioma pulsado.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is LanguageItem selectedLanguage) {
            App.AnimateFrameInOut(frame);
            if (selectedLanguage != null && Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != selectedLanguage.Code) {
                string langCode = selectedLanguage.Code;
                Preferences.Set(App.USER_LANGUAGE, langCode);

                var culture = new CultureInfo(langCode);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Application.Current.MainPage = new AppShell(); // Refresca toda la app
                Close();
            } else {
                Close();
            }
        }
    }
    #endregion
}
