using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class OnScreenScoreboardItem : MonoBehaviour
{
    [SerializeField] private Image m_BackgroundImage;
    [SerializeField] private Color m_IsMineColor, m_IsNotMineColor;
    [SerializeField] private TMP_Text m_PlacementText, m_UsernameText, m_ScoreText;
    private int m_Score;
    public int Score => m_Score;
    private Player m_Player;
    public Player Player => m_Player;

    public void SetUp(Player player)
    {
        m_Player = player;

        SetPlacement(1);
        SetUsername(player.NickName);
        SetScore(0);

        Debug.Log("SetUp Scoreboard Item!!!!!!!!!!! " + player.NickName);

        m_BackgroundImage.color = player == PhotonNetwork.LocalPlayer ? m_IsMineColor : m_IsNotMineColor;
    }

    public void SetPlacement(int placement)
    {
        m_PlacementText.text = placement.ToString() + ".";
    }

    public void SetUsername(string username)
    {
        m_UsernameText.text = username;
    }

    public void SetScore(int score)
    {
        m_ScoreText.text = score.ToString();
        m_Score = score;
    }
}
