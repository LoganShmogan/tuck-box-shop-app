using Microsoft.Extensions.Logging;
using TuckBoxApp.Services;
using TuckBoxApp.ViewModels;
using TuckBoxApp.Views;
using TuckBoxApp.Converters;

namespace TuckBoxApp;

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
        builder.Services.AddSingleton<IOrderService, OrderService>();
        
        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        
        // Register Views
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<OrderPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<OrderHistoryPage>();
        
        // Register Converters
        builder.Services.AddSingleton<NotEmptyConverter>();
        builder.Services.AddSingleton<StringToBoolConverter>();
        
        return builder.Build();
    }
}