using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Maui.Storage;

namespace AlanJayApp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private const string AuthKey = "UserIsLoggedIn";
        private UserData? _currentUser;

        public async Task<bool> IsLoggedInAsync()
        {
            return await SecureStorage.GetAsync(AuthKey) == "true";
        }

        public async Task SetLoggedInAsync(UserData user)
        {
            Preferences.Set("isLoggedIn", true);
            Preferences.Set("InspectorName", user.loginID);
            _currentUser = user;
            NotifyAuthenticationStateChanged(
              Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(
                  new[] { new Claim(ClaimTypes.Name, user.loginID) }, "auth")))));
        }



        public async Task LogoutAsync()
        {
            SecureStorage.Remove("isLoggedIn");
            SecureStorage.Remove("currentUser");
            Preferences.Remove("InspectorName");
        }


        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser is null)
            {
                var loggedIn = Preferences.Get("isLoggedIn", false);
                if (loggedIn)
                {
                    var id = Preferences.Get("InspectorName", string.Empty);
                    _currentUser = new UserData { loginID = id };
                }
            }

            var identity = _currentUser != null
              ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, _currentUser.loginID) }, "auth")
              : new ClaimsIdentity();

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}
