using System;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using Statiq.Web.Pipelines;

public class FeedsPipeline : Pipeline
{
    public FeedsPipeline()
    {
        Dependencies.Add(nameof(Content));

        ProcessModules = new ModuleList
        {
            new ConcatDocuments(nameof(Content)),
            new FilterDocuments(Config.FromDocument(doc => !doc.GetBool(Constants.Draft, false))),
            new OrderDocuments(Config.FromDocument((x => x.GetDateTime("Date")))).Descending(),
            new GenerateFeeds()
                .WithItemDescription(Config.FromDocument(doc => doc.GetString("Excerpt")))
                .WithItemPublished(Config.FromDocument(doc => (DateTime?)doc.GetDateTime("Date")))
                .WithRssPath(new NormalizedPath("feed.xml"))
                .WithAtomPath(new NormalizedPath("atom.xml"))
        };

        OutputModules = new ModuleList
        {
            new WriteFiles()
        };
    }
}