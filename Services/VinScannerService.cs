using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlanJayApp.Components.Pages;


namespace AlanJayApp.Services
{
    public static class VinScannerService
    {
        // Define an event that is raised when a VIN is scanned.
        public static event Action<string> OnBarcodeScanned;

        public static void NotifyBarcodeScanned(string vin)
        {
            OnBarcodeScanned?.Invoke(vin);
        }
    }

}



