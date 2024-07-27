using System.Threading.Tasks;
using EmbedIO;
using JetBrains.Annotations;

namespace RMServerAssist;

public class ApiKeyModule([NotNull] string baseRoute, string apiKey) : WebModuleBase(baseRoute)
{
    public const string ApiKeyHeaderKey = "X-Api-Key";
    
    protected override Task OnRequestAsync(IHttpContext context)
    {
        if (context.Request.HttpVerb == HttpVerbs.Get && context.Request.Url.AbsolutePath.EndsWith(".html"))
        {
            return Task.CompletedTask;
        }
        
        var requestKey = context.Request.Headers.Get(ApiKeyHeaderKey);
        if (!string.IsNullOrEmpty(apiKey) && requestKey == apiKey)
        {
            return Task.CompletedTask;
        }
        
        throw HttpException.Unauthorized();
    }

    public override bool IsFinalHandler => false;
}