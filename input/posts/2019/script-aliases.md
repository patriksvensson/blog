---
Title: Script aliases
Slug: script-aliases
Published: 2014-08-19
Tags:
- .NET
- C#
- Cake
---

Cake supports something called script aliases. Script aliases are convenience methods that are easily accessible directly from a Cake script. Every [API method](http://cake.readthedocs.org/en/latest/api-documentation.html) in Cake is implemented like this.

In this blog post I will show how to extend Cake with your own script aliases. It's really simple, I promise. ;-)

<!--excerpt-->

## Creating an extension

Start by creating a new class library project and add a reference to the `Cake.Core` NuGet package.

    PM> Install-Package Cake.Core

Add the script alias method that you want to expose to your Cake script. A script alias method is simply an extension method for `ICakeContext` that's been marked with the `CakeMethodAlias` attribute.

You could also add an script alias property, which works the same way as a script alias method, except that it accepts no arguments and is marked with the `CakePropertyAlias` attribute.

    using Cake.Core;
    using Cake.Core.Annotations;

    public static class MyCakeExtension
    {
       [CakeMethodAlias]
       public static int GetMagicNumber(this ICakeContext context, bool minValue)
       {
          return minValue ? int.MinValue : int.MaxValue;
       }

       [CakePropertyAlias]
       public static int TheAnswerToLife(this ICakeContext context)
       {
          return 42;
       }
    }

## Using the extension

Compile the assembly and add a reference to it in the build script via the `#r` directive.

    #r "tools/MyCakeExtension.dll"

Now you should be able to call the method from the script.

    Task("GetSomeAnswers")
       .Does(() =>
    {
        // Write the values to the console. 
        Information("Magic number: {0}", GetMagicNumber(false));
        Information("The answer to life: {0}", TheAnswerToLife);
    });

## Wrapping up

This covers the basics of how to create script aliases in Cake.  
If you create a cool extension, tell me about it and I will link to it from the [Cake repository](https://github.com/cake-build/cake).