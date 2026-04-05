using System;
using System.Collections.Generic;
using OpenCvSharp;

namespace FaceDetection.Services
{
    /// <summary>
    /// Implements face detection using the classic Haar Cascade algorithm.
    /// This engine is robust for front-facing detection.
    /// </summary>
    public class HaarCascadeFaceDetector : IFaceDetector
    {
        private readonly CascadeClassifier _faceClassifier;

        public HaarCascadeFaceDetector(string xmlFilePath)
        {
            // Initialize the classifier with the pre-trained model file
            _faceClassifier = new CascadeClassifier(xmlFilePath);
        }

        public IEnumerable<Rect> DetectFaces(Mat image)
        {
            if (image == null || image.Empty())
                return Array.Empty<Rect>();

            // Convert to grayscale as Haar Cascades work better/faster on single channel images
            using var gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // Equalize histogram to improve contrast
            Cv2.EqualizeHist(gray, gray);

            // Detect faces
            // scaleFactor: How much the image size is reduced at each image scale
            // minNeighbors: How many neighbors each candidate rectangle should have to retain it
            var faces = _faceClassifier.DetectMultiScale(
                gray,
                scaleFactor: 1.1,
                minNeighbors: 5,
                minSize: new Size(30, 30));

            return faces;
        }
    }
}