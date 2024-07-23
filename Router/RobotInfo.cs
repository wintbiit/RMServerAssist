using System;

namespace RMServerAssist.Router;

[Serializable]
public struct RobotInfo
{
    public string team;
    public int teamid;
    public int robotid;
    public string name;
    public float level;
    public float exp;
    public float expLevelUp;
    public bool isBalance;
    public int bullet;
    public float hp;
    public float maxHp;
}