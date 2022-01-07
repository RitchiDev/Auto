using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;
using Photon.Realtime;

public class Ring : MonoBehaviour
{
    private Collider m_Collider;
    private MeshRenderer m_MeshRenderer;

    [SerializeField] private float m_TimeBeforeReActivation = 20f;
    [SerializeField] private AudioSource m_PickUpNoise;
    [SerializeField] private GameObject m_FloatingTextPrefab;
    [SerializeField] private int m_ScoreToAdd = 50;
    public int Worth => m_ScoreToAdd;

    private IEnumerator m_ReActivate;

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController = other.GetComponentInParent<EliminationPlayerController>();
        if (playerController)
        {
            m_PickUpNoise.Play();
            FloatyText floatyText = Instantiate(m_FloatingTextPrefab, transform.position, Quaternion.identity).GetComponentInChildren<FloatyText>();
            floatyText.SetUp(m_ScoreToAdd.ToString(), m_MeshRenderer.materials[0]);

            playerController.Owner.AddScore(m_ScoreToAdd);

            if(m_ScoreToAdd >= 500)
            {
                RaiseActivateNew500RingEvent();
                Deactivate(false);
            }
            else
            {
                Deactivate(true);
            }
        }
    }

    private void RaiseActivateNew500RingEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.DeactivateAllItemBoxesEventCode, null, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
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

        if(!PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
        {
            Activate();
        }
    }
}
