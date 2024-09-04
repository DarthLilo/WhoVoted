using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace WhoVoted;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class WhoVoted : BaseUnityPlugin
{
    public static WhoVoted Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    private AssetBundle WhoVotedAssets;
    public static GameObject NewSpecatingBoxPrefab;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        string bundle_path = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "lilowhovotedassets");
        WhoVotedAssets = AssetBundle.LoadFromFile(bundle_path);
        if (WhoVotedAssets == null)
        {
            Logger.LogError("Could not find the required asset bundle! Please make sure the file \"lilobabymaneaterassets\" is in the same directory as this dll!");
        }
        NewSpecatingBoxPrefab = WhoVotedAssets.LoadAsset<GameObject>("Assets/WhoVoted/NewPlayerSpectateBox.prefab");

        Patch();

        Logger.LogInfo($"Running netcode patchers");

        var types = Assembly.GetExecutingAssembly().GetTypes();
           foreach (var type in types)
           {
                //Logger.LogInfo($"Type: {type}");
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {   
                    //Logger.LogInfo($"Method: {method}");
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        //Logger.LogInfo($"Invoking {method}");
                        method.Invoke(null, null);
                    }
                }
           }

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }
}
