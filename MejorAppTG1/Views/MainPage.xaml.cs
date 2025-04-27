using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Globalization;
using System.Text.Json;

namespace MejorAppTG1
{
    public partial class MainPage : ContentPage
    {
        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MainPage"/>.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            AnimateFrames();
            // No tiene ningún sentido, pero este Announce es una referencia nula en Windows (¿?¿?)
            if (DeviceInfo.Current.Platform != DevicePlatform.WinUI) SemanticScreenReader.Announce(Strings.str_SemanticProperties_MainPage_Welcome);
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Maneja el evento de pulsación del botón de Test de ansiedad rápido. Abre la pantalla de test de ansiedad rápido. Antes, comprueba si el usuario ha dejado un test a medias; si es así, le pregunta si quiere retomarlo. Si el usuario responde que sí, el test se posiciona en la primera pregunta sin responder (o en la última si se ha respondido a todas). Si el usuario responde que no, el test almacenado se elimina y se abre la pantalla con un test nuevo desde cero.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void BtnQuickTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;

            try {
                App.AnimateFrameInOut(sender);
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, App.QUICK_TEST_KEY);

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_QUICK), true), true);
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
                    Tipo = App.QUICK_TEST_KEY,
                    Fecha = DateTime.Now,
                    Terminado = false
                };

                await App.Database.AddTestAsync(newTest);
                await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_QUICK), false), true);
            }
            finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Test de ansiedad completo. Abre la pantalla de test de ansiedad completo. Antes, comprueba si el usuario ha dejado un test a medias; si es así, le pregunta si quiere retomarlo. Si el usuario responde que sí, el test se posiciona en la primera pregunta sin responder (o en la última si se ha respondido a todas). Si el usuario responde que no, el test almacenado se elimina y se abre la pantalla con un test nuevo desde cero.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void BtnFullTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                App.AnimateFrameInOut(sender);
                // ¿Tiene el usuario algún test de este tipo sin terminar?
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, App.FULL_TEST_KEY);

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    // Si el usuario quiere terminar el test a medias, inicia la página por donde se quedó
                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_FULL), true), true);
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
                    Tipo = App.FULL_TEST_KEY,
                    Fecha = DateTime.Now,
                    Terminado = false
                };

                await App.Database.AddTestAsync(newTest);

                await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_FULL), false), true);
            }
            finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Test de TCA. Abre la pantalla de test de TCA. Antes, comprueba si el usuario ha dejado un test a medias; si es así, le pregunta si quiere retomarlo. Si el usuario responde que sí, el test se posiciona en la primera pregunta sin responder (o en la última si se ha respondido a todas). Si el usuario responde que no, el test almacenado se elimina y se abre la pantalla con un test nuevo desde cero.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void BtnEatingDisordersTest_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                App.AnimateFrameInOut(sender);
                var unfinishedTest = await App.Database.GetUnfinishedTestAsync(App.CurrentUser.IdUsuario, App.TCA_TEST_KEY);

                if (unfinishedTest != null) {
                    var culture = CultureInfo.CurrentCulture;
                    var formattedDate = unfinishedTest.Fecha.ToString(Strings.DateFormat, culture);
                    bool resume = await DisplayAlert(Strings.str_MainPage_UnfinishedTest, string.Format(Strings.str_MainPage_UnfinishedTest_Msg, formattedDate), Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);

                    if (resume) {
                        await Navigation.PushAsync(new TestPage(unfinishedTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_TCA), true), true);
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
                        Tipo = App.TCA_TEST_KEY,
                        Fecha = DateTime.Now,
                        Terminado = false
                    };

                    await App.Database.AddTestAsync(newTest);
                    await Navigation.PushAsync(new TestPage(newTest, await GetQuestionsFromJSON(App.JSON_QUESTIONS_TCA), false), true);
                }
            }
            finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de aparición de la pantalla. Muestra el nombre del usuario actual en la pantalla.
        /// </summary>
        /// <param name="sender">La página que aparece.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            LoadUserName();
            LblPrueba.Focus();
        }
        #endregion

        #region Métodos        
        /// <summary>
        /// Anima los textos y los botones del menú principal.
        /// </summary>
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
        /// <summary>
        /// Carga el nombre del usuario que ha iniciado sesióna ctualmente y lo muestra en pantalla.
        /// </summary>
        internal async void LoadUserName()
        {
            if (App.CurrentUser == null) {
                App.CurrentUser = await App.Database.GetUserByIdAsync(Preferences.Get(App.USER_ID_KEY, 0));
            }
            LblWelcome.Text = string.Format(Strings.str_MainPage_LblWelcome_Dyn, App.CurrentUser.Nombre);
        }

        /// <summary>
        /// Extrae las preguntas de un JSON dado.
        /// </summary>
        /// <param name="fileName">La ruta del fichero JSON con las preguntas.</param>
        /// <returns>Una lista de preguntas.</returns>
        private async Task<List<Question>> GetQuestionsFromJSON(String fileName)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var contenido = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<List<Question>>(contenido);
        }
        #endregion
    }
}
