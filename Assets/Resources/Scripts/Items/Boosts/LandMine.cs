using UnityEngine;
#region New
public class LandMine : MainBoostClass
{
    private CarController m_Origin;
    ParticleSystem m_explosion;
    public override void UseBoost(GameObject EliminationCar)
    {
        m_Origin = EliminationCar.GetComponent<CarController>();
        //GameObject Free = Instantiate(BoostManager.m_singleton.m_RocketPrefab, EliminationCar.GetComponent<CarController>().LandMineSpawnPosition.transform.position, Quaternion.identity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<CarController>())
        {
            if (collision.collider.GetComponent<CarController>() != m_Origin)
            {
                //if (!collision.collider.GetComponent<CarController>().Shield)
                //{
                //other.GetComponent<CarController>().Death();
                //m_explosion.Play();
                m_PlayerBooster.RemoveBoost();
                Destroy(this.gameObject);
                //}
                //else
                //{
                // collision.collider.GetComponent<CarController>().Shield = false;
                //}
            }
        }
    }
}
#endregion
