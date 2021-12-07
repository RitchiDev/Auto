using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
public class RingCollision : MonoBehaviour
{
    [SerializeField] private Ring m_Ring;
    [SerializeField] private int m_ScoreToAdd = 50;
    public int Worth => m_ScoreToAdd;

    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController =  other.GetComponentInParent<EliminationPlayerController>();
        if(playerController)
        {
            ScoreManager.Instance.AddScore(playerController.Player, m_ScoreToAdd);

            if(Worth >= 500)
            {
                m_Ring.Deactivate(false);
            }
        }
    }
}
