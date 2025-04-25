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
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AdvicesPage"/>.
    /// </summary>
    /// <param name="category">La categoría de consejos seleccionada.</param>
    public AdvicesPage(AdviceCategory category)
	{
		InitializeComponent();
        _category = category;

    }
    #endregion

    #region Eventos    
    /// <summary>
    /// Maneja el evento de aparición de la pantalla. Muestra una pantalla de carga que dura mientras un hilo secundario carga los consejos de la categoría seleccionada.
    /// </summary>
    /// <param name="sender">La página que aparece.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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
            await Navigation.PopAsync(true);
        }
        finally {
            App.ButtonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de pulsación de la flecha izquierda. Mueve el carrusel un paso a la izquierda.
    /// </summary>
    /// <param name="sender">La flecha pulsada.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnLeft_Clicked(object sender, EventArgs e)
    {
        ClvAdvices_ItemSwiped(ClvAdvices, new ItemSwipedEventArgs(PanCardView.Enums.ItemSwipeDirection.Right, ClvAdvices.SelectedIndex, null));
        ClvAdvices.SelectedIndex -= 1;
    }

    /// <summary>
    /// Maneja el evento de pulsación de la flecha derecha. Mueve el carrusel un paso a la derecha.
    /// </summary>
    /// <param name="sender">La flecha pulsada.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnRight_Clicked(object sender, EventArgs e)
    {
        ClvAdvices_ItemSwiped(ClvAdvices, new ItemSwipedEventArgs(PanCardView.Enums.ItemSwipeDirection.Left, ClvAdvices.SelectedIndex, null));
        ClvAdvices.SelectedIndex += 1;
    }

    /// <summary>
    /// Maneja el evento de cambiar de elemento en el carrusel deslizando con el dedo a la derecha o a la izquierda. Actualiza las flechas de desplazamiento manual según la posición nueva en la que se encuentre el carrusel.
    /// </summary>
    /// <param name="view">El carrusel accionado.</param>
    /// <param name="args">La instancia <see cref="ItemSwipedEventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el evento de pulsación de un botón de Ver en YouTube. Abre el vídeo asociado al botón pulsado en la aplicación predeterminada del usuario.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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

    /// <summary>
    /// Maneja el cambio de contexto de cada botón de Ver en YouTube. Muestra cada vídeo cargado.
    /// </summary>
    /// <param name="sender">El botón detectado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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
    /// <summary>
    /// Muestra los consejos de la categoría actual en la pantalla.
    /// </summary>
    private async Task CargarConsejosAsync()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            LblTitle.Text = _category.Name;
            ClvAdvices.ItemsSource = _category.Advices;
        });
    }

    /// <summary>
    /// Devuelve el título de un vídeo de YouTube por su URL.
    /// </summary>
    /// <param name="url">La URL del vídeo.</param>
    /// <returns></returns>
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