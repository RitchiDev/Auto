using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInRoomItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text m_PlayerNameText;
    [SerializeField] private GameObject m_ColorSelectContainer;
    private Player m_Player;
    public Player Player => m_Player;

    public void SetUp(Player player, string name)
    {
        if (PhotonNetwork.LocalPlayer != player)
        {
            m_ColorSelectContainer.SetActive(false);
        }
        else
        {
            m_ColorSelectContainer.SetActive(true);
        }

        m_Player = player;
        m_PlayerNameText.text = name;
    }

    public void SetUp(string name)
    {
        m_PlayerNameText.text = name;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (m_Player != otherPlayer)
        {
            return;
        }

        Destroy(gameObject);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
