using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using PanCardView.EventArgs;
using System.Text.RegularExpressions;

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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is string video) {
            try {
                App.AnimateFrameInOut(Frame);
                Launcher.OpenAsync(new Uri(video));
            }
            catch {
                DisplayAlert(Strings.str_AdvicesPage_Alert_NoOpenVideo_Title, Strings.str_AdvicesPage_Alert_NoOpenVideo_Desc, Strings.str_ResultHistoryPage_BtnCheck_OK);
            }
        }
    }

    private async void Frame_BindingContextChanged(object sender, EventArgs e)
    {
        if (sender is not Frame frame || frame.BindingContext is not string url)
            return;

        string title = await GetYoutubeTitleAsync(url);

        if (frame.Content is Grid grid) {
            var titleLabel = grid.Children.OfType<Label>().FirstOrDefault();
            if (titleLabel != null) {
                titleLabel.Text = title;
            }
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

    private async Task<string> GetYoutubeTitleAsync(string url)
    {
        try {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/115.0.0.0 Safari/537.36");

            if (url.Contains("embed")) {
                var videoId = url.Split("/").Last();
                url = $"https://www.youtube.com/watch?v={videoId}";
            }

            var html = await httpClient.GetStringAsync(url);

            var match = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
            if (match.Success) {
                return match.Groups[1].Value.Replace("- YouTube", "").Replace("&#39;", "'").Trim();
            }
        }
        catch {
            return string.Empty;
        }

        return string.Empty;
    }
    #endregion
}