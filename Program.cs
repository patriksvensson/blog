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
