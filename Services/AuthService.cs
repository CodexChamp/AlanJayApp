using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage; // Import SecureStorage
using System.Diagnostics;
namespace AlanJayApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserData?> LoginAsync(string loginId, string password)
        {
            var loginRequest = new
            {
                loginID = loginId,
                Password = password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://alanjay20241025105445.azurewebsites.net/api/auth/login", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                Preferences.Remove("InspectorName");
                Debug.WriteLine("❌ Login failed: Invalid response from server."); // ✅ Debug log
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"📩 API Response: {responseContent}"); // ✅ Log full API response

            var userData = JsonSerializer.Deserialize<UserData>(responseContent);

            if (userData != null && !string.IsNullOrEmpty(userData.loginID))
            {
                await SecureStorage.SetAsync("isLoggedIn", "true");
                await SecureStorage.SetAsync("InspectorName", userData.loginID);
                Preferences.Set("InspectorName", userData.loginID);

                Debug.WriteLine($"✅ InspectorName stored: {userData.loginID}"); // ✅ Debug log
            }
            else
            {
                Debug.WriteLine("❌ Error: API response does not contain a valid LoginID."); // ✅ Debug log
            }

            return userData;
        }


        public async Task<string?> GetCurrentInspectorAsync()
        {
            var storedInspector = Preferences.Get("InspectorName", "Unknown"); // ✅ Try Preferences first
            if (!string.IsNullOrEmpty(storedInspector) && storedInspector != "Unknown")
            {
                return storedInspector;
            }

            storedInspector = await SecureStorage.GetAsync("InspectorName"); // ✅ Fallback to SecureStorage
            return storedInspector ?? "Unknown"; // Default to "Unknown" if null
        }




        public async Task LogoutAsync()
        {
            // ✅ Remove stored login states
            SecureStorage.Remove("isLoggedIn");
            SecureStorage.Remove("InspectorName");
            SecureStorage.Remove("currentUser");
            Preferences.Remove("InspectorName");

            // ✅ Force navigation to login page and reload
            await Task.Delay(100); // Short delay to ensure changes apply
        }

        public async Task SetLoggedInAsync()
        {
            await SecureStorage.SetAsync("isLoggedIn", "true");
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var isLoggedIn = await SecureStorage.GetAsync("isLoggedIn");
            return isLoggedIn == "true";
        }

        public async Task<string?> GetCurrentUserAsync()
        {
            var storedUser = await SecureStorage.GetAsync("username");
            Console.WriteLine($"Retrieved username from SecureStorage: {storedUser}"); // ✅ Debug log
            return storedUser;
        }

    }

    public class UserData
    {
        public string loginID { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
    }
}
