using HarmonyLib;
using WhoVoted.Helpers;
using Unity.Netcode;

namespace WhoVoted.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class GameNetworkManagerPatch
{

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void StartPatch(GameNetworkManager __instance)
    {
        __instance.gameObject.AddComponent<WhoVotedNetworkHelper>();
        __instance.gameObject.AddComponent<NetworkObject>();
        WhoVoted.Logger.LogInfo("Network Helper Added!");
    }
}