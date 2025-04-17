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
    public LanguagePopup()
    {
        InitializeComponent();

        _availableLanguages = new List<LanguageItem>
        {
            new LanguageItem("Español", "es", "🇪🇸"),
            new LanguageItem("English", "en", "🇬🇧"),
            new LanguageItem("Français", "fr", "🇫🇷")
        };

        ClvLanguages.ItemsSource = _availableLanguages;
    }
    #endregion

    #region Eventos
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
