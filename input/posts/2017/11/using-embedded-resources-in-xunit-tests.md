---
Title: Using embedded resources in xUnit tests
Slug: using-embedded-resources-in-xunit-tests
Date: 2017-11-11
RedirectFrom: 2017/11/using-embedded-resources-in-xunit-tests/index.html
Tags:
- .NET
- C#
- xUnit
---

I was reading [Andrew Lock](https://andrewlock.net)'s excellent blog post about 
[Creating parameterised tests in xUnit](https://andrewlock.net/creating-parameterised-tests-in-xunit-with-inlinedata-classdata-and-memberdata)
when I remembered something I wrote a while back that has proven to be quite useful.

The code in question allows you to use embedded resources with [xUnit's theory concept](https://xunit.github.io/docs/getting-started-desktop.html#write-first-theory)
(which is similar to a `TestCase` in NUnit).

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace MyProject.Tests
{
    public sealed class EmbeddedResourceDataAttribute : DataAttribute
    {
        private readonly string[] _args;

        public EmbeddedResourceDataAttribute(params string[] args)
        {
            _args = args;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var result = new object[_args.Length];
            for (var index = 0; index < _args.Length; index++)
            {
                result[index] = ReadManifestData(_args[index]);
            }
            return new[] { result };
        }

        public static string ReadManifestData(string resourceName)
        {
            var assembly = typeof(EmbeddedResourceDataAttribute).GetTypeInfo().Assembly;
            resourceName = resourceName.Replace("/", ".");
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
```

You can now use embedded resources in your tests like this:

```csharp
namespace MyProject.Tests
{
    public sealed class JediCouncilFixture
    {
        [Theory]
        [EmbeddedResourceData("MyProject.Tests/Data/File.json")]
        public void Should_Return_Expected_Advice_From_Jedi_Council(string json)
        {
            // Given
            var expected = JsonConvert.DeserializeObject<Advice>(json);

            // When
            var result = JediCouncil.AskForAdvice();

            // Then
            Assert.Equal(expected, result, new AdviceComparer());
        }
    }
}
```