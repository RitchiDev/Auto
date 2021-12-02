using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController =  other.GetComponentInParent<EliminationPlayerController>();
        if(playerController)
        {
            ScoreManager.Instance.AddScore(playerController.Player, 50);
        }
    }
}
