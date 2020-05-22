using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Yaml;

namespace site.Pipelines
{
    public class FaviconPipeline : Pipeline
    {
        public FaviconPipeline()
        {
            InputModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(ctx => 
                {
                    return new ReadFiles("assets/favicon/favicon.ico");
                }))
            };
            
            ProcessModules = new ModuleList
            {
                new SetDestination("favicon.ico")
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}