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
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_Image;
    [SerializeField] private GameObject m_BlockImage;
    [SerializeField] private TMP_Text m_ReadyStateText;
    private Player m_Player;
    public Player Player => m_Player;

    private void Awake()
    {
        PhotonNetwork.LocalPlayer.SetReadyState(false);
    }

    public void SetUp(Player player)
    {
        m_Player = player;

        if(PhotonNetwork.LocalPlayer != player)
        {
            m_BlockImage.SetActive(true);
            m_Button.interactable = false;
        }
        else
        {
            m_BlockImage.SetActive(false);
        }

        UpdateToggleState(player);
    }

    public void ReadyUp()
    {
        PhotonNetwork.LocalPlayer.SetReadyState(!PhotonNetwork.LocalPlayer.GetIfReady());
    }

    public void UpdateToggleState(Player player)
    {
        if(player == m_Player)
        {
            if(m_ReadyStateText)
            {
                m_ReadyStateText.text = player.GetIfReady() ? "Ready!" : "Not Ready";
            }

            if(m_Image)
            {
                m_Image.color = player.GetIfReady() ? Color.green : Color.red;
            }
        }
    }
}
