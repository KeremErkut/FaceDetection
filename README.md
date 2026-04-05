# FaceDetection

## Description
This application is a real-time face detection system developed using C# and OpenCV. It is built upon the MVVM (Model-View-ViewModel) architectural pattern to ensure a clean separation of concerns and high maintainability. The engine utilizes the Haar Cascade algorithm for efficient face tracking via a live camera feed. A key technical feature of this implementation is a custom file-sanitization process that handles Encoding/BOM issues during the transition between the Managed C# and Native C++ layers, ensuring robust XML model loading.

## Tech Stack
- Framework: .NET 9.0 / WPF

## Libraries:
- OpenCvSharp4: A comprehensive .NET wrapper for OpenCV.

- CommunityToolkit.Mvvm: A modern, fast, and modular MVVM library for .NET development.
