using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Maui.FreakyControls;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;

namespace MejorAppTG1.Views;

public partial class SignUpPopup : Popup
{
    #region Variables
    private bool _buttonPressed = false;
    private string _selectedGender;
    private string? _localPfpPath;
    #endregion

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SignUpPopup"/>.
    /// </summary>
    public SignUpPopup()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SignUpPopup"/> con un usuario cargado para su modificación.
    /// </summary>
    /// <param name="user">El usuario a modificar.</param>
    public SignUpPopup(User user)
    {
        InitializeComponent();
        LblTitle.Text = Strings.str_Shell_TbiModifyProfile;
        LoadSelectedUserData(user);
    }
    #endregion

    #region Eventos
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
    /// Maneja el evento de pulsación del botón de Confirmar. Valida las entradas de datos y, si son correctas, devuelve el resultado al formulario que llamó al popup.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnConfirm_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            bool entriesGoodToGo = true;
            int entryAgeNum = 0;
            if (String.IsNullOrWhiteSpace(EntryName.Text)) {
                entriesGoodToGo = false;
                Toast.Make(Strings.str_LoginPage_BtnLogin_entryNameError, ToastDuration.Long).Show(new CancellationTokenSource().Token);
                UpdateErrorTextInput(EntryName);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorName);
            } else {
                UpdateValidTextInput(EntryName);
            }

            try {
                entryAgeNum = Convert.ToInt32(EntryAge.Text);
                UpdateValidTextInput(EntryAge);
            }
            catch (FormatException exc) {
                entriesGoodToGo = false;
                Toast.Make(Strings.str_LoginPage_BtnLogin_entryAgeError, ToastDuration.Long).Show(new CancellationTokenSource().Token);
                UpdateErrorTextInput(EntryAge);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorAge1);
            }

            if (String.IsNullOrWhiteSpace(EntryAge.Text)) {
                entriesGoodToGo = false;
                Toast.Make(Strings.str_LoginPage_BtnLogin_entryAgeError, ToastDuration.Long).Show(new CancellationTokenSource().Token);
                UpdateErrorTextInput(EntryAge);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorAge1);
            }
            else if (entryAgeNum < 12 || entryAgeNum > 99) {
                entriesGoodToGo = false;
                Toast.Make(Strings.str_LoginPage_BtnLogin_entryAgeError2, ToastDuration.Long).Show(new CancellationTokenSource().Token);
                UpdateErrorTextInput(EntryAge);
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorAge2);
            } else {
                UpdateValidTextInput(EntryAge);
            }

            if (_selectedGender == null) {
                entriesGoodToGo = false;
                Toast.Make(Strings.str_LoginPage_BtnLogin_cmbGenderError, ToastDuration.Long).Show(new CancellationTokenSource().Token);
                LblGender.TextColor = (Color)Application.Current.Resources["ErrorColor1"];
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderError);
            } else {
                LblGender.TextColor = (Color)Application.Current.Resources["ButtonColor2"];
            }

            if (entriesGoodToGo) {
                Close((EntryName.Text, entryAgeNum, _selectedGender, _localPfpPath));
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_AllGood);
            }
        }
        finally {
            _buttonPressed = false;
        }
    }

    /// <summary>
    /// Marca el género Hombre.
    /// </summary>
    /// <param name="sender">El Frame pulsado.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private void TapGestureRecognizer_Tapped_Male(object sender, TappedEventArgs e)
    {
        App.AnimateFrameInOut(FrmMale);
        _selectedGender = App.GENDERS_MALE_KEY;
        FrmMale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
        FrmMale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmFemale.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmFemale.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        FrmNB.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmNB.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderMaleSelected);
    }

    /// <summary>
    /// Marca el género Mujer.
    /// </summary>
    /// <param name="sender">El Frame pulsado.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private void TapGestureRecognizer_Tapped_Female(object sender, TappedEventArgs e)
    {
        App.AnimateFrameInOut(FrmFemale);
        _selectedGender = App.GENDERS_FEMALE_KEY;
        FrmFemale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
        FrmFemale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmMale.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmMale.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        FrmNB.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmNB.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderFemaleSelected);
    }

    /// <summary>
    /// Marca el género No binario.
    /// </summary>
    /// <param name="sender">El Frame pulsado.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private void TapGestureRecognizer_Tapped_NB(object sender, TappedEventArgs e)
    {
        App.AnimateFrameInOut(FrmNB);
        _selectedGender = App.GENDERS_NB_KEY;
        FrmNB.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
        FrmNB.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmMale.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmMale.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        FrmFemale.BorderColor = (Color)Application.Current.Resources["ButtonColor2"];
        FrmFemale.BackgroundColor = (Color)Application.Current.Resources["GradientColor3"];
        SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderNBSelected);
    }

    /// <summary>
    /// Permite cambiar la foto de perfil del usuario. La aplicación pide permisos y, si el usuario los acepta, puede elegir una imagen personalizada, que se mostrará en la interfaz.
    /// </summary>
    /// <param name="sender">La imagen pulsada.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private async void TapGestureRecognizer_Tapped_ImgProfile(object sender, TappedEventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            App.AnimateFrameInOut(FrmImgProfile);
            await ImgEditProfile.ScaleTo(0.95, 100, Easing.CubicInOut);
            await ImgEditProfile.ScaleTo(1, 100, Easing.CubicInOut);
            var status = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted) {
                if (MediaPicker.Default.IsCaptureSupported) {
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null) {
                        _localPfpPath = GetUniqueFilePath(FileSystem.CacheDirectory, photo.FileName);
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(_localPfpPath);
                        await sourceStream.CopyToAsync(localFileStream);
                        UpdateUserImage();
                    }
                }
            }
            else {
                var toast = Toast.Make(Strings.str_ResultHistoryPage_ImgProfile_NoPermission, ToastDuration.Short, 14);
                await toast.Show(new CancellationTokenSource().Token);
            }
        }
        catch (Exception ex) {
            Console.WriteLine(string.Format(Strings.str_ResultHistoryPage_ImgProfile_ErrorMsg, ex.Message));
            await Toast.Make(Strings.str_ResultHistoryPage_ImgProfile_ErrorMsg2, ToastDuration.Long).Show(new CancellationTokenSource().Token);
        }
        finally {
            _buttonPressed = false;
        }
    }
    #endregion

    #region Métodos
    /// <summary>
    /// Método para evitar que se sobrescriban las imágenes si se llaman igual (al igual que Windows añade (1), (2)...)
    /// </summary>
    /// <param name="directory">El directorio donde se almacenará la imagen.</param>
    /// <param name="fileName">El nombre original del archivo de la imagen.</param>
    /// <returns>La ruta completa de la imagen con un nombre único en el sistema de archivos.</returns>
    private string GetUniqueFilePath(string directory, string fileName)
    {
        string filePath = Path.Combine(directory, fileName);
        string fileExtension = Path.GetExtension(fileName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        int counter = 1;

        // Incrementa el sufijo hasta encontrar un nombre que no exista
        while (File.Exists(filePath)) {
            string newFileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
            filePath = Path.Combine(directory, newFileName);
            counter++;
        }

        return filePath;
    }

    /// <summary>
    /// Actualiza la foto de perfil.
    /// </summary>
    private void UpdateUserImage()
    {
        if (_localPfpPath != null) {
            MainThread.BeginInvokeOnMainThread(() => {
                ImgProfile.Source = ImageSource.FromFile(_localPfpPath);
            });
        }
    }

    /// <summary>
    /// Actualiza un componente de entrada de texto dado si sus datos son incorrectos.
    /// </summary>
    /// <param name="entry">El componente de entrada de texto.</param>
    private void UpdateErrorTextInput(FreakyTextInputLayout entry)
    {
        entry.BorderStroke = (Color)Application.Current.Resources["ErrorColor1"];
        entry.TitleColor = (Color)Application.Current.Resources["ErrorColor1"];
    }

    /// <summary>
    /// Actualiza un componente de entrada de texto dado si sus datos son válidos.
    /// </summary>
    /// <param name="entry">El componente de entrada de texto.</param>
    private void UpdateValidTextInput(FreakyTextInputLayout entry)
    {
        entry.BorderStroke = (Color)Application.Current.Resources["ButtonColor2"];
        entry.TitleColor = (Color)Application.Current.Resources["ButtonColor2"];
    }

    /// <summary>
    /// Muestra en la interfaz los datos del usuario actual (si se está modificando).
    /// </summary>
    /// <param name="user">El usuario.</param>
    private void LoadSelectedUserData(User user)
    {
        EntryName.Text = user.Nombre;
        EntryAge.Text = user.Edad.ToString();
        _selectedGender = user.Genero;
        _localPfpPath = user.Imagen;
        if (_localPfpPath != null) {
            ImgProfile.Source = ImageSource.FromFile(_localPfpPath);
        }
        switch (_selectedGender) {
            case App.GENDERS_MALE_KEY:
                FrmMale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmMale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
            case App.GENDERS_FEMALE_KEY:
                FrmFemale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmFemale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
            case App.GENDERS_NB_KEY:
                FrmNB.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmNB.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
        }
    }
    #endregion
}