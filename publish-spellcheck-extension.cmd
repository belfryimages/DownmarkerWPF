@echo off & setlocal enableextensions

cd src

set repo="%APPDATA%\MarkPad\Packages"

echo Creating local repository
mkdir %repo%

echo Clearing existing packages
del SpellCheck.*.nupkg
del %repo%\SpellCheck.*.nupkg

echo Packing SpellCheck extension
.nuget\nuget pack Extensions\SpellCheck\SpellCheck.csproj

echo Publishing to local repository
copy SpellCheck.*.nupkg %repo%

cd ..
