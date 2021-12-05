using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class EliminationPlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private GameObject m_Car;

    PlayerManager m_PlayerManager;

    private Player m_Player;
    public Player Player => m_Player;
    private bool m_IsEliminated;

    private void Awake()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        PhotonNetwork.LocalPlayer.SetScore(0);
        
        if (m_PhotonView.IsMine)
        {
            //m_Player = PhotonNetwork.LocalPlayer;
            PhotonNetwork.LocalPlayer.SetEliminated(false);
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.AllBuffered);
        }
    }

    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList()
    {
        m_Player = PhotonNetwork.LocalPlayer;
        EliminateManager.Instance.AddAlivePlayer(this);
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        if (PhotonNetwork.LocalPlayer.GetIfEliminated()) // If the player is already eliminated
        {
            return;
        }

        Player[] otherPlayers = PhotonNetwork.PlayerListOthers;

        bool eliminate = false;

        foreach (Player otherPlayer in otherPlayers)
        {
            if(!otherPlayer.GetIfEliminated())
            {
                if (PhotonNetwork.LocalPlayer.GetScore() < otherPlayer.GetScore())
                {
                    Debug.Log("Eliminated");
                    PhotonNetwork.LocalPlayer.SetEliminated(true);
                    eliminate = true;
                }
            }
        }

        if (eliminate)
        {
            m_PlayerManager.RespawnPlayer();
        }
    }
}
