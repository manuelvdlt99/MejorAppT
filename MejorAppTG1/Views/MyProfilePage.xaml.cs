using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;
using Microcharts;
using Microsoft.ML;
using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;

namespace MejorAppTG1;

public partial class MyProfilePage : ContentPage
{
    #region Variables
    private static int _resultIndex = 0;
    private static int _currentPage = 1;
    private List<Test> _finishedTests;
    private List<Test> _fiveTests = new();
    private Dictionary<string,string> _tests = new Dictionary<string, string>
        {
            { Strings.str_QuickTest, App.QUICK_TEST_KEY },
            { Strings.str_FullTest, App.FULL_TEST_KEY },
            { Strings.str_EatingTest, App.TCA_TEST_KEY }
        };
    /*private DateTime? _dateFrom;
    private DateTime? _dateUntil;*/

    /// <summary>
    /// Permite recuperar o modificar el índice en el que el usuario está actualmente posicionado en la lista de tests realizados.
    /// </summary>
    /// <value>
    /// El índice de la lista de tests realizados.
    /// </value>
    public static int ResultIndex { get => _resultIndex; set => _resultIndex = value; }
    /// <summary>
    /// Permite recuperar o modificar la página en la que el usuario está actualmente posicionado en el historial paginado de tests realizados.
    /// </summary>
    /// <value>
    /// La página del historial de tests.
    /// </value>
    public static int CurrentPage { get => _currentPage; set => _currentPage = value; }
    #endregion

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="MyProfilePage"/>.
    /// </summary>
    public MyProfilePage()
    {
        InitializeComponent();
        ChartView.Chart = new LineChart();
        /*DtpFrom.Date = DateTime.UtcNow.AddDays(-120);
        DtpUntil.Date = DateTime.UtcNow;*/
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
                LoadResults()
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
            _resultIndex -= 5;
            _currentPage--;
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
            _resultIndex += 5;
            _currentPage++;
            await LoadNextFiveTests();
            await SlideCollectionView("Right");
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /*private void DtpUntil_DateSelected(object sender, DateChangedEventArgs e)
    {
        _dateUntil = e.NewDate;
    }

    private void DtpFrom_DateSelected(object sender, DateChangedEventArgs e)
    {
        _dateFrom = e.NewDate;
    }

    private async void BtnFilter_Clicked(object sender, EventArgs e)
    {
        List<string> selectedTypes = [];
        if (ChipQuick.IsSelected) {
            selectedTypes.Add(App.QUICK_TEST_KEY);
        }
        if (ChipFull.IsSelected) {
            selectedTypes.Add(App.FULL_TEST_KEY);
        }
        if (ChipTCA.IsSelected) {
            selectedTypes.Add(App.TCA_TEST_KEY);
        }
        _finishedTests = await App.Database.GetFinishedTestsByUserFilteredAsync(App.CurrentUser.IdUsuario, _dateFrom ?? DateTime.MinValue, _dateUntil ?? DateTime.UtcNow, selectedTypes);
        await LoadResults();
    }*/

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
            if (App.CurrentUser.Genero == "str_Genders_Man") {
                LblUserGenderIcon.Text = "♂";
            }
            else if (App.CurrentUser.Genero == "str_Genders_Woman") {
                LblUserGenderIcon.Text = "♀";
            }
            else if (App.CurrentUser.Genero == "str_Genders_NB") {
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
            if (_resultIndex == 0) {
                BtnPreviousFive.IsEnabled = false;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["BorderColor3"];
            }
            else {
                BtnPreviousFive.IsEnabled = true;
                BtnPreviousFive.BorderColor = (Color)Application.Current.Resources["SecondaryColor1"];
            }

            for (int i = _resultIndex; i < _resultIndex + 5; i++) {
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
            LblCurrentPage.Text = string.Format(Strings.str_ResultHistoryPage_LblCurrentPage_Dyn, _currentPage, (int)Math.Ceiling(_finishedTests.Count / 5.0));
        });
    }

    /// <summary>
    /// Muestra el historial de tests.
    /// </summary>
    private async Task LoadResults()
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
                await LoadNextFiveTests();
            }
        });
    }

    /// <summary>
    /// Abre la pantalla de consejos del test seleccionado.
    /// </summary>
    /// <param name="sender">El Frame del test sobre el que se ha pulsado.</param>
    private async void ConsultarRegistro(object sender)
    {
        Test selectedTest = (Test)((Frame)sender).BindingContext;
        CalculateFactorsAndShowResults(selectedTest);
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
    private async void EliminarRegistro(object sender)
    {
        bool answer = await DisplayAlert(Strings.str_ResultHistoryPage_BtnDelete_Question, Strings.str_ResultHistoryPage_BtnDelete_Msg, Strings.str_MainPage_BtnYes, Strings.str_MainPage_BtnNo);
        
        if (answer) {
            Test selectedTest = (Test)((Frame)sender).BindingContext;

            await App.Database.DeleteTestAndAnswersAsync(selectedTest);
            _finishedTests = await App.Database.GetFinishedTestsByUserAsync(App.CurrentUser.IdUsuario);
            LoadResults();

            // Si al eliminar este registro se queda la página vacía, vuelve una página para atrás
            if (_resultIndex >= _finishedTests.Count && _resultIndex > 0) {
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
        string selectedTest = PickTipos.SelectedItem.ToString();
        string selectedKey = _tests.FirstOrDefault(x => x.Key == selectedTest).Value;
        var tests = await App.Database.GetFinishedTestsByUserFilteredAsync(App.CurrentUser.IdUsuario, selectedKey);

        var entriesList = new List<ChartEntry>();

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

            ChartView.Chart = new LineChart {
                Entries = [.. entriesList],
                BackgroundColor = SKColor.Parse("#f6ffe3"),
                LineMode = LineMode.Straight,
                LineSize = 4,
                PointSize = 10,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal
            };

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

                AIData user = new AIData {
                    EdadRango = GetAgeRange(tests.Last().EdadUser),
                    Genero = tests.Last().GeneroUser
                };
                float prediction = AIService.GetAIPredictedAvgResult(new MLContext(), targetPath, user);
                LblPrediction.Text = AIService.InterpretPrediction(prediction, entriesList.Last().Value);
            } else {
                LblPrediction.Text = "[Placeholder donde iría el análisis de IA de la evolución del usuario]";
            }
        } else {
            VslNoResultsGraph.IsVisible = true;
            LblPrediction.IsVisible = false;
            BrdChartView.IsVisible = false;
            await VslNoResultsGraph.ScaleTo(1.05, 300, Easing.BounceOut);
            await VslNoResultsGraph.ScaleTo(1, 300, Easing.BounceIn);
        }
    }

    /// <summary>
    /// Calcula y devuelve el nivel más apropiado según los factores obtenidos en un test. Si hay mayoría de un factor, se devuelve el nivel asociado a dicho factor. Si todos los factores son distintos, se devuelve el nivel del factor cuya puntuación sea más cercana a la media.
    /// </summary>
    /// <param name="factores">La lista de factores calculados del test actual.</param>
    /// <returns>El nivel calculado.</returns>
    private string GetCategoria(List<Factor> factores)
    {
        if (factores.Count == 1) {
            return factores[0].Nivel;
        }

        var grupos = factores
        .GroupBy(obj => obj.Nivel)
        .Select(g => new { Nivel = g.Key, Frecuencia = g.Count() })
        .ToList();

        int maxFrecuencia = grupos.Max(g => g.Frecuencia);
        bool todosIguales = grupos.All(g => g.Frecuencia == maxFrecuencia);

        if (!todosIguales) {
            string categoriaMasComun = grupos
                .OrderByDescending(g => g.Frecuencia)
                .First()
                .Nivel;

            return categoriaMasComun;
        }
        else {
            double avg = factores.Average(obj => obj.Puntuacion);
            return factores
                .OrderBy(obj => Math.Abs(obj.Puntuacion - avg))
                .First().Nivel;
        }
    }

    /// <summary>
    /// Calcula los factores de un test realizado.
    /// </summary>
    /// <param name="preguntas">Las respuestas del test.</param>
    /// <param name="tipoTest">El test realizado.</param>
    /// <returns>La lista de factores calculados.</returns>
    private List<Factor> GetFactores(List<Answer> preguntas, Test tipoTest)
    {
        List<Factor> factores = new();
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