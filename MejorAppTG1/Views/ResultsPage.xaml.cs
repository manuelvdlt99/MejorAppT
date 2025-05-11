using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Collections.ObjectModel;
using Maui.FreakyControls;
using MejorAppTG1.Utils;

namespace MejorAppTG1;

public partial class ResultsPage : ContentPage
{
    #region Variables
    /// <summary>
    /// Permite recuperar o modificar las categorías de consejos que se mostrarán en la pantalla de recomendaciones.
    /// </summary>
    /// <value>
    /// Las categorías.
    /// </value>
    public List<AdviceCategory> Categories { get; set; } = [];
    private Factor? _factor1;
    private Factor? _factor2;
    private Factor? _factor3;
    private string _tipoTest;
    private int _puntuacionTotal = 0;
    #endregion

    #region Constructores    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ResultsPage"/>.
    /// </summary>
    /// <param name="factor1">El factor 1 del test realizado.</param>
    /// <param name="factor2">El factor 2 del test realizado.</param>
    /// <param name="factor3">El factor 3 del test realizado.</param>
    /// <param name="tipoTest">El tipo de test realizado.</param>
    public ResultsPage(Factor? factor1, Factor? factor2, Factor? factor3, string tipoTest)
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

            _puntuacionTotal = await Task.Run(() => AdviceManager.CalcularPuntuacionTotal(_factor1, _factor2, _factor3, _tipoTest));
            Categories = await Task.Run(() => AdviceManager.CargarCategoriasAsync(_tipoTest, _puntuacionTotal, _factor1, _factor2, _factor3));
            string introText = await Task.Run(() => AdviceManager.CargarLblIntro(_tipoTest, _puntuacionTotal, _factor1, _factor2, _factor3));
            await MainThread.InvokeOnMainThreadAsync(() => {
                LblIntro.Text = introText;
                ClvCategories.ItemsSource = Categories;
            });
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
    
    #endregion
}