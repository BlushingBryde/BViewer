using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XamlAnimatedGif;

namespace BViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image image;

        private string _currentFile;
        public string CurrentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                if (File.Exists(value))
                {
                    _currentFile = value;
                    CurrentDirectory = Path.GetDirectoryName(CurrentFile);
                }
            }
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory;
            }
            set
            {
                if (!Utils.ComparePaths(CurrentDirectory, value))
                {
                    _currentDirectory = value;
                    Files = Directory.EnumerateFiles(CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.EndsWith(".png",  StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".bmp",  StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpg",  StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tif",  StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tga",  StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    CurrentFileIndex = Array.FindIndex(Files, (f) => Utils.ComparePaths(f, CurrentFile));
                }
            }
        }

        public string[] Files { get; set; }

        private int _currentFileIndex;
        public int CurrentFileIndex
        {
            get
            {
                return _currentFileIndex;
            }
            set
            {
                _currentFileIndex = ((value % Files.Length) + Files.Length) % Files.Length;
                ShowImage(Files[CurrentFileIndex]);
            }
        }

        private bool _fullscreen;
        public bool Fullscreen
        {
            get
            {
                return _fullscreen;
            }
            set
            {
                _fullscreen = value;
                WindowStyle = Fullscreen ? WindowStyle.None : WindowStyle.SingleBorderWindow;
                WindowState = Fullscreen ? WindowState.Maximized : WindowState.Normal;
            }
        }

        public MainWindow() : this("") { }

        public MainWindow(string path)
        {
            InitializeComponent();

            image = FindName("Image") as Image;
            CurrentFile = path;
        }

        private void ShowImage(string path)
        {
            Uri uri = new Uri(path);
            AnimationBehavior.SetSourceUri(image, uri);
            string directory = System.IO.Path.GetDirectoryName(path);
            if (CurrentDirectory.Length == 0 || CurrentDirectory.SequenceEqual(directory))
            {
                CurrentDirectory = directory;
            }
            Application.Current.MainWindow.Title = $"{System.IO.Path.GetFileName(path)} ({CurrentDirectory}) - BViewer";
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            CurrentFileIndex++;
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            CurrentFileIndex--;
        }

        private void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }
    }
}
