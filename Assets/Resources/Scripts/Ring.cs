using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Car_Controller carController =  other.GetComponentInParent<Car_Controller>();
        if(carController)
        {
            ScoreboardManager scoreboard = carController.GetComponentInChildren<ScoreboardManager>();
            if(scoreboard)
            {
                scoreboard.AddScore(carController.Player, 50);
            }
            //Debug.Log(ScoreboardManager.Instance);
        }
    }
}
