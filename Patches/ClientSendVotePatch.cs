using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using WhoVoted.Helpers;

namespace WhoVoted.Patches;

[HarmonyPatch(typeof(HUDManager))]
public class WhoVotedUIPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void StartPatch(HUDManager __instance)
    {
        __instance.spectatingPlayerBoxPrefab = WhoVoted.NewSpecatingBoxPrefab;
    }

    [HarmonyPatch("RemoveSpectateUI")]
    [HarmonyPrefix]
    private static void RemoveSpectateUIPatch(HUDManager __instance)
    {
        for (int i = 0; i < __instance.spectatingPlayerBoxes.Count; i++)
        {
            __instance.spectatingPlayerBoxes.ElementAt(i).Key.gameObject.transform.Find("HasVoted").gameObject.GetComponent<RawImage>().enabled = false;
        }
    }
}

[HarmonyPatch(typeof(TimeOfDay))]
public class WhoVotedPatch
{
    [HarmonyPatch("VoteShipToLeaveEarly")]
    [HarmonyPrefix]
    private static void VoteShiptoLeaveEarlyPatch(TimeOfDay __instance)
    {
        if (!__instance.votedShipToLeaveEarlyThisRound)
        {
            WhoVotedNetworkHelper.Instance.SyncWhoVotedServerRpc(HUDManager.Instance.localPlayer.NetworkObjectId.ToString());
        }
    }
}