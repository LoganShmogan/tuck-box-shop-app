namespace TuckBoxApp.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
        
        // Navigate to login after delay
        Task.Delay(3000).ContinueWith(t => 
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            });
        });
    }
}