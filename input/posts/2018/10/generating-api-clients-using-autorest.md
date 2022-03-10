---
Title: Generating API clients using AutoRest
Slug: generating-api-clients-using-autorest
Date: 2018-10-02
RedirectFrom: 2018/10/generating-api-clients-using-autorest/index.html
Tags:
- C#
- AutoRest
- OpenAPI
- Swagger
---

This blog post looks at automatically generating an HTTP API client 
from a [OpenAPI specification](https://swagger.io/specification/) 
(formerly swagger) using [AutoRest](http://azure.github.io/autorest/).

We will also use another tool called [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) 
which is an ASP.NET Core library that allows us to annotate our 
controllers with metadata that is used to generate OpenAPI specifications.

<!--excerpt-->

## Installing AutoRest

The first thing we need to do is to install `AutoRest` globally on 
our machine. If you have NPM installed on your computer, then this
can  be done by running the following command:

```
> npm install -g autorest
```

## Setting up the code base

For this project we're going to create a standard ASP.NET Core
web project, a class library that will hold our API client and
a PowerShell script to generate the client for us.

Why PowerShell and not Cake you might wonder if you know about
my involvement in the Cake project, and the reason is simple; 
I want this guide to be as build agnostic as it can be, but 
don't worry, I will follow up with a detailed guide of how to
convert the PowerShell script to a fully fledged Cake build script.

Start with creating the project.

```
> mkdir autorest-example
> cd autorest-example
```

Now we want to create a source directory where the source code
for our API project will live.

```
> mkdir src
> cd src
```

We continue by creating a solution file, and the two projects I
mention above using the dotnet CLI.

```
> dotnet new sln --name AutoRestExample
> dotnet new webapi --name Sample.Api
> dotnet new classlib --name Sample.Client
> dotnet sln AutoRestExample.sln add ./Sample.Api/Sample.Api.csproj
> dotnet sln AutoRestExample.sln add ./Sample.Client/Sample.Client.csproj
```

After this is done you can open the solution file in your editor of choice.
For this tutorial/guide I will be using Visual Studio, but nothing
stops you from using Visual Studio Code, Rider or a similar IDE/editor.

## Modifying the API project

We're going to use a library called `Swashbuckle` to generate the OpenAPI
specification directly from our ASP.NET Core controllers, so we will need to
install the necessary NuGet packages.

* [Swashbuckle.AspNetCore](http://nuget.org/packages/Swashbuckle.AspNetCore)
* [Swashbuckle.AspNetCore.Annotations](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Annotations)

We will also need the `Swashbuckle.AspNetCore.Cli` tool installed as part of
our project. This is not a package reference but a [project tool](https://docs.microsoft.com/en-us/dotnet/core/tools/extensibility#per-project-based-extensibility), so we will need
to add the tool to our csproj manually. Right click on the `Sample.Api` project 
in the solution explorer and paste the following snippet somewhere under the
`<Project>` node.

```xml
<ItemGroup>
  <DotNetCliToolReference Include="Swashbuckle.AspNetCore.Cli" Version="3.0.0-beta1" />
</ItemGroup>
```

### The controller

Open up `Controllers/ValuesController.cs` and replace the content with the following
code:

```csharp
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Sample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<string>), 200)]
        [SwaggerOperation(OperationId = "Values_GetAll")]
        public ActionResult<ICollection<string>> Get()
        {
            return new string[] { "foo", "bar" };
        }
    }
}
```

As you can see we have a method that returns a collection of string. We've decorated
the method with two additional attributes that tells `Swashbuckle` that this method
will return a collection of strings if the HTTP response code is `200`. We're also
setting an [Swagger operation ID]() for the method that will be used when we generate
our client.

### Wiring up the API

We now have a controller that's decorated with the necessary attributes for `Swashbuckle`
to generate a Swagger specification from it. We now have to wire everything up in the
`Startup.cs` file.

In the `ConfigureServices` method, add the following code that will tell `Swashbuckle` that
we've annotated our controllers with annotations, and provide some metadata for the API.

```csharp
services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Info
    {
        Title = "Sample API",
        Version = "v1"
    });
});
```

We now have to add the `Swashbuckle` middleware to our application pipeline which we do by
adding the following code last in the `Configure` method. You can also (optionally) add 
the UI, but this is not required for what we're trying to achieve.

```csharp
app.UseSwagger();

// Optional
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API");
});
```

If you run the project and navigate to `/swagger/v1/swagger.json`, you will see
the generated Swagger specification that represents our API.

### Modifying the client project

The last thing we need to do before actually generating a client is to add a 
reference to the `Microsoft.Rest.ClientRuntime` NuGet package in the `Sample.Client` project.
This package contains everything that a generated `AutoRest` client requires to work.

## Generating the client code

Now we want to start generating the client code, but we don't want to get the Swagger
document by starting the API and retrieving it. It's absolutely possible but will be hard
to automate. 

It's here the `Swashbuckle.AspNetCore.Cli` package comes into play.  
What this package does is to allow to generate the Swagger document from a built assembly
instead of invoking the Swagger endpoint of our API project.

What we need to do now is:

1. Build the Sample.API project.
1. Generate the Swagger definition from the built assembly.
1. Generate the client from the Swagger definition using `AutoRest`.

Like I mentioned before, we're going to use PowerShell to do this, so go ahead and create
a `Generate.ps1` file in the root of our project structure. The file should look like following:

```powershell
Set-Location src

# Build project.
dotnet build

# Create the Swagger file from the API assembly.
Set-Location Sample.Api
dotnet swagger "tofile" --output "../../res/swagger.json" "../Sample.Api/bin/Debug/netcoreapp2.1/Sample.Api.dll" v1
Set-Location ..

# Clear the previously generated code.
Remove-Item "Sample.Client/Generated" -Force -Recurse

# Generate a C# client from the Swagger definition and
# override the namespace and the client name.
autorest --input-file="../res/swagger.json" `
         --output-folder="Sample.Client/Generated" `
         --namespace="Sample.Client" `
         --override-client-name="SampleClient" `
         --csharp

# Reset the location.
Set-Location ..
```

By running this script you should find a folder called `Generated` in the Client project 
containing the API client.

All classes are partial (as well as some methods) so if you need to inject your own logic
or change some behavior (such as authentication), you have the possibility to do so.

## Closing up

This tutorial just goes through the basics of how to generate an API client, but there is
a myriad of other options that you can use to customize the generated client to your
exact needs. I've personally haven't encountered a single scenario that couldn't be solved
by one of the extensibility points in the generated code.

I also recommend you to always commit the generated Swagger file as part of your repository.
That way you always have a way to quickly regenerate the client if you manage to break something.

You can find the source code to this blog post at 
[https://github.com/patriksvensson/autorest-example](https://github.com/patriksvensson/autorest-example)