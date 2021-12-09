using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Andrich.UtilityScripts;
using TMPro;
using UnityEngine.UI;

public class EliminationPlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private KeyCode m_RespawnKey = KeyCode.F;
    [SerializeField] private Image m_RespawnImage;
    [SerializeField] private TMP_Text m_RespawnText;
    [SerializeField] private float m_MaxRespawnTime = 5f;
    private float m_RespawnTimer;
    PlayerManager m_PlayerManager;

    private Player m_Player;
    public Player Player => m_Player;
    private bool m_IsEliminated;

    private void Awake()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        //PhotonNetwork.LocalPlayer.SetScore(0);

        if (m_PhotonView.IsMine)
        {
            m_RespawnTimer = 0;
            PhotonNetwork.LocalPlayer.SetEliminated(false);
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Destroy(GetComponent<AudioListener>());
        }
    }
    public void SetPlayer(Player player)
    {
        m_Player = player;
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        if(Input.GetKey(m_RespawnKey))
        {
            m_RespawnImage.SetActive(true);

            m_RespawnTimer += Time.deltaTime;

            if(m_RespawnTimer >= m_MaxRespawnTime)
            {
                m_RespawnTimer = 0;
                m_PhotonView.RPC("RPC_Respawn", RpcTarget.All);
            }
        }
        else
        {
            if(m_RespawnTimer > 0)
            {
                m_RespawnTimer -= Time.deltaTime * m_MaxRespawnTime;
            }
        }

        m_RespawnImage.fillAmount = m_RespawnTimer / m_MaxRespawnTime;
        m_RespawnText.text = m_RespawnTimer.ToString("0");

        if (m_RespawnTimer <= 0)
        {
            m_RespawnImage.SetActive(false);
        }
    }

    public void Eliminate()
    {
        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        m_PlayerManager.RespawnPlayer();
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        EliminateManager.Instance.RemoveAlivePlayer(this);
        Debug.Log("Eliminate Called");
        m_PlayerManager.RespawnPlayerAsSpectator();
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList(Player player)
    {
        EliminateManager.Instance.AddAlivePlayer(this, player);
    }
}
