# TuckBoxApp

TuckBoxApp is a .NET MAUI sample app for a fictional food-ordering service ("tuck box" delivery). It is a self-contained showcase project: everything runs on a single device, there is no server to stand up and no external account to create. All data (users, food items, cities, delivery time slots, orders, delivery addresses) lives in a single JSON file that the app manages on its own, seeded from sample data included in the repository.

This document is written for someone who has never seen this repository before. If you follow it top to bottom you should end up with the app running.

## What the app actually does today

- Splash screen, then Login and Register screens backed by the on-device data store.
- A Home screen showing the signed-in user's name, quick-action buttons, and any orders placed today.
- Order, Profile, and Order History screens exist as navigation destinations but are placeholder "Under Construction" pages with no logic behind them yet. See "Known limitations" below.

## Architecture

The app follows the MVVM (Model-View-ViewModel) pattern, which is the standard approach for MAUI apps. Everything lives inside the single `TuckBoxApp` project (`TuckBoxApp/TuckBoxApp.csproj`), referenced by the solution file `tuck-box-shop-app.sln` at the repository root.

```
TuckBoxApp/
  Models/       Plain data classes: User, Order, OrderItem, City, TimeSlot,
                FoodItem, FoodExtraDetails, DeliveryAddress, LocalStore.
  Views/        XAML pages (the UI): SplashPage, LoginPage, RegisterPage,
                MainPage, OrderPage, ProfilePage, OrderHistoryPage. Each
                .xaml file has a matching .xaml.cs "code-behind" file.
  ViewModels/   One class per screen that needs one, holding the screen's
                state and commands (BaseViewModel, LoginViewModel,
                RegisterViewModel, MainViewModel). Built with
                CommunityToolkit.Mvvm, so fields marked [ObservableProperty]
                and methods marked [RelayCommand] get their boilerplate
                generated automatically.
  Services/     The app's business logic and data access layer: AuthService,
                OrderService, LocalDataService, PasswordHasher.
  Converters/   Small IValueConverter classes used in XAML bindings
                (StringToBoolConverter, NotEmptyConverter).
  Platforms/    Per-platform entry points and manifests (Android, iOS,
                MacCatalyst, Windows, Tizen). You will rarely need to touch
                these.
  Resources/    Fonts, images, app icon, splash screen, XAML styles, and
                Resources/Raw/seed_data.json (the sample data the app is
                seeded with on first launch).
  AppShell.xaml Defines the app's navigation structure (which pages exist
                and how you get between them), using .NET MAUI Shell.
  MauiProgram.cs
                The app's startup file. Registers every Service, ViewModel,
                and View with the dependency injection container.
```

Data flow for a typical screen, using Login as the example:

1. `AppShell.xaml` routes to `LoginPage`. `MauiProgram.cs` has registered `LoginPage` and `LoginViewModel` with the DI container, and `LoginPage`'s constructor takes a `LoginViewModel` parameter, so the container builds one and injects it automatically.
2. `LoginPage.xaml.cs` sets `BindingContext = viewModel`, so every `{Binding ...}` expression in `LoginPage.xaml` reads from and writes to that `LoginViewModel` instance.
3. When the user taps "Login", the `LoginCommand` (generated from the `[RelayCommand]` attribute on `LoginViewModel.LoginAsync`) runs. It calls `IAuthService.LoginAsync`.
4. `AuthService` (the concrete implementation of `IAuthService`) asks `ILocalDataService` for every stored user, hashes the entered password, and checks for a match.
5. `LocalDataService` (the concrete implementation of `ILocalDataService`) is where the actual on-device storage lives. See the next section for exactly how that works.

For a more detailed technical walkthrough (full DI registration list, Shell routes, the local data file's schema, and why the two converters exist), see `docs/ARCHITECTURE.md`.

## How data storage works (no external services)

There is no Firebase, no cloud database, and no network calls anywhere in this app. All data is stored in one plain JSON file inside the app's own private storage folder (`FileSystem.AppDataDirectory`, provided by MAUI), named `tuckbox-data.json`.

The first time the app runs on a given device (or after that file has been deleted), `LocalDataService` copies `TuckBoxApp/Resources/Raw/seed_data.json` (bundled inside the app) into that file. From then on, every read and write goes through `tuckbox-data.json`. Reinstalling the app, or clearing its storage, resets it back to the seed data.

A demo account is included in the seed data so you can log in immediately after installing:

- Email: `user@example.com`
- Password: `password123`

Passwords are hashed with SHA-256 before being stored or compared (see `TuckBoxApp/Services/PasswordHasher.cs`). This is intentionally simple: there is no per-user salt and no external identity provider. That is a reasonable tradeoff for a local, single-device showcase app, but it is not how you would build authentication for a real production system.

## Prerequisites

You need the .NET 9 SDK and the MAUI workload, plus tooling for whichever platform you want to run the app on. You do not need all of these, just the ones for the platform(s) you care about.

- **.NET 9 SDK**: https://dotnet.microsoft.com/download (any OS)
- **MAUI workload**: after installing the SDK, run `dotnet workload install maui`
- **Android**: Android Studio (for the Android SDK and an emulator), or a physical Android device with USB debugging enabled. Works from Windows, macOS, or Linux.
- **iOS / Mac Catalyst**: a Mac with Xcode installed. These targets cannot be built on Windows or Linux.
- **Windows**: Visual Studio 2022 (17.8 or later) with the ".NET Multi-platform App UI development" workload, on Windows itself.

If you just want to confirm the code compiles without installing any of the above, see the Docker section further down. It only proves the app builds; it cannot run or display it.

## Setup and running

1. Clone the repository and open a terminal in it.
2. Install the MAUI workload if you have not already: `dotnet workload install maui`
3. Restore dependencies: `dotnet restore tuck-box-shop-app.sln`
4. Run the app for your platform:
   - **Android (CLI)**: `dotnet build TuckBoxApp/TuckBoxApp.csproj -t:Run -f net9.0-android` with an emulator running or a device connected.
   - **Windows (CLI, on Windows only)**: `dotnet build TuckBoxApp/TuckBoxApp.csproj -t:Run -f net9.0-windows10.0.19041.0`
   - **iOS / Mac Catalyst (CLI, on macOS only)**: `dotnet build TuckBoxApp/TuckBoxApp.csproj -t:Run -f net9.0-ios` or `-f net9.0-maccatalyst`
   - **Visual Studio**: open `tuck-box-shop-app.sln`, pick a target framework/device from the toolbar dropdown, and press Run (F5).
5. On first launch you will land on the splash screen, then Login. Sign in with the demo account above, or use Register to create a new local account.

## Docker (build verification only)

There is no `dotnet` CLI available in every environment (for example, a plain Linux CI machine without the SDK installed), and this app cannot be built for iOS, Mac Catalyst, or Windows outside of their native operating systems anyway. `docker/Dockerfile.build` gives you a way to prove, on any machine with Docker, that the Android target of this app compiles cleanly, using the official Microsoft .NET SDK image.

```
docker build -t tuckbox-build -f docker/Dockerfile.build .
docker run --rm -v "$PWD":/src -w /src/TuckBoxApp tuckbox-build sh -c \
  "dotnet restore -p:TargetFrameworks=net9.0-android && dotnet build -f net9.0-android --no-restore -p:AndroidSdkDirectory=/opt/android-sdk"
```

The `docker build` step installs the Android and MAUI-Android workloads and a full Android SDK inside the image (several hundred megabytes, can take a few minutes) so a plain `docker run` afterward does not need to download anything. The `docker run` step restores and compiles the app and prints the normal `dotnet build` output; look for `Build succeeded` at the end.

The `-p:TargetFrameworks=net9.0-android` override on restore is needed because the image only has the Android workload installed, not iOS or Mac Catalyst; without it, restore tries to resolve packs for every framework listed in `TuckBoxApp.csproj` and fails. `-p:AndroidSdkDirectory=/opt/android-sdk` points the build at the Android SDK baked into the image during `docker build`.

This container is for compilation only. It has no display and no Android emulator, so it cannot launch the app or show you the UI. To actually see and use the app, follow the "Setup and running" section above on a machine with the right platform tooling installed.

## Known limitations

- **Order, Profile, and Order History screens are placeholders.** They exist as navigation destinations (you can tap through to them) but currently just show an "Under Construction" label. There is no `OrderViewModel` or `ProfileViewModel` wired up yet; the empty files `TuckBoxApp/ViewModels/OrderViewModel.cs` and `TuckBoxApp/ViewModels/ProfileViewModel.cs` are left as a starting point for whoever picks this up next.
- **Authentication is intentionally minimal.** SHA-256 hashing with no salt is enough to avoid storing raw passwords on disk, but it is not a real authentication system. Do not reuse this pattern for anything beyond a local demo.
- **The Tizen platform folder (`TuckBoxApp/Platforms/Tizen`) is unused scaffolding.** `net9.0-tizen` is not listed in `TargetFrameworks` in `TuckBoxApp.csproj`, so this folder is never actually built. It is left over from the initial MAUI project template.
- **No automated tests exist yet.**

## Troubleshooting

- **"Workload not installed" or similar errors when building**: run `dotnet workload install maui` (or `dotnet workload update` if it is already installed but out of date), then try again.
- **Android build fails looking for an SDK/emulator**: make sure Android Studio's SDK Manager has an Android SDK platform installed, and that either an emulator (AVD) is running or a physical device is connected with USB debugging on. `adb devices` should list something.
- **iOS/Mac Catalyst build fails on Windows or Linux**: this is expected. Those targets require Xcode and can only be built on macOS.
- **Login fails with "Invalid email or password" even with the demo account**: this usually means `tuckbox-data.json` on the device is stale or corrupted. Uninstalling and reinstalling the app clears app storage and re-seeds it from `Resources/Raw/seed_data.json`.
- **Changes to `Resources/Raw/seed_data.json` are not showing up**: the seed file is only copied into `tuckbox-data.json` the first time the app runs on a given install. Uninstall and reinstall (or clear app storage) to force a re-seed.

## License

See `LICENSE` (MIT).
