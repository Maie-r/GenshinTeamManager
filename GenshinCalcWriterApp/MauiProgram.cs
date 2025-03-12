using CalcViewer.Data;
using GenshinTeamCalc;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace GenshinCalcWriterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
            try
            {
                Calc.LoadStuff(true);
            }
            catch
            {
                throw new Exception("Couldn't load data");
            }
            /*
            try
            {
                builder.Services.AddSingleton<ICalc, CalcService>();
            }
            catch
            {
                throw new Exception("Couldn't load data");
            }
            */

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
