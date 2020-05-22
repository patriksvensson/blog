---
Title: How to write a stateful Roslyn analyzer
Slug: how-to-write-a-stateful-roslyn-analyzer
Published: 2020-03-07
Tags:
- C#
- Roslyn
- .NET
---

I wrote a stateful Roslyn analyzer a couple of days ago to analyze the codebase
at work for irregularities, and I thought I would share my findings on how I did
it.

## The goal

Our goal for this blog post is to write an analyzer that finds all non-abstract
classes that implement `IRequest<TResult>`, and check whether or not they have
an associated request handler `RequestHandler<TRequest, TResult>`.

The algorithm for our analyzer is kind of straight forward.

1. Find all non-abstract classes that implement `IRequest<TResult>`
2. Find all non-abstract classes that inherit from `RequestHandler<TRequest,
   TResult>`
3. Once the compilation finished, find all requests that don't have a handler
   and report a diagnostic for each of them.

We won't cover the actual setup of the analyzer project, but head over to
https://devblogs.microsoft.com/dotnet/how-to-write-a-roslyn-analyzer and follow
Mika's excellent tutorial on this and comes back after that. I'll wait.

## Setting everything up

Ok, now, when you've got a project set up, let's create the boilerplate for the
request analyzer. Start by removing any existing analyzers and code fixes from the
project and create a new analyzer.

```csharp
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RequestAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RequestAnalyzer";
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, "Request does not have an associated handler",
            "The request '{0}' does not have an associated handler.", "Maintenance",
            DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: "Requests should have an associated handler");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
            analysisContext.EnableConcurrentExecution();
            analysisContext.RegisterCompilationStartAction(Analyze);
        }

        private void Analyze(CompilationStartAnalysisContext startContext)
        {
        }
    }
}
```

Once we have all the boilerplate in place, we'll create a test that will verify that
our analyzer works as expected.

```csharp
[TestClass]
public class UnitTest : CodeFixVerifier
{
    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
    {
        return new RequestAnalyzer();
    }

    [TestMethod]
    public void Should_Return_Diagnostic_For_Missing_Command_Handler()
    {
        var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Foo
{
    public interface IRequest<TResult>
    {
    }

    public sealed class MyRequest : IRequest<int>
    {
    }

    public abstract class AbstractRequest : IRequest<int>
    {
    }

    public sealed class MyOtherRequest : IRequest<int>
    {
    }

    public sealed class MyRequestHandler : RequestHandler<MyRequest, int>
    {
    }
}";
        var expected = new DiagnosticResult
        {
            Id = "RequestAnalyzer",
            Message = "The request 'MyOtherRequest' does not have an associated handler.",
            Severity = DiagnosticSeverity.Warning,
            Locations =
                new[] {
                        new DiagnosticResultLocation("Test0.cs", 23, 29)
                    }
        };

        VerifyCSharpDiagnostic(test, expected);
    }
}
```

The test will set up the source code that will be analyzed and some expectations.
Notice that we need to specify the `IRequest<TResult>` definition in our code.
When Roslyn encounters an unknown type during a diagnostic run, it will assume
it's a type, so by specifying this, Roslyn will know that it's an interface.

## Writing the actual analyzer

So, the first step is finding the requests and the request handlers and store
them somewhere. For this, we'll register a callback that will be called every
time the semantic analysis of a class, enum, or interface has been completed. In
the callback, we will check if the type fits the criteria and store it in a
`ConcurrentBag`.

```
private void Analyze(CompilationStartAnalysisContext startContext)
{
    var handlers = new ConcurrentBag<INamedTypeSymbol>();
    var requests = new ConcurrentBag<INamedTypeSymbol>();

    startContext.RegisterSymbolAction(context =>
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind == TypeKind.Class)
        {
            if (type.IsAbstract || type.IsStatic)
            {
                return;
            }

            // Is this a request handler?
            if (type.BaseType != null && type.BaseType.Name == "RequestHandler" &&
                type.BaseType.TypeArguments.Length == 2)
            {
                handlers.Add(type);
            }
            // Is this a request?
            else if (type.Interfaces.Any(x => x.Name == "IRequest" && x.TypeArguments.Length == 1))
            {
                requests.Add(type);
            }
        }
    }, SymbolKind.NamedType);
}
```

Now when we have all the information we need, we need to perform our actual
analysis of the data we've collected. For this, we register another callback
that will be invoked once the compilation is done using
`RegisterCompilationEndAction`.

One problem with using
`RegisterCompilationEndAction` is that it won't be fired unless full solution
analysis is enabled. So unless you only care about feedback when building
outside of Visual Studio, you will have to 
[turn that on](https://docs.microsoft.com/en-us/visualstudio/code-quality/how-to-enable-and-disable-full-solution-analysis-for-managed-code?view=vs-2019).

```
private void Analyze(CompilationStartAnalysisContext startContext)
{
    var handlers = new ConcurrentBag<INamedTypeSymbol>();
    var requests = new ConcurrentBag<INamedTypeSymbol>();

    // [Omitted]

    startContext.RegisterCompilationEndAction(context =>
    {
        // Check all requests.
        foreach (var request in requests)
        {
            var found = false;

            // Now check all handlers.
            foreach (var handler in handlers)
            {
                // Is this handler an implementation for our request?
                // I.e. RequestHandler<Foo, int> for Foo? Check this by comparing
                // the first type argument with the name of the request.
                if (handler.BaseType.TypeArguments[0].Name == request.Name)
                {
                    // Yes, we found it.
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // Report a diagnostic for the first occurance of the symbol.
                context.ReportDiagnostic(Diagnostic.Create(
                    MissingRequestHandler,
                    request.Locations.FirstOrDefault(),
                    request.Name));
            }
        }
    });
}
```

If we run our tests now, it will succeed!

## Wrapping up

Not as much code as you suspected, right? Of course, there are a gazillion
improvements that could be done to this analyzer (both when it comes to
performance and correctness), but I've tried to keep it as simple as possible
for this blog post. I'll leave any improvements as an exercise to the reader. ;)

You can find the source code for the analyzer at 
[https://github.com/patriksvensson/stateful-roslyn-analyzer](https://github.com/patriksvensson/stateful-roslyn-analyzer).