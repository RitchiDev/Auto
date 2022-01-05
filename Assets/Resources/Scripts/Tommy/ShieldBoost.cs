using UnityEngine;

namespace Tommy
{ 
    #region New
public class ShieldBoost : MainBoostClass
{
    public override void UseBoost(GameObject EliminationCar)
    {
        //EliminationCar.GetComponent<CarController>().TurnonShield();
        m_PlayerBooster.RemoveBoost();
    }
}
#endregion
}

