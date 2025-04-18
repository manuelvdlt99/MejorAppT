using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Utils;
using Microcharts;
using SkiaSharp;
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

    public static int ResultIndex { get => _resultIndex; set => _resultIndex = value; }
    public static int CurrentPage { get => _currentPage; set => _currentPage = value; }
    #endregion

    #region Constructores
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

    private async void BtnLastResult_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
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

    private async void BtnPreviousFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            _resultIndex -= 5;
            _currentPage--;
            await LoadNextFiveTests();
            await SlideCollectionView("Left");
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private async void BtnNextFive_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        FrmTabResults.Stroke = (Color)Application.Current.Resources["HeaderColor1"];
        FrmTabResults.BackgroundColor = (Color)Application.Current.Resources["SecondaryColor2"];
        FrmTabAnalysis.Stroke = (Color)Application.Current.Resources["ButtonColor2"];
        FrmTabAnalysis.BackgroundColor = (Color)Application.Current.Resources["ButtonColor4"];
        VslResults.IsVisible = true;
        VslAnalysis.IsVisible = false;
    }

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

    private void BtnCreateGraph_Clicked(object sender, EventArgs e)
    {
        CreateGraph();
    }
    #endregion

    #region Métodos
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

    internal void UpdateUserImage()
    {
        string image = App.CurrentUser.Imagen;
        if (image != null) {
            MainThread.BeginInvokeOnMainThread(() => {
                ImgProfile.Source = ImageSource.FromFile(image);
            });
        }
    }

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

    private async void ConsultarRegistro(object sender)
    {
        Test selectedTest = (Test)((Frame)sender).BindingContext;
        CalculateFactorsAndShowResults(selectedTest);
    }

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

    private async Task SlideCollectionView(string direction)
    {
        var offset = direction == "Left" ? -100 : 100;
        CtvResults.TranslationX = offset;
        await CtvResults.TranslateTo(0, 0, 300, Easing.CubicOut);
    }

    private async Task CreateGraph()
    {
        string selectedTest = PickTipos.SelectedItem.ToString();
        string selectedKey = _tests.FirstOrDefault(x => x.Key == selectedTest).Value;
        var tests = await App.Database.GetFinishedTestsByUserFilteredAsync(App.CurrentUser.IdUsuario, selectedKey);

        var entriesList = new List<ChartEntry>();

        if (tests.Count > 0) {
            VslNoResultsGraph.IsVisible = false;
            BrdChartView.IsVisible = true;

            foreach (var test in tests) {
                var answers = await App.Database.GetAnswersByTestIdAsync(test.IdTest);
                int totalPoints = answers.Sum(a => a.ValorRespuesta);
                var entry = new ChartEntry(totalPoints) {
                    Label = test.Fecha.ToString("dd/MM/yy"),
                    ValueLabel = totalPoints.ToString(),
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
                LabelTextSize = 40,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Vertical
            };
        } else {
            VslNoResultsGraph.IsVisible = true;
            BrdChartView.IsVisible = false;
            await VslNoResultsGraph.ScaleTo(1.05, 300, Easing.BounceOut);
            await VslNoResultsGraph.ScaleTo(1, 300, Easing.BounceIn);
        }
    }
    #endregion
}