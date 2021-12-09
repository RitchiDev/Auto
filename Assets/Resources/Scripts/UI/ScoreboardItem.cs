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
    [SerializeField] private TMP_Text m_Deaths;
    private Player m_Player;
    public Player Player => m_Player;
    public void SetUp(Player player)
    {
        m_Player = player;
        m_Username.text = player.NickName;
        //ScoreManager.Instance.AddScore(player, 0);
    }

    public void SetScore(int score)
    {
        m_InTabScore.text = score.ToString();
        //m_InTabScore.text = "Score: " + score.ToString();
    }

    public void SetDeaths(int deaths)
    {
        m_Deaths.text = deaths.ToString();
        //m_Deaths.text = "Deaths: " + deaths.ToString();
    }

    public void SetKOs(int kOs)
    {
        m_KOs.text = kOs.ToString();
        //m_KOs.text = "KO's: " + kOs.ToString();
    }
}


