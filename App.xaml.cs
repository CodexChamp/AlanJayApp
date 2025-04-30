using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;
using AlanJayApp.Services;

namespace AlanJayApp
{
    public partial class App : Application
    {
        private readonly AuthService _authService;


        public App(AuthService authService, NavigationManager navigation)
        {
            InitializeComponent();

            _authService = authService;

            MainPage = new NavigationPage(new MainPage( _authService)); // ✅ Pass the required dependencies
        }
    }
}
