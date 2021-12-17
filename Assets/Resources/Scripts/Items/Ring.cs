using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;

public class Ring : MonoBehaviour
{
    private PhotonView m_PhotonView;
    [SerializeField] private float m_TimeBeforeReActivation = 20f;
    [SerializeField] private Collider m_Collider;
    private MeshRenderer m_MeshRenderer;

    private void Awake()
    {
        m_PhotonView = GetComponentInParent<PhotonView>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        if(!m_PhotonView)
        {
            m_MeshRenderer.enabled = false;
            m_Collider.SetActive(false);
        }
    }

    public void Activate()
    {
        m_PhotonView.RPC("RPC_Activate", RpcTarget.All);
    }

    public void Deactivate(bool reActivate = true)
    {
        m_PhotonView.RPC("RPC_Deactivate", RpcTarget.All, reActivate);
    }

    [PunRPC]
    private void RPC_Activate()
    {
        m_MeshRenderer.enabled = true;
        m_Collider.SetActive(true);
    }

    [PunRPC]
    public void RPC_Deactivate(bool reActivate)
    {
        m_MeshRenderer.enabled = false;
        m_Collider.SetActive(false);

        if (reActivate)
        {
            StartCoroutine(ReActivate());
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

        if(!PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
        {
            Activate();
        }
    }
}
