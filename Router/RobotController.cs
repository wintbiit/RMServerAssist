using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace RMServerAssist.Router;

public partial class RobotController
{
    [Route(HttpVerbs.Post, "/{id}/exp")]
    public async Task<Response<string>> AddExperience(int id, [QueryField] float value)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-upgrade {0} {1}", id, value);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/levelup")]
    public async Task<Response<int>> LevelUp(int id, [QueryField] int value)
    {
        await Task.Yield();
        var user = _robots[id];
        if (user == null)
        {
            return Response<int>.Create(-1, "not found");
        }

        var level = user.GetVal(enumAttr.attr_Player_Level);
        var exp = user.GetVal(enumAttr.attr_Player_Exp);
        var expLevelUp = user.GetVal(enumAttr.attr_Player_ExpLevelUp);
        
        while (value > 0 && level <= 10)
        {
            await AddExperience(id, expLevelUp - exp);
            
            level++;
            exp = user.GetVal(enumAttr.attr_Player_Exp);
            expLevelUp = user.GetVal(enumAttr.attr_Player_ExpLevelUp);
            
            value--;
        }
        
        return Response<int>.Create((int)level);
    }

    [Route(HttpVerbs.Post, "/{id}/reset")]
    public async Task<Response<string>> Reset(int id)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-reset {0}", id);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/find")]
    public async Task<Response<string>> Find(int id)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-find {0}", id);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/kick")]
    public async Task<Response<string>> Kick(int id)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-kick {0}", id);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/kill")]
    public async Task<Response<string>> Kill(int id)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-kill {0}", id);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/balance")]
    public async Task<Response<string>> SetBalance(int id, [QueryField] bool isBalance)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-setbalance {0} {1}", id, isBalance ? 1 : 0);
        
        return Response<string>.Create("ok");
    }

    [Route(HttpVerbs.Post, "/{id}/yellow")]
    public async Task<Response<string>> YellowCard(int id)
    {
        await Task.Yield();
        
        var team = id > 100 ? 1 : 0;
        
        Utility.JudgeSendGMOrder("-warning {0} 2 {1}", team, id);
        
        return Response<string>.Create("ok");
    }
    
    [Route(HttpVerbs.Post, "/{id}/red")]
    public async Task<Response<string>> RedCard(int id)
    {
        await Task.Yield();
        
        Utility.JudgeSendGMOrder("-kill {0}", id);
        
        return Response<string>.Create("ok");
    }
}