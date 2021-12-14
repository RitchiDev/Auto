using UnityEngine;
#region New

public class RocketBoost : MainBoostClass
{
   
    ParticleSystem m_explosion;
    public override void UseBoost(GameObject EliminationCar)
    {
        GameObject Free = Instantiate(BoostManager.m_singleton.m_RocketPrefab, EliminationCar.transform.position, Quaternion.identity);
        Rigidbody RBFree = Free.GetComponent<Rigidbody>();
        Free.GetComponent<ExplosionBoostType>().m_Origin = EliminationCar.GetComponent<CarController>();
        RBFree.AddForce(Vector3.forward * BoostManager.m_singleton.m_RocketSpeed);
        m_PlayerBooster.RemoveBoost();
    }

}
public class ExplosionBoostType : MonoBehaviour
{
    public CarController m_Origin;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CarController>())
        {
            if (other.GetComponent<CarController>() != m_Origin)
            {
                //if (!other.GetComponent<CarController>().Shield)
                //{
                //other.GetComponent<CarController>().Death();
                //m_explosion.Play();

                Destroy(this.gameObject);
                //}
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
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
