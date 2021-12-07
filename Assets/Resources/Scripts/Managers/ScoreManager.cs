using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private List<Scoreboard> m_Scoreboards = new List<Scoreboard>();
    private PhotonView m_PhotonView;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

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

    public void AddScore(Player player, int scoreToAdd)
    {
        m_PhotonView.RPC("RPC_AddScore", RpcTarget.AllBuffered, player, scoreToAdd);
    }

    [PunRPC]
    private void RPC_AddScore(Player player, int scoreToAdd)
    {
        player.AddScore(scoreToAdd);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        for (int i = 0; i < m_Scoreboards.Count; i++)
        {
            Scoreboard scoreboard = m_Scoreboards[i];
            scoreboard.UpdateScoreboardItemText(targetPlayer, targetPlayer.GetScore());

            InGameUI inGameUI = scoreboard.GetComponentInParent<InGameUI>();
            if (inGameUI)
            {
                inGameUI.SetOnscreenScore(targetPlayer.GetScore());
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
