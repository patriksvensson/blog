---
Title: It's not a party without Cake
Slug: its-not-a-party-without-cake
Published: 2014-07-18
Tags:
- .NET
- C#
- Cake
---

I have during my nine years as a (professional) programmer used several different build automation systems such as [Rake](https://github.com/ruby/rake), [psake](https://github.com/psake/psake), [CMake](http://www.cmake.org/), [TFS Build](http://msdn.microsoft.com/en-us/library/ms181709.aspx) and [FAKE](http://fsharp.github.io/FAKE/), but none of these have allowed me to write my build scripts using C# - the language I use the most.

About two months ago I decided to change that and now it's time to formally introduce Cake, which is short for C# Make.

<!--excerpt-->

![Build Status](/images/cake-medium.png)

## What is Cake?

Cake is (like you probably already figured out) a build automation system with a C# DSL to do things like compiling code, copy files/folders, running unit tests, compress files and build NuGet packages. 

Cake uses a [dependency based programming model](http://martinfowler.com/articles/rake.html#DependencyBasedProgramming) just like Rake, FAKE and similar build automation systems where you declare *tasks* and the dependencies between those. When you execute a task, Cake will construct a directed acyclic graph containing all tasks and execute these in the correct order.

    Task("A")
        .Does(() =>
    {
    });

    Task("B")
        .IsDependentOn("A");
        .Does(() =>
    {
    });

    RunTarget("B");

The goal of Cake is to be a first class alternative to those who - like me - want to write their build scripts in C# instead of Ruby, F# or Powershell.

## What does a Cake script look like?

Assume our build script have four steps where we want to:

1. Clean up old artifacts.
2. Build the code.
3. Run unit tests.
4. Package generated artifacts.

A build script like that could look something like this.

    var target = Argument<string>("target", "Package");
    var config = Argument<string>("config", "Release");

    Task("Clean")
        .Does(() =>
    {
        // Clean directories.
        CleanDirectory("./output");
        CleanDirectory("./output/bin");
        CleanDirectories("./src/**/bin/" + config);
    });

    Task("Build")
        .IsDependentOn("Clean")
        .Does(() =>
    {
        // Build the solution using MSBuild.
        MSBuild("./src/Project.sln", settings => 
            settings.SetConfiguration(config));     
    });

    Task("RunUnitTests")
        .IsDependentOn("Build")
        .Does(() =>
    {
        // Run xUnit tests.
        XUnit("./src/**/bin/" + config + "/*.Tests.dll");
    });

    Task("CopyFiles")
        .IsDependentOn("RunUnitTests")
        .Does(() =>
    {
        var path = "./src/Project/bin/" + configuration;    
        var files = GetFiles(path + "/**/*.dll") 
            + GetFiles(path + "/**/*.exe");

        // Copy all exe and dll files to the output directory.
        CopyFiles(files, "./output/bin");
    });    

    Task("Package")
        .IsDependentOn("RunUnitTests")
        .Does(() =>
    {
        // Zip all files in the bin directory.
        Zip("./output/bin", "./output/build.zip");
    });

    RunTarget(target);

To run our build script we invoke `Cake.exe` with the script file as the first argument, and (optionally) the name of our target task as the second argument. We can also tell Cake how much information we're interested in with the built in `verbosity` parameter, which is very useful when debugging a script.

    C:> Cake.exe build.cake -target=Package -verbosity=diagnostic

This is just an example. Much more functionality is already implemented such as support for `MSBuild`, `MSTest`, `xUnit`, `NUnit`, `ILMerge`, `NuGet pack/restore` and the most common file system operations such as `file/folder manipulation` and `globbing`.

To see an actual build script in action, fork or clone the [GitHub repository](https://github.com/cake-build/cake) and run `build.cmd` which will download Cake from NuGet and run the `build.cake` script.

## What now?

This was only supposed to be an introduction to Cake but I will blog more about this in the near future.

If you find this project interesting and want to help or you just have opinions about stuff, you can check out the full repository at [https://github.com/cake-build/cake](https://github.com/cake-build/cake).