using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_RoomNameText;
    private RoomInfo m_RoomInfo;

    public RoomInfo Info => this.m_RoomInfo;

    public void SetUp(RoomInfo info)
    {
        m_RoomInfo = info;
        m_RoomNameText.text = info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(m_RoomInfo);
    }
}
