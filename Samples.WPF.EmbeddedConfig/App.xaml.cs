using MultiLanguageForXAML;
using MultiLanguageForXAML.DB;
using System.Windows;

namespace Samples.WPF.EmbeddedConfig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LanService.Init(new EmbeddedJsonDB("Samples.WPF.EmbeddedConfig.Languages"), true, "zh", "en");
            MainWindow mainwindow = new();
            mainwindow.Show();
            base.OnStartup(e);
        }
    }
}
