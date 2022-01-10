using Photon.Realtime;
using TMPro;
using UnityEngine;
using Andrich.UtilityScripts;
using Photon.Pun;
public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_RoomNameText;
    [SerializeField] private TMP_Text m_GameModeText;
    [SerializeField] private TMP_Text m_PlayersInRoomText;
    private RoomInfo m_RoomInfo;
    public RoomInfo Info => m_RoomInfo;

    public void SetUp(RoomInfo info, string gameModeName = null)
    {
        m_RoomInfo = info;
        m_RoomNameText.text = info.Name;
        m_GameModeText.text = gameModeName;
        m_PlayersInRoomText.text = info.PlayerCount + " / " + info.MaxPlayers;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(m_RoomInfo);
    }
}
