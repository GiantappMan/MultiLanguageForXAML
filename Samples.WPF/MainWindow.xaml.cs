using MultiLanguageForXAML;
using MultiLanguageForXAML.DB;
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
            CB.SelectionChanged += CB_SelectionChanged;
        }

        private void BtnReadInCode_Click(object sender, RoutedEventArgs e)
        {
            var l = LanService.Get("btn_readInCode");
            MessageBox.Show(l);
        }

        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem? item = e.AddedItems[0] as ComboBoxItem;
            string? culture = item?.Tag as string;

            LanService.UpdateCulture(culture);
        }
    }
}
