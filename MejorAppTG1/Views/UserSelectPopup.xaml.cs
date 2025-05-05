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
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UserSelectPopup"/>.
    /// </summary>
    /// <param name="usuarios">La lista de usuarios almacenados.</param>
    public UserSelectPopup(List<User> usuarios)
	{
		InitializeComponent();
		ClvUsuarios.ItemsSource = usuarios;
    }
    #endregion

    #region Eventos
    /// <summary>
    /// Maneja el cambio de contexto de cada texto de edad. Muestra cada edad cargada.
    /// </summary>
    /// <param name="sender">El texto detectado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el cambio de contexto de cada texto de género. Muestra cada género cargado.
    /// </summary>
    /// <param name="sender">El texto detectado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void LblGender_BindingContextChanged(object sender, EventArgs e)
    {
        var generoLabel = sender as Label;
        var user = (User)generoLabel.BindingContext;
        if (user != null) {
            string translatedGender = user.Genero switch {
                App.GENDERS_MALE_KEY => Strings.str_Genders_Man,
                App.GENDERS_FEMALE_KEY => Strings.str_Genders_Woman,
                App.GENDERS_NB_KEY => Strings.str_Genders_NB,
                _ => throw new NotImplementedException()
            };

            generoLabel.Text = translatedGender;
        }
    }

    /// <summary>
    /// Maneja el cambio de contexto de cada tarjeta de usuario. Permite adaptar el lector de pantalla a cada usuario concreto.
    /// </summary>
    /// <param name="sender">La tarjeta detectada.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private static void Frame_BindingContextChanged(object sender, EventArgs e)
    {
        var currentFrame = sender as Frame;
        if (currentFrame.BindingContext is User user) {
            string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
            string translatedAge = string.Format(translatedAgeFormat, user.Edad);
            string semanticDescription = string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser, user.Nombre, translatedAge, Strings.ResourceManager.GetString(user.Genero, CultureInfo.CurrentUICulture));
            SemanticProperties.SetDescription(currentFrame, semanticDescription);
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Cancelar. Cierra el popup.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnCancel_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            Close(null);
        }
        finally {
            _buttonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Confirmar. Cierra el popup y devuelve el usuario selecionado al formulario que llamó al popup.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnConfirm_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
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

    /// <summary>
    /// Maneja el evento de pulsación sobre una tarjeta. Selecciona el usuario asociado y lo marca en la interfaz.
    /// </summary>
    /// <param name="sender">El usuario pulsado.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
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
    /// <summary>
    /// Restablece un Frame a sus colores iniciales.
    /// </summary>
    /// <param name="frame">El Frame a modificar.</param>
    private static void ResetFrameColor(Frame frame)
    {
        frame.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        frame.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
    }

    /// <summary>
    /// Actualiza los colores del Frame del usuario seleccionado.
    /// </summary>
    /// <param name="selectedFrame">El Frame seleccionado.</param>
    private static void UpdateSelectedFrameBorder(Frame selectedFrame)
    {
        selectedFrame.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
        selectedFrame.BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];
    }
    #endregion
}