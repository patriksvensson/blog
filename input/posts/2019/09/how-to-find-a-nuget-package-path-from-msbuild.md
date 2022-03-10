---
Title: How to find a NuGet package path from MSBuild
Slug: how-to-find-a-nuget-package-path-from-msbuild
Date: 2019-09-30
RedirectFrom: 2019/09/how-to-find-a-nuget-package-path-from-msbuild/index.html
Tags:
- .NET
- NuGet
- MSBuild
---

Lately I've been porting some projects at a client from .NET Framework to .NET Core, 
and as part  of that I had to convert `csproj` files from the old project format 
to [the new one](https://docs.microsoft.com/en-us/dotnet/core/tools/csproj). 
That means getting rid of package references in `packages.config` 
and replacing them with `PackageReference` elements in the project files.

When doing this, I noticed that some files that was part of a NuGet package's 
[TFM](https://docs.microsoft.com/en-us/dotnet/standard/frameworks) folder 
wasn't copied to the application's output folder anymore. 
As annoying as breaking changes are I realized that this change was an improvement 
in many ways, but I still needed to find a way to solve the problem.

<!--excerpt-->

There was a [feature added](https://github.com/NuGet/NuGet.Client/pull/2271) 
in the NuGet client library that was shipped with 
Visual Studio 15.9.9 that introduced a new attribute to the `PackageReference` 
element called `GeneratePathProperty`. What this attribute does is that it creates 
a new MSBuild property that is set to the package's root path on disk. 

Knowing where the NuGet package would be unpacked on disk allowed me to write a 
MSBuild target, that after building my project would copy the file to the output 
directory.

```xml
<!-- Specify what packages we're dependent on -->
<ItemGroup>
    <PackageReference Include="Foo.Bar" 
                      Version="1.2.3" 
                      GeneratePathProperty="true" />
</ItemGroup>

<!-- Copy the files to the output directory -->
<Target Name="CopyFooBarFiles" AfterTargets="Build">
    <Copy Condition="'$(TargetFramework)' == 'net462'" 
          SourceFiles="$(PkgFoo_Bar)\lib\net40\qux.dat" 
          DestinationFolder="$(OutDir)" />
</Target>
```

So by setting `GeneratePathProperty="true"` for the `Foo.Bar` reference above, an
MSBuild property named `PkgFoo_Bar` automatically gets created set to the path to
the package on disk.