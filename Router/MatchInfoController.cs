using System;
using System.Reflection;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using TMPro;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace RMServerAssist.Router;

public class MatchInfoController : WebApiController
{
    private static App App => App.Instance;
    
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
    
    [Route(HttpVerbs.Get, "/type")]
    public async Task<Response<string>> GetMatchType()
    {
        await Task.Yield();

        return Response<string>.Create(App.GameType.ToString().ToLower());
    }

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
    
    [Route(HttpVerbs.Get, "/red")]
    public async Task<Response<TeamInfo>> GetRedTeamInfo()
    {
        await Task.Yield();

        return Response<TeamInfo>.Create(GetTeamInfo(0));
    }
    
    [Route(HttpVerbs.Get, "/blue")]
    public async Task<Response<TeamInfo>> GetBlueTeamInfo()
    {
        await Task.Yield();
        
        return Response<TeamInfo>.Create(GetTeamInfo(1));
    }

    private static TeamInfo GetTeamInfo(int id)
    {
        var robotId = (ulong) id * 100 + 10;
        var team = App.Team[id];
        var baseRobot = App.FindUser_ByUID(robotId);
        var outpostRobot = App.FindUser_ByUID(robotId);
        
        var teamInfo = new TeamInfo
        {
            score = team.score,
            coinNum = team.coinNum,
            totalHurt = team.totalHurt,
            @base = new TeamInfo.BaseInfo
            {
                shellStatus = App.baseRobotDevStatus[id].shell_status,
                health = new HealthInfo
                {
                    hp = baseRobot.GetVal(enumAttr.attr_Player_HP),
                    maxHp = baseRobot.GetVal(enumAttr.attr_Player_HPMax)
                },
                shieldHealth = new HealthInfo
                {
                    hp = id == 0 ? App.baseShield.redShield : App.baseShield.blueShield,
                    maxHp = App.baseShield.shieldMax
                },
                hasShield = id == 0 ? App.baseShield.redHasShield == 1 : App.baseShield.blueHasShield == 1,
                invincible = Utility.CheckInvincibleBuffExist((int)robotId)
            },
            sentry = new TeamInfo.SentryInfo
            {
                remainingRevivalCount = team.sentryRemainRevivalCount,
                controlPrice = team.sentryControlPrice,
            },
            outpost = new TeamInfo.OutpostInfo
            {
                health = new HealthInfo
                {
                    hp = outpostRobot.GetVal(enumAttr.attr_Player_HP),
                    maxHp = outpostRobot.GetVal(enumAttr.attr_Player_HPMax)
                },
            },
            user = new TeamInfo.UserInfo
            {
                teamId = team.team_id.ToString(),
                teamName = team.team_name,
                collegeName = team.college_name,
                collegeLogo = team.college_logo
            }
        };
        
        return teamInfo;
    }
}

[Serializable]
public struct HealthInfo
{
    public float hp;
    public float maxHp;
}

[Serializable]
public struct TeamInfo
{
    [Serializable]
    public struct BaseInfo
    {
        public int shellStatus;
        public HealthInfo health;
        public HealthInfo shieldHealth;
        public bool hasShield;
        public bool invincible;
    }
    
    [Serializable]
    public struct SentryInfo
    {
        public int remainingRevivalCount;
        public float controlPrice;
    }
    
    [Serializable]
    public struct UserInfo
    {
        public string teamId;
        public string teamName;
        public string collegeName;
        public string collegeLogo;
    }
    
    [Serializable]
    public struct OutpostInfo
    {
        public HealthInfo health;
    }
    
    public int score;
    public int coinNum;
    public int totalHurt;
    public BaseInfo @base;
    public SentryInfo sentry;
    public OutpostInfo outpost;
    public UserInfo user;
}