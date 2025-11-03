using Microsoft.Extensions.Logging;
using tuck_box_shop.Services;
using tuck_box_shop.ViewModels;

namespace tuck_box_shop
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

            // Register Services
            builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            
            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            // You'll add more ViewModels here as you create them
            
            // Register Pages
            builder.Services.AddTransient<LoginPage>();
            // You'll add more Pages here as you create them

            return builder.Build();
        }
    }
}