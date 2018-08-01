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



namespace MultiLanguageManager
{
    public class Xaml : DependencyObject
    {
        static Dictionary<Type, DependencyProperty> _maps = new Dictionary<Type, DependencyProperty>();

        static Xaml()
        {
            #region maps
            _maps.Add(typeof(Button), ContentControl.ContentProperty);
            _maps.Add(typeof(TextBlock), TextBlock.TextProperty);

#if WINDOWS_UWP

#else
            _maps.Add(typeof(Label), ContentControl.ContentProperty);
            _maps.Add(typeof(TabItem), HeaderedContentControl.HeaderProperty);
            _maps.Add(typeof(Expander), HeaderedContentControl.HeaderProperty);

#endif
            #endregion
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

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Xaml), new PropertyMetadata(null,
                new PropertyChangedCallback(
                    async (sender, e) =>
                    {
                        //WeakReference
                        FrameworkElement element = sender as FrameworkElement;

                        var key = e.NewValue.ToString();
                        var lan = await LanService.Get(key);

                        DependencyProperty targetProperty = GetTargetProperty(element);
                        if (targetProperty != null)
                            element.SetValue(targetProperty, lan);

                    })
                ));

        private static DependencyProperty GetTargetProperty(FrameworkElement element)
        {
            DependencyProperty result = null;
            if (CustomMaps != null)
            {
                var temp = CustomMaps.FirstOrDefault(m => m.Key == element.GetType());
                if (temp.Value != null)
                    result = temp.Value;
            }

            if (result != null)
                return result;

            var temp1 = _maps.FirstOrDefault(m => m.Key == element.GetType());
            if (temp1.Value != null)
                result = temp1.Value;

            return result;
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Unloaded -= Element_Unloaded;
        }

        #endregion
    }
}
