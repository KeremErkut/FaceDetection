using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaceDetection.Services;
using FaceDetection.Helpers;
using OpenCvSharp;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace FaceDetection.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CameraService _cameraService;
        private readonly IFaceDetector _faceDetector;

        [ObservableProperty]
        private BitmapSource _currentFrame;

        [ObservableProperty]
        private int _faceCount;

        public MainViewModel()
        {
            // For now, we hardcode the path. Later this can come from a config or UI.
            string modelPath = "Resources/haarcascade_frontalface_default.xml";

            _faceDetector = new HaarCascadeFaceDetector(modelPath);
            _cameraService = new CameraService();

            // Subscribe to the camera event
            _cameraService.FrameCaptured += OnFrameCaptured;
        }

        [RelayCommand]
        private void StartCamera() => _cameraService.Start();

        [RelayCommand]
        private void StopCamera() => _cameraService.Stop();

        private void OnFrameCaptured(object sender, Mat frame)
        {
            using (frame) // Ensure the Mat is disposed after processing
            {
                // 1. Detect faces
                var faces = _faceDetector.DetectFaces(frame);
                FaceCount = (faces as List<Rect>)?.Count ?? 0;

                // 2. Draw rectangles on the frame for visual feedback
                foreach (var rect in faces)
                {
                    Cv2.Rectangle(frame, rect, Scalar.Red, thickness: 2);
                }

                // 3. Convert to WPF format and update the UI
                // Application.Current.Dispatcher is needed because the event comes from a background thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentFrame = frame.ToBitmapSource();
                });
            }
        }
    }
}