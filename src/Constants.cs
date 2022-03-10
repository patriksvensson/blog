public class Constants
{
    public const int PostsPerPage = 10;

    public const string Draft = nameof(Draft);

    public const string BlogTitle = "Patrik Svensson";
    public const string Description = "Patrik Svensson's blog";
    public const string Creator = "@patriksvensson";
    public const string ProdSiteUrl = "https://patriksvensson.se";

    public static class Deployment
    {
        public const string NetlifySiteId = "NETLIFY_SITE_ID";
        public const string NetlifyAccessToken = "NETLIFY_ACCESS_TOKEN";
    }
}
