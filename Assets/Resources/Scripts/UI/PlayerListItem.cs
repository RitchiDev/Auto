using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text m_PlayerNameText;
    [SerializeField] private Toggle m_Toggle;
    private Player m_Player;
    public Player Player => m_Player;

    public void SetUp(Player player, string name)
    {
        m_Player = player;
        m_PlayerNameText.text = name;
        //m_PlayerNameText.text = "<noparse>" + itemText + "</noparse>";
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

    public void ToggleReadyState()
    {
        m_Toggle.isOn = !m_Toggle.isOn;
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
