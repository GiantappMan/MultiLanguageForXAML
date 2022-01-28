using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace MultiLanguageForXAML
{
    public class FormatParameters : Collection<object>
    {
    }

    public class Xaml : DependencyObject
    {
        static readonly Dictionary<Type, DependencyProperty> _maps = new();
        static List<WeakReference<FrameworkElement>> _referencesElements = new();
        static List<WeakReference<Run>> _referencesRuns = new();

        static Xaml()
        {
            #region maps
            _maps.Add(typeof(TextBlock), TextBlock.TextProperty);
            _maps.Add(typeof(HeaderedItemsControl), HeaderedItemsControl.HeaderProperty);
            _maps.Add(typeof(HeaderedContentControl), HeaderedContentControl.HeaderProperty);
            _maps.Add(typeof(Window), Window.TitleProperty);
            _maps.Add(typeof(Page), Page.TitleProperty);

            //必须放到最后，因为HeaderedContentControl继承自Content
            _maps.Add(typeof(ContentControl), ContentControl.ContentProperty);
            #endregion
        }

        public static Dictionary<Type, DependencyProperty> CustomMaps { private set; get; } = new Dictionary<Type, DependencyProperty>();

        #region ApplyImmediately

        public static bool GetApplyImmediately(DependencyObject obj)
        {
            return (bool)obj.GetValue(ApplyImmediatelyProperty);
        }

        public static void SetApplyImmediately(DependencyObject obj, bool value)
        {
            obj.SetValue(ApplyImmediatelyProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplyImmediately.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplyImmediatelyProperty =
            DependencyProperty.RegisterAttached("ApplyImmediately", typeof(bool), typeof(Xaml), new PropertyMetadata(false));

        #endregion

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

                        if (sender is FrameworkElement element)
                        {
                            bool immediately = GetApplyImmediately(element);
                            if (immediately)
                                ApplyFrameworkElement(element);
                            else
                                element.Loaded += Element_Loaded;
                        }
                        else if (sender is Run run)
                        {
                            var key = run.GetValue(KeyProperty);
                            if (key != null)
                            {
                                bool ok = ApplyLanguage(run, key.ToString()!, (bool)run.GetValue(ToolTipProperty));

                                if (ok && LanService.CanHotUpdate)
                                    _referencesRuns.Add(new WeakReference<Run>(run));
                            }
                        }
                    })));

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Loaded -= Element_Loaded;
                ApplyFrameworkElement(element);
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

        #region ToolTip

        public static bool GetToolTip(DependencyObject obj)
        {
            return (bool)obj.GetValue(ToolTipProperty);
        }

        public static void SetToolTip(DependencyObject obj, bool value)
        {
            obj.SetValue(ToolTipProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplyTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.RegisterAttached("ToolTip", typeof(bool), typeof(Xaml), new PropertyMetadata(false));

        #endregion

        #region methods

        internal static void UpdateLanguage()
        {
            //处理控件
            if (_referencesElements.Count > 0)
            {
                var oldList = new List<WeakReference<FrameworkElement>>(_referencesElements);
                var newList = new List<WeakReference<FrameworkElement>>();
                foreach (var item in oldList)
                {
                    bool live = item.TryGetTarget(out var element);
                    if (!live || element == null)
                        //控件已释放
                        continue;
                    newList.Add(item);

                    var key = element.GetValue(KeyProperty);
                    if (key != null)
                        ApplyLanguage(element, key.ToString()!, (bool)element.GetValue(ToolTipProperty));
                }

                _referencesElements = newList;
            }
            //处理run
            if (_referencesRuns.Count > 0)
            {
                var oldList = new List<WeakReference<Run>>(_referencesRuns);
                var newList = new List<WeakReference<Run>>();
                foreach (var item in oldList)
                {
                    bool live = item.TryGetTarget(out Run? element);
                    if (!live || element == null)
                        //控件已释放
                        continue;
                    newList.Add(item);

                    var key = element.GetValue(KeyProperty);
                    if (key != null)
                        ApplyLanguage(element, key.ToString()!, (bool)element.GetValue(ToolTipProperty));
                }

                _referencesRuns = newList;
            }
        }

        private static void ApplyFrameworkElement(DependencyObject element)
        {
            var key = element.GetValue(KeyProperty);
            if (key != null)
            {
                bool ok = ApplyLanguage(element, key.ToString()!, (bool)element.GetValue(ToolTipProperty));

                if (ok && LanService.CanHotUpdate)
                {
                    if (element is FrameworkElement frameworkElement)
                        _referencesElements.Add(new WeakReference<FrameworkElement>(frameworkElement));
                }
            }
        }

        //应用一个控件的语言
        private static bool ApplyLanguage(DependencyObject element, string key, bool applyTooltips)
        {
            var lan = LanService.Get(key);
            DependencyProperty? targetProperty = MapProperty(element);
            if (targetProperty != null)
            {
                if (applyTooltips)
                    element.SetValue(ToolTipService.ToolTipProperty, lan);

                //需要格式化字符串
                if (element.GetValue(ParametersProperty) is FormatParameters parameter &&
                    parameter.Count > 0 &&
                    !IsSampeOrSubClass(element.GetType(), typeof(Window)) &&
                    !IsSampeOrSubClass(element.GetType(), typeof(Page)) &&
                    (IsSampeOrSubClass(element.GetType(), typeof(TextBlock)) ||
                    IsSampeOrSubClass(element.GetType(), typeof(ContentControl))
                    ))
                {
                    List<Run>? runs = GetRuns(parameter, lan);

                    if (runs != null && runs.Count > 0)
                    {
                        var tempTextBlock = element as TextBlock;

                        if (element is HeaderedContentControl tempHeaderContentControl)
                        {
                            tempTextBlock = new TextBlock();
                            tempHeaderContentControl.Header = tempTextBlock;
                        }

                        else if (element is ContentControl tempContentControl)
                        {
                            tempTextBlock = new TextBlock();
                            tempContentControl.Content = tempTextBlock;
                        }

                        if (tempTextBlock != null)
                        {
                            tempTextBlock.Inlines.Clear();
                            runs.ForEach((item) =>
                            {
                                tempTextBlock.Inlines.Add(item);
                            });
                        }
                    }
                }
                else
                    //直接设置字符串的情况
                    element.SetValue(targetProperty, lan);
                return true;
            }
            else if (element is Run run)
            {//run特殊处理
                run.Text = lan;
                if (applyTooltips)
                    run.SetValue(ToolTipService.ToolTipProperty, lan);

                return true;
            }

            return false;
        }

        private static List<Run>? GetRuns(FormatParameters formatParameters, string? input)
        {
            if (input == null)
                return null;

            List<Run> result = new();
            //处理输入格式化参数
            if (formatParameters != null && formatParameters.Count > 0)
            {
                Match match;
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
                        _ = int.TryParse(templateIndexStr, out int templateIndex);
                        if (templateIndex < formatParameters.Count)
                        {
                            var item = formatParameters[templateIndex];
                            if (item is Run tmpRun)
                                result.Add(tmpRun);
                            else if (item is TextBlock textBlock)
                            {
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

        private static DependencyProperty? MapProperty(DependencyObject element)
        {
            if (element is Run)
                return null;

            DependencyProperty? result = null;

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
            //防止设计器报错
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return true;
            return false;
        }

        #endregion
    }
}
