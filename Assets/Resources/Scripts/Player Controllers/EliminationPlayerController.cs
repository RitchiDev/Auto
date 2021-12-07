using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Andrich.UtilityScripts;

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
        //PhotonNetwork.LocalPlayer.SetScore(0);
        
        if (m_PhotonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.SetEliminated(false);
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All);
        }
    }

    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
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
        Debug.Log("Check If Eliminated");

        if (PhotonNetwork.LocalPlayer.GetIfEliminated()) // If the player is already eliminated
        {
            return;
        }

        Player[] otherPlayers = PhotonNetwork.PlayerListOthers;

        bool eliminate = false;

        foreach (Player otherPlayer in otherPlayers)
        {
            Debug.Log(otherPlayer.NickName + ": Check If Score Is Higher");
            Debug.Log(PhotonNetwork.LocalPlayer.GetScore() + " - " + otherPlayer.GetScore());
            if (PhotonNetwork.LocalPlayer.GetScore() < otherPlayer.GetScore())
            {
                Debug.Log("Eliminated");
                PhotonNetwork.LocalPlayer.SetEliminated(true);
                eliminate = true;
            }
        }

        if (eliminate)
        {
            EliminateManager.Instance.RemoveAlivePlayer(this);
            m_PlayerManager.RespawnPlayerAsSpectator();
        }
    }
}
