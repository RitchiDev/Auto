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
    [SerializeField] private GameObject m_Camera;
    [SerializeField] private CarController m_CarController;
    [SerializeField] private GameObject m_RespawnEffect;
    [SerializeField] private GameObject m_EliminateEffect;
    [SerializeField] private KeyCode m_RespawnKey = KeyCode.F;
    [SerializeField] private Image m_RespawnImage;
    [SerializeField] private TMP_Text m_RespawnText;
    [SerializeField] private float m_MaxRespawnTime = 3f;
    [SerializeField] private float m_MaxRespawnDelay = 1.1f;
    [SerializeField] private float m_MaxEliminateDelay = 1.1f;
    [SerializeField] private List<GameObject> m_ObjectsToHideDuringRespawn = new List<GameObject>();
    private bool m_DisableRespawning;
    private float m_RespawnTimer;
    PlayerManager m_PlayerManager;
    private Player m_Player;
    public Player Player => m_Player;
    private Rigidbody m_Rigidbody;
    private IEnumerator m_Respawn;
    private IEnumerator m_Eliminate;

    private void Start()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        //PhotonNetwork.LocalPlayer.SetScore(0);

        if (m_PhotonView.IsMine)
        {
            m_DisableRespawning = false;
            m_RespawnTimer = m_MaxRespawnTime;
            m_Rigidbody = GetComponent<Rigidbody>();
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

    private void OnDestroy()
    {
        if(m_Camera)
        {
            Destroy(m_Camera);
        }
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine || PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon() || m_DisableRespawning)
        {
            if(m_RespawnImage)
            {
                m_RespawnImage.SetActive(false);
            }

            return;
        }

        if(Input.GetKey(m_RespawnKey))
        {
            m_RespawnImage.SetActive(true);

            m_RespawnTimer -= Time.deltaTime;

            if(m_RespawnTimer <= 0)
            {
                m_RespawnTimer = m_MaxRespawnTime;
                m_Respawn = RespawnDelay();
                StartCoroutine(m_Respawn);
            }
        }
        else
        {
            if(m_RespawnTimer <= m_MaxRespawnTime)
            {
                m_RespawnTimer += Time.deltaTime * m_MaxRespawnTime;
            }
        }

        m_RespawnImage.fillAmount = m_RespawnTimer / m_MaxRespawnTime;
        m_RespawnText.text = m_RespawnTimer.ToString("0");

        if (m_RespawnTimer >= m_MaxRespawnTime)
        {
            m_RespawnImage.SetActive(false);
        }
    }

    private IEnumerator RespawnDelay()
    {
        m_DisableRespawning = true;
        m_CarController.Disable(true);

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, false);

        m_PhotonView.RPC("RPC_InstantiateRespawnEffect", RpcTarget.All);

        float totalTime = m_MaxRespawnDelay;
        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            yield return null;
        }

        m_Rigidbody.constraints = RigidbodyConstraints.None;

        m_PhotonView.RPC("RPC_Respawn", RpcTarget.All);

        m_CarController.Disable(false);

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, true);

        m_DisableRespawning = false;
    }


    public void Eliminate()
    {
        m_Eliminate = EliminateDelay();
        StartCoroutine(m_Eliminate);
    }

    private IEnumerator EliminateDelay()
    {
        if(m_Respawn != null)
        {
            StopCoroutine(m_Respawn);
        }

        m_DisableRespawning = true;

        if(m_CarController)
        {
            m_CarController.Disable(true);
        }

        if(m_Rigidbody)
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, false);

        m_PhotonView.RPC("RPC_InstantiateEliminateEffect", RpcTarget.All);

        float totalTime = m_MaxEliminateDelay;
        while (totalTime >= 0)
        {
            m_DisableRespawning = true;

            totalTime -= Time.deltaTime;

            yield return null;
        }

        m_PhotonView.RPC("RPC_Eliminate", RpcTarget.All);
    }

    public void RemovePlayerFromAliveList()
    {
        m_PhotonView.RPC("RPC_RemovePlayerFromAliveList", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetActiveObjects(bool value)
    {
        for (int i = 0; i < m_ObjectsToHideDuringRespawn.Count; i++)
        {
            m_ObjectsToHideDuringRespawn[i].SetActive(value);
        }
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        m_PlayerManager.RespawnPlayer();
    }

    [PunRPC]
    private void RPC_InstantiateRespawnEffect()
    {
        Instantiate(m_RespawnEffect, transform.position, Quaternion.identity);
    }

    [PunRPC]
    private void RPC_Eliminate()
    {
        EliminationGameManager.Instance.RemoveAlivePlayer(this);
        //Debug.Log("Eliminate Called");
        m_PlayerManager.RespawnPlayerAsSpectator();
    }

    [PunRPC]
    private void RPC_InstantiateEliminateEffect()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Instantiate(m_EliminateEffect, position, Quaternion.identity);
    }

    [PunRPC]
    private void RPC_AddPlayerToAliveList(Player player)
    {
        //Debug.Log(EliminationGameManager.Instance);
        EliminationGameManager.Instance.AddAlivePlayer(this, player);
    }

    [PunRPC]
    private void RPC_RemovePlayerFromAliveList()
    {
        //Debug.Log(EliminationGameManager.Instance);
        EliminationGameManager.Instance.RemoveAlivePlayer(this);
    }
}
