using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Globalization;
using System.Text.Json;

namespace MejorAppTG1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            AnimateFrames();
            // No tiene ningún sentido, pero este Announce es una referencia nula en Windows (¿?¿?)
            if (DeviceInfo.Current.Platform != DevicePlatform.WinUI) SemanticScreenReader.Announce(Strings.str_SemanticProperties_MainPage_Welcome);
        }

        private async void AnimateFrames()
        {
            LblWelcome.TranslationY = 50;
            LblWelcome.Opacity = 0;

            LblPrueba.TranslationY = 50;
            LblPrueba.Opacity = 0;

            BtnQuickTest.TranslationY = 50;
            BtnQuickTest.Opacity = 0;

            BtnFullTest.TranslationY = 50;
            BtnFullTest.Opacity = 0;

            BtnEatingDisordersTest.TranslationY = 50;
            BtnEatingDisordersTest.Opacity = 0;

            await LblWelcome.TranslateTo(0, 0, 250, Easing.CubicOut);
            await LblWelcome.FadeTo(1, 250);

            await LblPrueba.TranslateTo(0, 0, 250, Easing.CubicOut);
            await LblPrueba.FadeTo(1, 250);

            await BtnQuickTest.TranslateTo(0, 0, 250, Easing.CubicOut);
            await BtnQuickTest.FadeTo(1, 250);

            await BtnFullTest.TranslateTo(0, 0, 250, Easing.CubicOut);
            await BtnFullTest.FadeTo(1, 250);

            await BtnEatingDisordersTest.TranslateTo(0, 0, 250, Easing.CubicOut);
            await BtnEatingDisordersTest.FadeTo(1, 250);
        }
        
        // Parece ser que MainPage tarda menos en cargarse que SQLite en devolver el usuario logeado y CurrentUser es null cuando se lanza esta página
        // Llevo 3 horas intentando buscar una solución medianamente eficiente y esto es lo único que se me ha ocurrido, aunque es hacer la misma consulta a la B. D. otra vez
        internal async void LoadUserName()
        {
            if (App.CurrentUser == null) {
                App.CurrentUser = await App.Database.GetUserByIdAsync(Preferences.Get(App.USER_ID_KEY, 0));
            }
            LblWelcome.Text = string.Format(Strings.str_MainPage_LblWelcome_Dyn, App.CurrentUser.Nombre);


        }

        private async void BtnQuickTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;

            try {
                App.AnimateFrameInOut(sender);
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, "str_QuickTest");

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON("ShortAnxietyTestQuestions.json"), true), true);
                        return;
                    }
                    else {
                        await App.Database.DeleteTestAndAnswersAsync(unfinishedTest);
                    }
                }

                var newTest = new Test {
                    IdUser = App.CurrentUser.IdUsuario,
                    EdadUser = App.CurrentUser.Edad,
                    GeneroUser = App.CurrentUser.Genero,
                    Tipo = "str_QuickTest",
                    Fecha = DateTime.Now,
                    Terminado = false
                };

                await App.Database.AddTestAsync(newTest);
                await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON("ShortAnxietyTestQuestions.json"), false), true);
            } finally {
                App.ButtonPressed = false;
            }
        }
        private async void BtnFullTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                App.AnimateFrameInOut(sender);
                // ¿Tiene el usuario algún test de este tipo sin terminar?
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, "str_FullTest");

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    // Si el usuario quiere terminar el test a medias, inicia la página por donde se quedó
                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON("FullAnxietyTestQuestions.json"), true), true);
                        return;
                        // Si no, borra esa instancia de Test y empieza desde cero
                    }
                    else {
                        await App.Database.DeleteTestAndAnswersAsync(unfinishedTest);
                    }
                }

                // Si no hay tests a medias o el usuario quiere empezar de 0, se crea una nueva instancia de Test en la B. D.
                var newTest = new Test {
                    IdUser = App.CurrentUser.IdUsuario,
                    EdadUser = App.CurrentUser.Edad,
                    GeneroUser = App.CurrentUser.Genero,
                    Tipo = "str_FullTest",
                    Fecha = DateTime.Now,
                    Terminado = false
                };

                await App.Database.AddTestAsync(newTest);

                await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON("FullAnxietyTestQuestions.json"), false), true);
            } finally {
                App.ButtonPressed = false;
            }
        }
        private async void BtnEatingDisordersTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                App.AnimateFrameInOut(sender);
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, "str_EatingTest");

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON("EatingDisordersTestQuestions.json"), true), true);
                        return;
                    }
                    else {
                        await App.Database.DeleteTestAndAnswersAsync(unfinishedTest);
                    }
                }



                bool avisoGanarPeso = await DisplayAlert(Strings.str_MainPage_Warning, Strings.str_MainPage_EatingTestWarning, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                if (avisoGanarPeso) {
                    var newTest = new Test {
                        IdUser = App.CurrentUser.IdUsuario,
                        EdadUser = App.CurrentUser.Edad,
                        GeneroUser = App.CurrentUser.Genero,
                        Tipo = "str_EatingTest",
                        Fecha = DateTime.Now,
                        Terminado = false
                    };

                    await App.Database.AddTestAsync(newTest);
                    await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON("EatingDisordersTestQuestions.json"), false), true);
                }
            } finally {
                App.ButtonPressed = false;
            }
        }

        private async Task<List<Question>> GetQuestionsFromJSON(String fileName)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var contenido = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<List<Question>>(contenido);
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            LoadUserName();
            LblPrueba.Focus();
        }
    }
}
