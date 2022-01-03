using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DisconnectPlayerManager : MonoBehaviourPunCallbacks
{
    public static DisconnectPlayerManager Instance { get; private set; }
    private GameObject m_PlayerGameObject;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0); // Titlescreen

        base.OnLeftRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if(m_PlayerGameObject)
        {
            PhotonNetwork.Destroy(m_PlayerGameObject);
        }

        base.OnDisconnected(cause);
    }

    public void SetPlayerGameObject(GameObject playerGameObject)
    {
        m_PlayerGameObject = playerGameObject;
    }
}
