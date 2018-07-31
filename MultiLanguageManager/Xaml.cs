using System;
using System.Collections.Generic;
using System.Linq;

#if WINDOWS_UWP

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else

using System.Windows;
using System.Windows.Controls;

#endif



namespace MultiLanguageManager.WPF
{
    public class Xaml
    {
        static Dictionary<Type, DependencyProperty> _maps = new Dictionary<Type, DependencyProperty>();

        static Xaml()
        {
            _maps.Add(typeof(Button), ContentControl.ContentProperty);
        }

        public static Dictionary<Type, DependencyProperty> CustomMaps { private set; get; } = new Dictionary<Type, DependencyProperty>();


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
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Xaml), new PropertyMetadata(new PropertyChangedCallback(async (sender, e) =>
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Unloaded += Element_Unloaded;

                var key = e.NewValue.ToString();
                var lan = await LanService.Get(key);

                DependencyProperty targetProperty = GetTargetProperty(element);
                element.SetValue(targetProperty, lan);
            })));

        private static DependencyProperty GetTargetProperty(FrameworkElement element)
        {
            DependencyProperty result = null;
            if (CustomMaps != null)
            {
                var temp = CustomMaps.FirstOrDefault(m => m.Key == element.GetType());
                if (temp.Value != null)
                    result = temp.Value;
            }

            return result;
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
