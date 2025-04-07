using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
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

internal class UsersPopup : Popup {
    public UsersPopup(List<User> usuarios) {
        var layout = new Grid {
            HeightRequest = 400,
            WidthRequest = 350,
            BackgroundColor = (Color)Application.Current.Resources["SecondaryColor3"],
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Título
                new RowDefinition { Height = GridLength.Star }  // Usuarios
            }
        };

        var image = new Image {
            Source = "fondo_provisional1.jpeg",
            Aspect = Aspect.AspectFill
        };


        var titulo = new Label {
            Text = Strings.str_LoginPage_LblSelectUser,
            FontSize = 22,
            Margin = new Thickness(0, 20, 0, 5),
            TextColor = (Color)Application.Current.Resources["FontColor1"],
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold
        };
        SemanticProperties.SetHeadingLevel(titulo, SemanticHeadingLevel.Level1);
        SemanticProperties.SetDescription(titulo, Strings.str_SemanticProperties_LoginPage_UsersPopup_Title);
        layout.Add(image);
        layout.Add(titulo);
        Grid.SetRow(titulo, 0);
        Grid.SetRowSpan(image, 2);

        var collectionView = new CollectionView {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            SelectionMode = SelectionMode.Single,
            ItemsSource = usuarios,
            ItemTemplate = new DataTemplate(() => {
                var frame = new Frame {
                    BorderColor = (Color)Application.Current.Resources["ButtonColor2"],
                    CornerRadius = 10,
                    Margin = 15,
                    Padding = 10,
                    BackgroundColor = (Color)Application.Current.Resources["SecondaryColor4"]
                };

                var grid = new Grid {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star }, // Texto
                        new ColumnDefinition { Width = GridLength.Auto }  // Icono
                    },
                    Padding = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center
                };

                var verticalLayout = new VerticalStackLayout {
                    Spacing = 2,
                    VerticalOptions = LayoutOptions.Center
                };

                var nombreLabel = new Label {
                    FontSize = 20,
                    TextColor = (Color)Application.Current.Resources["FontColor4"],
                    FontAttributes = FontAttributes.Bold
                };
                nombreLabel.SetBinding(Label.TextProperty, "Nombre");
                SemanticProperties.SetDescription(nombreLabel, Strings.str_SemanticProperties_LoginPage_UsersPopup_Name);

                var edadLabel = new Label {
                    FontSize = 15,
                    TextColor = (Color)Application.Current.Resources["FontColor5"],
                    FontAttributes = FontAttributes.Bold
                };
                edadLabel.SetBinding(Label.TextProperty, "Edad");
                edadLabel.BindingContextChanged += (sender, e) =>
                {
                    var user = (User)((Label)sender).BindingContext;
                    if (user != null) {
                        int edad = user.Edad;

                        // Traducir el formato de la edad según el idioma actual
                        string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;

                        // Formatear la edad con la cadena traducida
                        string translatedAge = string.Format(translatedAgeFormat, edad);

                        // Asignar el texto traducido al Label
                        edadLabel.Text = translatedAge;
                    }
                };
                SemanticProperties.SetDescription(edadLabel, Strings.str_SemanticProperties_LoginPage_UsersPopup_Age);

                var generoLabel = new Label {
                    FontSize = 12,
                    TextColor = (Color)Application.Current.Resources["FontColor6"],
                    FontAttributes = FontAttributes.Bold
                };

                generoLabel.SetBinding(Label.TextProperty, "Genero");
                generoLabel.BindingContextChanged += (sender, e) =>
                {
                    var user = (User)((Label)sender).BindingContext;
                    if (user != null) {
                        // Traducir el género según el idioma actual
                        string translatedGender = user.Genero switch {
                            "str_Genders_Man" => Strings.str_Genders_Man,
                            "str_Genders_Woman" => Strings.str_Genders_Woman,
                            "str_Genders_NB" => Strings.str_Genders_NB
                        };

                        // Asignar el texto traducido al Label
                        generoLabel.Text = translatedGender;
                    }
                };
                SemanticProperties.SetDescription(generoLabel, Strings.str_SemanticProperties_LoginPage_UsersPopup_Gender);

                verticalLayout.Add(nombreLabel);
                verticalLayout.Add(edadLabel);
                verticalLayout.Add(generoLabel);

                // Icono a la derecha
                var icono = new Image {
                    Source = "profile_icon.png",
                    BackgroundColor = Colors.Transparent,
                    HeightRequest = 60,
                    WidthRequest = 60,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Aspect = Aspect.AspectFill,
                    Clip = new EllipseGeometry {
                        Center = new Point(30, 30),
                        RadiusX = 30,
                        RadiusY = 30
                    }
                };

                icono.SetBinding(Image.SourceProperty, new Binding("Imagen", converter: new NullToDefaultImageConverter()));
                SemanticProperties.SetDescription(icono, Strings.str_SemanticProperties_LoginPage_UsersPopup_Image);

                grid.Add(verticalLayout, 0, 0);
                grid.Add(icono, 1, 0);

                frame.Content = grid;

                frame.BindingContextChanged += (sender, e) =>
                {
                    var currentFrame = (Frame)sender;
                    if (currentFrame.BindingContext is User user) {
                        // Formatear la edad con la cadena traducida
                        string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
                        string translatedAge = string.Format(translatedAgeFormat, user.Edad);

                        // Crear la descripción combinada
                        string semanticDescription = string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser, user.Nombre, translatedAge, Strings.ResourceManager.GetString(user.Genero, CultureInfo.CurrentUICulture));

                        // Asignar la descripción al SemanticProperties.Description del Frame
                        SemanticProperties.SetDescription(currentFrame, semanticDescription);
                    }
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => {
                    App.AnimateFrameInOut(frame);
                    if (frame.BindingContext is User usuarioSeleccionado) {
                        Close(usuarioSeleccionado);
                    }
                };
                frame.GestureRecognizers.Add(tapGesture);

                frame.SetBinding(BindingContextProperty, new Binding("."));

                return frame;
            }),
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        //SemanticProperties.SetDescription(collectionView, Strings.str_SemanticProperties_LoginPage_UsersPopup_List);
        collectionView.SelectionChanged += (sender, args) => {
            if (args.CurrentSelection.FirstOrDefault() is User usuarioSeleccionado) {
                SemanticScreenReader.Announce(string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser2, usuarioSeleccionado.Nombre));
                Close(usuarioSeleccionado);
            }
        };

        layout.Add(collectionView);
        Grid.SetRow(collectionView, 1);
        Content = layout;
    }
}

public partial class LoginPage : ContentPage {

    public LoginPage() {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);


        if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
            this.SemanticOrderView.ViewOrder = new List<View> { LblDos, ImageUno, LblTitulo, LblUno };
        }


        //Preferences
        //LoadSavedName();

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

                /*
                //guardar preferences del name
                string name = nickUsuario.Text;
                Preferences.Set(NameKey, name);

                //guardar preferences de la edad
                string age = edadUsuario.Text;
                Preferences.Set(AgeKey, age);

                //guardar preferences del genero
                string gender = generoUsuario.SelectedItem.ToString();
                Preferences.Set(GenderKey, gender);
                */

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

    // Verificar si está almacenado en las preferencias
    /*private void LoadSavedName() {
        if (Preferences.ContainsKey(NameKey)) {
            string savedName = Preferences.Get(NameKey, string.Empty);
            nickUsuario.Text = savedName;
        }

        if (Preferences.ContainsKey(AgeKey)) {
            string savedAge = Preferences.Get(AgeKey, string.Empty);
            edadUsuario.Text = savedAge;
        }

        if (Preferences.ContainsKey(GenderKey)) {
            string savedGenero = Preferences.Get(GenderKey, string.Empty);

            // Actualizar el Picker para reflejar la selección guardada
            int savedIndex = generoUsuario.Items.IndexOf(savedGenero);
            if (savedIndex != -1) {
                generoUsuario.SelectedIndex = savedIndex;
            }
        }
    }*/

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
                var popup = new UsersPopup(activeUsers);
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

public class NullToDefaultImageConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string imagePath = value as string;
        return string.IsNullOrEmpty(imagePath) ? "profile_icon.png" : imagePath;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
