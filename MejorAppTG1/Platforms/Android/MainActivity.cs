using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace MejorAppTG1
{
    [Activity(Theme = "@style/Maui.SplashTheme", ResizeableActivity = true, LaunchMode = LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Forzar el tema claro
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
        }
    }
}
