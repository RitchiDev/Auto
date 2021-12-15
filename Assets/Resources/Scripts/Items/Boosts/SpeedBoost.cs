using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MainBoostClass
{
    public override void UseBoost(GameObject EliminationCar)
    {
        EliminationCar.GetComponent<CarController>().GetComponent<Rigidbody>().AddForce(transform.forward * BoostManager.m_singleton.m_SpeedBoostSpeed);       
        m_PlayerBooster.RemoveBoost();
    }
}
