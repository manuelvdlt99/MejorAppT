using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;
#if !NO_CHARTS
using Microcharts;
using Microcharts.Maui;

#endif
using Microsoft.ML;
using SkiaSharp;
using System.Globalization;

namespace MejorAppTG1;

public partial class MyProfilePage : ContentPage
{
    #region Variables
    private List<Test> _finishedTests = [];
    private readonly List<Test> _fiveTests = [];
    private readonly Dictionary<string,string> _tests = new() {
            { Strings.str_QuickTest, App.QUICK_TEST_KEY },
            { Strings.str_FullTest, App.FULL_TEST_KEY },
            { Strings.str_EatingTest, App.TCA_TEST_KEY }
        };

    /// <summary>
    /// Permite recuperar o modificar el índice en el que el usuario está actualmente posicionado en la lista de tests realizados.
    /// </summary>
    /// <value>
    /// El índice de la lista de tests realizados.
    /// </value>
    public static int ResultIndex { get; set; } = 0;
    /// <summary>
    /// Permite recuperar o modificar la página en la que el usuario está actualmente posicionado en el historial paginado de tests realizados.
    /// </summary>
    /// <value>
    /// La página del historial de tests.
    /// </value>
    public static int CurrentPage { get; set; } = 1;
    #endregion

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="MyProfilePage"/>.
    /// </summary>
    public MyProfilePage()
    {
        InitializeComponent();
        PickTipos.ItemsSource = _tests.Keys.ToList();
        PickTipos.SelectedIndex = 0;
        SemanticProperties.SetDescription(PickTipos, Strings.str_SemanticProperties_ResultHistoryPage_PickTipos_Desc);
        SemanticProperties.SetHint(PickTipos, Strings.str_SemanticProperties_ResultHistoryPage_PickTipos_Hint);
    }
    #endregion

    #region Eventos
    /// <summary>
    /// Maneja el evento de aparición de la pantalla. Muestra una pantalla de carga que dura mientras un hilo secundario carga los datos del usuario y los tests realizados.
    /// </summary>
    /// <param name="sender">La página que aparece.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void OnPageAppearing(object sender, EventArgs e)
    {
        try {
            StkLoading.IsVisible = true;
            GrdData.IsVisible = false;
            await Task.Delay(800);  // Forzar a que se espere un poco antes de cargar, porque de lo contrario el indicador de carga ni siquiera aparece
            _finishedTests = await App.Database.GetFinishedTestsByUserAsync(App.CurrentUser.IdUsuario);

            await Task.WhenAll(
                Task.Run(UpdateUserImage),
                Task.Run(UpdateUserLabels),
                Task.Run(LoadResults)
            );
        }
        finally {
            StkLoading.IsVisible = false;
            GrdData.IsVisible = true;
            string translatedAgeFormat = Strings.str_ResultHistoryPage_LblAge_Dyn;
            string translatedAge = string.Format(translatedAgeFormat, App.CurrentUser.Edad);
            string semanticDescription = string.Format(Strings.str_SemanticProperties_LoginPage_UsersPopup_SelectedUser, App.CurrentUser.Nombre, translatedAge, Strings.ResourceManager.GetString(App.CurrentUser.Genero, CultureInfo.CurrentUICulture));
            SemanticScreenReader.Announce(semanticDescription);
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación sobre un test realizado en el historial. Pregunta al usuario si quiere consultarlo o eliminarlo.
    /// </summary>
    /// <param name="sender">El test pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateFrameInOut(sender);

            var action = await DisplayActionSheet(Strings.str_ResultHistoryPage_ChooseOption_Question, Strings.str_ResultHistoryPage_BtnCancel, null, Strings.str_ResultHistoryPage_BtnCheck, Strings.str_ResultHistoryPage_BtnDelete);

            if (action == Strings.str_ResultHistoryPage_BtnCheck) {
                ConsultarRegistro(sender);
            }
            else if (action == Strings.str_ResultHistoryPage_BtnDelete) {
                EliminarRegistro(sender);
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Consultar último test. Abre la pantalla de consejos del último test terminado cronológicamente (el primero del historial).
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnLastResult_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            if (_finishedTests == null || _finishedTests.Count == 0) {
                await DisplayAlert(Strings.str_ResultHistoryPage_BtnCheck_NoResults, Strings.str_ResultHistoryPage_BtnCheck_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
            }
            else {
                await CalculateFactorsAndShowResults(_finishedTests[0]);
            }
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Anteriores. Mueve la lista paginada 5 tests a la izquierda.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnPreviousFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            ResultIndex -= 5;
            CurrentPage--;
            await LoadNextFiveTests();
            await SlideCollectionView("Left");
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Siguientes. Mueve la lista paginada 5 tests a la derecha.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnNextFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            ResultIndex += 5;
            CurrentPage++;
            await LoadNextFiveTests();
            await SlideCollectionView("Right");
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación de la pestaña de Historial de tests. Muestra la interfaz del historial.
    /// </summary>
    /// <param name="sender">La pestaña pulsada.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        FrmTabResults.Stroke = (Color)Application.Current.Resources["HeaderColor1"];
        FrmTabResults.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmTabAnalysis.Stroke = (Color)Application.Current.Resources["ButtonColor2"];
        FrmTabAnalysis.BackgroundColor = (Color)Application.Current.Resources["ButtonColor4"];
        VslResults.IsVisible = true;
        VslAnalysis.IsVisible = false;
    }

    /// <summary>
    /// Maneja el evento de pulsación de la pestaña de Análisis de resultados. Muestra la interfaz del análisis.
    /// </summary>
    /// <param name="sender">La pestaña pulsada.</param>
    /// <param name="e">La instancia <see cref="TappedEventArgs"/> que contiene los datos del evento.</param>
    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        FrmTabAnalysis.Stroke = (Color)Application.Current.Resources["HeaderColor1"];
        FrmTabAnalysis.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmTabResults.Stroke = (Color)Application.Current.Resources["ButtonColor2"];
        FrmTabResults.BackgroundColor = (Color)Application.Current.Resources["ButtonColor4"];
        await CreateGraph();
        VslResults.IsVisible = false;
        VslAnalysis.IsVisible = true;
    }

    /// <summary>
    /// Maneja el evento de pulsación del botón de Crear gráfico.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnCreateGraph_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            await CreateGraph();
        } finally {
            App.ButtonPressed = false;
        }
    }
    #endregion

    #region Métodos    
    /// <summary>
    /// Actualiza los datos del usuario actual.
    /// </summary>
    internal void UpdateUserLabels()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblUsername.Text = App.CurrentUser.Nombre;
            LblUserAge.Text = string.Format(Strings.str_ResultHistoryPage_LblAge_Dyn, App.CurrentUser.Edad.ToString());
            LblUserGender.Text = Strings.ResourceManager.GetString(App.CurrentUser.Genero, CultureInfo.CurrentUICulture);
            if (App.CurrentUser.Genero == App.GENDERS_MALE_KEY) {
                LblUserGenderIcon.Text = "♂";
            }
            else if (App.CurrentUser.Genero == App.GENDERS_FEMALE_KEY) {
                LblUserGenderIcon.Text = "♀";
            }
            else if (App.CurrentUser.Genero == App.GENDERS_NB_KEY) {
                LblUserGenderIcon.Text = "🜬";
            }
        });
    }

    /// <summary>
    /// Actualiza la imagen de perfil del usuario actual.
    /// </summary>
    internal void UpdateUserImage()
    {
        string image = App.CurrentUser.Imagen;
        if (image != null) {
            MainThread.BeginInvokeOnMainThread(() => {
                ImgProfile.Source = ImageSource.FromFile(image);
            });
        }
    }

    /// <summary>
    /// Muestra los siguientes cinco tests del historial.
    /// </summary>
    private async Task LoadNextFiveTests()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            CtvResults.ItemsSource = null;
            _fiveTests.Clear();

            // No puede haber una página -1, evidentemente
            if (ResultIndex == 0) {
                BtnPreviousFive.IsEnabled = false;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
            }
            else {
                BtnPreviousFive.IsEnabled = true;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["SecondaryColor1"];
            }

            for (int i = ResultIndex; i < ResultIndex + 5; i++) {
                if (i < _finishedTests.Count) {
                    Test currentTest = _finishedTests[i];
                    if (currentTest != null) {
                        _fiveTests.Add(currentTest);
                    }
                    // Si i se pasa del último índice de la lista entera en el bucle, es que no hay más entradas. Deshabilita página siguiente y sal del bucle
                }
                else {
                    BtnNextFive.IsEnabled = false;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
                    break;
                }

                // Si por ejemplo solo hay 5 entradas, va a mostrar esas 5 siempre, pero no se pueden comprobar en el bucle las siguientes 5
                // Hay que comprobar de antemano si podría haber más entradas en una nueva página o no
                //Ej: i se ha quedado en [4] de 5 entradas totales (la última). Hay que comprobar si i es, efectivamente, el último índice de la lista entera
                if (i >= _finishedTests.Count - 1) {
                    BtnNextFive.IsEnabled = false;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
                }
                else {
                    BtnNextFive.IsEnabled = true;
                    BtnNextFive.BorderColor = (Color)Application.Current.Resources["SecondaryColor1"];
                }
            }
            CtvResults.ItemsSource = _fiveTests;
            LblCurrentPage.Text = string.Format(Strings.str_ResultHistoryPage_LblCurrentPage_Dyn, CurrentPage, (int)Math.Ceiling(_finishedTests.Count / 5.0));
        });
    }

    /// <summary>
    /// Muestra el historial de tests.
    /// </summary>
    private void LoadResults()
    {
        MainThread.BeginInvokeOnMainThread(async () => {
            if (_finishedTests == null || _finishedTests.Count == 0) {
                CtvResults.IsVisible = false;
                HslNavigationBtns.IsVisible = false;
                VslNoResults.IsVisible = true;
                await VslNoResults.ScaleTo(1.05, 300, Easing.BounceOut);
                await VslNoResults.ScaleTo(1, 300, Easing.BounceIn);
            }
            else {
                VslNoResults.IsVisible = false;
                HslNavigationBtns.IsVisible = true;
                CtvResults.IsVisible = true;
                LoadNextFiveTests();
            }
        });
    }

    /// <summary>
    /// Abre la pantalla de consejos del test seleccionado.
    /// </summary>
    /// <param name="sender">El Frame del test sobre el que se ha pulsado.</param>
    private async Task ConsultarRegistro(object sender)
    {
        Test selectedTest = (Test)((Frame)sender).BindingContext;
        await CalculateFactorsAndShowResults(selectedTest);
    }

    /// <summary>
    /// Calcula los factores de un test dado y muestra la pantalla con los consejos pertinentes.
    /// </summary>
    /// <param name="test">El test.</param>
    private async Task CalculateFactorsAndShowResults(Test test)
    {
        List<Answer> answers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
        var resultados = await Task.WhenAll(
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "1", test)),
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "2", test)),
            Task.Run(() => ScoreCalculator.CalculoFactores(answers, "3", test))
        );

        Factor factor1 = resultados[0];
        Factor factor2 = resultados[1];
        Factor factor3 = resultados[2];

        await Navigation.PushAsync(new ResultsPage(factor1, factor2, factor3, test.Tipo), true);
    }

    /// <summary>
    /// Elimina el test seleccionado.
    /// </summary>
    /// <param name="sender">El Frame del test sobre el que se ha pulsado.</param>
    private async Task EliminarRegistro(object sender)
    {
        bool answer = await DisplayAlert(Strings.str_ResultHistoryPage_BtnDelete_Question, Strings.str_ResultHistoryPage_BtnDelete_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);
        
        if (answer) {
            Test selectedTest = (Test)((Frame)sender).BindingContext;

            await App.Database.DeleteTestAndAnswersAsync(selectedTest);
            _finishedTests = await App.Database.GetFinishedTestsByUserAsync(App.CurrentUser.IdUsuario);
            LoadResults();

            // Si al eliminar este registro se queda la página vacía, vuelve una página para atrás
            if (ResultIndex >= _finishedTests.Count && ResultIndex > 0) {
                BtnPreviousFive_Clicked(null, null);
            }
        }
    }

    /// <summary>
    /// Anima lateralmente el historial de tests.
    /// </summary>
    /// <param name="direction">La dirección ("Left" o "Right").</param>
    private async Task SlideCollectionView(string direction)
    {
        var offset = direction == "Left" ? -100 : 100;
        CtvResults.TranslationX = offset;
        await CtvResults.TranslateTo(0, 0, 300, Easing.CubicOut);
    }

    /// <summary>
    /// Calcula los datos de los tests realizados del tipo seleccionado y muestra un gráfico con los datos obtenidos y un texto con una valoración del usuario utilizando modelos de IA.
    /// </summary>
    private async Task CreateGraph()
    {
#if !NO_CHARTS
        string selectedTest = PickTipos.SelectedItem.ToString();
        string selectedKey = _tests.FirstOrDefault(x => x.Key == selectedTest).Value;
        var tests = await App.Database.GetFinishedTestsByUserFilteredAsync(App.CurrentUser.IdUsuario, selectedKey);

        var entriesList = new List<ChartEntry>();
        var chart = new ChartView {
            HeightRequest = 400
        };

        if (tests.Count > 0) {
            VslNoResultsGraph.IsVisible = false;
            LblPrediction.IsVisible = true;
            BrdChartView.IsVisible = true;

            foreach (var test in tests) {
                var answers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
                int totalPoints = answers.Sum(a => a.ValorRespuesta);
                var factores = GetFactores(answers, test);
                var entry = new ChartEntry(totalPoints) {
                    Label = test.Fecha.ToString("dd/MM"),
                    ValueLabel = Strings.ResourceManager.GetString(GetCategoria(factores), CultureInfo.CurrentUICulture),
                    Color = SKColor.Parse("#31be33")
                };

                entriesList.Add(entry);
            }

            chart.Chart = new LineChart {
                Entries = [.. entriesList],
                BackgroundColor = SKColor.Parse("#f6ffe3"),
                LineMode = LineMode.Straight,
                LineSize = 4,
                PointSize = 10,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal
            };

            BrdChartView.Content = chart;

            if (tests.Count < 3) {
                string path = string.Empty;
                switch (selectedKey) {
                    case App.QUICK_TEST_KEY:
                        path = App.AI_GENERAL_QUICK_TEST_PATH;
                        break;
                    case App.FULL_TEST_KEY:
                        path = App.AI_GENERAL_FULL_TEST_PATH;
                        break;
                    case App.TCA_TEST_KEY:
                        path = App.AI_GENERAL_TCA_TEST_PATH;
                        break;
                }
                var targetPath = Path.Combine(FileSystem.AppDataDirectory, path);

                if (!File.Exists(targetPath)) {
                    using var stream = await FileSystem.OpenAppPackageFileAsync(path);
                    using var fileStream = File.Create(targetPath);
                    await stream.CopyToAsync(fileStream);
                }

                AIData user = new() {
                    EdadRango = GetAgeRange(tests[^1].EdadUser),
                    Genero = tests[^1].GeneroUser
                };
                float prediction = AIService.GetAIPredictedAvgResult(new MLContext(), targetPath, user);
                LblPrediction.Text = AIService.InterpretPrediction(prediction, entriesList[^1].Value);
            } else {
                string path = string.Empty;
                switch (selectedKey) {
                    case App.QUICK_TEST_KEY:
                        path = App.AI_HISTORY_QUICK_TEST_PATH;
                        break;
                    case App.FULL_TEST_KEY:
                        path = App.AI_HISTORY_FULL_TEST_PATH;
                        break;
                    case App.TCA_TEST_KEY:
                        path = App.AI_HISTORY_TCA_TEST_PATH;
                        break;
                }
                var targetPath = Path.Combine(FileSystem.AppDataDirectory, path);

                if (!File.Exists(targetPath)) {
                    using var stream = await FileSystem.OpenAppPackageFileAsync(path);
                    using var fileStream = File.Create(targetPath);
                    await stream.CopyToAsync(fileStream);
                }

                var lastThree = entriesList.TakeLast(3).ToList();

                AIProgressiveData user = new() {
                    EdadRango = GetAgeRange(tests[^1].EdadUser),
                    Genero = tests[^1].GeneroUser,
                    P1 = lastThree[0].Value ?? 0,
                    P2 = lastThree[1].Value ?? 0,
                    P3 = lastThree[2].Value ?? 0
                };

                float prediction = AIService.GetAIPredictedEvolutionResult(new MLContext(), targetPath, user);
                LblPrediction.Text = AIService.GetPredictionEvaluation(prediction, user.P3);
            }
        } else {
            VslNoResultsGraph.IsVisible = true;
            LblPrediction.IsVisible = false;
            BrdChartView.IsVisible = false;
            await VslNoResultsGraph.ScaleTo(1.05, 300, Easing.BounceOut);
            await VslNoResultsGraph.ScaleTo(1, 300, Easing.BounceIn);
        }
#endif
    }

    /// <summary>
    /// Calcula y devuelve el nivel más apropiado según los factores obtenidos en un test. Se hacen condicionales para devolver el nivel más apropiado e intermedio entre los factores dados. Por ejemplo, una combinación de "Bajo" y "Alto" devuelve "Medio".
    /// </summary>
    /// <param name="factores">La lista de factores calculados del test actual.</param>
    /// <returns>El nivel calculado.</returns>
    private static string GetCategoria(List<Factor> factores)
    {
        if (factores.Count == 1) {  // Pensado para TCA
            return factores[0].Nivel;
        }

        var grupos = factores
        .GroupBy(obj => obj.Nivel)
        .Select(g => g.Key)
        .ToList();

        if (grupos.Contains(App.FACTORS_LEVEL_LOW) && !grupos.Contains(App.FACTORS_LEVEL_MEDIUM) && !grupos.Contains(App.FACTORS_LEVEL_HIGH)) {
            return App.FACTORS_LEVEL_LOW;
        }
        else if (grupos.Contains(App.FACTORS_LEVEL_LOW) && grupos.Contains(App.FACTORS_LEVEL_MEDIUM) && !grupos.Contains(App.FACTORS_LEVEL_HIGH)) {
            return App.FACTORS_LEVEL_LOW_MEDIUM;
        }
        else if (grupos.Contains(App.FACTORS_LEVEL_LOW) && grupos.Contains(App.FACTORS_LEVEL_HIGH) && !grupos.Contains(App.FACTORS_LEVEL_MEDIUM)) {
            return App.FACTORS_LEVEL_MEDIUM;
        }
        else if (grupos.Contains(App.FACTORS_LEVEL_MEDIUM) && !grupos.Contains(App.FACTORS_LEVEL_LOW) && !grupos.Contains(App.FACTORS_LEVEL_HIGH)) {
            return App.FACTORS_LEVEL_MEDIUM;
        }
        else if (grupos.Contains(App.FACTORS_LEVEL_MEDIUM) && grupos.Contains(App.FACTORS_LEVEL_HIGH) && !grupos.Contains(App.FACTORS_LEVEL_LOW)) {
            return App.FACTORS_LEVEL_MEDIUM_HIGH;
        }
        else if (grupos.Contains(App.FACTORS_LEVEL_HIGH) && !grupos.Contains(App.FACTORS_LEVEL_LOW) && !grupos.Contains(App.FACTORS_LEVEL_MEDIUM)) {
            return App.FACTORS_LEVEL_HIGH;
        }
        else {
            return App.FACTORS_LEVEL_MEDIUM;
        }
    }

    /// <summary>
    /// Calcula los factores de un test realizado.
    /// </summary>
    /// <param name="preguntas">Las respuestas del test.</param>
    /// <param name="tipoTest">El test realizado.</param>
    /// <returns>La lista de factores calculados.</returns>
    private static List<Factor> GetFactores(List<Answer> preguntas, Test tipoTest)
    {
        List<Factor> factores = [];
        if (tipoTest.Tipo.Equals(App.QUICK_TEST_KEY) || tipoTest.Tipo.Equals(App.FULL_TEST_KEY)) {
            for (int i = 1; i <= 3; i++) {
                factores.Add(ScoreCalculator.CalculoFactores(preguntas, i.ToString(), tipoTest));
            }
        } else {
            factores.Add(ScoreCalculator.CalculoFactores(preguntas, App.FACTORS_1, tipoTest));
        }
        return factores;
    }

    /// <summary>
    /// Convierte una edad en una edad simbólica de un rango determinado (14, 15 o 17) para facilitar el entrenamiento de la IA.
    /// </summary>
    /// <param name="age">La edad real.</param>
    /// <returns>La edad representativa del rango al que pertenece la edad pasada (14, 15 o 17).</returns>
    private static int GetAgeRange(int age)
    {
        if (age <= 14)
            return 14;
        else if (age <= 16)
            return 15;
        else
            return 17;
    }
    #endregion
}