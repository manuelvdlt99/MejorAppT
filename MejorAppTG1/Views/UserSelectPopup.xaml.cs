using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Globalization;
using CommunityToolkit.Maui.Core;
using Application = Microsoft.Maui.Controls.Application;
using Color = Microsoft.Maui.Graphics.Color;

namespace MejorAppTG1.Views;

public partial class UserSelectPopup : Popup
{
    #region Variables
    private User? _selectedUser;
    private Frame? _previousSelectedFrame;
    private bool _buttonPressed = false;
    #endregion

    #region Constructores
    public UserSelectPopup(List<User> usuarios)
	{
		InitializeComponent();
		ClvUsuarios.ItemsSource = usuarios;
    }
    #endregion

    #region Eventos
    private void LblAge_BindingContextChanged(object sender, EventArgs e)
    {
        var edadLabel = sender as Label;
        var user = (User)edadLabel.BindingContext;
        if (user != null) {
            int edad = user.Edad;
            string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
            string translatedAge = string.Format(translatedAgeFormat, edad);
            edadLabel.Text = translatedAge;
        }
    }

    private void LblGender_BindingContextChanged(object sender, EventArgs e)
    {
        var generoLabel = sender as Label;
        var user = (User)generoLabel.BindingContext;
        if (user != null) {
            string translatedGender = user.Genero switch {
                "str_Genders_Man" => Strings.str_Genders_Man,
                "str_Genders_Woman" => Strings.str_Genders_Woman,
                "str_Genders_NB" => Strings.str_Genders_NB
            };

            generoLabel.Text = translatedGender;
        }
    }

    private void Frame_BindingContextChanged(object sender, EventArgs e)
    {
        var currentFrame = sender as Frame;
        if (currentFrame.BindingContext is User user) {
            string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
            string translatedAge = string.Format(translatedAgeFormat, user.Edad);
            string semanticDescription = string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser, user.Nombre, translatedAge, Strings.ResourceManager.GetString(user.Genero, CultureInfo.CurrentUICulture));
            SemanticProperties.SetDescription(currentFrame, semanticDescription);
        }
    }

    private void BtnCancel_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            Close(null);
        }
        finally {
            _buttonPressed = false;
        }
    }

    private void BtnConfirm_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            if (_selectedUser == null) {
                Toast.Make(Strings.str_UserSelectPopup_ChooseUserToast, ToastDuration.Short).Show();
            }
            else {
                Close(_selectedUser);
            }
        }
        finally {
            _buttonPressed = false;
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame?.BindingContext is User usuarioSeleccionado) {
            if (_previousSelectedFrame != null) {
                ResetFrameColor(_previousSelectedFrame);
            }
            App.AnimateFrameInOut(frame);
            _selectedUser = usuarioSeleccionado;
            UpdateSelectedFrameBorder(frame);
            _previousSelectedFrame = frame;
            SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_BtnLogIn_Success);
        }
    }
    #endregion

    #region Métodos
    private void ResetFrameColor(Frame frame)
    {
        frame.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        frame.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor3"];
    }

    private void UpdateSelectedFrameBorder(Frame selectedFrame)
    {
        selectedFrame.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
        selectedFrame.BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];
    }
    #endregion
}