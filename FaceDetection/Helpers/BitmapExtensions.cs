using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace FaceDetection.Helpers
{
    public static class BitmapExtensions
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Converts an OpenCV Mat to a WPF compatible BitmapSource.
        /// This is an extension method for ease of use.
        /// </summary>
        public static BitmapSource ToBitmapSource(this Mat image)
        {
            if (image == null || image.IsDisposed) return null;

            // Using the OpenCvSharp helper for conversion
            return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(image);
        }
    }
}