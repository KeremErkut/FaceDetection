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
            // Important: Using 'using' ensures the Mat is disposed to prevent memory leaks
            using (frame)
            {
                // 1. Run the detection algorithm
                var faces = _faceDetector.DetectFaces(frame);

                // 2. Calculate the count safely using LINQ
                // OpenCvSharp usually returns an array, so 'as List<Rect>' might return null.
                // .Count() is the most robust way here.
                int currentFaceCount = System.Linq.Enumerable.Count(faces);

                // 3. Draw rectangles on the frame for visual feedback
                foreach (var rect in faces)
                {
                    Cv2.Rectangle(frame, rect, Scalar.Red, thickness: 2);
                }

                // 4. Update UI properties ONLY on the UI Dispatcher thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // This triggers PropertyChanged notifications for the UI
                    FaceCount = currentFaceCount;
                    CurrentFrame = frame.ToBitmapSource();
                });
            }
        }
    }
}