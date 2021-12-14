using UnityEngine;
#region New

public class RocketBoost : MainBoostClass
{
    private CarController m_Origin;
    ParticleSystem m_explosion;
    public override void UseBoost(GameObject EliminationCar)
    {
        m_Origin = EliminationCar.GetComponent<CarController>();
        GameObject Free = Instantiate(BoostManager.m_singleton.m_RocketPrefab, EliminationCar.transform.position, Quaternion.identity);
        Rigidbody RBFree = Free.GetComponent<Rigidbody>();

        RBFree.AddForce(Vector3.forward * BoostManager.m_singleton.m_RocketSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CarController>())
        {
            if(other.GetComponent<CarController>() != m_Origin)
            {
                //if (!other.GetComponent<CarController>().Shield)
                //{
                    //other.GetComponent<CarController>().Death();
                    //m_explosion.Play();
                    m_PlayerBooster.RemoveBoost();
                    Destroy(this.gameObject);
                //}
            }
        }
    }
}
#endregion
