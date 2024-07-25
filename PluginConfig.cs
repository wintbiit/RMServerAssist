using System;
using System.IO;
using Swan.Formatters;
using UnityEngine;

namespace RMServerAssist;

[Serializable]
public struct PluginConfig
{
    public string apiKey;

    public static PluginConfig Load()
    {
        var path = Path.Join(Application.dataPath, $"../BepInEx/config/{PluginInfo.PLUGIN_GUID}.json");
        
        if (!File.Exists(path))
        {
            return new PluginConfig();
        }
        
        var json = File.ReadAllText(path);
        
        return Json.Deserialize<PluginConfig>(json);
    }
}