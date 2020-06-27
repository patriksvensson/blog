using System;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Feeds;
using Statiq.Web;

namespace site
{
    internal static class Program
    {
        private static Task<int> Main(string[] args)
        {
            return Bootstrapper
                .Factory
                .CreateDefault(args)
                .AddHostingCommands()
                .AddSetting(Constants.Deployment.Owner, "patriksvensson")
                .AddSetting(Constants.Deployment.Repository, "blog")
                .AddSetting(Constants.Deployment.TargetBranch, "master")
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
                .RunAsync();
        }
    }
}
