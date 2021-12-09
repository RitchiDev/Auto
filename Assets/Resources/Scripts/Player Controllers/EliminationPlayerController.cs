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
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Destroy(GetComponent<AudioListener>());
        }
    }

    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        EliminateManager.Instance.RemoveAlivePlayer(this);
        Debug.Log("Eliminate Called");
        m_PlayerManager.RespawnPlayerAsSpectator();
    }

    public void SetPlayer(Player player)
    {
        m_Player = player;
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList(Player player)
    {
        EliminateManager.Instance.AddAlivePlayer(this, player);
    }
}
