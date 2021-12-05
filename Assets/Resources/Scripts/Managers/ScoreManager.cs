using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
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
        if(player.GetScore() <= 0)
        {
            player.AddScore(scoreToAdd);
        }

        player.AddScore(scoreToAdd);

        foreach (Scoreboard scoreboard in m_Scoreboards)
        {
            scoreboard.UpdateScoreboardItemText(player, player.GetScore());
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
