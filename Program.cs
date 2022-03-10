using System;
using Statiq.App;
using Statiq.Common;
using Statiq.Feeds;
using Statiq.Web;
using Statiq.Web.Pipelines;

var dotnetPath = System.Environment.GetEnvironmentVariable("DOTNET_PATH") ?? "dotnet";

await Bootstrapper.Factory
    .CreateWeb(args)
    .SetOutputPath("public")
    .AddSetting(Keys.LinkLowercase, true)
    .AddSetting(Keys.LinksUseHttps, true)
    .AddSetting(Keys.Host, "patriksvensson.se")
    .AddSetting(Keys.Title, "Patrik Svensson")
    .AddSetting(FeedKeys.Author, "Patrik Svensson")
    .AddSetting(FeedKeys.Description,
        "This is my blog, where I write about stuff that interests me such as .NET, " +
        "Rust, DevOps and technology in general. I am a husband and a father, " +
        "and I enjoy contributing to Open Source projects.")
    .AddSetting(FeedKeys.Copyright, DateTime.UtcNow.Year.ToString())
    .AddShortcode<FullUrlShortCode>("FullUrl")
    .ModifyPipeline(nameof(Content), pipeline =>
    {
        pipeline.PostProcessModules.Add(new RoslynHighlightModule());
    })
    .AddProcess(ProcessTiming.Initialization,
        _ => new ProcessLauncher("npm", "install") { LogErrors = false })
    .AddProcess(ProcessTiming.Initialization,
        _ => new ProcessLauncher(dotnetPath, "tool restore") { LogErrors = false })
    .AddProcess(ProcessTiming.Initialization,
        _ => new ProcessLauncher(dotnetPath, "tool run playwright install chromium") { LogErrors = false })
    .AddProcess(ProcessTiming.AfterExecution,
        _ => new ProcessLauncher("npm", "run", "build:tailwind") { LogErrors = false, LogOutput = true, })
    .AddProcess(ProcessTiming.BeforeDeployment,
        _ => new ProcessLauncher("npm", "run", "build:tailwind") { LogErrors = false, LogOutput = true, })
    .RunAsync();
