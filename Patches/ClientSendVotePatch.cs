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
        WhoVotedNetworkHelper.Instance.ClearVotedServerRpc();
    }

    [HarmonyPatch("UpdateBoxesSpectateUI")]
    [HarmonyPostfix]
    private static void SyncVotedClientsOnDeath(HUDManager __instance)
    {   
        PlayerControllerB playerScript;
        for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
        {
            playerScript = StartOfRound.Instance.allPlayerScripts[i];
            if (WhoVotedNetworkHelper.VotedClients.ContainsKey(playerScript.NetworkObjectId.ToString()))
            {
                Animator key = __instance.spectatingPlayerBoxes.FirstOrDefault((KeyValuePair<Animator, PlayerControllerB> x) => x.Value == playerScript).Key;
                if (key != null)
                    {
                        GameObject HasVotedImage = key.gameObject.transform.Find("HasVoted").gameObject;
                        HasVotedImage.GetComponent<RawImage>().enabled = true;
                    }
            }
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
        if (HUDManager.Instance != null && HUDManager.Instance.localPlayer != null && !__instance.votedShipToLeaveEarlyThisRound)
        {
            string playerNetworkObjectID = HUDManager.Instance.localPlayer.NetworkObjectId.ToString();
            WhoVotedNetworkHelper.Instance.SyncWhoVotedServerRpc(playerNetworkObjectID);
        }
    }
}