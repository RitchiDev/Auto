using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class RingCollision : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private GameObject m_FloatingTextPrefab;
    [SerializeField] private Ring m_Ring;
    [SerializeField] private int m_ScoreToAdd = 50;
    public int Worth => m_ScoreToAdd;
    public Ring Ring => m_Ring;

    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController =  other.GetComponentInParent<EliminationPlayerController>();
        if(playerController)
        {
            FloatyText floatyText = Instantiate(m_FloatingTextPrefab, transform.position, Quaternion.identity).GetComponentInChildren<FloatyText>();
            floatyText.SetUp(m_ScoreToAdd.ToString(), m_MeshRenderer.materials[0]);

            playerController.Player.AddScore(m_ScoreToAdd);
            bool reactivate = m_ScoreToAdd <= 499;
            m_Ring.Deactivate(reactivate);
        }
    }
}
