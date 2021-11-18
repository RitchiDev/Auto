using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Username;
    [SerializeField] private TMP_Text m_Score;
    [SerializeField] private TMP_Text m_KOs;
    private Player m_Player;

    public void SetUp(Player player)
    {
        m_Player = player;
        m_Username.text = player.NickName;
    }

    public Player Player()
    {
        return m_Player;
    }
}


