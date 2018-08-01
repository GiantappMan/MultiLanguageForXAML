using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        static List<WeakReference<FrameworkElement>> _referencesElements = new List<WeakReference<FrameworkElement>>();

        static Xaml()
        {
            #region maps
            _maps.Add(typeof(TextBlock), TextBlock.TextProperty);
            _maps.Add(typeof(Button), ContentControl.ContentProperty);
            _maps.Add(typeof(ComboBoxItem), ContentControl.ContentProperty);

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

        internal static async Task UpdateLanguage()
        {
            if (_referencesElements.Count == 0)
                return;

            var oldList = new List<WeakReference<FrameworkElement>>(_referencesElements);
            var newList = new List<WeakReference<FrameworkElement>>();
            foreach (var item in oldList)
            {
                bool live = item.TryGetTarget(out FrameworkElement element);
                if (!live)
                    continue;
                newList.Add(item);

                var key = element.GetValue(KeyProperty);
                if (key != null)
                    await ApplyLanguage(element, key.ToString());
            }

            _referencesElements = newList;
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
                        FrameworkElement element = sender as FrameworkElement;
                        var key = e.NewValue.ToString();
                        await ApplyLanguage(element, key);
                    })
                ));

        //应用一个控件的语言
        private static async Task ApplyLanguage(FrameworkElement element, string key)
        {
            var lan = await LanService.Get(key);
            DependencyProperty targetProperty = MapProperty(element);
            if (targetProperty != null)
            {
                if (LanService.CanHotUpdate)
                {
                    _referencesElements.Add(new WeakReference<FrameworkElement>(element));
                }
                element.SetValue(targetProperty, lan);
            }
        }

        private static DependencyProperty MapProperty(FrameworkElement element)
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
