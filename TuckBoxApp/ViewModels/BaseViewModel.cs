using CommunityToolkit.Mvvm.ComponentModel;

namespace TuckBoxApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private string _successMessage = string.Empty;
    
    public bool IsNotBusy => !IsBusy;
    
    protected void ShowError(string message)
    {
        ErrorMessage = message;
        SuccessMessage = string.Empty;
    }
    
    protected void ShowSuccess(string message)
    {
        SuccessMessage = message;
        ErrorMessage = string.Empty;
    }
    
    protected void ClearMessages()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }
}