# 打包

使用nuget脚本
* vs2019 打开工程
* 选择release any cpu
* 运行Nuget/create-packages.cmd


使用vs
dotnet pack MultiLanguageForXAML.WPF -o ../LocalNuget/Packages
dotnet pack MultiLanguageForXAML.UWP -o ../LocalNuget/Packages