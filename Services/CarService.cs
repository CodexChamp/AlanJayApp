using System;
using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace AlanJayApp.Services
{
    public class CarService
    {
        public int CarCount
        {
            get => Preferences.Get("CarCount", 0); // Load persisted value
            private set
            {
                Preferences.Set("CarCount", value); // Save to local storage
            }
        }

        public event Action? OnCarCountChanged;

        public void AddCar()
        {
            CarCount++; // Increase count
            Debug.WriteLine($"[CarService] CarCount increased: {CarCount}");
            OnCarCountChanged?.Invoke(); // Notify UI
        }

        public void RemoveCar()
        {
            if (CarCount > 0)
            {
                CarCount--; // Decrease count
                Debug.WriteLine($"[CarService] CarCount decreased: {CarCount}");
                OnCarCountChanged?.Invoke();
            }
            else
            {
                Debug.WriteLine($"[CarService] CarCount is already at 0, cannot decrease.");
            }
        }
    }
}
