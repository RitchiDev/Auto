using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class ItemBox : MonoBehaviour, IOnEventCallback
{
    [Header("Components")]
    [SerializeField] private MeshRenderer m_MeshRenderer;
    private Collider m_Collider;
    private IEnumerator m_ReActivate;

    [Header("Misc")]
    [SerializeField] private float m_TimeBeforeReActivation = 20f;

    [Header("Effects")]
    [SerializeField] private PoolAbleObject m_PickUpEffect;
    //[SerializeField] private GameObject m_PickUpEffect;

    //private PhotonView m_PhotonView;
    private bool m_GameHasBeenWon;

    private void Awake()
    {
        //m_PhotonView = GetComponentInParent<PhotonView>();
        m_Collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.GetComponent<ItemController>();

        if(itemController)
        {
            itemController.StartItemRoulette();

            GameObject pickUpEffect = PoolManager.Instance.GetObjectFromPool(m_PickUpEffect);
            pickUpEffect.transform.position = transform.position;
            pickUpEffect.transform.rotation = transform.rotation;

            Deactivate();
        }
    }

    public void Activate()
    {
        if (m_ReActivate != null)
        {
            StopCoroutine(m_ReActivate);
        }

        m_MeshRenderer.enabled = true;
        m_Collider.enabled = true;
    }

    public void Deactivate(bool reActivate = true)
    {
        if (m_ReActivate != null)
        {
            StopCoroutine(m_ReActivate);
        }

        m_MeshRenderer.enabled = false;
        m_Collider.enabled = false;

        if (reActivate)
        {
            m_ReActivate = ReActivate();
            StartCoroutine(m_ReActivate);
        }
    }

    private IEnumerator ReActivate()
    {
        float totalTime = m_TimeBeforeReActivation;
        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            yield return null;
        }

        if (!m_GameHasBeenWon)
        {
            Activate();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.CheckIfGameHasBeenWonEventCode:

                object[] data = (object[])photonEvent.CustomData;

                m_GameHasBeenWon = (bool)data[0];

                break;

            default:
                break;
        }
    }
}
