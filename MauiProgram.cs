using Microsoft.Extensions.Logging;
using WorkoutApp.Services;
using WorkoutApp.ViewModels;

namespace WorkoutApp
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<WorkoutDetailsViewModel>();
            builder.Services.AddTransient<WorkoutDetailsPage>();

            return builder.Build();
        }
    }
}