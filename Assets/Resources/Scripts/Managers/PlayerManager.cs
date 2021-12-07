using Photon.Pun;
using System.Collections;
using System.IO;
using UnityEngine;
using Andrich.UtilityScripts;

[RequireComponent(typeof(PhotonView))]
public abstract class PlayerManager : MonoBehaviourPunCallbacks
{
    public abstract void CreatePlayerController();
    public abstract void CreatePlayerSpectator();

    public abstract void RespawnPlayer();
    public abstract void RespawnPlayerAsSpectator();

    public virtual void AddDeathToUI(string name)
    {
        Debug.Log("Added death to UI");
    }
}
