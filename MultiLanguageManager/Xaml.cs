using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;

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
#if WINDOWS_UWP

#else
            _maps.Add(typeof(HeaderedItemsControl), HeaderedItemsControl.HeaderProperty);
            _maps.Add(typeof(HeaderedContentControl), HeaderedContentControl.HeaderProperty);
            _maps.Add(typeof(Window), Window.TitleProperty);
            _maps.Add(typeof(Page), Page.TitleProperty);

#endif
            //必须放到最后，因为HeaderedContentControl继承自Content
            _maps.Add(typeof(ContentControl), ContentControl.ContentProperty);

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
                        bool isInDesignMode = CheckIsInDesignMode();
                        if (isInDesignMode)
                            return;

                        FrameworkElement element = sender as FrameworkElement;
                        var key = e.NewValue.ToString();
                        bool ok = await ApplyLanguage(element, key);

                        if (ok && LanService.CanHotUpdate)
                        {
                            _referencesElements.Add(new WeakReference<FrameworkElement>(element));
                        }
                    })));

        #endregion

        #region methods

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

        //应用一个控件的语言
        private static async Task<bool> ApplyLanguage(FrameworkElement element, string key)
        {
            var lan = await LanService.Get(key);
            DependencyProperty targetProperty = MapProperty(element);
            if (targetProperty != null)
            {
                element.SetValue(targetProperty, lan);
                return true;
            }
            return false;
        }

        private static bool IsSampeOrSubClass(Type type, Type type2)
        {
            bool result = type == type2;
            if (!result)
                result = type.GetTypeInfo().IsSubclassOf(type2);
            return result;
        }

        private static DependencyProperty MapProperty(FrameworkElement element)
        {
            DependencyProperty result = null;
            if (CustomMaps != null)
            {
                var temp = CustomMaps.FirstOrDefault(m => IsSampeOrSubClass(element.GetType(), m.Key));
                if (temp.Value != null)
                    result = temp.Value;
            }

            if (result != null)
                return result;

            var temp1 = _maps.FirstOrDefault(m => IsSampeOrSubClass(element.GetType(), m.Key));
            if (temp1.Value != null)
                result = temp1.Value;

            return result;
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Unloaded -= Element_Unloaded;
        }

        private static bool CheckIsInDesignMode()
        {
#if WINDOWS_UWP
            if (DesignMode.DesignModeEnabled)
                return true;
#else
            //防止设计器报错
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return true;
#endif
            return false;
        }

        #endregion
    }
}
