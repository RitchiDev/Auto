using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Username;
    [SerializeField] private TMP_Text m_InTabScore;
    [SerializeField] private TMP_Text m_KOs;
    private Player m_Player;
    private int m_CurrentScore;
    public int CurrentScore => m_CurrentScore;

    public void SetUp(Player player)
    {
        m_Player = player;
        m_Username.text = player.NickName;
    }

    public void SetScore(int score)
    {
        m_CurrentScore = score;
        m_InTabScore.text = "Score: " + m_CurrentScore.ToString();
        Debug.Log(m_InTabScore.text);
    }

    public Player Player()
    {
        return m_Player;
    }
}


