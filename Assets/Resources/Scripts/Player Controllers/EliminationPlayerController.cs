using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Andrich.UtilityScripts;

public class EliminationPlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;

    PlayerManager m_PlayerManager;

    private Player m_Player;
    public Player Player => m_Player;
    private bool m_IsEliminated;

    private void Awake()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        //PhotonNetwork.LocalPlayer.SetScore(0);
        
        if (m_PhotonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.SetEliminated(false);
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All);
        }
    }

    public void Eliminate()
    {
        EliminateManager.Instance.RemoveAlivePlayer(this);
        Debug.Log("Eliminate Called");
        if(!m_Player.GetIfEliminated())
        {
            Debug.Log("Hier");
            m_PlayerManager.RespawnPlayerAsSpectator();
        }
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList()
    {
        m_Player = PhotonNetwork.LocalPlayer;
        EliminateManager.Instance.AddAlivePlayer(this);
    }
}
