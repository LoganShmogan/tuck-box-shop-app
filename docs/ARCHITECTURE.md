# Architecture

This document goes one level deeper than the README, for anyone about to make changes to the app. Read the README first for the general overview, setup steps, and known limitations.

## MVVM data flow, end to end

TuckBoxApp uses the standard .NET MAUI MVVM (Model-View-ViewModel) split, wired together with Microsoft's built-in dependency injection container and the CommunityToolkit.Mvvm source generator.

1. **Views** (`TuckBoxApp/Views/*.xaml` plus their `.xaml.cs` code-behind) define the UI in XAML and bind to a ViewModel through `BindingContext`. A page that needs a ViewModel takes it as a constructor parameter, for example `LoginPage(LoginViewModel viewModel)`, and the DI container supplies the instance automatically. Pages that do not need one yet (`OrderPage`, `ProfilePage`, `OrderHistoryPage`) have parameterless constructors.
2. **ViewModels** (`TuckBoxApp/ViewModels/*.cs`) hold the UI state for a screen and expose commands. They inherit `BaseViewModel`, which provides `IsBusy`, `Title`, `ErrorMessage`, and `SuccessMessage` plus `ShowError`/`ShowSuccess`/`ClearMessages` helpers. Fields marked `[ObservableProperty]` get a public property with change notification generated for them by CommunityToolkit.Mvvm; methods marked `[RelayCommand]` get a matching `ICommand` property generated (for example, a method named `LoginAsync` produces a bindable `LoginCommand`).
3. **Services** (`TuckBoxApp/Services/*.cs`) hold the actual logic: `IAuthService`/`AuthService` for login, registration, logout, and password changes; `IOrderService`/`OrderService` for reading and writing orders; `ILocalDataService`/`LocalDataService` for the on-device data store both of the above sit on top of.
4. **Models** (`TuckBoxApp/Models/*.cs`) are plain data classes with no behavior: `User`, `Order`, `OrderItem`, `OrderStatus`, `City`, `TimeSlot`, `FoodItem`, `FoodExtraDetails`, `DeliveryAddress`, and `LocalStore` (the root container that mirrors the shape of the on-device JSON file).

## Dependency injection registrations (`MauiProgram.cs`)

Everything is wired up in `TuckBoxApp/MauiProgram.cs`, inside `CreateMauiApp()`:

**Services** (all singletons, so there is exactly one instance for the app's lifetime, and `LocalDataService` keeps its in-memory copy of the data file consistent across every screen):
- `ILocalDataService` -> `LocalDataService`
- `IAuthService` -> `AuthService`
- `IOrderService` -> `OrderService`

**ViewModels** (transient, a new instance is created every time a page is navigated to):
- `LoginViewModel`, `RegisterViewModel`, `MainViewModel`

**Views** (transient, same reasoning):
- `SplashPage`, `LoginPage`, `RegisterPage`, `MainPage`, `OrderPage`, `ProfilePage`, `OrderHistoryPage`

**Converters** (singletons, they hold no state):
- `NotEmptyConverter`, `StringToBoolConverter`

`App.xaml` separately declares both converters as XAML `StaticResource` entries (`NotEmptyConverter` and `StringToBoolConverter`), which is how XAML bindings reference them without going through the DI container.

## Navigation (`AppShell.xaml`)

The app uses .NET MAUI Shell for navigation. `TuckBoxApp/AppShell.xaml` defines these routes:

- `SplashPage` (route `SplashPage`): shown first. `SplashPage.xaml.cs` waits three seconds, then navigates to `//LoginPage`.
- `LoginPage` (route `LoginPage`)
- `RegisterPage` (route `RegisterPage`)
- A `TabBar` (route `Main`) containing three tabs, each pointing at a page: `MainPage` (route `MainPage`, tab title "Home"), `OrderPage` (route `OrderPage`, tab title "Order"), `ProfilePage` (route `ProfilePage`, tab title "Profile").

`MainViewModel` also navigates to `OrderHistoryPage` (route `OrderHistoryPage`) from its "View Order History" command, even though that page is not part of the tab bar.

Routes prefixed with `//` (for example `await Shell.Current.GoToAsync($"//{nameof(MainPage)}")`) are absolute navigations that reset the navigation stack, used after login and logout. Plain routes (`await Shell.Current.GoToAsync(nameof(RegisterPage))`) push onto the stack, used for things like "Create Account" from the login screen.

## The on-device data store

`ILocalDataService` is the single point of contact between the rest of the app and storage. Its concrete implementation, `LocalDataService`, keeps everything in one JSON file and one in-memory `LocalStore` instance:

```csharp
public class LocalStore
{
    public Dictionary<string, User> Users { get; set; } = new();
    public Dictionary<string, FoodItem> FoodItems { get; set; } = new();
    public Dictionary<string, City> Cities { get; set; } = new();
    public Dictionary<string, TimeSlot> TimeSlots { get; set; } = new();
    public Dictionary<string, Order> Orders { get; set; } = new();
    public Dictionary<string, DeliveryAddress> DeliveryAddresses { get; set; } = new();
}
```

Each dictionary is keyed by the entity's own `Id` field. This mirrors the shape of `TuckBoxApp/Resources/Raw/seed_data.json`, the file bundled into the app that becomes the initial contents of the on-device data file.

On first access, `LocalDataService.GetStoreAsync()` checks whether `tuckbox-data.json` already exists in `FileSystem.AppDataDirectory` (the app's private storage folder, provided by MAUI and different per platform). If it does not exist yet, the bundled `seed_data.json` is opened via `FileSystem.OpenAppPackageFileAsync("seed_data.json")` and copied byte-for-byte into that location. From then on, the file at `FileSystem.AppDataDirectory/tuckbox-data.json` is the single source of truth. It is deserialized once into a private `LocalStore` field (guarded by a `SemaphoreSlim` so concurrent calls do not race on the first load), and every mutating method (`AddUserAsync`, `UpdateOrderAsync`, and so on) re-serializes the whole `LocalStore` back to that file with `Newtonsoft.Json` after making its change.

Because `Resources/Raw/**` is auto-included by the .NET MAUI SDK's default SingleProject item globs, dropping a file into `TuckBoxApp/Resources/Raw/` is enough to have it bundled with the app; you do not need to add anything to `TuckBoxApp.csproj` for that to happen. `seed_data.json`'s logical name inside the app package is just its file name, which is why `OpenAppPackageFileAsync("seed_data.json")` finds it directly.

## Authentication

`AuthService` implements `IAuthService` on top of `ILocalDataService`. `RegisterAsync` checks `GetAllUsersAsync()` for an existing user with the same email, then hashes the new user's password with `PasswordHasher.Hash` (a thin wrapper around `System.Security.Cryptography.SHA256`, UTF-8 encoding, lowercase hex output) before calling `AddUserAsync`. `LoginAsync` hashes the entered password the same way and looks for a user whose stored hash matches. `ChangePasswordAsync` re-hashes and re-saves. There is no salting and no external identity provider; this is a deliberate simplification appropriate for a local, single-device showcase, not a pattern to copy for production authentication.

## Converters

XAML bindings cannot run arbitrary C# inline, so two small `IValueConverter` implementations in `TuckBoxApp/Converters/` bridge the gap between a ViewModel's data and a boolean the UI needs:

- `StringToBoolConverter` (`TuckBoxApp/Converters/StringToBoolConverter.cs`): converts a string to `true` when it is non-null and not just whitespace. Used to show or hide the error/success message labels on `LoginPage.xaml` and `RegisterPage.xaml`, bound to `ErrorMessage`/`SuccessMessage` on `BaseViewModel`.
- `NotEmptyConverter` (`TuckBoxApp/Converters/NotEmptyConverter.cs`): converts an `int` to `true` when it is greater than zero. Used on `MainPage.xaml` to only show the "Today's Orders" card when `CurrentOrders.Count` is greater than zero.

Both are declared once in `App.xaml`'s resource dictionary (`xmlns:converters="clr-namespace:TuckBoxApp.Converters"`, then `<converters:StringToBoolConverter x:Key="StringToBoolConverter" />` and the equivalent for `NotEmptyConverter`), and referenced from XAML with `{StaticResource ...}` or `{x:StaticResource ...}`.

## Adding a new screen

If you are picking up `OrderPage`, `ProfilePage`, or `OrderHistoryPage`, the pattern to follow is the same one `LoginPage`/`LoginViewModel` already use:

1. Create a ViewModel in `TuckBoxApp/ViewModels/` inheriting `BaseViewModel`, with `[ObservableProperty]` fields for the screen's state and `[RelayCommand]` methods for its actions.
2. Register it in `MauiProgram.cs` with `builder.Services.AddTransient<YourViewModel>();`.
3. Give the matching page's code-behind a constructor that takes the ViewModel and sets `BindingContext = viewModel`.
4. Bind the XAML to the ViewModel's properties and commands.
5. If the screen needs data, add methods to `ILocalDataService`/`LocalDataService` (or reuse existing ones through `IOrderService`/`IAuthService`) rather than reading or writing `tuckbox-data.json` directly from the ViewModel.
