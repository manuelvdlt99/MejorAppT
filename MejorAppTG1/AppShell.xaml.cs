using CommunityToolkit.Maui.Views;
using MejorAppTG1;
using MejorAppTG1.Resources.Localization;

namespace MejorAppTG1
{
    using MejorAppTG1.Views;
    using Microsoft.Maui.Controls;

    public partial class AppShell : Shell
    {
        #region Constructores
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));


            // Verificar si los controles son accesibles
            var lblTitulo = this.FindByName<Label>("LblTituloApp");
            var btnAbout = this.FindByName<ImageButton>("BtnAbout");
            var btnHelp = this.FindByName<ImageButton>("BtnHelp");

            if (lblTitulo != null && btnAbout != null && btnHelp != null) {
                // Configurar el orden semántico
                if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                    this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp };
                }
            }

            /*lo que deberia ir de normal
            if (DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp };
            }
            */
        }
        #endregion

        #region Eventos
        private async void BtnHelp_Clicked(object sender, EventArgs e)
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

        private async void BtnAbout_Clicked(object sender, EventArgs e)
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
