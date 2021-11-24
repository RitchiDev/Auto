using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    public static ScoreboardManager Instance { get; private set; }
    [SerializeField] private PhotonView m_PhotonView;

    [SerializeField] private Transform m_Container;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<ScoreboardItem> m_ScoreboardItems = new List<ScoreboardItem>();

    private bool m_Test;

    private void Awake()
    {
        if(m_PhotonView.IsMine)
        {
            if (Instance)
            {
                Debug.LogError("An instance of: " + this.ToString() + " already existed!");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }

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
                if (m_PhotonView.IsMine)
                {
                    InGameUIManager.Instance.SetOnscreenScore(score);
                }

                item.SetScore(score);
            }
        }
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
