using CommunityToolkit.Maui.Views;
using MejorAppTG1;
using MejorAppTG1.Resources.Localization;
using Microsoft.Maui.Controls.Shapes;

internal class AboutPopup : Popup
{
    public AboutPopup()
    {
        var mainTitleLabel = new Label {
            Text = Strings.str_Shell_AboutPage_Title,
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources["FontColor1"],
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 10)
        };

        var introContent = new Label {
            Text = Strings.str_Shell_AboutPage_LblIntro,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources["FontColor1"],
            HorizontalTextAlignment = TextAlignment.Start
        };
        SemanticProperties.SetHeadingLevel(mainTitleLabel, SemanticHeadingLevel.Level1);
        SemanticProperties.SetDescription(mainTitleLabel, Strings.str_Shell_AboutPage_Title);
        SemanticProperties.SetDescription(introContent, Strings.str_Shell_AboutPage_LblIntro);

        var baseExpander = new Expander {
            Header = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor1"],
                Text = Strings.str_Shell_AboutPage_ScientificBase,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current.Resources["FontColor3"],
                Padding = 10
            },
            Content = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"],
                Text = Strings.str_Shell_AboutPage_ScientificBase_Msg,
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FontColor1"],
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = 10
            }
        };
        var bindableBaseHeader = baseExpander.Header as BindableObject;
        var bindableBaseContent = baseExpander.Content as BindableObject;
        SemanticProperties.SetHeadingLevel(bindableBaseHeader, SemanticHeadingLevel.Level2);
        SemanticProperties.SetDescription(bindableBaseHeader, Strings.str_Shell_AboutPage_ScientificBase);
        SemanticProperties.SetDescription(bindableBaseContent, Strings.str_Shell_AboutPage_ScientificBase_Msg);


        var devExpander = new Expander {
            Header = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor1"],
                Text = Strings.str_Shell_AboutPage_Credits,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current.Resources["FontColor3"],
                Padding = 10
            },
            Content = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"],
                Text = Strings.str_Shell_AboutPage_Credits_Msg,
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FontColor1"],
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = 10
            }
        };
        var bindableDevHeader = devExpander.Header as BindableObject;
        var bindableDevContent = baseExpander.Content as BindableObject;
        SemanticProperties.SetHeadingLevel(bindableDevHeader, SemanticHeadingLevel.Level2);
        SemanticProperties.SetHeadingLevel(bindableDevContent, SemanticHeadingLevel.Level2);
        SemanticProperties.SetDescription(bindableDevContent, Strings.str_Shell_AboutPage_Credits_Msg);

        var closeButton = new Button {
            Text = Strings.str_Shell_BtnClose,
            BackgroundColor = (Color)Application.Current.Resources["ButtonColor3"],
            TextColor = (Color)Application.Current.Resources["FontColor3"],
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            CornerRadius = 20,
            BorderWidth = 2,
            BorderColor = (Color)Application.Current.Resources["BorderColor2"],
            Padding = 20,
            Margin = new Thickness(0, 20, 0, 0)
        };
        SemanticProperties.SetDescription(closeButton, Strings.str_SemanticProperties_Shell_AboutPage_BtnClose);
        closeButton.Clicked += (sender, args) => Close();

        var layout = new VerticalStackLayout {
            Padding = 20,
            Spacing = 10,
            Children = {
        mainTitleLabel,
        introContent,
        baseExpander,
        devExpander,
        closeButton
    }
        };

        Content = new Border {
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            BackgroundColor = (Color)Application.Current.Resources["GradientColor2"],
            Content = new ScrollView { Content = layout }
        };

    }
}


internal class HelpPopup : Popup
{
    public HelpPopup()
    {
        var mainTitleLabel = new Label {
            Text = Strings.str_Shell_HelpPage_Title,
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources["FontColor1"],
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 10)
        };
        SemanticProperties.SetHeadingLevel(mainTitleLabel, SemanticHeadingLevel.Level1);
        SemanticProperties.SetDescription(mainTitleLabel, Strings.str_Shell_HelpPage_Title);

        var mainTitleContent = new Label {
            Text = Strings.str_Shell_HelpPage_MainHeader,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources["FontColor1"],
            Margin = new Thickness(0, 10, 0, 5)
        };
        SemanticProperties.SetDescription(mainTitleContent, Strings.str_Shell_HelpPage_MainHeader);

        var topBarExpander = new Expander {
            Header = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor1"],
                Text = Strings.str_Shell_HelpPage_TopBar,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current.Resources["FontColor3"],
                Padding = 10
            },
            Content = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"],
                Text = Strings.str_Shell_HelpPage_TopBar_Msg,
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FontColor1"],
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = 10
            }
        };
        var bindableTopBarHeader = topBarExpander.Header as BindableObject;
        var bindableTopBarContent = topBarExpander.Content as BindableObject;
        SemanticProperties.SetHeadingLevel(bindableTopBarHeader, SemanticHeadingLevel.Level2);
        SemanticProperties.SetDescription(bindableTopBarHeader, Strings.str_Shell_HelpPage_TopBar);
        SemanticProperties.SetDescription(bindableTopBarContent, Strings.str_Shell_HelpPage_TopBar_Msg);

        var mainMenuExpander = new Expander {
            Header = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor1"],
                Text = Strings.str_Shell_HelpPage_MainMenu,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current.Resources["FontColor3"],
                Padding = 10
            },
            Content = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"],
                Text = Strings.str_Shell_HelpPage_MainMenu_Msg,
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FontColor1"],
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = 10
            }
        };
        var bindableMainMenuHeader = mainMenuExpander.Header as BindableObject;
        var bindableMainMenuContent = mainMenuExpander.Content as BindableObject;
        SemanticProperties.SetHeadingLevel(bindableMainMenuHeader, SemanticHeadingLevel.Level2);
        SemanticProperties.SetDescription(bindableMainMenuHeader, Strings.str_Shell_HelpPage_MainMenu);
        SemanticProperties.SetDescription(bindableMainMenuContent, Strings.str_Shell_HelpPage_MainMenu_Msg);

        var profileExpander = new Expander {
            Header = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor1"],
                Text = Strings.str_Shell_MyProfileTab,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current.Resources["FontColor3"],
                Padding = 10
            },
            Content = new Label {
                BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"],
                Text = Strings.str_Shell_HelpPage_MyProfile_Msg,
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FontColor1"],
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = 10
            }
        };
        var bindableProfileHeader = profileExpander.Header as BindableObject;
        var bindableProfileContent = profileExpander.Content as BindableObject;
        SemanticProperties.SetHeadingLevel(bindableProfileHeader, SemanticHeadingLevel.Level2);
        SemanticProperties.SetDescription(bindableProfileHeader, Strings.str_Shell_MyProfileTab);
        SemanticProperties.SetDescription(bindableProfileContent, Strings.str_Shell_HelpPage_MyProfile_Msg);

        var closeButton = new Button {
            Text = Strings.str_Shell_BtnClose,
            BackgroundColor = (Color)Application.Current.Resources["ButtonColor3"],
            TextColor = (Color)Application.Current.Resources["FontColor3"],
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            CornerRadius = 20,
            BorderWidth = 2,
            BorderColor = (Color)Application.Current.Resources["BorderColor2"],
            Padding = 20,
            Margin = new Thickness(0, 20, 0, 0)
        };
        SemanticProperties.SetDescription(closeButton, Strings.str_SemanticProperties_Shell_HelpPage_BtnClose);
        closeButton.Clicked += (sender, args) => Close();

        var layout = new VerticalStackLayout {
            Padding = 20,
            Spacing = 10,
            Children = {
        mainTitleLabel,
        mainTitleContent,
        topBarExpander,
        mainMenuExpander,
        profileExpander,
        closeButton
    }
        };

        Content = new Border {
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            BackgroundColor = (Color)Application.Current.Resources["GradientColor2"],
            Content = new ScrollView { Content = layout }
        };

    }
}

internal class ModifyProfilePopup : Popup
{
    public ModifyProfilePopup()
    {
        var labelName = new Label { Text = Strings.str_LoginPage_BtnSignUp_Name, TextColor = (Color)Application.Current.Resources["FontColor1"] };
        SemanticProperties.SetHeadingLevel(labelName, SemanticHeadingLevel.Level2);

        var entryName = new Entry { Placeholder = Strings.str_ResultHistoryPage_BtnSignUp_EntName, PlaceholderColor = (Color)Application.Current.Resources["ButtonColor2"], Text = App.CurrentUser.Nombre, TextColor = (Color)Application.Current.Resources["FontColor1"] };
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
        
        // ¿DatePicker quizás?
        var entryAge = new Entry { Placeholder = Strings.str_ResultHistoryPage_BtnSignUp_EntAge, PlaceholderColor = (Color)Application.Current.Resources["ButtonColor2"], Text = App.CurrentUser.Edad.ToString(), MaxLength = 2, Keyboard = Keyboard.Numeric, TextColor = (Color)Application.Current.Resources["FontColor1"] };
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
        var selectedKey = genderOptions.FirstOrDefault(pair => pair.Value == App.CurrentUser.Genero).Key;
        entryGender.SelectedItem = selectedKey;

        var acceptButton = new Button { Text = Strings.str_Shell_ModifyPage_Apply, BackgroundColor = (Color)Application.Current.Resources["ButtonColor1"], BorderWidth = 2, CornerRadius = 15, TextColor = (Color)Application.Current.Resources["FontColor2"] };
        SemanticProperties.SetDescription(acceptButton, Strings.str_Shell_ModifyPage_Apply);

        acceptButton.Clicked += (s, e) => {
            bool entriesGoodToGo = true;
            int entryAgeNum = 0;
            if (String.IsNullOrWhiteSpace(entryName.Text)) {
                entriesGoodToGo = false;
                ShowErrorAsync(errorLabelName); //Aparece un Label rojo oculto indicando el error y se desvanece después de 3 segundos
            }

            try {
                entryAgeNum = Convert.ToInt32(entryAge.Text);
            }
            catch (FormatException exc) {
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

            if (entriesGoodToGo) {
                string selectedText = entryGender.SelectedItem.ToString();
                string selectedKey = genderOptions.FirstOrDefault(x => x.Key == selectedText).Value;
                Close((entryName.Text, entryAgeNum, selectedKey));
            }
        };

        var cancelButton = new Button { Text = Strings.str_ResultHistoryPage_BtnCancel, BackgroundColor = (Color)Application.Current.Resources["ButtonColor2"], TextColor = (Color)Application.Current.Resources["FontColor2"] };
        SemanticProperties.SetDescription(cancelButton, Strings.str_ResultHistoryPage_BtnCancel);
        cancelButton.Clicked += (s, e) => Close(null);

        Content = new VerticalStackLayout {
            Padding = 20,
            Spacing = 15,
            WidthRequest = 300,
            Children = {
                new Label { Text = Strings.str_Shell_TbiModifyProfile, FontSize = 18, HorizontalOptions = LayoutOptions.Center, TextColor = (Color)Application.Current.Resources["FontColor1"] },
                labelName, entryName, errorLabelName, labelAge, entryAge, errorLabelAge, errorLabelAge2, labelGender, entryGender,
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { cancelButton, acceptButton }
                }
            }
        };

        async Task ShowErrorAsync(Label errorLabel)
        {
            errorLabel.IsVisible = true;
            await errorLabel.FadeTo(1, 250);
            await Task.Delay(3000);
            await errorLabel.FadeTo(0, 250);
            errorLabel.IsVisible = false;
        }
    }
}

namespace MejorAppTG1
{
    using Microsoft.Maui.Controls;

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));


            // Verificar si los controles son accesibles
            var lblTitulo = this.FindByName<Label>("LblTituloApp");
            var btnAbout = this.FindByName<ImageButton>("BtnAbout");
            var btnHelp = this.FindByName<ImageButton>("BtnHelp");

            if (lblTitulo != null && btnAbout != null && btnHelp != null) {
                // Configurar el orden semántico
                if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                    this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp };
                }
            }

            /*lo que deberia ir de normal
            if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp };
            }
            */
        }

        private async void BtnHelp_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var popup = new HelpPopup();
                await Application.Current.MainPage.ShowPopupAsync(popup);
            } finally {
                App.ButtonPressed = false;
            }
        }

        //modificar usario
        private async void TbiModifyProfile_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var popup = new ModifyProfilePopup();
                var result = await Application.Current.MainPage.ShowPopupAsync(popup);

                if (result is ValueTuple<string, int, string> inputs) {
                    string newName = inputs.Item1;
                    int newAge = inputs.Item2;
                    string newGender = inputs.Item3;

                    App.CurrentUser.Nombre = newName;
                    App.CurrentUser.Edad = newAge;
                    App.CurrentUser.Genero = newGender;

                    var currentPage = Shell.Current.CurrentPage;

                    if (currentPage is MainPage mainPage) {
                        mainPage.LoadUserName();
                    }
                    else if (currentPage is MyProfilePage resultHistoryPage) {
                        resultHistoryPage.UpdateUserLabels();
                    }

                    await App.Database.UpdateUsuarioAsync(App.CurrentUser);
                }
            } finally {
                App.ButtonPressed = false;
            }
        }


        //eliminar usuario
        private async void TbiDeleteProfile_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                bool respuesta = await DisplayAlert(Strings.str_Shell_DeleteProfile_Question, Strings.str_Shell_DeleteProfile_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);
                if (respuesta) {
                    await App.Database.DeleteTestsByUserAsync(App.CurrentUser.IdUsuario);
                    await App.Database.DeleteUsuarioAsync(App.CurrentUser);
                    App.CurrentUser = null;
                    Preferences.Clear();
                    MyProfilePage.ResultIndex = 0;
                    MyProfilePage.CurrentPage = 1;
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
            } finally {
                App.ButtonPressed = false;
            }
        }

        private async void TbiLogOut_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                Preferences.Clear();
                MyProfilePage.ResultIndex = 0;
                MyProfilePage.CurrentPage = 1;
                App.CurrentUser = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            } finally {
                App.ButtonPressed = false;
            }
        }

        private async void BtnAbout_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var popup = new AboutPopup();
                await Application.Current.MainPage.ShowPopupAsync(popup);
            } finally {
                App.ButtonPressed = false;
            }
        }
    }
}
