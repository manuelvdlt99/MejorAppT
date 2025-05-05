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
    private List<Advice> _consejosDisponibles = [];
    /// <summary>
    /// Permite recuperar o modificar las categorías de consejos que se mostrarán en la pantalla de recomendaciones.
    /// </summary>
    /// <value>
    /// Las categorías.
    /// </value>
    public ObservableCollection<AdviceCategory> Categories { get; set; } = [];

    private Factor _factor1;
    private Factor _factor2;
    private Factor _factor3;
    private string _tipoTest;
    private int _puntuacionTotal = 0;
    private bool _loaded = false;
    #endregion

    #region Constructores    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ResultsPage"/>.
    /// </summary>
    /// <param name="factor1">El factor 1 del test realizado.</param>
    /// <param name="factor2">El factor 2 del test realizado.</param>
    /// <param name="factor3">El factor 3 del test realizado.</param>
    /// <param name="tipoTest">El tipo de test realizado.</param>
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
    /// <summary>
    /// Maneja el evento de aparición de la pantalla. Muestra una pantalla de carga que dura mientras un hilo secundario carga las categorías del test realizado.
    /// </summary>
    /// <param name="sender">La página que aparece.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el evento de pulsación del botón de Volver. Cierra la pantalla actual.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private async void BtnFinish_Clicked(object sender, EventArgs e)
    {
        if (App.ButtonPressed) return;
        App.ButtonPressed = true;
        try {
            App.AnimateButtonInOut(sender);
            await Navigation.PopAsync(true);
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación sobre una categoría. Abre la pantalla de consejos de la categoría seleccionada.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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
    /// <summary>
    /// Calcula la puntuación total del test realizado.
    /// </summary>
    private void CalcularPuntuacionTotal()
    {
        if (_tipoTest == App.TCA_TEST_KEY) {
            _puntuacionTotal = _factor1.Puntuacion;
        } else {
            _puntuacionTotal = _factor1.Puntuacion + _factor2.Puntuacion + _factor3.Puntuacion;
        }
    }

    /// <summary>
    /// Recupera los consejos del JSON y los procesa dependiendo del tipo de test realizado.
    /// </summary>
    private async Task CargarCategoriasAsync()
    {
        if (_loaded) return;

        if (_tipoTest == App.FULL_TEST_KEY || _tipoTest == App.QUICK_TEST_KEY) {
            using var stream = await FileSystem.OpenAppPackageFileAsync(App.JSON_ADVICES_FULL);
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

    /// <summary>
    /// Calcula y crea las categorías con los consejos asociados según los resultados del test de ansiedad rápido realizado.
    /// </summary>
    /// <param name="consejosDisponibles">La lista con todos los consejos disponibles.</param>
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
                #region CASO 1: Para TODO BIEN
                if (_factor1.Puntuacion <= 14 && _factor3.Puntuacion <= 14 && _factor2.Puntuacion <= 1) {
                    AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                    category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                    Categories.Add(category);
                }
                #endregion
                #region CASO 2: PARA SOLO EL FACTOR COGNITIVO (FACTOR 1)
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
                #endregion
                #region CASO 3: PARA SOLO EL FACTOR FISIOLÓGICO (FACTOR 3)
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
                #endregion
                #region CASO 4: PARA EL FACTOR DE EVITACIÓN (FACTOR 2)
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
                #endregion
                #region CASO 5: PARA LOS FACTORES COGNITIVO, FISIOLÓGICO Y EVITACIÓN (FACTOR 1, 2 Y 3)
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
                #endregion
                #region CASO 6: PARA FACTOR COGNITIVO Y FISIOLÓGICO (FACTORES 1 y 2)
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
                #endregion
                #region CASO 7: PARA FACTORES COGNITIVO Y EVITACION (FACTORES 1 y 3)
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
                #endregion
                #region CASO 8: PARA FACTORES FISIOLÓGICO Y EVITACIÓN (FACTORES 2 y 3)
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
                #endregion
            }
        });
    }

    /// <summary>
    /// Calcula y crea las categorías con los consejos asociados según los resultados del test de ansiedad completo realizado.
    /// </summary>
    /// <param name="consejosDisponibles">La lista con todos los consejos disponibles.</param>
    private void AnsiedadCompletoConsejos(List<Advice> consejosDisponibles)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            #region BAJO-BAJO-BAJO
            if (_factor1.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor2.Nivel.Equals(App.FACTORS_LEVEL_LOW) && _factor3.Nivel.Equals(App.FACTORS_LEVEL_LOW)) {
                LblIntro.Text = Strings.str_ResultsPage_LblIntro_Low_2;
                AdviceCategory category = new AdviceCategory { ImagePath = "thumbsup_icon.png", Name = Strings.str_ResultsPage_Category_KeepItUp, Advices = new List<Advice>() };
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[12].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[12].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[12].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[13].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[13].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[13].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[14].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[14].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[14].Imagen });
                category.Advices.Add(new Advice { Titulo = Strings.ResourceManager.GetString(consejosDisponibles[20].Titulo, CultureInfo.CurrentUICulture), Contenido = Strings.ResourceManager.GetString(consejosDisponibles[20].Contenido, CultureInfo.CurrentUICulture), Imagen = consejosDisponibles[20].Imagen });
                Categories.Add(category);
            }
            #endregion
            #region BAJO-BAJO-MEDIO
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
            #endregion
            #region BAJO-BAJO-ALTO
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
            #endregion
            #region BAJO-MEDIO-BAJO
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
            #endregion
            #region BAJO-MEDIO-MEDIO
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
            #endregion
            #region BAJO-MEDIO-ALTO
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
            #endregion
            #region BAJO-ALTO-BAJO
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
            #endregion
            #region BAJO-ALTO-MEDIO
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
            #endregion
            #region BAJO-ALTO-ALTO
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
            #endregion
            #region MEDIO-BAJO-BAJO
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
            #endregion
            #region MEDIO-BAJO-MEDIO
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
            #endregion
            #region MEDIO-BAJO-ALTO
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
            #endregion
            #region MEDIO-MEDIO-BAJO
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
            #endregion
            #region MEDIO-MEDIO-MEDIO
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
            #endregion
            #region MEDIO-MEDIO-ALTO
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
            #endregion
            #region MEDIO-ALTO-BAJO
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
            #endregion
            #region MEDIO-ALTO-MEDIO
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
            #endregion
            #region MEDIO-ALTO-ALTO
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
            #endregion
            #region ALTO-BAJO-BAJO
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
            #endregion
            #region ALTO-BAJO-MEDIO
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
            #endregion
            #region ALTO-BAJO-ALTO
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
            #endregion
            #region ALTO-MEDIO-BAJO
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
            #endregion
            #region ALTO-MEDIO-MEDIO
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
            #endregion
            #region ALTO-MEDIO-ALTO
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
            #endregion
            #region ALTO-ALTO-BAJO
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
            #endregion
            #region ALTO-ALTO-MEDIO
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
            #endregion
            #region ALTO-ALTO-ALTO
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
            #endregion
        });
    }

    /// <summary>
    /// Calcula y crea las categorías con los consejos asociados según los resultados del test de TCA realizado.
    /// </summary>
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