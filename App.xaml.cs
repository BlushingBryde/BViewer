using BViewer.Properties;
using System.Configuration;
using System.IO;
using System.Windows;

namespace BViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow;

            // If a file is passed as an argument, show it
            if (e.Args.Length > 0 && File.Exists(e.Args[0]))
            {
                mainWindow = new MainWindow(e.Args[0]);
            }
            else
            {
                mainWindow = new MainWindow();
            }

            mainWindow.Show();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
