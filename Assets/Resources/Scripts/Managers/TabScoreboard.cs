using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Andrich.UtilityScripts;
public class TabScoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform m_Container;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<TabScoreboardItem> m_ScoreboardItems = new List<TabScoreboardItem>();
    private Player m_Player;
    public Player Player => m_Player;

    private void Awake()
    {
        m_Player = PhotonNetwork.LocalPlayer;
    }

    private void Start()
    {
        InGameStatsManager.Instance.AddTabScoreboard(this);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            AddScoreboardItem(PhotonNetwork.PlayerList[i]);
        }

        //foreach (Player player in PhotonNetwork.PlayerList)
        //{
        //    AddScoreboardItem(player);
        //}
    }

    private void OnDestroy()
    {
        InGameStatsManager.Instance.RemoveTabScoreboard(this);
    }

    private void AddScoreboardItem(Player player)
    {
        TabScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_Container).GetComponent<TabScoreboardItem>();
        m_ScoreboardItems.Add(item);

        item.SetUp(player);
        UpdateScoreboardItemText(player);
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
            TabScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                item.SetUsernameColor(player.GetIfEliminated());
                item.SetScore(player.GetScore());
                item.SetDeaths(player.GetDeaths());
                item.SetKOs(0);
            }

            //m_ScoreboardItems.Sort(SortByScore());
        }

        //m_ScoreboardItems = m_ScoreboardItems.OrderBy(item => item.Player.GetScore()).ToList();
    }

    private void RemoveScoreboardItem(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            TabScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                Destroy(item.gameObject);
                m_ScoreboardItems.Remove(item);
            }
        }
    }
}
