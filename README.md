# MultiLanguageForXAML

针对 WPF 的多语言支持库

## [Nuget](https://www.nuget.org/packages/MultiLanguageForXAML/)

## 效果预览

- **WPF**

![steup](https://raw.githubusercontent.com/DaZiYuan/MultiLanguageForXAML/master/screenshots/WPF.gif)

## 用法

- **定义语言文件**

```
//Languages/zh.json 编译时拷贝到目录
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

- **初始化**

```csharp
//WPF
//怀疑用Environment.CurrentDirectory开机启动时目录会出错，待验证
string appDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
string path = Path.Combine(appDir, "Res\\Languages");
LanService.Init(new JsonFileDB(path), true,"zh");
```

- **XAML**

```XAML

<Window
    ...
    xmlns:lan="clr-namespace:MultiLanguageForXAML;assembly=MultiLanguageForXAML">
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

- **切换语言**

```csharp
//WPF
LanService.UpdateCulture("en");
```

- **自定义控件映射（可选）**

```csharp
Xaml.CustomMaps.Add(typeof(CustomTitleBar), CustomTitleBar.TitleProperty);

```

## 广而告之

[应用推荐](https://giantapp.cn/categories/products)

全栈开发QQ群：191034956
