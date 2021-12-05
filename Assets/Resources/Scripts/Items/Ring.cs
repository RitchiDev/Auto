using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
public class Ring : MonoBehaviour
{
    [SerializeField] private int m_ScoreToAdd = 50;

    private void OnTriggerEnter(Collider other)
    {
        EliminationPlayerController playerController =  other.GetComponentInParent<EliminationPlayerController>();
        if(playerController)
        {
            ScoreManager.Instance.AddScore(playerController.Player, m_ScoreToAdd);

            InGameUI inGameUI = playerController.GetComponentInChildren<InGameUI>();
            if(inGameUI)
            {
                inGameUI.SetOnscreenScore(playerController.Player.GetScore());
            }
        }
    }
}
