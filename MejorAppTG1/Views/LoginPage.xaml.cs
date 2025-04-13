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
    private async void CrearUsuario_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            var popup = new SignUpPopup();
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

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
        finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnLogIn_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            var activeUsers = await App.Database.GetUsuariosAsync();

            if (activeUsers.Count == 0) {
                await DisplayAlert(Strings.str_LoginPage_BtnLogIn_NoUsers, Strings.str_LoginPage_BtnLogIn_WarningMsg, Strings.str_ResultHistoryPage_BtnCheck_OK);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_BtnLogIn_NoUsers);
            }
            else {
                var popup = new UserSelectPopup(activeUsers);
                var selectedUser = await Application.Current.MainPage.ShowPopupAsync(popup) as User;

                if (selectedUser != null) {
                    App.CurrentUser = selectedUser;

                    Preferences.Set(App.USER_ID_KEY, selectedUser.IdUsuario);

                    Application.Current.MainPage = new AppShell();

                    SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_BtnLogIn_Success, selectedUser.Nombre));
                }
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }
    #endregion
}