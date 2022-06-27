using MultiLanguageForXAML;
using MultiLanguageForXAML.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            LanService.Init(new EmbeddedJsonDB("Samples.WPF.EmbeddedConfig.Languages"), true, "en");
            MainWindow mainwindow = new();
            mainwindow.Show();
            base.OnStartup(e);
        }
    }
}
