using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJayApp.Data
{
    public class AppSettings
    {
        public string AppName { get; set; } = "Alan Jay Fleet";
        public string Version { get; set; } = "1.0.6";
        public string Theme { get; set; } = "Dark";
        public string FontSize { get; set; } = "Medium";
        public string AccentColor { get; set; } = "#007bff";
        public bool EnableCaching { get; set; } = true;
        public bool EnableCompression { get; set; } = false;
        public bool EnableCDN { get; set; } = false;
        public string Environment { get; set; } = "Development";
        public bool Enable2FA { get; set; } = false;
        public int AutoLogoutMinutes { get; set; } = 10;
        public bool ShowDebugLogs { get; set; } = false;
        public string Language { get; set; } = "English";
        
        public string BlobStorageConnectionString { get; set; }
    }
}


