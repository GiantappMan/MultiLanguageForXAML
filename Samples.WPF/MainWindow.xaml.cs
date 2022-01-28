﻿using MultiLanguageForXAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Samples.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Languages");
            LanService.Init(new JsonDB(path), true);
            InitializeComponent();
        }

        private void BtnReadInCode_Click(object sender, RoutedEventArgs e)
        {
            var l = LanService.Get("btn_readInCode");
            MessageBox.Show(l);
            Button btn = sender as Button;
            stcPanel.Children.Remove(btn);
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
            string culture = item.Tag as string;

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            LanService.UpdateLanguage();
        }
    }
}
