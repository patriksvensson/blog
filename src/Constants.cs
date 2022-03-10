public class Constants
{
    public const int PostsPerPage = 10;

    public const string BlogTitle = "Patrik Svensson";
    public const string Description = "Patrik Svensson's blog";
    public const string Creator = "@patriksvensson";
    public const string ProdSiteUrl = "https://patriksvensson.se";

    public static class Deployment
    {
        public const string Owner = "DEPLOYMENT_OWNER";
        public const string Repository = "DEPLOYMENT_REPOSITORY";
        public const string GitHubToken = "GITHUB_TOKEN";
        public const string TargetBranch = "DEPLOYMENT_TARGET_BRANCH";
    }
}
