using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace WhoVoted.Helpers
{
    internal class WhoVotedNetworkHelper : NetworkBehaviour
    {
        public static WhoVotedNetworkHelper Instance { get; private set; }

        private void Start()
        {
            Instance = this;
            WhoVoted.Logger.LogInfo("WhoVotedNetworkHelper.Start() initialized!");
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncWhoVotedServerRpc(string inc_ulong)
        {
            if (inc_ulong != null)
            {
                ulong TargetPlayerID = ulong.Parse(inc_ulong);

                foreach(PlayerControllerB playerScript in StartOfRound.Instance.allPlayerScripts)
                {
                    if (playerScript.NetworkObjectId == TargetPlayerID)
                    {
                        if (HUDManager.Instance != null && HUDManager.Instance.spectatingPlayerBoxes != null)
                        {
                            Animator key = HUDManager.Instance.spectatingPlayerBoxes.FirstOrDefault((KeyValuePair<Animator, PlayerControllerB> x) => x.Value == playerScript).Key;
                            if (key != null)
                            {
                                GameObject HasVotedImage = key.gameObject.transform.Find("HasVoted").gameObject;
                                HasVotedImage.GetComponent<RawImage>().enabled = true;
                            }
                            
                        }
                        
                    }
                }
                SyncWhoVotedClientRpc(inc_ulong);
            } else {
                WhoVoted.Logger.LogError("Received null ulong, skipping");
            }
        }

        [ClientRpc]
        public void SyncWhoVotedClientRpc(string inc_ulong)
        {
            if (inc_ulong != null)
            {
                ulong TargetPlayerID = ulong.Parse(inc_ulong);

                foreach(PlayerControllerB playerScript in StartOfRound.Instance.allPlayerScripts)
                {
                    if (playerScript.NetworkObjectId == TargetPlayerID)
                    {
                        if (HUDManager.Instance != null && HUDManager.Instance.spectatingPlayerBoxes != null)
                        {
                            Animator key = HUDManager.Instance.spectatingPlayerBoxes.FirstOrDefault((KeyValuePair<Animator, PlayerControllerB> x) => x.Value == playerScript).Key;
                            if (key != null)
                            {
                                GameObject HasVotedImage = key.gameObject.transform.Find("HasVoted").gameObject;
                                HasVotedImage.GetComponent<RawImage>().enabled = true;
                            }
                            
                        }
                        
                    }
                }
            } else {
                WhoVoted.Logger.LogError("Received null ulong, skipping");
            }
        }
    }
}