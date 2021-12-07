using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class RingCollision : MonoBehaviour
{
    [SerializeField] private Ring m_Ring;
    [SerializeField] private int m_ScoreToAdd = 50;
    public int Worth => m_ScoreToAdd;
    public Ring Ring => m_Ring;

    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController =  other.GetComponentInParent<EliminationPlayerController>();
        if(playerController)
        {
            playerController.Player.AddScore(m_ScoreToAdd);
            bool reactivate = m_ScoreToAdd <= 499;
            m_Ring.Deactivate(reactivate);
        }
    }
}
