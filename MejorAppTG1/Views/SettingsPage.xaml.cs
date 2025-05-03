using CommunityToolkit.Maui.Views;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Views;

namespace MejorAppTG1;

public partial class SettingsPage : ContentPage
{
    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SettingsPage"/>.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        BtnLanguage.Text = Thread.CurrentThread.CurrentUICulture.DisplayName;
    }
    #endregion

    #region Eventos
    /// <summary>
    /// Maneja el evento de pulsación del botón de Modificar perfil. Abre un popup modal con el formulario de edición de un usuario.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnEditProfile_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            var popup = new SignUpPopup(App.CurrentUser);
            var result = await Shell.Current.ShowPopupAsync(popup);

            EditUser(result);
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Cerrar sesión. Borra el usuario actual de las preferencias y vuelve a la pantalla de inicio.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el evento de pulsación del botón de Eliminar perfil. Elimina el usuario actual de la base de datos, lo borra de las preferencias y vuelve a la pantalla de inicio.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el evento de pulsación del botón de Cambiar idioma. Abre el popup modal de selección de idioma.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el evento de pulsación del botón de Limpiar historial. Elimina de la base de datos todos los tests realizados por el usuario actual.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    #region Métodos    
    /// <summary>
    /// Actualiza el usuario en la base de datos.
    /// </summary>
    /// <param name="result">El resultado del formulario de modificación de usuario.</param>
    private async void EditUser(object result)
    {
        if (result is ValueTuple<string, int, string, string> inputs) {
            App.CurrentUser.Nombre = inputs.Item1;
            App.CurrentUser.Edad = inputs.Item2;
            App.CurrentUser.Genero = inputs.Item3;
            App.CurrentUser.Imagen = inputs.Item4;
            await App.Database.UpdateUsuarioAsync(App.CurrentUser);
        }
    }
    #endregion
}