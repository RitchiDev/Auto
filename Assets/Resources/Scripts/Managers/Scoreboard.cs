using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Andrich.UtilityScripts;
public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform m_Container;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<ScoreboardItem> m_ScoreboardItems = new List<ScoreboardItem>();
    private Player m_Player;
    public Player Player => m_Player;

    private void Awake()
    {
        m_Player = PhotonNetwork.LocalPlayer;
    }

    private void Start()
    {
        InGameStatsManager.Instance.AddScoreboard(this);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    private void OnDestroy()
    {
        InGameStatsManager.Instance.RemoveScoreboard(this);
    }

    private void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_Container).GetComponent<ScoreboardItem>();
        m_ScoreboardItems.Add(item);

        item.SetUp(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    public void UpdateScoreboardItemText(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            ScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                item.SetScore(player.GetScore());
                item.SetDeaths(player.GetDeaths());
            }

            //m_ScoreboardItems.Sort(SortByScore());
        }

        //m_ScoreboardItems = m_ScoreboardItems.OrderBy(item => item.Player.GetScore()).ToList();
    }

    private void RemoveScoreboardItem(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            ScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                Destroy(item.gameObject);
                m_ScoreboardItems.Remove(item);
            }
        }
    }
}
