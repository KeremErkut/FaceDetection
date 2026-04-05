using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

namespace FaceDetection.Services
{
    /// <summary>
    /// Handles camera stream operations using OpenCV's VideoCapture.
    /// Implementation focuses on capturing frames in a background thread.
    /// </summary>
    public class CameraService : IDisposable
    {
        private VideoCapture _capture;
        private CancellationTokenSource _cts;
        private readonly int _cameraId;

        // Event to notify the ViewModel when a new frame is ready
        public event EventHandler<Mat> FrameCaptured;

        public CameraService(int cameraId = 0)
        {
            _cameraId = cameraId;
        }

        public void Start()
        {
            _capture = new VideoCapture(_cameraId);
            if (!_capture.IsOpened())
                throw new Exception("Could not open camera.");

            _cts = new CancellationTokenSource();

            // Start capturing frames in a separate task to keep UI responsive
            Task.Run(() => CaptureLoop(_cts.Token));
        }

        private void CaptureLoop(CancellationToken token)
        {
            using var frame = new Mat();
            while (!token.IsCancellationRequested)
            {
                if (_capture.Read(frame) && !frame.Empty())
                {
                    // Trigger the event with a clone of the frame to avoid memory corruption
                    FrameCaptured?.Invoke(this, frame.Clone());
                }

                // Control frame rate (approx. 30 FPS)
                Thread.Sleep(33);
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            _capture?.Release();
        }

        public void Dispose()
        {
            Stop();
            _capture?.Dispose();
            _cts?.Dispose();
        }
    }
}