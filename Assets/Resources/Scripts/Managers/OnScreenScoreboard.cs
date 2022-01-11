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

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            AddScoreboardItem(PhotonNetwork.PlayerList[i]);
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
        //OnScreenScoreboardItem[] temp = m_ScoreboardItems.ToArray();
        for (int i = 0; i < scoreboards.Length; i++)
        {
            scoreboards[i].transform.SetSiblingIndex(i);
            scoreboards[i].SetPlacement(i + 1); //+1 because index starts at 0


            if (EliminationGameManager.Instance)
            {
                if(i >= scoreboards.Length - 1)
                {
                    Player player = scoreboards[i].Player;
                    //Debug.Log("Is In Danger: " + player.NickName);

                    EliminationGameManager.Instance.UpdatePlayerWhoIsInDanger(player);
                }
            }
            //player.SetIfInDangerZone(i >= scoreboards.Length);
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
