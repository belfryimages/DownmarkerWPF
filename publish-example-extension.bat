@echo off & setlocal enableextensions

set repo=C:\markpad-nuget-repository

echo Clear existing packages
del MarkPadExtension.Example.*.nupkg
del %repo%\MarkPadExtension.Example.*.nupkg

echo Packing example extension
.nuget\nuget pack src\MarkPadExtension.Example\MarkPadExtension.Example.csproj

echo Publishing to test repository
mkdir %repo%
copy MarkPadExtension.Example.*.nupkg %repo%

