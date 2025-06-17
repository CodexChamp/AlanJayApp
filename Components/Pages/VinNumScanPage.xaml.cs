using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;                          // MediaPicker
using Microsoft.Maui.Storage;                        // FileSystem
using Microsoft.Maui.ApplicationModel.Communication; // Email APIs
using Microsoft.Maui.Dispatching;                    // MainThread
using Microsoft.Maui.Graphics;                       // Brush, Color, PathGeometry, etc.
using SkiaSharp.Views.Maui.Controls;
using Microsoft.AspNetCore.Components.WebView.Maui;
using AlanJayApp.Services;
using Camera.MAUI.ZXing;
using Microsoft.Maui.Controls.Shapes;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Path = System.IO.Path;
namespace AlanJayApp.Components.Pages
{
    public partial class VinNumScanPage : ContentPage
    {
        readonly string _initialVin;
        readonly BlobServiceClient _blobClient;// whatever came from the previous page
        string _currentVin;            // the one we show & upload        // ← the VIN passed in
        bool playing = false;
        readonly List<string> photoPaths = new();

        public VinNumScanPage()
            : this(ServiceHelper.GetService<BlobServiceClient>(), string.Empty)
        { }
        public VinNumScanPage(BlobServiceClient blobClient, string initialVin)
        {
            InitializeComponent();
            _initialVin = initialVin;
            _currentVin = initialVin;
            _blobClient = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            if (!string.IsNullOrWhiteSpace(_currentVin))
                barcodeResult.Text = $"VIN: {_currentVin}";

            // Scanner events
            cameraView.CamerasLoaded += CameraView_CamerasLoaded;
            cameraView.BarcodeDetected += CameraView_BarcodeDetected;

            // ZXing options
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new BarcodeDecodeOptions
            {
                AutoRotate = true,
                TryHarder = true,
                TryInverted = true,
                ReadMultipleCodes = false,
                PossibleFormats = new List<Camera.MAUI.BarcodeFormat>
                {
                    Camera.MAUI.BarcodeFormat.CODE_128,
                    Camera.MAUI.BarcodeFormat.CODE_93,
                    Camera.MAUI.BarcodeFormat.CODE_39,
                    Camera.MAUI.BarcodeFormat.QR_CODE
                }
            };

            // Shape toggle and initial states
            SetTriangleGeometry();
            controlButton.IsEnabled = false;
            uploadButton.IsEnabled = false;
        }

        // Once cameras list is ready, enable the toggle
        private async void CameraView_CamerasLoaded(object? sender, EventArgs e)
        {
            await Task.Delay(500);
            controlButton.IsEnabled = cameraView.NumCamerasDetected > 0;
        }

        // Start/Stop camera
        private void controlButton_Clicked(object sender, EventArgs e)
            => StartStopCamera();

        private async void StartStopCamera()
        {
            if (playing)
            {
                if (await cameraView.StopCameraAsync() == CameraResult.Success)
                {
                    playing = false;
                    controlButton.Text = "Start";
                    toggleShape.Fill = Brush.Green;
                    SetTriangleGeometry();
                }
            }
            else if (cameraView.NumCamerasDetected > 0)
            {
                cameraView.Camera = cameraView.Cameras.First();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (await cameraView.StartCameraAsync() == CameraResult.Success)
                    {
                        playing = true;
                        controlButton.Text = "Stop";
                        toggleShape.Fill = Brush.Red;
                        SetSquareGeometry();
                    }
                });
            }
        }

        // On each barcode detection
        private void CameraView_BarcodeDetected(object? s, BarcodeEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _currentVin = args.Result[0].Text?.Trim();
                barcodeResult.Text = $"VIN: {_currentVin}";

                cameraBorder.Stroke = Colors.Green;
                Task.Delay(1500).ContinueWith(_ =>
                    MainThread.BeginInvokeOnMainThread(() =>
                        cameraBorder.Stroke = Color.FromArgb("#CCCCCC")));

                uploadButton.IsEnabled =
                    !string.IsNullOrWhiteSpace(_currentVin) &&
                    photoPaths.Count > 0;
            });
        }

        // Navigate back
        private async void BackButton_Clicked(object sender, EventArgs e)
            => await Navigation.PopAsync();

        // Draw right-pointing triangle
        private void SetTriangleGeometry()
        {
            var geo = new PathGeometry();
            var fig = new PathFigure { StartPoint = new Point(5, 5), IsClosed = true, IsFilled = true };
            fig.Segments.Add(new LineSegment { Point = new Point(5, 65) });
            fig.Segments.Add(new LineSegment { Point = new Point(65, 35) });
            geo.Figures.Add(fig);
            toggleShape.Data = geo;
        }

        // Draw square
        private void SetSquareGeometry()
        {
            var geo = new PathGeometry();
            var fig = new PathFigure { StartPoint = new Point(5, 5), IsClosed = true, IsFilled = true };
            fig.Segments.Add(new LineSegment { Point = new Point(65, 5) });
            fig.Segments.Add(new LineSegment { Point = new Point(65, 65) });
            fig.Segments.Add(new LineSegment { Point = new Point(5, 65) });
            geo.Figures.Add(fig);
            toggleShape.Data = geo;
        }

        // ── Capture & thumbnail ──
        private async void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            if (!MediaPicker.Default.IsCaptureSupported)
                return;

            var result = await MediaPicker.Default.CapturePhotoAsync(
                new MediaPickerOptions { Title = "Take a VIN photo" });

            if (result == null)
                return;

            using var srcStream = await result.OpenReadAsync();
            var filename = System.IO.Path.GetFileName(result.FileName);
            var dest = System.IO.Path.Combine(FileSystem.AppDataDirectory, filename);

            using var destStream = System.IO.File.OpenWrite(dest);
            await srcStream.CopyToAsync(destStream);

            // store & display thumbnail
            photoPaths.Add(dest);
            var thumb = new Image
            {
                Source = ImageSource.FromFile(dest),
                WidthRequest = 100,
                HeightRequest = 100,
                Aspect = Aspect.AspectFill
            };
            photosContainer.Children.Add(thumb);

            uploadButton.IsEnabled =
                !string.IsNullOrWhiteSpace(_currentVin) &&
                photoPaths.Count > 0;
        }

        private async void OnUploadClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentVin))
            {
                await DisplayAlert("No VIN", "Scan or seed a VIN first.", "OK");
                return;
            }

            if (photoPaths.Count == 0)
            {
                await DisplayAlert("No Photos", "Capture at least one photo.", "OK");
                return;
            }

            try
            {
                var container = _blobClient.GetBlobContainerClient("vin-photos");
                await container.CreateIfNotExistsAsync(PublicAccessType.None);

                foreach (var path in photoPaths)
                {
                    var blobName = $"{_currentVin}/{Path.GetFileName(path)}";
                    var blob = container.GetBlobClient(blobName);

                    using var fs = File.OpenRead(path);
                    var headers = new BlobHttpHeaders { ContentType = "image/jpeg" };
                    await blob.UploadAsync(fs, headers);
                }

                await DisplayAlert("Done",
                    $"Uploaded {photoPaths.Count} photos for {_currentVin}.", "OK");

                photoPaths.Clear();
                photosContainer.Children.Clear();
                uploadButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Upload Failed", ex.Message, "OK");
            }
        }
    }
}