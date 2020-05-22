using System;
using System.Dynamic;
using System.Linq;
using Statiq.Common;
using Statiq.Feeds;

namespace site.Extensions
{
    public static class DocumentExtensions
    {
        public static string GetTagName(this IDocument document) 
        {
            var tag = document.GetString(Keys.GroupKey);
            tag = tag.Replace("#", "sharp");
            tag = tag.Replace("+", "plus");
            return tag;
        }

        public static NormalizedPath GetBlogPostPath(this IDocument document)
        {
            var date = document.GetDateTime(FeedKeys.Published, defaultValue: DateTime.MinValue);
            if (date == DateTime.MinValue)
            {
                throw new InvalidOperationException("Blog post is missing published date.");
            }

            var slug = document.GetString(Constants.Slug);
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new InvalidOperationException("Blog post is missing slug.");
            }

            return new NormalizedPath($"{date.Year}/{date.Month:D2}/{slug}/index.html");
        }

        public static object AsTag(this IDocument document, IExecutionContext context) => new
        {
            link = context.GetLink(document),
            title = document.GetString(Keys.GroupKey),
            has_posts = document.GetChildren().Any(d => !d.GetBool(Constants.Draft, false)),
            count = document.GetChildren().Where(d => !d.GetBool(Constants.Draft, false)).Count()
        };

        public static object AsPost(this IDocument document, IExecutionContext context) => new
        {
            link = context.GetLink(document),
            title = document.GetString(Keys.Title),
            excerpt = document.GetString(FeedKeys.Excerpt),
            draft = document.GetBool(Constants.Draft, false),
            date = document.GetDateTime(FeedKeys.Published).ToLongDateString()
        };
    }
}