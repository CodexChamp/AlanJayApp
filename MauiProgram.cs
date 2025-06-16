using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Blazored.LocalStorage;
using Camera.MAUI;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Handlers;
using AlanJayApp.Data;
using AlanJayApp.Services;

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

            // ─── Load appsettings.json ─────────────────────────────────────────────
            var config = LoadConfiguration();
            builder.Configuration.AddConfiguration(config);

            // ─── Register AppSettings ─────────────────────────────────────────────
            builder.Services.Configure<AppSettings>(options =>
                config.GetSection("AppSettings").Bind(options));
            // expose AppSettings as a concrete singleton
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<AppSettings>>().Value);

            // ─── Register other services ───────────────────────────────────────────
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddLogging();

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<CustomAuthStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(
                sp => sp.GetRequiredService<CustomAuthStateProvider>());
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            builder.Services.AddSingleton<LocalStorageService>();
            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddSingleton<DatabaseConnectionService>();
            builder.Services.AddSingleton<CarService>();
            builder.Services.AddBlazoredLocalStorage();

            // ─── Register your BlobServiceClient ───────────────────────────────────
            // grabs connection string from AppSettings and constructs the client
            builder.Services.AddSingleton(sp => {
                var settings = sp.GetRequiredService<AppSettings>();
                return new Azure.Storage.Blobs.BlobServiceClient(settings.BlobStorageConnectionString);
            });

            // ─── Register HttpClient (API URL set later) ───────────────────────────
            var httpClient = new HttpClient();
            builder.Services.AddSingleton(httpClient);

            // ─── Configure WebView for camera access ───────────────────────────────
            WebViewHandler.Mapper.AppendToMapping("WebViewPermissions", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowContentAccess = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.DatabaseEnabled = true;
                handler.PlatformView.Settings.MixedContentMode =
                    Android.Webkit.MixedContentHandling.AlwaysAllow;
#endif
            });

#if WINDOWS
            builder.Services.AddSingleton<IFileProvider>(_ =>
                new PhysicalFileProvider(
                    Path.Combine(FileSystem.AppDataDirectory, "wwwroot")));
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // ─── Build & initialize ────────────────────────────────────────────────
            var app = builder.Build();
            ServiceProvider = app.Services;

            // ensure DatabaseConnectionService is warmed up
            _ = app.Services.GetRequiredService<DatabaseConnectionService>();

            // load base API URL into HttpClient
            _ = InitializeApiUrlAsync(httpClient, app.Services);

            // request camera permission
            _ = RequestPermissions();

            return app;
        }

        private static IConfiguration LoadConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AlanJayApp.wwwroot.JSON.appsettings.json";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                          ?? throw new FileNotFoundException(
                              $"Embedded resource {resourceName} not found.");

            return new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        private static async Task InitializeApiUrlAsync(
            HttpClient httpClient,
            IServiceProvider services)
        {
            try
            {
                var localStore = services.GetRequiredService<LocalStorageService>();
                var apiUrl = await localStore.GetAsync<string>("apiUrl");
                if (string.IsNullOrEmpty(apiUrl) ||
                    apiUrl.Contains("alanjay-dev"))
                {
                    apiUrl = "https://alanjay20241025105445.azurewebsites.net/";
                    await localStore.SetAsync("apiUrl", apiUrl);
                }
                httpClient.BaseAddress = new Uri(apiUrl);
            }
            catch
            {
                // swallow or log as needed
            }
        }

        private static async Task RequestPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                _ = await Permissions.RequestAsync<Permissions.Camera>();
        }
    }

    public static class ServiceHelper
    {
        private static IServiceProvider? _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public static T GetService<T>() where T : class =>
            _serviceProvider?.GetRequiredService<T>()
            ?? throw new InvalidOperationException("Service provider is not set.");
    }
}
