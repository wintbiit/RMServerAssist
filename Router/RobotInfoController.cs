﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using TMPro;
using UnityEngine;

namespace RMServerAssist.Router;

public partial class RobotController : WebApiController
{
    private readonly Dictionary<int, RobotItem> _redRobotItems = new Dictionary<int, RobotItem>();
    private readonly Dictionary<int, RobotItem> _blueRobotItems = new Dictionary<int, RobotItem>();
    private readonly Dictionary<int, User> _redRobots = new Dictionary<int, User>();
    private readonly Dictionary<int, User> _blueRobots = new Dictionary<int, User>();

    public RobotController()
    {
        var robotItems = Object.FindObjectsOfType<RobotItem>();
        
        foreach (var robotItem in robotItems)
        {
            var team = robotItem.GetFieldValue<int>("team_id");
            var robotId = robotItem.GetFieldValue<int>("robot_id");
            var user = robotItem.GetFieldValue<User>("robot");
            
            if (team == 0)
            {
                _redRobotItems[robotId] = robotItem;
                _redRobots[robotId] = user;
            }
            else
            {
                _blueRobotItems[robotId] = robotItem;
                _blueRobots[robotId] = user;
            }
        }
    }
    
    [Route(HttpVerbs.Get, "/list")]
    public async Task<Response<RobotInfo[]>> ListRobots()
    {
        var redRobotInfos = await Task.WhenAll(_redRobotItems.Values.Select(i => GetRobotInfo(i)));
        var blueRobotInfos = await Task.WhenAll(_blueRobotItems.Values.Select(i => GetRobotInfo(i)));
        
        var robotInfos = redRobotInfos.Concat(blueRobotInfos).ToArray();
        
        return Response<RobotInfo[]>.Create(robotInfos);
    }

    [Route(HttpVerbs.Get, "/{id}")]
    public async Task<Response<RobotInfo>> GetRobotInfo(int id)
    {
        if (id > 100)
        {
            if (!_blueRobotItems.TryGetValue(id, out var robotItem))
            {
                return Response<RobotInfo>.Create(default, "not found");
            }
            
            return Response<RobotInfo>.Create(await GetRobotInfo(robotItem));
        }
        else
        {
            if (!_redRobotItems.TryGetValue(id, out var robotItem))
            {
                return Response<RobotInfo>.Create(default, "not found");
            }
            
            return Response<RobotInfo>.Create(await GetRobotInfo(robotItem));
        }
        
        return Response<RobotInfo>.Create(default, "invalid team");
    }
    
    private async Task<RobotInfo> GetRobotInfo(RobotItem robotItem, User user = null)
    {
        await Task.Yield();
        user ??= robotItem.GetFieldValue<User>("robot");

        int bullet;
        if (user.m_data.m_tid == 2U)
        {
            bullet = (int) user.m_data.bigShoot.GetVal(enumAttr.attr_Item_Bullet);
        }
        else if (user.m_data.m_tid == 1U || user.m_data.m_tid == 4U || user.m_data.m_tid == 5U)
        {
            bullet = (int) user.m_data.smallShoot1.GetVal(enumAttr.attr_Item_Bullet);
        }
        else
        {
            bullet = 0;
        }
        
        return new RobotInfo
        {
            team = robotItem.GetFieldValue<string>("team"),
            teamid = robotItem.GetFieldValue<int>("team_id"),
            robotid = robotItem.GetFieldValue<int>("robot_id"),
            name = robotItem.GetFieldValue<TMP_Text>("RobotName").text,
            exp = user.GetVal(enumAttr.attr_Player_Exp),
            level = user.GetVal(enumAttr.attr_Player_Level),
            expLevelUp = user.GetVal(enumAttr.attr_Player_ExpLevelUp),
            isBalance = user.m_data.m_performance.isBalance == 1,
            bullet = bullet,
            hp = user.GetVal(enumAttr.attr_Player_HP),
            maxHp = user.GetVal(enumAttr.attr_Player_HPMax)
        };
    }
}