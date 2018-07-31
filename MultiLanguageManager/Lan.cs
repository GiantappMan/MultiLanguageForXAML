using System;

#if WINDOWS_UWP

using Windows.UI.Xaml;

#else

using System.Windows;

#endif



namespace MultiLanguageManager.WPF
{
    public class Lan
    {
        #region Key

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for LKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Lan), new PropertyMetadata(new PropertyChangedCallback(async (sender, e) =>
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Unloaded += Element_Unloaded;
                var key = e.NewValue.ToString();
                var lan = await LanService.SharedInstance.Get(key);
            })));

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
