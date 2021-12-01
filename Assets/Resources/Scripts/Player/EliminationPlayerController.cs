using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EliminationPlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private GameObject m_CarBody;

    private Player m_Player;
    public Player Player => m_Player;
    private void Awake()
    {
        if (m_PhotonView.IsMine)
        {
            m_Player = PhotonNetwork.LocalPlayer;

            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All);
        }
    }

    
    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
        //m_CarBody.SetActive(false);
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        m_CarBody.SetActive(false);
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList()
    {
        EliminateManager.Instance.AddAlivePlayer(gameObject);
    }
}
