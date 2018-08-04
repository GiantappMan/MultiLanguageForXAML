# MultiLanguageForXAML
针对WPF和UWP的通用多语言支持库

## [Nuget](https://www.nuget.org/packages/MultiLanguageForXAML/)

## 效果预览
* **WPF**

![steup](https://raw.githubusercontent.com/DaZiYuan/MultiLanguageForXAML/master/screenshots/WPF.gif)


* **UWP**

![steup](https://github.com/DaZiYuan/MultiLanguageForXAML/blob/master/screenshots/UWP.gif?raw=true)

## 用法

* **定义语言文件**
 ```Languages/json 编译时拷贝到目录
  //zh.json
    {
    "txt": "一",
    "format":"你好 {0} !",
    "world":"世界"
    }

//Languages/en.json 编译时拷贝到目录
    {
    "txt": "one",
    "format":"hello {0} !",
    "world":"world"
    }

 ```
* **初始化**
```csharp
            //WPF
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Languages");
            LanService.Init(new JsonDB(path), true);
            
            //UWP
            string path = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Languages");
            LanService.Init(new JsonDB(path), true);
```

* **XAML**
```XAML

<Window
    ...
    xmlns:lan="clr-namespace:MultiLanguageManager;assembly=MultiLanguageManager.WPF">
    <StackPanel>
        <Button lan:Xaml.Key="txt" />
        <Button lan:Xaml.Key="format">
            <lan:Xaml.Parameters>
                <lan:FormatParameters>
                    <Run
                        lan:Xaml.Key="world"
                        FontStyle="Italic"
                        Foreground="Red" />
                </lan:FormatParameters>
            </lan:Xaml.Parameters>
        </Button>
    </StackPanel>
</Window/>

```
* **切换语言**
```csharp
            //WPF
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CultureName);
            await LanService.UpdateLanguage();
            
            //UWP
            ApplicationLanguages.PrimaryLanguageOverride = CultureName;
            await LanService.UpdateLanguage();
```

全栈开发qq群：191034956
