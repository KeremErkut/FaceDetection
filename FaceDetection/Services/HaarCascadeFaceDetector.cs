using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;

namespace FaceDetection.Services
{
    /// <summary>
    /// Professional implementation of face detection using Haar Cascades.
    /// This class includes a specific workaround for Encoding/BOM issues 
    /// commonly found when passing file paths from C# to Native OpenCV.
    /// </summary>
    public class HaarCascadeFaceDetector : IFaceDetector, IDisposable
    {
        private CascadeClassifier _faceClassifier;

        public HaarCascadeFaceDetector(string xmlFilePath)
        {
            try
            {
                // 1. Read the original XML content to a string.
                // This lets .NET handle any encoding/BOM issues of the source file.
                string xmlContent = File.ReadAllText(xmlFilePath);

                // 2. Create a temporary "clean" XML file in the system temp directory.
                // UTF8Encoding(false) ensures NO BOM (Byte Order Mark) is added, 
                // which often crashes the native OpenCV XML parser.
                string tempPath = Path.Combine(Path.GetTempPath(), "face_detector_clean.xml");
                File.WriteAllText(tempPath, xmlContent, new UTF8Encoding(false));

                // 3. Convert backslashes to forward slashes for C++ engine compatibility.
                string safePath = tempPath.Replace("\\", "/");

                // 4. Initialize the classifier with the sanitized path.
                _faceClassifier = new CascadeClassifier(safePath);

                // Validation: Ensure the classifier is actually populated.
                if (_faceClassifier.Empty())
                {
                    throw new Exception("The classifier was loaded but it is empty. Check XML schema.");
                }
            }
            catch (Exception ex)
            {
                // Catching and rethrowing with context helps in finding the exact failure point.
                throw new Exception($"Native Engine Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Detects faces within the provided BGR image.
        /// </summary>
        /// <param name="image">OpenCV Mat image (BGR format).</param>
        /// <returns>A collection of Rect objects representing face boundaries.</returns>
        public IEnumerable<Rect> DetectFaces(Mat image)
        {
            if (image == null || image.Empty())
                return Array.Empty<Rect>();

            // Grayscale conversion: Requirement for Haar Cascade algorithms.
            using var gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // Histogram equalization: Enhances contrast for more reliable detection.
            Cv2.EqualizeHist(gray, gray);

            // Detection logic
            // scaleFactor=1.1 : Downsizes image by 10% each pass to find faces of different sizes.
            // minNeighbors=5  : Filters out false positives by requiring 5 hits per region.
            var faces = _faceClassifier.DetectMultiScale(
                gray,
                scaleFactor: 1.1,
                minNeighbors: 5,
                minSize: new Size(30, 30));

            return faces;
        }

        /// <summary>
        /// Properly releases unmanaged OpenCV resources.
        /// </summary>
        public void Dispose()
        {
            _faceClassifier?.Dispose();
        }
    }
}