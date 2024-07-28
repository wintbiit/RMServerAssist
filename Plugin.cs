using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using RMServerAssist.Router;
using UnityEngine;

namespace RMServerAssist;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private new static ManualLogSource Logger { get; set; }
    
    public const int Port = 35333;

    private WebServer _webServer;
    private PluginConfig _pluginConfig;

    private void Awake()
    {
        Logger = base.Logger;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => App.Instance != null && App.Instance.GetState() == eAppState.Room);
        
        Logger.LogInfo("Initializing RMServerAssist...");
        
        _pluginConfig = PluginConfig.Load();

        App.Instance.ServeruiID = 71;

        _webServer = new WebServer(o => o
                .WithUrlPrefix($"http://*:{Port}/")
                .WithMode(HttpListenerMode.Microsoft))
            .WithCors()
            .WithLocalSessionManager()
            .WithWebApi("/api/v1/game", m => m
                .WithController<MatchInfoController>())
            .WithWebApi("/api/v1/judge", m => m
                .WithController<JudgeController>())
            .WithWebApi("/api/v1/robot", m => m
                .WithController<RobotController>())
            .WithWebApi("/api", m => m
                .WithController<ApiRootController>());

        if (!string.IsNullOrEmpty(_pluginConfig.apiKey))
        {
            _webServer.WithModule(new ApiKeyModule("/api/v1", _pluginConfig.apiKey));
            Logger.LogInfo($"Web server successfully initialized with API key, send requests with header '{ApiKeyModule.ApiKeyHeaderKey}'");
        }

        _webServer.StateChanged += (sender, args) =>
        {
            Logger.LogInfo($"Server state changed to {args.NewState}");
        };

        _webServer.HandleHttpException(async (context, exception) =>
        {
            context.Response.StatusCode = exception.StatusCode;
            
            if (context.Response.StatusCode == 404)
            {
                await context.SendDataAsync(Response<string>.Create("", "not found"));
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.SendDataAsync(Response<string>.Create("", "bad request"));
                
                Logger.LogError($"Http server exception: {exception.Message}");
            }
            
            await Task.CompletedTask;
        });
        
        yield return _webServer.RunAsync();
                    
        Logger.LogInfo($"Server started at port {Port}");
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ProtoGen();
        }
    }

    private static async void ProtoGen()
    {
        Logger.LogInfo("Generating proto files...");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
        
        await Task.Yield();
        var dir = Path.Combine(Application.dataPath, "../proto");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ProtoDumper.DumpProtoId(Path.Combine(dir, "proto.csv"));
        ProtoDumper.DumpBattleProtoId(Path.Combine(dir, "battle_proto.csv"));
        await ProtoDumper.DumpProtoDef(dir);
        
        watch.Stop();
        
        Logger.LogInfo($"Proto files generated in {watch.ElapsedMilliseconds}ms");
    }
    
    private void OnDestroy()
    {
        _webServer?.Dispose();
    }
}

public class ApiRootController : WebApiController
{
    [Route(HttpVerbs.Get, "/ping")]
    public async Task<Response<string>> Ping()
    {
        await Task.Yield();
        
        return Response<string>.Create("pong");
    }

    [Route(HttpVerbs.Get, "/version")]
    public async Task<Response<AppVersion>> Version()
    {
        await Task.Yield();
        
        return Response<AppVersion>.Create(new AppVersion(App.Instance.serverVersion, App.Instance.ServerUIVersion, PluginInfo.PLUGIN_VERSION));
    }
    
    private string _apiRes = ResourceHelper.GetEmbeddedResource("RMServerAssist.RMServerAssist.html");

    [Route(HttpVerbs.Get, "/openapi.html")]
    public async Task OpenApi()
    {
        await Task.Yield();

        await using var writer = HttpContext.OpenResponseText();
        await writer.WriteAsync(_apiRes);
    }

    [Serializable]
    public struct AppVersion(string serverVersion, string uiVersion, string pluginVersion)
    {
        public string serverVersion = serverVersion;
        public string uiVersion = uiVersion;
        public string pluginVersion = pluginVersion;
    }
}