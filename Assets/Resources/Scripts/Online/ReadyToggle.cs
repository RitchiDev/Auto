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
    [SerializeField] private Toggle m_Toggle;
    [SerializeField] private TMP_Text m_ReadyStateText;
    private PhotonHashtable m_Hashtable;
    private PhotonView m_PhotonView;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        m_Hashtable = new PhotonHashtable();
    }

    public void Interactable(bool value)
    {
        m_Toggle.interactable = value;
    }

    public void UpdateReadyState()
    {
        m_PhotonView.RPC("RPC_UpdateReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);

        m_Hashtable["ReadyState"] = m_Toggle.isOn;
        PhotonNetwork.SetPlayerCustomProperties(m_Hashtable);
        //Launcher.Instance.UpdateReadyState(m_Toggle.isOn);
    }

    [PunRPC]
    private void RPC_UpdateReadyState(string test)
    {
        PlayerListItem[] items = FindObjectsOfType<PlayerListItem>();
        Debug.Log(items.Length);
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i].Player?.NickName == test)
            {
                Debug.Log(test + " Toggled");
                items[i].ToggleReadyState();
                //m_ReadyStateText.text = m_Toggle.isOn ? "Ready!" : "Not Ready";
                //m_ReadyStateText.text = m_Toggle.isOn ? "Ready!" : "Not Ready";
            }
        }
    }
    
    public void UpdateToggle(Player player)
    {
        if(player == PhotonNetwork.LocalPlayer)
        {
            Debug.Log(player.NickName + " Edited Toggle");
            m_ReadyStateText.text = m_Toggle.isOn ? "Ready!" : "Not Ready";
        }
    }
}
