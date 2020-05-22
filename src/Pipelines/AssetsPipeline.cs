using Statiq.Common;
using Statiq.Core;

namespace site.Pipelines
{
    public class AssetsPipeline : Pipeline
    {
        public AssetsPipeline()
        {
            Isolated = true;
            ProcessModules = new ModuleList
            {
                new CopyFiles("./assets/{css,favicon,fonts,images,js}/**/*", "*.{png,ico,webmanifest}"),
                new CopyFiles("./images/**/*", "*.{png,jpg}")
            };
        }
    }
}