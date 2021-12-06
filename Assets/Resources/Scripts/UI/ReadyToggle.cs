using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Andrich.UtilityScripts;

public class ReadyToggle : MonoBehaviourPunCallbacks
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

        if(m_PhotonView.IsMine)
        {
            m_PlayerIsReady = false;
            PhotonNetwork.LocalPlayer.SetReadyState(m_PlayerIsReady);
            m_Toggle.onValueChanged.AddListener(delegate { ReadyUp(); });
        }
    }

    public void SetUp(Player player)
    {
        m_Player = player;

        if(PhotonNetwork.LocalPlayer != player)
        {
            m_Toggle.interactable = false;
        }
    }

    public void ReadyUp()
    {
        Debug.Log("Readied Up");
        m_PlayerIsReady = !m_PlayerIsReady;
        Debug.Log(m_PlayerIsReady);

        PhotonNetwork.LocalPlayer.SetReadyState(m_PlayerIsReady);
        Debug.Log(PhotonNetwork.LocalPlayer.GetIfReady());
    }

    public void UpdateToggleState(Player player)
    {
        //m_Toggle.isOn = !m_Toggle.isOn;
        Debug.Log(player.NickName + " is Ready");
        m_ReadyStateText.text = player.GetIfReady() ? "Ready!" : "Not Ready";
    }
}
