using System.Threading.Tasks;
using EmbedIO;
using JetBrains.Annotations;

namespace RMServerAssist;

public class ApiKeyModule([NotNull] string baseRoute, string apiKey) : WebModuleBase(baseRoute)
{
    public const string ApiKeyHeaderKey = "X-Api-Key";
    
    protected override Task OnRequestAsync(IHttpContext context)
    {
        var requestKey = context.Request.Headers.Get(ApiKeyHeaderKey);
        if (!string.IsNullOrEmpty(apiKey) && requestKey == apiKey)
        {
            return Task.CompletedTask;
        }
        
        throw HttpException.Unauthorized();
    }

    public override bool IsFinalHandler => false;
}