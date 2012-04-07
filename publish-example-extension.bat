@echo off & setlocal enableextensions

set repo=C:\markpad-nuget-repository

echo Packing example extension
.nuget\nuget pack src\MarkPadExtension.Example\MarkPadExtension.Example.csproj

echo Publishing to test repository
mkdir %repo%
copy MarkPadExtension.Example.*.nupkg %repo%

