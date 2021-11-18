using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class ReadyToggle : MonoBehaviourPun
{
    private PhotonView m_PhotonView;
    [SerializeField] private Toggle m_Toggle;
    [SerializeField] private TMP_Text m_ReadyStateText;
    private bool m_PlayerIsReady = false;
    private Player m_Player;
    public Player Player => m_Player;

    private void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public void SetUp(Player player)
    {
        m_Player = player;

        if(PhotonNetwork.LocalPlayer != player)
        {
            m_Toggle.gameObject.SetActive(false);
            //m_Toggle.interactable = false;
        }
        else
        {
            //m_Toggle.interactable = true;
            PhotonHashtable initialProperties = new PhotonHashtable() { { PlayerProperties.PlayerIsReady, m_PlayerIsReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProperties);
            m_Toggle.onValueChanged.AddListener(delegate { UpdateToggle(); }) ;
        }
    }

    public void UpdateToggle()
    {
        m_PlayerIsReady = !m_PlayerIsReady;
        m_PhotonView.RPC("RPCUpdateToggle", RpcTarget.All);

        PhotonHashtable newProperties = new PhotonHashtable() { { PlayerProperties.PlayerIsReady, m_PlayerIsReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
    }

    [PunRPC]
    private void RPCUpdateToggle()
    {
        m_ReadyStateText.text = m_PlayerIsReady ? "Ready!" : "Not Ready";
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Ready: " + m_PlayerIsReady);
    }
}
