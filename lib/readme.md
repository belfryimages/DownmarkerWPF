**Note re NuGet.Core**

The Nuget.Core available via NuGet has a bug when the application is 
started with a debugger attached and `IPackageRepository.GetPackages()` 
is called, a `TypeLoadException` relating to inheritance security rules is 
thrown (see [here](http://nuget.codeplex.com/discussions/246086)).

The `NuGet.Core.dll` here is built from source 
([6a435c53abb2 ](http://nuget.codeplex.com/SourceControl/changeset/changes/6a435c53abb2))
with the `SecurityTransparent` attribute in `NuGet.Core`'s `AssemblyInfo.cs` commented out.

