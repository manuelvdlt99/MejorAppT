using CommunityToolkit.Maui.Views;
using MejorAppTG1.Views;
using Microsoft.Maui.Controls;

namespace MejorAppTG1
{
    public partial class AppShell : Shell
    {
        #region Constructores        
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AppShell"/>.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            // Verificar si los controles son accesibles
            var lblTitulo = this.FindByName<Label>("LblTituloApp");
            var btnAbout = this.FindByName<ImageButton>("BtnAbout");
            var btnHelp = this.FindByName<ImageButton>("BtnHelp");

            if (lblTitulo != null && btnAbout != null && btnHelp != null && DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                // Configurar el orden semántico
                this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp };
            }
        }
        #endregion

        #region Eventos        
        /// <summary>
        /// Maneja el evento de pulsación del botón de Ayuda. Abre un popup modal con ayuda para el usuario.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private static async void BtnHelp_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var popup = new HelpPopup();
                await Shell.Current.ShowPopupAsync(popup);
            } finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Acerca de. Abre un popup modal con información sobre la aplicación.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private static async void BtnAbout_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                var popup = new AboutPopup();
                await Shell.Current.ShowPopupAsync(popup);
            } finally {
                App.ButtonPressed = false;
            }
        }

        /// <summary>
        /// Maneja el evento de pulsación del botón de Ajustes. Abre una nueva pantalla con el menú de ajustes.
        /// </summary>
        /// <param name="sender">El botón pulsado.</param>
        /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
        private async void BtnSettings_Clicked(object sender, EventArgs e)
        {
            if (App.ButtonPressed) return;
            App.ButtonPressed = true;
            try {
                await Navigation.PushAsync(new SettingsPage(), true);
            }
            finally {
                App.ButtonPressed = false;
            }
        }
        #endregion
    }
}
