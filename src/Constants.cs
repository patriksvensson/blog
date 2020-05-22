using System.Linq;
using site.Extensions;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using Statiq.Handlebars;

namespace site
{
    public class Constants
    {
        public const string Slug = nameof(Slug);
        public const string Draft = nameof(Draft);

        public static class Deployment 
        {
            public const string Owner = "DEPLOYMENT_OWNER";
            public const string Repository = "DEPLOYMENT_REPOSITORY";
            public const string GitHubToken = "GITHUB_TOKEN";
            public const string TargetBranch = "DEPLOYMENT_TARGET_BRANCH";
        }
    }
}