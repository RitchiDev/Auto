using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Andrich.UtilityScripts;
public class OnScreenScoreboard : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform m_ScoresContainer;
    [SerializeField] private GameObject m_ScoreboardItemPrefab;
    private List<OnScreenScoreboardItem> m_ScoreboardItems = new List<OnScreenScoreboardItem>();
    private Player m_Player;
    public Player Player => m_Player;

    private void Awake()
    {
        m_Player = PhotonNetwork.LocalPlayer;
    }

    private void Start()
    {
        InGameStatsManager.Instance.AddOnScreenScoreboard(this);

        Player[] playersInRoom = PhotonNetwork.PlayerList;
        for (int i = 0; i < playersInRoom.Length; i++)
        {
            AddScoreboardItem(playersInRoom[i]);
            UpdateScoreboardItemsColor(playersInRoom[i]);
        }

        UpdateScoreboardItemsPlacement();
    }

    private void OnDestroy()
    {
        InGameStatsManager.Instance.RemoveOnScreenScoreboard(this);
    }

    private void AddScoreboardItem(Player player)
    {
        OnScreenScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_ScoresContainer).GetComponent<OnScreenScoreboardItem>();
        m_ScoreboardItems.Add(item);

        item.SetUp(player);
        UpdateScoreboardItem(player);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomProperties.TimeProperty))
        {
            UpdateScoreboardItemsPlacement();
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(PlayerProperties.IsEliminatedProperty))
        {
            UpdateScoreboardItemsColor(targetPlayer);
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    public void UpdateScoreboardItem(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            OnScreenScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                item.SetScore(player.GetScore());
            }
        }

        UpdateScoreboardItemsPlacement();
    }

    private void UpdateScoreboardItemsPlacement()
    {
        OnScreenScoreboardItem[] scoreboards = m_ScoreboardItems.OrderByDescending(item => item.Player.GetScore()).ToArray();
        //CleanScoreboardItemsList(scoreboards);

        for (int i = 0; i < scoreboards.Length; i++)
        {
            scoreboards[i].transform.SetSiblingIndex(i);
            scoreboards[i].SetPlacement(i + 1); // +1 because index starts at 0

            if (EliminationGameManager.Instance)
            {
                if(i >= scoreboards.Length - 1)
                {
                    Player player = scoreboards[i].Player;
                    //Debug.Log("Is In Danger: " + player.NickName);

                    EliminationGameManager.Instance.UpdatePlayerWhoIsInDanger(player);
                }
            }
        }
    }

    private void UpdateScoreboardItemsColor(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            if(m_ScoreboardItems[i].Player == player)
            {
                m_ScoreboardItems[i].SetEliminated(player.GetIfEliminated());
            }
        }
    }

    private void RemoveScoreboardItem(Player player)
    {
        for (int i = 0; i < m_ScoreboardItems.Count; i++)
        {
            OnScreenScoreboardItem item = m_ScoreboardItems[i];

            if (player == item.Player)
            {
                Destroy(item.gameObject);
                m_ScoreboardItems.Remove(item);
            }
        }
    }
}
