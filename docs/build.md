# 打包

使用nuget脚本
* vs2019 打开工程
* 选择release any cpu
* 运行Nuget/create-packages.cmd


使用命令打包，uwp暂时不行
dotnet pack MultiLanguageForXAML.WPF -o ../LocalNuget/Packages -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack MultiLanguageForXAML.UWP -o ../LocalNuget/Packages -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg