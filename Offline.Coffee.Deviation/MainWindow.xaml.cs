using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace Offline.Coffee.Devication;

public partial class MainWindow : Window
{
    #region Fields  
    private readonly Yolo _yolo;
    private SKImage _currentFrame;
    private readonly Dispatcher _dispatcher;
    private VideoCapture _capture;
    private DispatcherTimer _timer;
    #endregion

    #region Constants  
    const int WEBCAM_WIDTH = 1080;
    const int WEBCAM_HEIGHT = 608;
    const int FPS = 30;
    const string FRAME_FORMAT_EXTENSION = ".png";
    #endregion

    public MainWindow()
    {
        InitializeComponent();

        // Instantiate YOLO  
        _yolo = new Yolo(new YoloOptions
        {
            OnnxModel = "yolov11s.onnx",
            ModelType = ModelType.ObjectDetection,
            Cuda = false,
            PrimeGpu = false
        });

        _dispatcher = Dispatcher.CurrentDispatcher;
        _currentFrame = SKImage.FromBitmap(new SKBitmap(WEBCAM_WIDTH, WEBCAM_HEIGHT));

        // Configure webcam  
        _capture = new VideoCapture(0, VideoCapture.API.DShow);
        _capture.Set(CapProp.Fps, FPS);
        _capture.Set(CapProp.FrameWidth, WEBCAM_WIDTH);
        _capture.Set(CapProp.FrameHeight, WEBCAM_HEIGHT);

        // Set up a timer to capture frames  
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000 / FPS)
        };
        _timer.Tick += CaptureFrame;
        _timer.Start();
    }

    private void CaptureFrame(object sender, EventArgs e)
    {
        try
        {
            using var mat = new Mat();
            using var buffer = new VectorOfByte();

            // Capture current frame from webcam  
            if (!_capture.Read(mat) || mat.IsEmpty)
            {
                return; // Skip if frame is not captured  
            }

            // Encode mat to a valid image format and to a buffer  
            CvInvoke.Imencode(FRAME_FORMAT_EXTENSION, mat, buffer);

            // Read buffer to an SKImage  
            _currentFrame = SKImage.FromEncodedData(buffer);

            // Clean up  
            buffer.Clear();

            // Run inference on frame  
            var results = _yolo.RunObjectDetection(_currentFrame);

            // Check if a cup is detected  
            bool cupDetected = results.Any(result => result.Label.Name == "cup");

            // Update the TextBlock based on detection  
            _dispatcher.Invoke(() =>
            {
                DetectionMessage.Visibility = cupDetected ? Visibility.Visible : Visibility.Collapsed;
                DetectionMessage.Text = cupDetected ? "Deviation Detected!" : string.Empty;
            });

            // Draw results  
            _currentFrame = _currentFrame.Draw(results);

            // Update GUI  
            WebCamFrame.InvalidateVisual();
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log them)  
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void UpdateWebcamFrame(object sender, SKPaintSurfaceEventArgs e)
    {
        using var canvas = e.Surface.Canvas;
        var info = e.Info;
        var scaleX = (float)info.Width / WEBCAM_WIDTH;
        var scaleY = (float)info.Height / WEBCAM_HEIGHT;
        var scale = Math.Min(scaleX, scaleY);

        var scaledWidth = scale * WEBCAM_WIDTH;
        var scaledHeight = scale * WEBCAM_HEIGHT;
        var x = (info.Width - scaledWidth) / 2;
        var y = (info.Height - scaledHeight) / 2;

        canvas.Clear(SKColors.Black);
        canvas.DrawImage(_currentFrame, new SKRect(x, y, x + scaledWidth, y + scaledHeight));
        canvas.Flush();
    }

    private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _timer.Stop();
        _capture?.Dispose();
        _yolo?.Dispose();
        _currentFrame?.Dispose();
    }
}