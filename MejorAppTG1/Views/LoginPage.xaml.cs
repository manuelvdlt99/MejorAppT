using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils.Converters;
using MejorAppTG1.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using System.Globalization;

namespace MejorAppTG1;

internal class SignUpPopup : Popup {
    public SignUpPopup() {
        var titleLabel = new Label {
            Text = Strings.str_LoginPage_BtnSignUp,
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = (Color)Application.Current.Resources["FontColor1"]
        };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);

        var labelName = new Label { Text = Strings.str_LoginPage_BtnSignUp_Name, TextColor = (Color)Application.Current.Resources["FontColor1"] };


        var entryName = new Entry { Placeholder = Strings.str_ResultHistoryPage_BtnSignUp_EntName, PlaceholderColor = (Color)Application.Current.Resources["ButtonColor2"], TextColor = (Color)Application.Current.Resources["FontColor1"] };
        SemanticProperties.SetDescription(entryName, Strings.str_SemanticProperties_LoginPage_SignUpPopup_Name);
        SemanticProperties.SetHint(entryName, Strings.str_SemanticProperties_LoginPage_SignUpPopup_Name_Hint);
        var errorLabelName = new Label {
            Text = Strings.str_LoginPage_BtnLogin_entryNameError,
            TextColor = (Color)Application.Current.Resources["ErrorColor1"],
            FontSize = 13,
            Opacity = 0,
            IsVisible = false
        };
        SemanticProperties.SetDescription(errorLabelName, Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorName);

        var labelAge = new Label { Text = Strings.str_LoginPage_BtnSignUp_Age, TextColor = (Color)Application.Current.Resources["FontColor1"] };
        var entryAge = new Entry { Placeholder = Strings.str_ResultHistoryPage_BtnSignUp_EntAge, PlaceholderColor = (Color)Application.Current.Resources["ButtonColor2"], MaxLength = 2, Keyboard = Keyboard.Numeric, TextColor = (Color)Application.Current.Resources["FontColor1"] };
        SemanticProperties.SetDescription(entryAge, Strings.str_SemanticProperties_LoginPage_SignUpPopup_Age_Desc);
        SemanticProperties.SetHint(entryAge, Strings.str_SemanticProperties_LoginPage_SignUpPopup_Age_Hint);

        var errorLabelAge = new Label {
            Text = Strings.str_LoginPage_BtnLogin_entryAgeError,
            TextColor = (Color)Application.Current.Resources["ErrorColor1"],
            FontSize = 13,
            Opacity = 0,
            IsVisible = false
        };
        SemanticProperties.SetDescription(errorLabelAge, Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorAge1);

        var errorLabelAge2 = new Label {
            Text = Strings.str_LoginPage_BtnLogin_entryAgeError2,
            TextColor = (Color)Application.Current.Resources["ErrorColor1"],
            FontSize = 13,
            Opacity = 0,
            IsVisible = false
        };
        SemanticProperties.SetDescription(errorLabelAge2, Strings.str_SemanticProperties_LoginPage_SignUpPopup_ErrorAge2);

        var labelGender = new Label { Text = Strings.str_LoginPage_BtnSignUp_Gender, TextColor = (Color)Application.Current.Resources["FontColor1"] };
        var entryGender = new Picker {
            Title = Strings.str_ResultHistoryPage_BtnSignUp_CmbGender,
            TextColor = (Color)Application.Current.Resources["FontColor1"]
        };

        // Diccionario para almacenar las opciones y las claves asociadas
        var genderOptions = new Dictionary<string, string>
        {
            { Strings.str_Genders_Man, "str_Genders_Man" },
            { Strings.str_Genders_Woman, "str_Genders_Woman" },
            { Strings.str_Genders_NB, "str_Genders_NB" }
        };

        // Asignar las opciones al Picker
        entryGender.ItemsSource = genderOptions.Keys.ToList();
        SemanticProperties.SetDescription(entryGender, Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderPicker_Desc);
        SemanticProperties.SetHint(entryGender, Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderPicker_Hint);


        var errorLabelGender = new Label {
            Text = Strings.str_LoginPage_BtnLogin_cmbGenderError,
            TextColor = (Color)Application.Current.Resources["ErrorColor1"],
            FontSize = 13,
            Opacity = 0,
            IsVisible = false
        };
        SemanticProperties.SetDescription(errorLabelGender, Strings.str_SemanticProperties_LoginPage_SignUpPopup_GenderError);


        var acceptButton = new Button { Text = Strings.str_LoginPage_BtnConfirm, BackgroundColor = (Color)Application.Current.Resources["ButtonColor1"], BorderColor = (Color)Application.Current.Resources["BorderColor1"], BorderWidth = 2, CornerRadius = 15, TextColor = (Color)Application.Current.Resources["FontColor2"] };
        acceptButton.Clicked += (s, e) => {
            bool entriesGoodToGo = true;
            int entryAgeNum = 0;
            if (String.IsNullOrWhiteSpace(entryName.Text)) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelName);
            }

            try {
                entryAgeNum = Convert.ToInt32(entryAge.Text);
            } catch (FormatException exc) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelAge);
            }

            if (String.IsNullOrWhiteSpace(entryAge.Text)) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelAge);
            } else if (entryAgeNum < 12 || entryAgeNum > 99) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelAge2);
            }

            if (entryGender.SelectedIndex < 0 || entryGender.SelectedItem == null) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelGender);
            }

            if (entriesGoodToGo) {
                string selectedText = entryGender.SelectedItem.ToString();
                string selectedKey = genderOptions.FirstOrDefault(x => x.Key == selectedText).Value;
                Close((entryName.Text, entryAgeNum, selectedKey));
                SemanticScreenReader.Announce(Strings.str_SemanticProperties_LoginPage_SignUpPopup_AllGood);
            }

        };
        SemanticProperties.SetDescription(acceptButton, Strings.str_SemanticProperties_LoginPage_BtnSignUp);



        var cancelButton = new Button { Text = Strings.str_ResultHistoryPage_BtnCancel, BackgroundColor = (Color)Application.Current.Resources["ButtonColor2"], TextColor = (Color)Application.Current.Resources["FontColor2"] };
        cancelButton.Clicked += (s, e) => Close(null);
        SemanticProperties.SetDescription(cancelButton, Strings.str_SemanticProperties_LoginPage_SignUpPopup_BtnCancel);

        Content = new VerticalStackLayout {
            Padding = 20,
            Spacing = 15,
            WidthRequest = 300,
            Children = {
                titleLabel, labelName, entryName, errorLabelName, labelAge, entryAge, errorLabelAge, errorLabelAge2, labelGender, entryGender, errorLabelGender,
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { cancelButton, acceptButton }
                }
            }
        };

        async Task ShowErrorAsync(Label errorLabel) {
            errorLabel.IsVisible = true;
            await errorLabel.FadeTo(1, 250);
            SemanticScreenReader.Announce(errorLabel.Text); // Notifica el error al lector de pantalla
            await Task.Delay(3000);
            await errorLabel.FadeTo(0, 250);
            errorLabel.IsVisible = false;
        }

    }
}

public partial class LoginPage : ContentPage {

    public LoginPage() {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);

        if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
            this.SemanticOrderView.ViewOrder = new List<View> { LblDos, ImageUno, LblTitulo, LblUno };
        }
    }

    private async void CrearUsuario_Clicked(object sender, EventArgs e) {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            var popup = new SignUpPopup();
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

            if (result is ValueTuple<string, int, string> inputs) {
                string newName = inputs.Item1;
                int newAge = inputs.Item2;
                string newGender = inputs.Item3;

                User newUser = new User {
                    Nombre = newName,
                    Edad = newAge,
                    Genero = newGender
                };


                int localId = await App.Database.AddUsuarioAsync(newUser);
                Preferences.Set(App.USER_ID_KEY, localId);
                newUser.IdUsuario = localId;
                App.CurrentUser = newUser;

                Application.Current.MainPage = new AppShell();

                SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_UserCreated, newName));
            }
        } finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnLogIn_Clicked(object sender, EventArgs e) {
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
                App.ButtonPressed = false;
                var selectedUser = await Application.Current.MainPage.ShowPopupAsync(popup) as User;

                if (selectedUser != null) {
                    App.CurrentUser = selectedUser;

                    Preferences.Set(App.USER_ID_KEY, selectedUser.IdUsuario);

                    Application.Current.MainPage = new AppShell();

                    SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_BtnLogIn_Success, selectedUser.Nombre));
                }
            }
        } finally {
            App.ButtonPressed = false;
        }
    }
}