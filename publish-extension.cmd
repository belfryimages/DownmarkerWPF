@echo off & setlocal enableextensions
set extname=%1
set repo="%APPDATA%\MarkPad\Packages"

echo Publishing %extname% extension

cd src

echo Creating local repository
mkdir %repo%

echo Clearing existing packages
del %extname%.*.nupkg
del %repo%\%extname%.*.nupkg

echo Packing extension
.nuget\nuget pack Extensions\%extname%\%extname%.csproj

echo Publishing to local repository
copy %extname%.*.nupkg %repo%

cd ..
