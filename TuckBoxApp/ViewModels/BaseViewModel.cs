using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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