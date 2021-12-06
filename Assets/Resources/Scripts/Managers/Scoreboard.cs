using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform m_Container;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<ScoreboardItem> m_ScoreboardItems = new List<ScoreboardItem>();

    private void Start()
    {
        ScoreManager.Instance.AddScoreboard(this);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboard(player);
        }
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.RemoveScoreboard(this);
    }

    private void AddScoreboard(Player player)
    {
        ScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_Container).GetComponent<ScoreboardItem>();
        m_ScoreboardItems.Add(item);

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

    public void UpdateScoreboardItemText(Player player, int score)
    {
        foreach (ScoreboardItem item in m_ScoreboardItems)
        {
            if (player == item.Player())
            {
                item.SetScore(score);
            }
        }
    }

    private void RemoveScoreboardItem(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            ScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player())
            {
                Destroy(item.gameObject);
                m_ScoreboardItems.Remove(item);
            }
        }
    }
}
