using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.Components;
using AlanJayApp.Services;
using Camera.MAUI;

namespace AlanJayApp
{
    public partial class MainPage : ContentPage
    {
        private readonly AuthService _authService;

        // REMOVE NavigationManager from constructor
        public MainPage(AuthService authService)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _authService = authService;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            // If you just want to check login status, you can do that here
            bool isLoggedIn = await _authService.IsLoggedInAsync();
            // But let your Blazor code do any actual route-based navigation
            // e.g., in a .razor file with NavigationManager injected.
        }
    }
}
