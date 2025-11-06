using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TuckBoxApp.Models;
using TuckBoxApp.Services;

namespace TuckBoxApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;
    
    public MainViewModel(IAuthService authService, IOrderService orderService)
    {
        _authService = authService;
        _orderService = orderService;
        Title = "Tuck Box";
        
        LoadCurrentOrders();
    }
    
    [ObservableProperty]
    private List<Order> _currentOrders = new();
    
    [ObservableProperty]
    private string _userName = string.Empty;
    
    public void OnAppearing()
    {
        UserName = _authService.CurrentUser?.FullName ?? "User";
        LoadCurrentOrders();
    }
    
    private async void LoadCurrentOrders()
    {
        if (_authService.CurrentUser != null)
        {
            var orders = await _orderService.GetUserOrdersAsync(_authService.CurrentUser.Id);
            CurrentOrders = orders
                .Where(o => o.OrderDate.Date == DateTime.UtcNow.Date)
                .ToList();
        }
    }
    
    [RelayCommand]
    private async Task PlaceOrderAsync()
    {
        await Shell.Current.GoToAsync(nameof(OrderPage));
    }
    
    [RelayCommand]
    private async Task ViewProfileAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }
    
    [RelayCommand]
    private async Task ViewOrderHistoryAsync()
    {
        await Shell.Current.GoToAsync(nameof(OrderHistoryPage));
    }
    
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}