using UnityEngine;
#region New
public class LandMine : MainBoostClass
{
    private CarController m_Origin;
    ParticleSystem m_explosion;
    public override void UseBoost(GameObject EliminationCar)
    {
        //GameObject Free = Instantiate(BoostManager.m_singleton.m_RocketPrefab, EliminationCar.GetComponent<CarController>().LandMineSpawnPosition.transform.position, Quaternion.identity);
        //Free.GetComponent<ExplosionBoostType>().m_Origin = EliminationCar.GetComponent<CarController>();
        m_PlayerBooster.RemoveBoost();
    }

   
}
#endregion
