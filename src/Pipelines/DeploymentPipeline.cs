using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Web.GitHub;
using Statiq.Yaml;

namespace site.Pipelines
{
    public class DeploymentPipeline : Pipeline
    {
        public DeploymentPipeline()
        {
            Deployment = true;
            OutputModules = new ModuleList
            {
                new DeployGitHubPages(
                    Config.FromSetting<string>(Constants.Deployment.Owner),
                    Config.FromSetting<string>(Constants.Deployment.Repository),
                    Config.FromSetting<string>(Constants.Deployment.GitHubToken))
                        .ToBranch(Config.FromSetting<string>(Constants.Deployment.TargetBranch))
            };
        }
    }
}