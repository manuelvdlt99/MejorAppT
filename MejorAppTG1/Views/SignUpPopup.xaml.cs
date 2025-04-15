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
    #endregion
    public SignUpPopup()
    {
        InitializeComponent();
    }

    public SignUpPopup(User user)
    {
        InitializeComponent();
        LblTitle.Text = Strings.str_Shell_TbiModifyProfile;
        LoadSelectedUserData(user);
    }

    #region Eventos
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
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
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

    private void UpdateUserImage()
    {
        if (_localPfpPath != null) {
            MainThread.BeginInvokeOnMainThread(() => {
                ImgProfile.Source = ImageSource.FromFile(_localPfpPath);
            });
        }
    }

    private void UpdateErrorTextInput(FreakyTextInputLayout entry)
    {
        entry.BorderStroke = (Color)Application.Current.Resources["ErrorColor1"];
        entry.TitleColor = (Color)Application.Current.Resources["ErrorColor1"];
    }

    private void UpdateValidTextInput(FreakyTextInputLayout entry)
    {
        entry.BorderStroke = (Color)Application.Current.Resources["ButtonColor2"];
        entry.TitleColor = (Color)Application.Current.Resources["ButtonColor2"];
    }

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
            case "str_Genders_Man":
                FrmMale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmMale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
            case "str_Genders_Woman":
                FrmFemale.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmFemale.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
            case "str_Genders_NB":
                FrmNB.BorderColor = (Color)Application.Current.Resources["HeaderColor1"];
                FrmNB.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
                break;
        }
    }
    #endregion
}