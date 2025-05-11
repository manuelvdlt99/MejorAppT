using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using PanCardView;
using Maui.FreakyControls.Extensions;
#if !NO_CHARTS
using Microcharts.Maui;
#endif
using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.LocalNotification;

namespace MejorAppTG1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .UseCardsView()
                .UseMauiCommunityToolkit()
                .InitializeFreakyControls()
#if !NO_CHARTS
                .UseMicrocharts()
#endif
                .UseSkiaSharp()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("K2D-Regular.ttf", "K2D");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
