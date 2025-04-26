using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MejorAppTG1.Data;
using MejorAppTG1.Models;
using MejorAppTG1.Resources.Localization;
using System.Globalization;


namespace MejorAppTG1
{
    /// <summary>
    /// Clase general que inicializa la aplicaci�n y contiene todos los m�todos, objetos y constantes utilizadas en todas (o la mayor�a) las partes de la misma.
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.Application" />
    public partial class App : Application
    {
        /// <summary>
        /// La clave en las preferencias del usuario que ha iniciado sesi�n.
        /// </summary>
        public const string USER_ID_KEY = "user_id";
        /// <summary>
        /// La clave en las preferencias del idioma seleccionado.
        /// </summary>
        public const string USER_LANGUAGE = "language";
        /// <summary>
        /// Modo de sincronizaci�n para cuando se inicia la aplicaci�n.
        /// </summary>
        public const string SYNC_MODE_OPEN = "open";
        /// <summary>
        /// Modo de sincronizaci�n para cuando se cierra la aplicaci�n.
        /// </summary>
        public const string SYNC_MODE_CLOSE = "close";
        /// <summary>
        /// Clave en recursos del test de TCA.
        /// </summary>
        public const string TCA_TEST_KEY = "str_EatingTest";
        /// <summary>
        /// Clave en recursos del test de ansiedad r�pido.
        /// </summary>
        public const string QUICK_TEST_KEY = "str_QuickTest";
        /// <summary>
        /// Clave en recursos del test de ansiedad completo.
        /// </summary>
        public const string FULL_TEST_KEY = "str_FullTest";
        /// <summary>
        /// Identificador del factor 1 (tests de ansiedad).
        /// </summary>
        public const string FACTORS_1 = "1";
        /// <summary>
        /// Identificador del factor 2 (tests de ansiedad).
        /// </summary>
        public const string FACTORS_2 = "2";
        /// <summary>
        /// Identificador del factor 3 (tests de ansiedad).
        /// </summary>
        public const string FACTORS_3 = "3";
        /// <summary>
        /// Identificador del factor 4 (tests de ansiedad).
        /// </summary>
        public const string FACTORS_4 = "4";
        /// <summary>
        /// Clave en recursos del g�nero masculino.
        /// </summary>
        public const string GENDERS_MALE_KEY = "str_Genders_Man";
        /// <summary>
        /// Clave en recursos del g�nero femenino.
        /// </summary>
        public const string GENDERS_FEMALE_KEY = "str_Genders_Woman";
        /// <summary>
        /// Clave en recursos del g�nero no binario.
        /// </summary>
        public const string GENDERS_NB_KEY = "str_Genders_NB";
        /// <summary>
        /// Clave en recursos del nivel bajo de resultado de test.
        /// </summary>
        public const string FACTORS_LEVEL_LOW = "str_FACTORS_LEVEL_LOW";
        /// <summary>
        /// Clave en recursos del nivel medio-bajo de resultado de test.
        /// </summary>
        public const string FACTORS_LEVEL_LOW_MEDIUM = "str_FACTORS_LEVEL_LOW_MEDIUM";
        /// <summary>
        /// Clave en recursos del nivel medio de resultado de test.
        /// </summary>
        public const string FACTORS_LEVEL_MEDIUM = "str_FACTORS_LEVEL_MEDIUM";
        /// <summary>
        /// Clave en recursos del nivel medio-alto de resultado de test.
        /// </summary>
        public const string FACTORS_LEVEL_MEDIUM_HIGH = "str_FACTORS_LEVEL_MEDIUM_HIGH";
        /// <summary>
        /// Clave en recursos del nivel alto de resultado de test.
        /// </summary>
        public const string FACTORS_LEVEL_HIGH = "str_FACTORS_LEVEL_HIGH";
        /// <summary>
        /// Ruta del modelo entrenado de IA para calcular resultados generales de tests de ansiedad r�pidos.
        /// </summary>
        public static readonly string AI_GENERAL_QUICK_TEST_PATH = Path.Combine(AppContext.BaseDirectory, "AI_Models", "model_general_quickTests.zip");
        /// <summary>
        /// Ruta del modelo entrenado de IA para calcular resultados generales de tests de ansiedad completos.
        /// </summary>
        public static readonly string AI_GENERAL_FULL_TEST_PATH = Path.Combine(AppContext.BaseDirectory, "AI_Models", "model_general_fullTests.zip");
        /// <summary>
        /// Ruta del modelo entrenado de IA para calcular resultados generales de tests de TCA.
        /// </summary>
        public static readonly string AI_GENERAL_TCA_TEST_PATH = Path.Combine(AppContext.BaseDirectory, "AI_Models", "model_general_TCATests.zip");

        /// <summary>
        /// Variable global para evitar que se puedan pulsar varios botones o el mismo bot�n varias veces y provocar errores inesperados. 
        /// </summary>
        public static bool ButtonPressed = false;
        private static MejorAppTDatabase _database;
        private static FirebaseService _firebase;
        private bool _isSyncing = false;

        /// <summary>
        /// Permite recuperar o modificar el usuario que ha iniciado sesi�n actualmente en la aplicaci�n.
        /// </summary>
        /// <value>
        /// El usuario con la sesi�n activa.
        /// </value>
        public static User CurrentUser { get; set; }

        /// <summary>
        /// La base de datos local y offline de SQLite que utiliza la aplicaci�n para almacenar los datos de los usuarios.
        /// </summary>
        /// <value>
        /// La base de datos de SQLite.
        /// </value>
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

        /// <summary>
        /// La base de datos en la nube de Firebase que utiliza la aplicaci�n para almacenar estad�sticas de todos los usuarios.
        /// </summary>
        /// <value>
        /// La base de datos de Firebase.
        /// </value>
        public static FirebaseService Firebase
        {
            get {
                if (_firebase == null) {
                    _firebase = new FirebaseService("https://mejorappt-g1-default-rtdb.europe-west1.firebasedatabase.app");
                }
                return _firebase;
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="App"/>.
        /// </summary>
        /// <remarks>
        /// Entre otras cosas, este constructor fuerza el modo claro, establece la localizaci�n almacenada o predeterminada y, dependiendo de si un usuario ten�a su sesi�n abierta o la cerr�, abre el men� principal o la pantalla de inicio.
        /// </remarks>
        public App()
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            this.RequestedThemeChanged += (s, e) => { Application.Current.UserAppTheme = AppTheme.Light; };
            CultureInfo culture;
            if (Preferences.ContainsKey(USER_LANGUAGE)) {
                var userLang = Preferences.Get(USER_LANGUAGE, "en");
                culture = new(userLang);
            } else {
                culture = CultureInfo.CurrentCulture;
            }
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

        /// <summary>
        /// Almacena en CurrentUser el usuario que ha iniciado sesi�n y que est� guardado en las preferencias de la aplicaci�n.
        /// </summary>
        public async void GetCurrentUser()
        {
            App.CurrentUser = await App.Database.GetUserByIdAsync(Preferences.Get(USER_ID_KEY, 0));
        }

        /// <summary>
        /// M�todo gen�rico que permite animar un componente Frame que se le pase con un efecto de pulsaci�n (reduce su tama�o y vuelve a su estado original).
        /// </summary>
        /// <param name="sender">El Frame a animar.</param>
        public static async void AnimateFrameInOut(object sender)
        {
            await ((Frame)sender).ScaleTo(0.95, 100, Easing.CubicInOut);    // Reducir un poco el tama�o
            await ((Frame)sender).ScaleTo(1, 100, Easing.CubicInOut);   // Volver al tama�o original
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

        /// <summary>
        /// Inicia la tarea de sincronizaci�n de las bases de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelaci�n utilizado para la interrupci�n del proceso.</param>
        /// <param name="mode">La fase del ciclo de vida de la aplicaci�n ("open" o "close") durante la que se realiza la sincronizaci�n.</param>
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

        /// <summary>
        /// Comprueba la conexi�n a Internet cada cinco segundos y, cuando la detecta, inicia el proceso de sincronizaci�n de las bases de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelaci�n utilizado para la interrupci�n del proceso.</param>
        public async Task WaitForInternetAndSync(CancellationToken cancellationToken)
        {
            bool noInternet = true;
            while (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) {  // Bucle "infinito" hasta que no detecte conexi�n
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

        /// <summary>
        /// Compruba la conexi�n a Internet una �nica vez y, si la encuentra, inicia el proceso de sincronizaci�n de las bases de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelaci�n utilizado para la interrupci�n del proceso.</param>
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
