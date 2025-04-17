#pragma warning disable CS8601

using CommunityToolkit.Maui.Views;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;

namespace MejorAppTG1;

using Maui.FreakyControls;
using Microsoft.Maui.Controls;

public partial class ResultsPage : ContentPage
{
    #region Variables
    private List<Advice> _consejosDisponibles = new();
    public ObservableCollection<AdviceCategory> Categories { get; set; } = new();

    private Factor _factor1;
    private Factor _factor2;
    private Factor _factor3;
    private string _tipoTest;
    private int _puntuacionTotal = 0;
    private bool _loaded = false;
    #endregion

    #region Constructores
    public ResultsPage(Factor factor1, Factor factor2, Factor factor3, string tipoTest)
    {
        _tipoTest = tipoTest;
        _factor1 = factor1;
        _factor2 = factor2;
        _factor3 = factor3;

        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
    }
    #endregion

    #region Eventos
    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        try {
            StkLoading.IsVisible = true;
            GrdData.IsVisible = false;
            await Task.Delay(800);  // Forzar a que se espere un poco antes de cargar, porque de lo contrario el indicador de carga ni siquiera aparece

            BindingContext = this;

            await Task.WhenAll(
                Task.Run(CalcularPuntuacionTotal),
                Task.Run(CargarCategoriasAsync)
            );
        }
        finally {
            SemanticScreenReader.Announce(LblIntro.Text + Strings.str_SemanticProperties_ResultsPage_LblIntro);
            StkLoading.IsVisible = false;
            GrdData.IsVisible = true;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        /*var label = (Label)sender;
        var formattedString = label.FormattedText;
        string url = formattedString.Spans[1].Text;

        if (!string.IsNullOrWhiteSpace(url)) {
            try {
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex) {
                Console.WriteLine(string.Format(Strings.str_ResultsPage_LinkError, ex.Message));
                await DisplayAlert(Strings.str_ResultHistoryPage_ImgProfile_Error, Strings.str_ResultsPage_LinkError_Msg, Strings.str_ResultHistoryPage_BtnCheck_OK);
            }
        }*/
    }

    private async void BtnFinish_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            await Navigation.PopAsync(true);
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    private void FreakyButton_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            if (sender is FreakyButton button && button.BindingContext is AdviceCategory selectedCategory) {
                Navigation.PushAsync(new AdvicesPage(selectedCategory), true);
            }
        } finally {
            App.ButtonPressed = false;
        }
    }
    #endregion

    #region Métodos
    // Para saber la puntuación total del test
    private void CalcularPuntuacionTotal()
    {
        if (_tipoTest == App.TCA_TEST_KEY) {
            _puntuacionTotal = _factor1.Puntuacion;
        } else {
            _puntuacionTotal = _factor1.Puntuacion + _factor2.Puntuacion + _factor3.Puntuacion;
        }

        
    }

    // Carga los consejos en la pantalla
    private async Task CargarCategoriasAsync()
    {
        if (_loaded) return;

        if (_tipoTest == App.FULL_TEST_KEY || _tipoTest == App.QUICK_TEST_KEY) {
            using var stream = await FileSystem.OpenAppPackageFileAsync("ConsejosParaAnsiedad.json");
            using var reader = new StreamReader(stream);
            var contenido = await reader.ReadToEndAsync();
            _consejosDisponibles = JsonSerializer.Deserialize<List<Advice>>(contenido);
        }
        switch (_tipoTest)
        {
            case App.FULL_TEST_KEY:
                AnsiedadCompletoConsejos(_consejosDisponibles);
                break;
            case App.QUICK_TEST_KEY:
                AnsiedadRapidoConsejos(_consejosDisponibles);
                break;
            case App.TCA_TEST_KEY:
                TCAConsejos();
                break;
        }

        _loaded = true;
    }

    // Método para mostrar los consejos de Ansiedad Rápido
    private void AnsiedadRapidoConsejos(List<Advice> consejosDisponibles)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            if (consejosDisponibles.Count > 0) {
                // Punto A
                if (_puntuacionTotal >= 32 && _puntuacionTotal <= 49) {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                }
                // Punto B
                else if (_puntuacionTotal >= 50 && _puntuacionTotal <= 80) {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                }
                // Opción 3
                else // esta sería para todo bien
                {
                    LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low;
                }
                // CASO 1: Para TODO BIEN
                if (_factor1.Puntuacion <= 14 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    Categories.Add(category);
                }
                // CASO 2: PARA SOLO EL FACTOR COGNITIVO (FACTOR 1)
                else if (_factor1.Puntuacion >= 15 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                }
                // CASO 3: PARA SOLO EL FACTOR FISIOLÓGICO (FACTOR 3)
                else if (_factor1.Puntuacion >= 15 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }
                // CASO 4: PARA EL FACTOR DE EVITACIÓN (FACTOR 2)
                else if (_factor1.Puntuacion <= 14 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }
                // CASO 5: PARA LOS FACTORES COGNITIVO, FISIOLÓGICO Y EVITACIÓN (FACTOR 1, 2 Y 3)
                else if (_factor1.Puntuacion >= 15 && _factor3.Puntuacion >= 15 && _factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[11].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[11].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[11].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }
                // CASO 6: PARA FACTOR COGNITIVO Y FISIOLÓGICO (FACTORES 1 y 2)
                else if (_factor1.Puntuacion >= 15 && _factor3.Puntuacion >= 15 && _factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }
                // CASO 7: PARA FACTORES COGNITIVO Y EVITACION (FACTORES 1 y 3)
                else if (_factor1.Puntuacion >= 15 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }
                // CASO 8: PARA FACTORES FISIOLÓGICO Y EVITACIÓN (FACTORES 2 y 3)
                else if (_factor1.Puntuacion <= 14 && _factor3.Puntuacion >= 15 && _factor2.Puntuacion >= 2) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                    Categories.Add(category);

                    category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                    Categories.Add(category);
                }

                // Estos se muestran siempre excepto para TODO BIEN
                /*if (_factor1.Puntuacion > 14 && _factor3.Puntuacion > 14 && _factor2.Puntuacion > 1)
                {
                    category.Advices.Add(new Consejo { Titulo = Strings.str_ResultsPage_Category_BadSituation, Contenido = null, Imagen = null });
                    category.Advices.Add(new Consejo { Titulo = Strings.ResourceManager.GetString(_consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(_consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture) });
                    category.Advices.Add(new Consejo { Titulo = Strings.ResourceManager.GetString(_consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(_consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture) });
                }*/
            }
        });
    }

    // Método para mostratr los consejos de Ansiedad Completo
    private void AnsiedadCompletoConsejos(List<Advice> consejosDisponibles)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low_2;
                AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[0].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[0].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[0].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[3].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[3].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[3].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "seemore_icon.png", Name = Strings.str_ResultsPage_Category_SeeMore, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[17].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[17].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[17].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[18].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[18].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[18].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[19].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[19].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[19].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[4].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[4].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[4].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Medium;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[1].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[1].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[1].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[2].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[2].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[2].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[10].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[10].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[10].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_HIGH) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_High;
                AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[5].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[5].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[5].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[6].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[6].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[6].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[7].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[7].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[7].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[8].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[8].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[8].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[9].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[9].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[9].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "prevent_icon.png", Name = Strings.str_ResultsPage_Category_Prevent, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[21].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[21].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[21].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[22].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[22].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[22].Imagen });
                Categories.Add(category);

                category = new AdviceCategory { ImagePath = "burdened_icon.png", Name = Strings.str_ResultsPage_Category_BadSituation, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[15].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[15].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[15].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[16].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[16].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[16].Imagen });
                Categories.Add(category);
            }
        });
    }

    // Método para mostrar los consejos de TCA
    private void TCAConsejos()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblIntro.Text = Strings.str_ResultsPage_LblIntro_EatingTest;
            AdviceCategory category = new AdviceCategory { ImagePath = "advice_icon.png", Name = Strings.str_ResultsPage_Category_GeneralAdvice, Advices = new List<Advice>() };
            if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice1_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice1_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_HIGH)) {
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice2_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice2_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_MEDIUM_HIGH)) {
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice3_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice3_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
            }
            else if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW_MEDIUM)) {
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString("str_EatingAdvice4_Title", CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString("str_EatingAdvice4_Content", CultureInfo.CurrentUICulture), Imagen = "eatingadvice.png" });
                
            }
            Categories.Add(category);
        });
    }
    #endregion
}