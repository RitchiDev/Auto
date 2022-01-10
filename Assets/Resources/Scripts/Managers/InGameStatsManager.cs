using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;
public class InGameStatsManager : MonoBehaviourPunCallbacks
{
    public static InGameStatsManager Instance { get; private set; }
    [SerializeField] private List<TabScoreboard> m_TabScoreboards = new List<TabScoreboard>();
    [SerializeField] private List<OnScreenScoreboard> m_OnScreenScoreboards = new List<OnScreenScoreboard>();

    private void Awake()
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        for (int i = 0; i < m_TabScoreboards.Count; i++)
        {
            TabScoreboard scoreboard = m_TabScoreboards[i];
            scoreboard.UpdateScoreboardItemText(targetPlayer);

            //if(targetPlayer == scoreboard.Player) // Old for checking to set the on screen score
            //{
            //    InGameUI inGameUI = scoreboard.GetComponentInParent<InGameUI>();
            //    if (inGameUI)
            //    {
            //        inGameUI.SetOnscreenScore(targetPlayer.GetScore());
            //    }
            //}
        }

        for (int i = 0; i < m_OnScreenScoreboards.Count; i++)
        {
            OnScreenScoreboard scoreboard = m_OnScreenScoreboards[i];
            scoreboard.UpdateScoreboardItem(targetPlayer);
        }
    }

    public void AddTabScoreboard(TabScoreboard scoreboard)
    {
        m_TabScoreboards.Add(scoreboard);
    }

    public void RemoveTabScoreboard(TabScoreboard scoreboard)
    {
        m_TabScoreboards.Remove(scoreboard);
    }

    public void AddOnScreenScoreboard(OnScreenScoreboard scoreboard)
    {
        m_OnScreenScoreboards.Add(scoreboard);
    }

    public void RemoveOnScreenScoreboard(OnScreenScoreboard scoreboard)
    {
        m_OnScreenScoreboards.Remove(scoreboard);
    }
}
