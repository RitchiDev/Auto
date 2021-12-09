using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;

public class Ring : MonoBehaviour
{
    [SerializeField] private float m_TimeBeforeReActivation = 20f;
    [SerializeField] private RingCollision m_RingCollision;
    private MeshRenderer m_MeshRenderer;

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    public void Activate()
    {
        m_MeshRenderer.enabled = true;
        m_RingCollision.enabled = true;
    }

    public void Deactivate(bool reActivate = true)
    {
        m_MeshRenderer.enabled = false;
        m_RingCollision.enabled = false;

        if(reActivate)
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
            m_MeshRenderer.enabled = true;
            m_RingCollision.enabled = true;
        }
    }
}
