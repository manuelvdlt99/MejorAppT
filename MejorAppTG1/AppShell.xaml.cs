using CommunityToolkit.Maui.Views;
using MejorAppTG1.Resources.Localization;
using MejorAppTG1.Views;
using Plugin.LocalNotification;

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
            var btnSettings = this.FindByName<ImageButton>("BtnSettings");

            if (lblTitulo != null && btnAbout != null && btnHelp != null && btnSettings != null && DeviceInfo.Current.Platform != DevicePlatform.iOS) {
                // Configurar el orden semántico
                this.SemanticOrderView.ViewOrder = new List<View> { LblTituloApp, BtnAbout, BtnHelp, BtnSettings };
            }

            if (DeviceInfo.Current.Platform != DevicePlatform.WinUI) {
                ScheduleNotification();
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

        #region Métodos
        /// <summary>
        /// Programa una notificación cada 60 días para recordar al usuario que vuelva a usar la app.
        /// </summary>
        private static void ScheduleNotification()
        {
            if (Preferences.Get(App.USER_NOTIFICATIONS, true)) {
                // Comprobar si hay alguna notificación pendiente para cancelarla y crear la nueva
                LocalNotificationCenter.Current.Cancel(App.NOTIFICATION_ID_OTHERS);

                var notification = new NotificationRequest {
                    NotificationId = App.NOTIFICATION_ID_OTHERS,
                    Title = Strings.str_Notifications_Title_ComeBack,
                    Subtitle = Strings.str_Notifications_Title_Monitoring,
                    Description = Strings.str_Notifications_Message_ComeBack,
                    CategoryType = NotificationCategoryType.Reminder,
                    BadgeNumber = 42,
                    Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions {
                        IconSmallName =
                        {
                            ResourceName = "noti_icon"
                        }
                    },
                    iOS = new Plugin.LocalNotification.iOSOption.iOSOptions {
                        PlayForegroundSound = true
                    },
                    Schedule = new NotificationRequestSchedule {
                        NotifyTime = DateTime.Now.AddDays(60),
                        NotifyRepeatInterval = TimeSpan.FromDays(60)
                    }
                };

                LocalNotificationCenter.Current.Show(notification);
            }
        }
        #endregion
    }
}
