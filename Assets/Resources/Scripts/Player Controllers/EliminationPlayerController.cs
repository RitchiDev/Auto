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

    private Player m_Player;
    public Player Player => m_Player;
    private bool m_IsEliminated;

    private void Awake()
    {
        PhotonNetwork.LocalPlayer.SetScore(0);
        
        if (m_PhotonView.IsMine)
        {
            //m_Player = PhotonNetwork.LocalPlayer;
            PhotonNetwork.LocalPlayer.SetEliminated(false);
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All);
        }
    }

    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        //if (!m_PhotonView.IsMine)
        //{
        //    return;
        //}

        if (PhotonNetwork.LocalPlayer.GetIfEliminated())
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
            //Debug.Log(PhotonNetwork.LocalPlayer + ": Eliminated");
            Destroy(GetComponent<Wheel_Effects>());
            Destroy(GetComponent<Car_Controller>());
            Destroy(m_Car);
        }
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList()
    {
        m_Player = PhotonNetwork.LocalPlayer;
        EliminateManager.Instance.AddAlivePlayer(this);
    }
}
