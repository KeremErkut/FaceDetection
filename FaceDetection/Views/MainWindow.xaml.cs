using System.Windows;
using FaceDetection.ViewModels;

namespace FaceDetection.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Following MVVM, this class only handles View initialization and DataContext binding.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the DataContext to our ViewModel to enable Data Binding
            // This links our UI elements to the logic in MainViewModel
            this.DataContext = new MainViewModel();
        }
    }
}