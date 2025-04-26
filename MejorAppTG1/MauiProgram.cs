using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using PanCardView;
using Maui.FreakyControls.Extensions;
using Microcharts.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

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
                .UseMicrocharts()
                .UseSkiaSharp()
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
