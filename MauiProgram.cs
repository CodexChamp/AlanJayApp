using Microsoft.Extensions.Logging;
using AlanJayApp.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Extensions.Configuration;
using AlanJayApp.Data;
using System.Reflection;
using Microsoft.Extensions.Options;
using Blazored.LocalStorage;
using Camera.MAUI;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Handlers;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Hosting;


namespace AlanJayApp
{
    public static class MauiProgram
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // ✅ Load appsettings.json
            var config = LoadConfiguration();
            builder.Configuration.AddConfiguration(config);

            // ✅ Register Services Before Building App

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddLogging();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<CustomAuthStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddSingleton<LocalStorageService>();
            // ✅ Register AppSettings in Dependency Injection (DI)
            builder.Services.Configure<AppSettings>(options => config.GetSection("AppSettings").Bind(options));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
            builder.Services.AddSingleton<DatabaseConnectionService>();
            builder.Services.AddSingleton<CarService>(); // Make it available everywhere
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddScoped<FordRecallScraperService>();
            builder.Services.AddScoped<StellantisRecallScraperService>();

            // ✅ Register HttpClient with default API URL (will be updated later)
            var httpClient = new HttpClient();
            builder.Services.AddSingleton(httpClient);

            // ✅ Modify WebView settings to allow camera access
            WebViewHandler.Mapper.AppendToMapping("WebViewPermissions", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowContentAccess = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.DatabaseEnabled = true;
                handler.PlatformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
#endif
            });

#if WINDOWS
            builder.Services.AddSingleton<IFileProvider>(_ =>
                new PhysicalFileProvider(Path.Combine(FileSystem.AppDataDirectory, "wwwroot")));
#endif

            // ✅ Register Blazor Developer Tools & Debugging BEFORE building app
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // ✅ Build the app **after** registering all services
            var app = builder.Build();
            ServiceProvider = app.Services;
            // ✅ Force Database Service to Load Initial Connection
            var dbService = app.Services.GetRequiredService<DatabaseConnectionService>();
            
            // ✅ Ensure HttpClient Uses the Correct API URL
            Task.Run(() => InitializeApiUrlAsync(httpClient, app.Services));

            // ✅ Request Camera Permission on App Start
            Task.Run(RequestPermissions);

            return app;
        }

        // ✅ Function to Load appsettings.json
        private static IConfiguration LoadConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AlanJayApp.wwwroot.JSON.appsettings.json"; // Ensure correct path

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException($"Embedded resource {resourceName} not found.");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            return config;
        }

        // ✅ Async function to fetch API URL from local storage
        private static async Task InitializeApiUrlAsync(HttpClient httpClient, IServiceProvider services)
        {
            try
            {
                var localStorageService = services.GetRequiredService<LocalStorageService>();
                string? apiUrl = await localStorageService.GetAsync<string>("apiUrl");

                if (string.IsNullOrEmpty(apiUrl) || apiUrl.Contains("alanjay-dev"))
                {
                    apiUrl = "https://alanjay20241025105445.azurewebsites.net/";
                    await localStorageService.SetAsync("apiUrl", apiUrl);
                    Console.WriteLine($"[FIXED] API URL reset to: {apiUrl}");
                }

                httpClient.BaseAddress = new Uri(apiUrl);
                Console.WriteLine($"[DEBUG] Using API URL: {httpClient.BaseAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load API URL: {ex.Message}");
            }
        }

        // ✅ Function to Request Camera Permission
        private static async Task RequestPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
        }
    }

    // ✅ Helper to access DI services
    public static class ServiceHelper
    {
        private static IServiceProvider? _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProvider?.GetRequiredService<T>() ?? throw new InvalidOperationException("Service provider is not set.");
        }
    }
}
