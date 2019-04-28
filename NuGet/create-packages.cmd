@ECHO OFF
del *.nupkg
.\nuget.exe pack .\MultiLanguageForXAML.nuspec -OutputDirectory ..\..\LocalNuget\Packages -symbols
