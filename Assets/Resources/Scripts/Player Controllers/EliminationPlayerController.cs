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
    public Player Player => m_Player;

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

    [Header("Misc")]
    private bool m_GameHasBeenWon;

    private void Start()
    {
        m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<PlayerManager>();
        m_Rigidbody = GetComponent<Rigidbody>();

        if (m_PhotonView.IsMine)
        {
            m_DisableRespawning = false;
            m_RespawnTimer = m_MaxRespawnTime;
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
        if(!m_PhotonView.IsMine || m_DisableRespawning || m_GameHasBeenWon)
        {
            if(m_RespawnImage)
            {
                m_RespawnImage.SetActive(false);
            }

            return;
        }

        CheckForRespawn();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey(RoomProperties.GameHasBeenWonProperty))
        {
            m_GameHasBeenWon = PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon();
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
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
        m_DisableRespawning = true;
        m_CarController.Disable(true);

        RaiseAddRespawnToUIEvent(m_Player.NickName, deathCause, afterKO);
        //m_PhotonView.RPC("RPC_AddRespawnToUI", RpcTarget.All, name, deathCause, afterKO);
        //m_InGameUI.AddRespawnToUI(m_Player.NickName, deathCause, afterKO);

        m_PhotonView.RPC("RPC_InstantiateRespawnEffect", RpcTarget.All, afterKO);

        FreezeRigidbody(true);
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_PhotonView.RPC("RPC_SetActiveObjects", RpcTarget.All, false);

        float totalTime = afterKO ? m_MaxRespawnDelayAfterKO : m_MaxRespawnDelay;

        while (totalTime >= 0)
        {
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

        if(!m_Rigidbody)
        {
            Debug.LogWarning("There is no Rigidbody");
            return;
        }

        if(doFreeze)
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.freezeRotation = true;
            gameObject.isStatic = true;
            m_Camera.isStatic = true;
        }
        else
        {
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            m_Rigidbody.useGravity = true;
            m_Rigidbody.freezeRotation = false;
            gameObject.isStatic = false;
            m_Camera.isStatic = false;
        }
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
        RaiseAddEliminateToUIEvent(m_Player.NickName);
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

    private void RaiseAddEliminateToUIEvent(string name)
    {
        object[] content = new object[] { name };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.AddPlayerGotEliminatedToUIEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    private void RaiseAddRespawnToUIEvent(string name, string deathCause, bool afterKO)
    {
        object[] content = new object[] { name, deathCause, afterKO };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.AddPlayerRespawnedToUIEventCode, content, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    [PunRPC]
    private void RPC_SetActiveObjects(bool value)
    {
        for (int i = 0; i < m_ObjectsToHideDuringRespawn.Count; i++)
        {
            m_ObjectsToHideDuringRespawn[i].SetActive(value);
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
