using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Andrich.UtilityScripts;
using TMPro;
using UnityEngine.UI;

public class EliminationPlayerController : MonoBehaviourPunCallbacks
{
    [Header("Components")]
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private ItemController m_ItemController;
    [SerializeField] private InGameUI m_InGameUI;
    [SerializeField] private CarController m_CarController;
    [SerializeField] private CameraController m_CameraController;
    [SerializeField] private Collider m_Collider;
    [SerializeField] private GameObject m_Camera;
    private Rigidbody m_Rigidbody;
    private PlayerManager m_PlayerManager;
    private Player m_Player;
    public Player Owner => m_Player;

    [Header("Controls")]
    [SerializeField] private KeyCode m_RespawnKey = KeyCode.F;

    [Header("Respawn")]
    [SerializeField] private float m_MaxRespawnTime = 3f;
    [SerializeField] private float m_MaxRespawnDelay = 1.1f;
    [SerializeField] private TMP_Text m_RespawnTimeText;
    [SerializeField] private Image m_RespawnImage;
    [SerializeField] private GameObject m_RespawnEffect;
    [SerializeField] private List<GameObject> m_ObjectsToHideDuringRespawn = new List<GameObject>();
    private IEnumerator m_Respawn;
    private bool m_DisableRespawning;
    private float m_RespawnTimer;

    [Header("Respawn After KO")]
    [SerializeField] private float m_MaxRespawnDelayAfterKO = 3f;
    [SerializeField] private GameObject m_RespawnAfterKOEffect;

    [Header("Eliminated")]
    [SerializeField] private float m_MaxEliminateDelay = 1.1f;
    [SerializeField] private GameObject m_EliminateEffect;
    private IEnumerator m_Eliminate;

    [Header("UI")]
    [SerializeField] private GameObject m_UsernameText;

    [Header("UI Effect")]
    [SerializeField] private float m_TimeBeforeInDanger = 10f;
    [SerializeField] private GameObject m_UIEffectCamera;
    [SerializeField] private GameObject m_InDangerEffect;
    private bool m_IsInDanger;

    [Header("Misc")]
    private bool m_GameHasBeenWon;
    public bool GameHasBeenWon => m_GameHasBeenWon;

    private void Awake()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (m_PhotonView.IsMine)
        {
            m_DisableRespawning = false;
            m_RespawnTimer = m_MaxRespawnTime;
            m_PhotonView.RPC("RPC_AddPlayerToAliveList", RpcTarget.All, PhotonNetwork.LocalPlayer);
            m_InDangerEffect.SetActive(false);
        }
        else
        {
            //m_UIEffectCamera.SetActive(false);
            Destroy(m_UIEffectCamera);
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
        if(!m_PhotonView.IsMine || m_DisableRespawning || m_GameHasBeenWon)
        {
            if(m_RespawnImage)
            {
                m_RespawnImage.SetActive(false);
            }

            return;
        }

        CheckIfInDanger();
        CheckForRespawn();
    }


    public void SetInDanger(bool isInDanger)
    {
        m_IsInDanger = isInDanger;
    }

    private void CheckIfInDanger()
    {
        if (!m_UIEffectCamera)
        {
            //Debug.Log("No Camera");
            return;
        }

        if (!m_IsInDanger || GameModeManager.Instance.SelectedGameMode.GameModeName != "Elimination" || PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
        {
            //Debug.Log("Can't be put in danger");

            m_InDangerEffect.SetActive(false);
            return;
        }

        //Debug.Log((float)PhotonNetwork.CurrentRoom.GetTime());
        if(m_IsInDanger)
        {
            if ((float)PhotonNetwork.CurrentRoom.GetTime() <= m_TimeBeforeInDanger && !PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
            {
                //Debug.Log("Effect active");

                m_InDangerEffect.SetActive(true);
            }
        }
    }

    private void CheckForRespawn()
    {
        if (Input.GetKey(m_RespawnKey))
        {
            m_RespawnImage.SetActive(true);

            m_RespawnTimer -= Time.deltaTime;

            if (m_RespawnTimer <= 0)
            {
                m_RespawnTimer = m_MaxRespawnTime;

                m_Respawn = RespawnDelay("Respawned", false);
                StartCoroutine(m_Respawn);
            }
        }
        else
        {
            if (m_RespawnTimer <= m_MaxRespawnTime)
            {
                m_RespawnTimer += Time.deltaTime * m_MaxRespawnTime;
            }
        }

        m_RespawnImage.fillAmount = m_RespawnTimer / m_MaxRespawnTime;
        m_RespawnTimeText.text = m_RespawnTimer.ToString("0");

        if (m_RespawnTimer >= m_MaxRespawnTime)
        {
            m_RespawnImage.SetActive(false);
        }
    }

    private IEnumerator RespawnDelay(string deathCause, bool afterKO)
    {
        Vector3 freezePoisition = transform.position;

        m_DisableRespawning = true;
        m_CarController.Disable(true);

        PhotonEvents.RaiseAddRespawnToUIEvent(m_Player.NickName, deathCause, afterKO);
        //RaiseAddRespawnToUIEvent(m_Player.NickName, deathCause, afterKO);

        //m_PhotonView.RPC("RPC_AddRespawnToUI", RpcTarget.All, name, deathCause, afterKO);
        //m_InGameUI.AddRespawnToUI(m_Player.NickName, deathCause, afterKO);

        m_PhotonView.RPC("RPC_InstantiateRespawnEffect", RpcTarget.All, afterKO);

        FreezeRigidbody(true);
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, false);

        float totalTime = afterKO ? m_MaxRespawnDelayAfterKO : m_MaxRespawnDelay;

        while (totalTime >= 0)
        {
            transform.position = freezePoisition;

            totalTime -= Time.deltaTime;

            yield return null;
        }

        FreezeRigidbody(false);
        //m_Rigidbody.constraints = RigidbodyConstraints.None;

        m_PhotonView.RPC("RPC_Respawn", RpcTarget.All);

        m_CarController.Disable(false);

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, true);

        m_DisableRespawning = false;
    }

    private void FreezeRigidbody(bool doFreeze)
    {
        if(m_CameraController)
        {
            m_CameraController.FreezeCameras(doFreeze);
        }

        if(m_ItemController)
        {
            m_ItemController.Disabled(doFreeze);
        }

        if(!m_Rigidbody)
        {
            Debug.LogWarning("There is no Rigidbody");
            return;
        }

        if(doFreeze)
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.freezeRotation = true;
        }
        else
        {
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            m_Rigidbody.useGravity = true;
            m_Rigidbody.freezeRotation = false;
        }
    }

    public void SetStunned(bool isStunned)
    {
        if(m_DisableRespawning)
        {
            return;
        }

        FreezeRigidbody(isStunned);
        m_CarController.Disable(isStunned);
    }

    public void Eliminate()
    {
        m_Eliminate = EliminateDelay();
        StartCoroutine(m_Eliminate);
    }

    public void KO(string deathCause)
    {
        m_RespawnTimer = m_MaxRespawnTime; // Just In Case So You Can't Respawn

        m_Respawn = RespawnDelay(deathCause, true);
        StartCoroutine(m_Respawn);
    }

    private IEnumerator EliminateDelay()
    {
        PhotonEvents.RaiseAddEliminateToUIEvent(m_Player.NickName);
        //RaiseAddEliminateToUIEvent(m_Player.NickName);

        //m_PhotonView.RPC("RPC_AddEliminateToUI", RpcTarget.All, name);
        //m_InGameUI.AddEliminateToUI(m_Player.NickName);

        if (m_Respawn != null)
        {
            StopCoroutine(m_Respawn);
        }

        m_DisableRespawning = true;

        if(m_CarController)
        {
            m_CarController.Disable(true);
        }

        FreezeRigidbody(true);
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

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

        if (m_UsernameText)
        {
            if (m_PhotonView.IsMine && m_Player != PhotonNetwork.LocalPlayer)
            {
                m_UsernameText.SetActive(value);
            }
        }

        m_Collider.enabled = value;

        m_ItemController.HideItems();
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        m_PlayerManager.RespawnPlayer();
    }

    [PunRPC]
    private void RPC_InstantiateRespawnEffect(bool afterKO)
    {
        if(afterKO)
        {
            Instantiate(m_RespawnAfterKOEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(m_RespawnEffect, transform.position, Quaternion.identity);
        }
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
