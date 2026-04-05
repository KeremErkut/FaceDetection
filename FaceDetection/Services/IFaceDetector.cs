using System.Collections.Generic;
using OpenCvSharp;

namespace FaceDetection.Services
{
    /// <summary>
    /// Defines the contract for face detection engines.
    /// This allows us to swap different detection algorithms (Haar, DNN, etc.) easily.
    /// </summary>
    public interface IFaceDetector
    {
        // Detects faces in the given image and returns their boundary rectangles
        IEnumerable<Rect> DetectFaces(Mat image);
    }
}