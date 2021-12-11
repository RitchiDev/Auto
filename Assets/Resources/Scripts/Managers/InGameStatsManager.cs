using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;
public class InGameStatsManager : MonoBehaviourPunCallbacks
{
    public static InGameStatsManager Instance { get; private set; }
    [SerializeField] private List<Scoreboard> m_Scoreboards = new List<Scoreboard>();

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
        for (int i = 0; i < m_Scoreboards.Count; i++)
        {
            Scoreboard scoreboard = m_Scoreboards[i];
            scoreboard.UpdateScoreboardItemText(targetPlayer);

            if(targetPlayer == scoreboard.Player)
            {
                InGameUI inGameUI = scoreboard.GetComponentInParent<InGameUI>();
                if (inGameUI)
                {
                    inGameUI.SetOnscreenScore(targetPlayer.GetScore());
                }
            }
        }
    }

    public void AddScoreboard(Scoreboard scoreboard)
    {
        m_Scoreboards.Add(scoreboard);
    }

    public void RemoveScoreboard(Scoreboard scoreboard)
    {
        m_Scoreboards.Remove(scoreboard);
    }
}
