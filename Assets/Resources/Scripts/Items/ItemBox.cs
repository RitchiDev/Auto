using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private float m_TimeBeforeReActivation = 20f;
    [SerializeField] private MeshRenderer m_MeshRenderer;

    //private PhotonView m_PhotonView;
    private Collider m_Collider;
    private IEnumerator m_ReActivate;

    private void Awake()
    {
        //m_PhotonView = GetComponentInParent<PhotonView>();
        m_Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.GetComponent<ItemController>();

        if(itemController)
        {
            itemController.StartItemRoulette();
            Deactivate();
        }
    }

    public void Activate()
    {
        m_MeshRenderer.enabled = true;
        m_Collider.enabled = true;
    }

    public void Deactivate(bool reActivate = true)
    {
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

        if (!PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
        {
            Activate();
        }
    }
}
