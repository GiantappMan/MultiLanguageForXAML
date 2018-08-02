using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Documents;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

#endif



namespace MultiLanguageManager
{
    public class FormatParameters : Collection<object>
    {
        public string Test { get; set; }
    }

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
                     (sender, e) =>
                    {
                        bool isInDesignMode = CheckIsInDesignMode();
                        if (isInDesignMode)
                            return;

                        //不支持Run，因为WPF和UWP兼容有点恼火
                        FrameworkElement element = sender as FrameworkElement;
                        if (element != null)
                            element.Loaded += Element_Loaded;
                    })));

        private static async void Element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Loaded -= Element_Loaded;

            var key = element.GetValue(KeyProperty);
            if (key != null)
            {
                bool ok = await ApplyLanguage(element, key.ToString());

                if (ok && LanService.CanHotUpdate)
                {
                    _referencesElements.Add(new WeakReference<FrameworkElement>(element));
                }
            }
        }

        #endregion

        #region Parameters

        public static FormatParameters GetParameters(DependencyObject obj)
        {
            return (FormatParameters)obj.GetValue(ParametersProperty);
        }

        public static void SetParameters(DependencyObject obj, FormatParameters value)
        {
            obj.SetValue(ParametersProperty, value);
        }

        // Using a DependencyProperty as the backing store for Parameters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.RegisterAttached("Parameters", typeof(FormatParameters), typeof(Xaml), new PropertyMetadata(null));

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
                    //控件已释放
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

                //根据不同的类型有不同的赋值方式
                if (IsSampeOrSubClass(element.GetType(), typeof(TextBlock)))
                {
                    var parameter = element.GetValue(ParametersProperty) as FormatParameters;
                    List<Run> runs = GetRuns(parameter, lan);

                    if (runs != null && runs.Count > 0)
                    {
                        var tempTextBlock = element as TextBlock;
                        tempTextBlock.Inlines.Clear();
                        runs.ForEach((item) =>
                            {
                                tempTextBlock.Inlines.Add(item);
                            });
                    }
                }
                else
                    element.SetValue(targetProperty, lan);
                return true;
            }
            return false;
        }

        private static List<Run> GetRuns(FormatParameters formatParameters, string input)
        {
            List<Run> result = new List<Run>();
            //处理输入格式化参数
            if (formatParameters != null && formatParameters.Count > 0)
            {
                Match match = null;
                do
                {
                    match = Regex.Match(input, @"(.*?)({(\d+)})");
                    if (match.Groups.Count >= 4)
                    {
                        var source = match.Groups[0].Value;
                        var text = match.Groups[1].Value;
                        //插入正文
                        result.Add(new Run() { Text = text });

                        //插入format
                        var templateIndexStr = match.Groups[3].Value;
                        int.TryParse(templateIndexStr, out int templateIndex);
                        if (templateIndex < formatParameters.Count)
                        {
                            var item = formatParameters[templateIndex];
                            if (item is Run)
                                result.Add(item as Run);
                            else if (item is TextBlock)
                            {
                                var textBlock = item as TextBlock;
                                result.Add(new Run() { Text = textBlock.Text });
                                foreach (Run inlineItem in textBlock.Inlines)
                                    result.Add(inlineItem);
                            }
                            else
                                result.Add(new Run() { Text = item.ToString() });
                        }

                        //删除已经处理过的文字input
                        input = input.Remove(0, source.Length);
                    }
                } while (match != null && match.Success);

                if (!string.IsNullOrEmpty(input))
                    result.Add(new Run() { Text = input });

            }
            else
                result.Add(new Run() { Text = input });

            return result;
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
