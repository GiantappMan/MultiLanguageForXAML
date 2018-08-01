using MultiLanguageManager;
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
            InitializeComponent();
        }

        private async void btnReadInCode_Click(object sender, RoutedEventArgs e)
        {
            var l = await LanService.Get("btn_readInCode");
            MessageBox.Show(l);
            Button btn = sender as Button;
            stcPanel.Children.Remove(btn);
        }

        private async void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
            string culture = item.Tag as string;

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            await LanService.UpdateLanguage();
        }
    }
}
