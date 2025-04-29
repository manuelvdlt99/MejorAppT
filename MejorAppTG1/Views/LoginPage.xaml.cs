using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils.Converters;
using MejorAppTG1.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using System.Globalization;

namespace MejorAppTG1;

public partial class LoginPage : ContentPage {

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="LoginPage"/>.
    /// </summary>
    public LoginPage()
    {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);

        if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
            this.SemanticOrderView.ViewOrder = new List<View> { LblDos, ImageUno, LblTitulo, LblUno };
        }
    }
    #endregion

    #region Eventos
    /// <summary>
    /// Maneja el evento de pulsación del botón de Crear usuario. Abre un popup modal con el formulario de creación de usuario.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void CrearUsuario_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            var popup = new SignUpPopup();
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

            createUser(result);
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Seleccionar usuario. Abre un popup modal con todos los usuarios almacenados.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnLogIn_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            var activeUsers = await App.Database.GetUsuariosAsync();

            if (activeUsers.Count == 0) {
                await DisplayAlert(Strings.str_LoginPage_BtnLogIn_NoUsers, Strings.str_LoginPage_BtnLogIn_WarningMsg, Strings.str_ResultHistoryPage_BtnCheck_OK);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_BtnLogIn_NoUsers);
            }
            else {
                var popup = new UserSelectPopup(activeUsers);
                var selectedUser = await Application.Current.MainPage.ShowPopupAsync(popup) as User;

                selectUser(selectedUser);
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }
    #endregion

    #region Métodos    
    /// <summary>
    /// Almacena los datos del usuario en la base de datos, lo guarda como el usuario actual y abre el menú principal de la aplicación.
    /// </summary>
    /// <param name="result">El resultado del formulario de creación de usuario.</param>
    private async void createUser(object result)
    {
        if (result is ValueTuple<string, int, string, string> inputs) {
            string newName = inputs.Item1;
            int newAge = inputs.Item2;
            string newGender = inputs.Item3;
            string pfpFilePath = inputs.Item4;

            User newUser = new() {
                Nombre = newName,
                Edad = newAge,
                Genero = newGender,
                Imagen = pfpFilePath
            };

            int localId = await App.Database.AddUsuarioAsync(newUser);
            Preferences.Set(App.USER_ID_KEY, localId);
            newUser.IdUsuario = localId;
            App.CurrentUser = newUser;

            Application.Current.MainPage = new AppShell();

            SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_UserCreated, newName));
        }
    }

    /// <summary>
    /// Guarda el usuario seleccionado como el actual y abre el menú principal de la aplicación.
    /// </summary>
    /// <param name="selectedUser">El usuario seleccionado.</param>
    private async void selectUser(User selectedUser)
    {
        if (selectedUser != null) {
            App.CurrentUser = selectedUser;

            Preferences.Set(App.USER_ID_KEY, selectedUser.IdUsuario);

            Application.Current.MainPage = new AppShell();

            SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_BtnLogIn_Success, selectedUser.Nombre));
        }
    }
    #endregion
}