using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace RMServerAssist.Router;

public class JudgeController : WebApiController
{
    [Route(HttpVerbs.Post, "/reset")]
    public async Task<Response<string>> StartReset()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-free");
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/prepare")]
    public async Task<Response<string>> StartPrepare()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-pre");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/check")]
    public async Task<Response<string>> StartSelfCheck()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-check");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/countdown")]
    public async Task<Response<string>> StartCountdown()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-start");
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/settle/red")]
    public async Task<Response<string>> RedDefeat()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-win 1");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/settle/blue")]
    public async Task<Response<string>> BlueDefeat()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-win 0");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/settle/error")]
    public async Task<Response<string>> ErrorDefeat()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-win -2");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/killall")]
    public async Task<Response<string>> KillAll()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-killall -1");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/kickall")]
    public async Task<Response<string>> KickAll()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-kickall -1");
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/resetall")]
    public async Task<Response<string>> ResetAll()
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-reliveall -1");
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/coin/red")]
    public async Task<Response<string>> RedAddMoney([QueryField] int value)
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-addmoney 0 {0}", value);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/coin/blue")]
    public async Task<Response<string>> BlueAddMoney([QueryField] int value)
    {
        await Task.Yield();
        Utility.JudgeSendGMOrder("-addmoney 1 {0}", value);
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/pause/{team}/{time}")]
    public async Task<Response<string>> TechnicalPause(string team, int time)
    {
        await Task.Yield();
        var teamId = team.ToLower() switch
        {
            "red" => 0,
            "blue" => 1,
            _ => -1,
        };
        
        if (teamId == -1)
        {
            return Response<string>.Create("", "invalid team");
        }

        var timeId = time switch
        {
            120 => 2,
            180 => 3,
            _ => -1,
        };
        
        if (timeId == -1)
        {
            return Response<string>.Create("", "invalid time");
        }
        
        Utility.JudgeSendGMOrder($"-pause {teamId} {timeId}");
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/command")]
    public async Task<Response<string>> SendCommand([FormField] string content)
    {
        await Task.Yield();
        if (string.IsNullOrWhiteSpace(content))
        {
            return Response<string>.Create("", "empty command");
        }
        
        Utility.JudgeSendGMOrder(content);
        
        return Response<string>.Create("ok");
    }
}