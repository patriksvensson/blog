---
Title: Cake and AppVeyor integration
Slug: cake-and-appveyor-integration
Published: 2014-11-23
Tags:
- .NET
- C#
- Cake
- AppVeyor
---

In this blog post I will show you how to use [Cake](https://github.com/cake-build/cake) with your AppVeyor CI builds.

<!--excerpt-->

## 1. Create the build script

Add a build script called `build.cake` to the project root. In this tutorial, we'll just create a really simple build script for demonstration.

	// Get the target.
	var target = Argument<string>("target", "Default");

	Task("Default")
	  .Does(() =>
	{
		Information("Hello from Cake!");
	});

	RunTarget(target);

## 2. Create a bootstrapper script

Create a old fashioned batch file called `build.cmd` that will download Cake and execute the build script.

	@echo off

	:Build
	cls

	if not exist tools\Cake\Cake.exe ( 
		echo Installing Cake...
		tools\NuGet.exe install Cake -OutputDirectory tools -ExcludeVersion -NonInteractive -Prerelease
	)

	echo Starting Cake...
	tools\Cake\Cake.exe build.cake -target=Default -verbosity=diagnostic

## 3. Add NuGet.exe to your repository

Start by copying `NuGet.exe` to your tools folder. Cake uses the `tools` path as a convention for finding stuff it needs such as unit test runners and other tools.

* MyProject/
  * tools/
     * **NuGet.exe**
  * build.cake
  * build.cmd

## 4. Tell AppVeyor what to do

Now we need to tell AppVeyor how to start the Cake build. Do this by setting the build script for your AppVeyor project to `build.cmd`. Save your settings and you should be done.

![AppVeyor Build Settings](/images/cake-appveyor-build-settings.png)

## 5. Profit

The next triggered build will now execute the Cake build script as expected.

![AppVeyor Build](/images/cake-appveyor-profit.png)

For more information about Cake, see [http://cake.readthedocs.org](http://cake.readthedocs.org).