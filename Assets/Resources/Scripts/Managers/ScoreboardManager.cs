using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform m_Container;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<ScoreboardItem> m_ScoreboardItems = new List<ScoreboardItem>();

    // Start is called before the first frame update
    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboard(player);
        }
    }

    private void AddScoreboard(Player player)
    {
        ScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_Container).GetComponent<ScoreboardItem>();
        item.SetUp(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboard(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    private void RemoveScoreboardItem(Player player)
    {
        foreach (ScoreboardItem item in m_ScoreboardItems)
        {
            if(player == item.Player())
            {
                Destroy(item.gameObject);
                m_ScoreboardItems.Remove(item);
            }
        }
    }
}
