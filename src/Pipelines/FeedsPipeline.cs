using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;

namespace site.Pipelines
{
    public class FeedsPipeline : Pipeline
    {
        public FeedsPipeline()
        {
            Dependencies.Add(nameof(BlogPostPipeline));

            ProcessModules = new ModuleList
            {
                new ConcatDocuments(nameof(BlogPostPipeline)),
                new FilterDocuments(Config.FromDocument(doc => !doc.GetBool(Constants.Draft, false))),
                new OrderDocuments(Config.FromDocument((x => x.GetDateTime(FeedKeys.Published)))).Descending(),
                new GenerateFeeds()
                    .WithItemDescription(Config.FromDocument(doc => doc.GetString("Excerpt")))
                    .WithRssPath(new NormalizedPath("rss.xml"))
                    .WithAtomPath(new NormalizedPath("feed.xml"))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}