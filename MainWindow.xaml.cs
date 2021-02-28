using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XamlAnimatedGif;
using BViewer.Properties;

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
                    if (!File.Exists(CurrentFile) || Files == null)
                    {
                        BuildFileArray();
                    }
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

        private bool _isPlaying;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
            }
        }

        private FileStream Stream { get; set; }

        public MainWindow() : this("") { }

        public MainWindow(string path)
        {
            InitializeComponent();
            InitializeSettings();

            image = FindName("Image") as Image;
            CurrentFile = path;
        }

        private void InitializeSettings()
        {
        }

        private void BuildFileArray()
        {
            var fileCollection = Directory.EnumerateFiles(CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".tga", StringComparison.OrdinalIgnoreCase)
                                 || s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));

            if ((bool)Settings.Default["Shuffle"])
            {
                Random random = new Random();
                fileCollection.OrderBy(c => random.Next());
            }

            Files = fileCollection.ToArray();
        }

        private void ShowImage(string path)
        {
            Stream?.Dispose();
            Stream = new FileStream(path, FileMode.Open);
            AnimationBehavior.SetSourceStream(image, Stream);

            string directory = Path.GetDirectoryName(path);
            if (CurrentDirectory.Length == 0 || CurrentDirectory.SequenceEqual(directory))
            {
                CurrentDirectory = directory;
            }
            Application.Current.MainWindow.Title = $"{Path.GetFileName(path)} ({CurrentDirectory}) - BViewer";
        }

        private void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
        }
        
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        private void NextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NextImageDirection = 1;
            CurrentFileIndex++;
        }

        private void PreviousCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PreviousCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NextImageDirection = -1;
            CurrentFileIndex--;
        }

        private void FullscreenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FullscreenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Fullscreen = !Fullscreen;
        }

        private void ExitSlideshowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Fullscreen;
        }

        private void ExitSlideshowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Fullscreen = false;
        }

        private void PlayPauseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Fullscreen;
        }

        private void PlayPauseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        private void ShuffleCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShuffleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default["Shuffle"] = !(bool)Settings.Default["Shuffle"];
            BuildFileArray();
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Next = new RoutedUICommand(
            "Next",
            "Next",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.Right) });

        public static readonly RoutedUICommand Previous = new RoutedUICommand(
            "Previous",
            "Previous",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.Left) });

        public static readonly RoutedUICommand Fullscreen = new RoutedUICommand(
            "Fullscreen",
            "Fullscreen",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.F5) });

        public static readonly RoutedUICommand ExitSlideshow = new RoutedUICommand(
            "ExitSlideshow",
            "ExitSlideshow",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.Escape) });

        public static readonly RoutedUICommand PlayPause = new RoutedUICommand(
            "PlayPause",
            "PlayPause",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.Space) });

        public static readonly RoutedUICommand Shuffle = new RoutedUICommand(
            "Shuffle",
            "Shuffle",
            typeof(CustomCommands)/*,
            new InputGestureCollection { new KeyGesture(Key.S) }*/);
    }
}
