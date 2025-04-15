using MejorAppTG1.Models;
using System.Collections.ObjectModel;

namespace MejorAppTG1;

public partial class AdvicesPage : ContentPage
{
    #region Variables
    private AdviceCategory _category;
    #endregion

    #region Constructores
    public AdvicesPage(AdviceCategory category)
	{
		InitializeComponent();
        _category = category;

    }
    #endregion

    #region Eventos
    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        try {
            StkLoading.IsVisible = true;
            GrdData.IsVisible = false;
            await Task.Delay(800);  // Forzar a que se espere un poco antes de cargar, porque de lo contrario el indicador de carga ni siquiera aparece

            BindingContext = this;

            await Task.WhenAll(
                Task.Run(CargarConsejosAsync)
            );
        }
        finally {
            SemanticScreenReader.Announce(LblTitle.Text);
            StkLoading.IsVisible = false;
            GrdData.IsVisible = true;
        }
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
    #endregion

    #region Métodos
    private async Task CargarConsejosAsync()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblTitle.Text = _category.Name;
            ClvAdvices.ItemsSource = _category.Advices;
        });
    }
    #endregion
}