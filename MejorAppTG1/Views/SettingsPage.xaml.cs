using CommunityToolkit.Maui.Views;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Views;
using System.Globalization;

namespace MejorAppTG1;

public partial class SettingsPage : ContentPage
{
    #region Variables
    private readonly Dictionary<string, string> _availableLanguages = new()
    {
        { "Español", "es" },
        { "English", "en" },
        { "Français", "fr" }
    };
    #endregion

    #region Constructores
    public SettingsPage()
    {
        InitializeComponent();
        BtnLanguage.Text = Thread.CurrentThread.CurrentUICulture.DisplayName;
    }
    #endregion

    #region Eventos
    private async void BtnEditProfile_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            var popup = new SignUpPopup(App.CurrentUser);
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

            if (result is ValueTuple<string, int, string, string> inputs) {
                App.CurrentUser.Nombre = inputs.Item1;
                App.CurrentUser.Edad = inputs.Item2;
                App.CurrentUser.Genero = inputs.Item3;
                App.CurrentUser.Imagen = inputs.Item4;
                await App.Database.UpdateUsuarioAsync(App.CurrentUser);
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private void BtnLogOut_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            Preferences.Remove(App.USER_ID_KEY);
            MyProfilePage.ResultIndex = 0;
            MyProfilePage.CurrentPage = 1;
            App.CurrentUser = null;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnDeleteProfile_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            bool respuesta = await DisplayAlert(Strings.str_Shell_DeleteProfile_Question, Strings.str_Shell_DeleteProfile_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);
            if (respuesta) {
                await App.Database.DeleteTestsByUserAsync(App.CurrentUser.IdUsuario);
                await App.Database.DeleteUsuarioAsync(App.CurrentUser);
                App.CurrentUser = null;
                Preferences.Remove(App.USER_ID_KEY);
                MyProfilePage.ResultIndex = 0;
                MyProfilePage.CurrentPage = 1;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnLanguage_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            await Shell.Current.ShowPopupAsync(new LanguagePopup());
            BtnLanguage.Text = Thread.CurrentThread.CurrentUICulture.DisplayName;
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnDeleteTests_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            bool answer = await DisplayAlert(Strings.str_ResultHistoryPage_BtnClearHistory_Question, Strings.str_ResultHistoryPage_BtnClearHistory_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

            if (answer) {
                await App.Database.DeleteTestsByUserAsync(App.CurrentUser.IdUsuario);
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }
    #endregion
}