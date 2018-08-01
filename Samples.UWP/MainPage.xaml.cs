using MultiLanguageManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Samples.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StackPanel stcPanel;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnReadInCode_Click(object sender, RoutedEventArgs e)
        {
            var l = await LanService.Get("btn_readInCode");
            ContentDialog d = new ContentDialog();
            d.PrimaryButtonText = l;
            d.Content = l;      
            await d.ShowAsync();
            Button btn = sender as Button;
            stcPanel.Children.Remove(btn);
        }

        private async void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
            string culture = item.Tag as string;
            ApplicationLanguages.PrimaryLanguageOverride = culture;
            await LanService.UpdateLanguage();
        }

        private void stcPanel_Loaded(object sender, RoutedEventArgs e)
        {
            stcPanel = sender as StackPanel;
        }
    }
}
