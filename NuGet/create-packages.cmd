@ECHO OFF
del *.nupkg
.\nuget.exe pack .\MultiLanguageForXAML.nuspec -symbols
