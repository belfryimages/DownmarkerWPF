@echo off & setlocal enableextensions

cd src

set repo="%APPDATA%\MarkPad\Packages"

echo Creating local repository
mkdir %repo%

echo Clearing existing packages
del MarkPadExtension.Example.*.nupkg
del %repo%\MarkPadExtension.Example.*.nupkg

echo Packing example extension
.nuget\nuget pack MarkPadExtension.Example\MarkPadExtension.Example.csproj

echo Publishing to local repository
copy MarkPadExtension.Example.*.nupkg %repo%

cd ..
