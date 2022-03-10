using System.Collections.Generic;
using System.Linq;
using Statiq.Common;

internal class FullUrlShortCode : SyncShortcode
{
    public override ShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document,
        IExecutionContext context)
    {
        string host = Constants.ProdSiteUrl;

        if (host.EndsWith("/"))
        {
            host = host.Substring(0, host.Length - 1);
        }

        if (host.StartsWith("https://"))
        {
            host = host.Substring(8);
        }

        var path = args.First(x => x.Key == "path").Value ?? "";
        if (path.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        return $"https://{host}/{path}";
    }
}
