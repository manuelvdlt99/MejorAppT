using MejorAppTG1.Models;
using PanCardView.EventArgs;

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
            if (ClvAdvices.Count <= 1) {
                BtnRight.IsVisible = false;
                BtnRight.IsEnabled = false;
                BtnLeft.IsVisible = false;
                BtnLeft.IsEnabled = false;
            }
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

    private void BtnLeft_Clicked(object sender, EventArgs e)
    {
        ClvAdvices_ItemSwiped(ClvAdvices, new ItemSwipedEventArgs(PanCardView.Enums.ItemSwipeDirection.Right, ClvAdvices.SelectedIndex, null));
        ClvAdvices.SelectedIndex -= 1;
    }

    private void BtnRight_Clicked(object sender, EventArgs e)
    {
        ClvAdvices_ItemSwiped(ClvAdvices, new ItemSwipedEventArgs(PanCardView.Enums.ItemSwipeDirection.Left, ClvAdvices.SelectedIndex, null));
        ClvAdvices.SelectedIndex += 1;
    }

    private void ClvAdvices_ItemSwiped(PanCardView.CardsView view, ItemSwipedEventArgs args)
    {
        if (args.Index + 1 == view.ItemsCount - 1 && args.Direction == PanCardView.Enums.ItemSwipeDirection.Left) {
            BtnRight.IsVisible = false;
            BtnRight.IsEnabled = false;
            BtnLeft.IsVisible = true;
            BtnLeft.IsEnabled = true;
        } else if (args.Index - 1 == 0 && args.Direction == PanCardView.Enums.ItemSwipeDirection.Right) {
            BtnLeft.IsVisible = false;
            BtnLeft.IsEnabled = false;
            BtnRight.IsVisible = true;
            BtnRight.IsEnabled = true;
        } else {
            BtnLeft.IsVisible = true;
            BtnLeft.IsEnabled = true;
            BtnRight.IsVisible = true;
            BtnRight.IsEnabled = true;
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