using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
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
        m_PhotonView.RPC("RPCAddScore", RpcTarget.AllBuffered, player, scoreToAdd);
    }

    [PunRPC]
    private void RPC_AddScore(Player player, int scoreToAdd)
    {
        player.AddScore(scoreToAdd);
        if(ScoreboardManager.Instance)
        {
            ScoreboardManager.Instance.UpdateScoreboardItemText(player, player.GetScore());
        }
    }

    public void AddScoreboard(Player player)
    {
        //ScoreboardItem item = Instantiate(m_ScoreboardItemPrefab, m_Container).GetComponent<ScoreboardItem>();
        //m_ScoreboardItems.Add(item);

        //ScoreExtensions.SetScore(player, 0);
        //item.SetUp(player);
    }
}
