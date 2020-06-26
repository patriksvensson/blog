---
Title: Deploying to NuGet from GitHub Actions using Cake and MinVer
Slug: deploying-to-nuget-from-github-actions-using-cake-and-minver
Published: 2020-06-22
Tags:
- .NET
- Cake
- MinVer
- GitHub
- GitHub Actions
---

I recently started moving some builds from TeamCity, Travis, and AppVeyor to GitHub actions, 
and while doing that, I thought I would also move to a more straightforward deployment process.  

Before, I used to push to NuGet by tagging the main branch, and while this is a perfectly reasonable 
approach, I wanted to change this to publish previews to NuGet on every push to the main branch.
If it's a tagged commit on the main branch, a non-preview version should be published.

To calculate the version number from Git history, I'll use 
[MinVer](https://github.com/adamralph/minver) by Adam Ralph.  
I've been a long-time user of [GitVersion](https://github.com/GitTools/GitVersion), 
which always worked well for me, but I wanted to try out something else for this.

## Install prerequisites

Before gettings started, let's add Cake and MinVer as local tools in the repository.

```
> dotnet add tool-manifest
> dotnet tool install cake.tool
> dotnet tool install minver-cli
> dotnet tool restore
```

There should now be a new directory in your repository root called `.config` which contains 
the .NET tool manifest `dotnet-tools.json`. By restoring the tools in the repository root, 
we can run them without installing anything globally on our computer. Make sure that this 
folder is not excluded in your _.gitignore_ file.

```
# Restore tools
> dotnet tool restore

# Run cake
> dotnet cake

# Run minver
> dotnet minver
```

## The GitHub Actions YAML

Now let us create our GitHub Actions YAML file that describes how to bootstrap
our build process.

```yaml
name: Publish

on:
  push:
    branches:
      - main

jobs:
  release:
    name: Release
    if: "!contains(github.event.head_commit.message, 'skip-ci')"
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v2
        with:
          fetch-depth: 0

      - name: Setup dotnet
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.1.200

      - name: Build
        shell: bash
        env:
            NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
            DOTNET_CLI_TELEMETRY_OPTOUT: true
        run: |
          # Restore .NET tools
          dotnet tool restore

          # Get the version number
          VERSION=$(dotnet minver -t v -v e -d preview)

          # Run the build script
          dotnet cake --target="Publish" --buildversion="$VERSION" 
```

Nothing super complicated going on there. On every commit to _main_,
unless the commit message contains the text `skip-ci`, we do the following 
in bash on an Ubuntu image:

1. Check out the code.
2. Install the .NET Core SDK
3. Restore the .NET tools
4. Get the version number from _MinVer_ and store it in a variable
5. Pass the version number to the _Cake_ build script as an argument and run 
   the _Publish_ target.

## The build script

Our build script which we save as `build.cake` in the repository root
looks like something like this:

```csharp
#tool "nuget:?package=NuGet.CommandLine&version=5.5.1"

// Get the version argument and if none is provided, use 0.0.1.
// This is just for demonstrational purposes, you probably want to
// abort if publishing without providing a version number.
var semanticVersion = Argument("buildversion", "0.0.1");
var version = semanticVersion.Split(new char[] { '-' }).FirstOrDefault() ?? semanticVersion;

Task("Clean")
    .Does(context => 
{
    CleanDirectory("./.artifacts");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(context => 
{
    DotNetCoreBuild("./src/MyProject.sln", new DotNetCoreBuildSettings {
        Configuration = "Release",
        NoIncremental = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", version)
            .WithProperty("AssemblyVersion", version)
            .WithProperty("FileVersion", version)
    });
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(context => 
{
    // Make sure that there is an API key.
    var apiKey =  context.EnvironmentVariable("NUGET_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key specified.");
    }

    // Pack all projects
    context.DotNetCorePack($"./src/MyProject.sln", new DotNetCorePackSettings {
        Configuration = "Release",
        OutputDirectory = "./.artifacts",
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("PackageVersion", semanticVersion)
    });

    // Publish all projects
    foreach(var file in GetFiles("./.artifacts/*.nupkg"))
    {
        context.Information("Publishing {0}...", file.Path.GetFilename().FullPath);
        context.NuGetPush(file, new NuGetPushSettings {
            ApiKey = apiKey,
            Source = "https://api.nuget.org/v3/index.json"
        });
    }
});

RunTarget(Argument("target", "Build"))
```

You would probably want to do a lot of other stuff there as well such
as running tests and separating packing from publishing so you can get
the packages without doing an actual publish, but this is just for
demonstrational purposes. You could also call _MinVer_ directly 
from your _Cake_ script, or use it 
[in other ways](https://github.com/adamralph/minver#usage), but I
like this approach since it's simple, easy to understand and keeps the 
build script easy to understand.

Why not do everything in the GitHub Action, you might think? Well, 
I like to decouple my builds from the build server I'm using, 
making it possible to run the same build on my machine.

# Summary

In this blog post, I've described how to install local .NET tools whose 
definitions we store in the repository, creating GitHub YAML build definitions
that is used to bootstrap our build process, and how to write and run a 
Cake build script which does the heavy lifting.