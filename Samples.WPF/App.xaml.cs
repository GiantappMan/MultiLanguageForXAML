﻿using MultiLanguageForXAML;
using MultiLanguageForXAML.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Samples.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Languages");
            LanService.Init(new JsonFileDB(path), true, "en");
            MainWindow mainwindow = new();
            mainwindow.Show();
            base.OnStartup(e);
        }
    }
}
