using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Database;
using MejorAppTG1.Data;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Globalization;


namespace MejorAppTG1
{
    public partial class App : Application
    {
        public const string USER_ID_KEY = "user_id";
        public static bool ButtonPressed = false;
        public const string SYNC_MODE_OPEN = "open";
        public const string SYNC_MODE_CLOSE = "close";

        private static MejorAppTDatabase _database;
        private static FirebaseService _firebase;
        private bool _isSyncing = false;
        public static User CurrentUser { get; set; }
        public static MejorAppTDatabase Database
        {
            get {
                if (_database == null) {
                    _database = new MejorAppTDatabase(Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), "MejorAppT.db3"));
                }
                return _database;
            }
        }

        public static FirebaseService Firebase
        {
            get {
                if (_firebase == null) {
                    _firebase = new FirebaseService("https://mejorappt-g1-default-rtdb.europe-west1.firebasedatabase.app");
                }
                return _firebase;
            }
        }

        public App()
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            this.RequestedThemeChanged += (s, e) => { Application.Current.UserAppTheme = AppTheme.Light; };
            var culture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            InitializeComponent();
            if (Preferences.ContainsKey(USER_ID_KEY)){
                GetCurrentUser();
                MainPage = new AppShell();
            } else {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.MinimumHeight = 700;
            window.MinimumWidth = 700;
            return window;
        }

        public async void GetCurrentUser()
        {
            App.CurrentUser = await App.Database.GetUserByIdAsync(Preferences.Get(USER_ID_KEY, 0));
        }

        public static async void AnimateFrameInOut(object sender)
        {
            await ((Frame)sender).ScaleTo(0.95, 100, Easing.CubicInOut);    // Reducir un poco el tamaño
            await ((Frame)sender).ScaleTo(1, 100, Easing.CubicInOut);   // Volver al tamaño original
        }

        protected override void OnStart()
        {
            base.OnStart();
            var cancellationTokenSource = new CancellationTokenSource();
            StartSyncTask(cancellationTokenSource.Token, SYNC_MODE_OPEN);
        }

        protected override void OnSleep()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            StartSyncTask(cancellationTokenSource.Token, SYNC_MODE_CLOSE);
            base.OnSleep();
        }

        private async Task StartSyncTask(CancellationToken cancellationToken, string mode)
        {
            if (_isSyncing) return;

            _isSyncing = true;
            try {
                if (mode == SYNC_MODE_OPEN) {
                    await WaitForInternetAndSync(cancellationToken);
                } else {
                    await CheckInternetAndSync(cancellationToken);
                }
            }
            finally {
                _isSyncing = false;
            }
        }

        public async Task WaitForInternetAndSync(CancellationToken cancellationToken)
        {
            bool noInternet = true;
            while (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) {  // Bucle "infinito" hasta que no detecte conexión
                if (noInternet) {
                    var toast = Toast.Make(Strings.str_App_NoConnection, ToastDuration.Short, 14);
                    await toast.Show(new CancellationTokenSource().Token);
                    noInternet = false;
                }
                try {
                    await Task.Delay(5000, cancellationToken); // Espera 5 segundos
                }
                catch (TaskCanceledException) {
                    return;
                }
            }

            if (!noInternet) {
                var toast = Toast.Make(Strings.str_App_ConnectionBack, ToastDuration.Short, 14);
                await toast.Show(new CancellationTokenSource().Token);
            }

            var syncService = new SyncService(App.Firebase, App.Database);
            await syncService.SyncTests();
            Console.WriteLine(Strings.str_App_DataSynced);
            await App.Database.DeleteSyncedTestsWithDeletedUserInLocal();
        }

        public async Task CheckInternetAndSync(CancellationToken cancellationToken)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet) {
                try {
                    var syncService = new SyncService(Firebase, Database);
                    await syncService.SyncTests();
                    Console.WriteLine(Strings.str_App_DataSynced);
                }
                catch (TaskCanceledException) {
                    return;
                }
            }
            await App.Database.DeleteSyncedTestsWithDeletedUserInLocal();
        }

    }
}
