using System;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
                    BuildFileArray();
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
                if (File.Exists(Files[CurrentFileIndex]))
                {
                    ShowImage(Files[CurrentFileIndex]);
                }
                else
                {
                    Files[CurrentFileIndex] = null;
                    CurrentFileIndex += NextImageDirection;
                }
            }
        }

        private int NextImageDirection { get; set; }

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

        private void BuildFileArray()
        {
            Files = Directory.EnumerateFiles(CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
        }

        private void ShowImage(string path)
        {
            Uri uri = new Uri(path);
            if (!path.EndsWith(".gif"))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = uri;
                bitmapImage.EndInit();
                image.Source = bitmapImage;
            }
            else
            {
                AnimationBehavior.SetSourceUri(image, uri);
            }
            
            string directory = Path.GetDirectoryName(path);
            if (CurrentDirectory.Length == 0 || CurrentDirectory.SequenceEqual(directory))
            {
                CurrentDirectory = directory;
            }
            Application.Current.MainWindow.Title = $"{Path.GetFileName(path)} ({CurrentDirectory}) - BViewer";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    Previous(null, null);
                    break;
                case Key.Right:
                    Next(null, null);
                    break;
                case Key.F10:
                    ToggleFullscreen();
                    break;
            }
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            NextImageDirection = 1;
            CurrentFileIndex++;
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            NextImageDirection = -1;
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
