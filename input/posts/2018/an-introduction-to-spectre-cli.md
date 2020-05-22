---
Title: An introduction to Spectre.Cli
Slug: an-introduction-to-spectre-cli
Published: 2018-04-10
Tags:
- C#
- CLI
---

I've been writing a lot of CLI apps, both at work and in my free time, and there's never really 
been a command line parsing framework that has fitted my needs. While being either too complex 
or too simple, it's always nagged me that I need to write so much code myself. What I wanted 
was a way to be declarative about my commands, options and arguments while still allowing 
composition of them.

So a year back or something like that I decided to build a small framework upon 
`Microsoft.Extensions.CommandLineUtils` that would allow me to write my applications like I wanted to, 
and it worked great (although with a lot of duct tape).

That was until I ran into bugs and other annoyances in the underlying library and discovered 
that they didn't accept any pull request since the library was stable for their needs 
and not under active development. So I decided to rip out Microsoft's library and replace 
it with something else, and [Spectre.Cli](https://github.com/spectresystems/spectre.cli) was born.

## How does it work?

The underlying philosophy behing Spectre.Cli is to rely on the .NET type system to 
declare the commands, but tie everything together via composition.

Imagine the following command structure:

* dotnet *(executable)*
  * add `[PROJECT]`
    * package `<PACKAGE_NAME>`
    * reference `<PROJECT_REFERENCE>`

For this I would like to implement the commands (the different levels in the tree that 
executes something) separately from the settings (the options, flags and arguments), 
which I want to be able to inherit from each other.

```csharp
public class AddSettings : CommandSettings
{
    [CommandArgument(0, "[PROJECT]")]
    public string Project { get; set; }
}

public class AddPackageSettings : AddSettings
{
    [CommandArgument(0, "<PACKAGE_NAME>")]
    public string PackageName { get; set; }

    [CommandOption("-v|--version <VERSION>")]
    public string Version { get; set; }
}

public class AddReferenceSettings : AddSettings
{
    [CommandArgument(0, "<PROJECT_REFERENCE>")]
    public string ProjectReference { get; set; }
}
```

```csharp
public class AddPackageCommand : Command<AddPackageSettings>
{
    public override int Execute(AddPackageSettings settings, ILookup<string, string> remaining)
    {
        // Omitted
    }
}

public class AddReferenceCommand : Command<AddReferenceSettings>
{
    public override int Execute(AddReferenceSettings settings, ILookup<string, string> remaining)
    {
        // Omitted
    }
}
```

Now when we have our commands and settings implemented, we can compose a command tree
that tells the parser how to interpret user input.

```csharp
using Spectre.Cli;

namespace MyApp
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddBranch<AddSettings>("add", add =>
                {
                    add.AddCommand<AddPackageCommand>("package");
                    add.AddCommand<AddReferenceCommand>("reference");
                });
            });

            return app.Run(args);
        }
    }
}
```

Now you might wonder, why do things like this? Well, for starters the different parts
of the application are separated, while still having the option to share things like options,
flags and arguments between them.

This make the resulting code very clean and easy to navigate, not to mention to unit test.
And most importantly at all, the type system guides me to do the right thing. I can't configure 
commands in non-compatible ways, and if I want to add a new top-level `add-package` command 
(or move the command completely), it's just a single line to change. This makes it easy to 
experiment and makes the CLI experience a first class citizen of your application.

## Errors

I've also put some time in making the help screens and errors more intuitive than most other
command line parsing frameworks. 

![Help](/images/command_help.png)  
![Error](/images/command_error.png)

## What else?

This article was meant as an introduction to Spectre.Cli, but the framework itself contains a 
lot of other things as well such as dependency injection, validation and async/await support. 
More about that in a later blog post!

If you want to see Spectre.Cli in action I recommend you to take a look at the 
[sample applications](https://github.com/spectresystems/spectre.cli/tree/develop/samples) in the 
source code repository.