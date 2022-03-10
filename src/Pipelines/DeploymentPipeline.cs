using Statiq.Common;
using Statiq.Web.Netlify;

public class DeploymentPipeline : Statiq.Core.Pipeline
{
    public DeploymentPipeline()
    {
        Deployment = true;
        OutputModules = new ModuleList
        {
            new DeployNetlifySite(
                siteId: Config.FromSetting<string>(Constants.Deployment.NetlifySiteId),
                accessToken: Config.FromSetting<string>(Constants.Deployment.NetlifyAccessToken)
            )
        };
    }
}