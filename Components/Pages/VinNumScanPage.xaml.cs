using System.Diagnostics; // For Debug.WriteLine
using Camera.MAUI;
using Camera.MAUI.ZXing;
using Camera.MAUI.ZXingHelper;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Dispatching; // For MainThread
using System;
using System.Linq;
using AlanJayApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Shapes; // For Path, PathGeometry, etc.
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace AlanJayApp.Components.Pages
{
    public partial class VinNumScanPage : ContentPage
    {
        // Track if the camera is playing
        bool playing = false;
        // Holds the currently blurred bitmap image for drawing

        public VinNumScanPage()
        {
            InitializeComponent();

            // Subscribe to camera events
            cameraView.CamerasLoaded += CameraView_CamerasLoaded;
            cameraView.BarcodeDetected += CameraView_BarcodeDetected;

            // Set up barcode detection options
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new BarcodeDecodeOptions
            {
                AutoRotate = true,
                PossibleFormats = {
                    Camera.MAUI.BarcodeFormat.CODE_128,
                    Camera.MAUI.BarcodeFormat.CODE_93,
                    Camera.MAUI.BarcodeFormat.CODE_39,
                    Camera.MAUI.BarcodeFormat.QR_CODE
                },
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true
            };

            // Initialize the custom toggle control with a green triangle.
            SetTriangleGeometry();
            toggleShape.Fill = Brush.Green;
        }

        // Called when the cameras have been loaded. Enables the control button after a short delay.
        private async void CameraView_CamerasLoaded(object? sender, EventArgs e)
        {
            await Task.Delay(1000);
            if (cameraView.NumCamerasDetected > 0)
            {
                controlButton.IsEnabled = true;
            }
        }

        // Toggles the camera on and off, and updates the toggle control's shape accordingly.
        private async void StartStopCamera()
        {
            if (playing)
            {
                if (await cameraView.StopCameraAsync() == CameraResult.Success)
                {
                    playing = false;
                    controlButton.Text = "Start";
                    // Revert the toggle control to a green triangle.
                    toggleShape.Fill = Brush.Green;
                    SetTriangleGeometry();
                }
            }
            else
            {
                if (cameraView.NumCamerasDetected > 0)
                {
                    cameraView.Camera = cameraView.Cameras.First();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (await cameraView.StartCameraAsync() == CameraResult.Success)
                        {
                            controlButton.Text = "Stop";
                            playing = true;
                            // Change the toggle control to a red square when the camera is on.
                            toggleShape.Fill = Brush.Red;
                            SetSquareGeometry();
                        }
                    });
                }
            }
        }

        // Event handler for the control button click. Toggles camera start/stop.
        private void controlButton_Clicked(object sender, EventArgs e)
        {
            StartStopCamera();
        }

        // Event handler for barcode detection.
        private void CameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var vin = args.Result[0].Text?.Trim();
                barcodeResult.Text = string.Format("BarcodeText on {0:HH:mm:ss}: {1}", DateTime.Now, vin);
                VinScannerService.NotifyBarcodeScanned(vin);

                // Change border stroke to green for a thicker effect
                cameraBorder.Stroke = Colors.Green;

                // Revert after 1.5 seconds
                Task.Delay(1500).ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        cameraBorder.Stroke = Color.FromArgb("#CCCCCC");
                    });
                });
            });
        }



        // Event handler for the back button to navigate back.
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // ---------------------------------------------------------------------
        // SKCanvasView PaintSurface event handlers for blurred background halves.
        // ---------------------------------------------------------------------

       

        // ---------------------------------------------------------------------
        // Helper Methods for the Toggle Control UI
        // ---------------------------------------------------------------------

        // Sets the toggle control shape to a green triangle (pointing right).
        private void SetTriangleGeometry()
        {
            var triangleGeometry = new PathGeometry();
            var triangleFigure = new PathFigure
            {
                StartPoint = new Point(5, 5),
                IsClosed = true,
                IsFilled = true
            };
            // Triangle: from (5,5) to (5,65) to (65,35) forms a right-pointing triangle.
            triangleFigure.Segments.Add(new LineSegment { Point = new Point(5, 65) });
            triangleFigure.Segments.Add(new LineSegment { Point = new Point(65, 35) });
            triangleGeometry.Figures.Add(triangleFigure);
            toggleShape.Data = triangleGeometry;
        }

        // Sets the toggle control shape to a red square.
        private void SetSquareGeometry()
        {
            var squareGeometry = new PathGeometry();
            var squareFigure = new PathFigure
            {
                StartPoint = new Point(5, 5),
                IsClosed = true,
                IsFilled = true
            };
            // Square: (5,5) -> (65,5) -> (65,65) -> (5,65)
            squareFigure.Segments.Add(new LineSegment { Point = new Point(65, 5) });
            squareFigure.Segments.Add(new LineSegment { Point = new Point(65, 65) });
            squareFigure.Segments.Add(new LineSegment { Point = new Point(5, 65) });
            squareGeometry.Figures.Add(squareFigure);
            toggleShape.Data = squareGeometry;
        }
    }
}
