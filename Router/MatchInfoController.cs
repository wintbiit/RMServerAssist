using System.Reflection;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using TMPro;
using UnityEngine;

namespace RMServerAssist.Router;

public class MatchInfoController : WebApiController
{
    private static App App => App.Instance;

    [Route(HttpVerbs.Get, "/round/current")]
    public async Task<Response<int>> GetCurrentRound()
    {
        await Task.Yield();
        
        return Response<int>.Create(App.currentRound);
    }
    
    [Route(HttpVerbs.Get, "/round/total")]
    public async Task<Response<int>> GetTotalRound()
    {
        await Task.Yield();

        return Response<int>.Create(App.totalRound);
    }
    
    [Route(HttpVerbs.Get, "/score/red")]
    public async Task<Response<int>> GetRedScore()
    {
        await Task.Yield();

        return Response<int>.Create(App.Team[0].score);
    }
    
    [Route(HttpVerbs.Get, "/score/blue")]
    public async Task<Response<int>> GetBlueScore()
    {
        await Task.Yield();

        return Response<int>.Create(App.Team[1].score);
    }

    private GameProgress _gameProgress;
    [Route(HttpVerbs.Get, "/progress")]
    public async Task<Response<string>> GetMatchProgress()
    {
        if (_gameProgress == null)
        {
            _gameProgress = Object.FindObjectOfType<GameProgress>();
        }
        
        if (_gameProgress == null)
        {
            return Response<string>.Create("", "not in game");
        }
        
        await Task.Yield();
        
        var progressText = _gameProgress.GetFieldValue<TMP_Text>("ProgressText");
        return Response<string>.Create(progressText?.text ?? "");
    }
    
    [Route(HttpVerbs.Get, "/time")]
    public async Task<Response<float>> GetMatchTime()
    {
        await Task.Yield();

        return Response<float>.Create(App.GameLeftTime);
    }
    
    [Route(HttpVerbs.Get, "/coin/red")]
    public async Task<Response<int>> GetRedCoin()
    {
        await Task.Yield();

        return Response<int>.Create(App.Team[0].coinNum);
    }
    
    [Route(HttpVerbs.Get, "/coin/blue")]
    public async Task<Response<int>> GetBlueCoin()
    {
        await Task.Yield();

        return Response<int>.Create(App.Team[1].coinNum);
    }
}