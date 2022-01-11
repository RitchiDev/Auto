using UnityEngine;

#region AddtoController
//public void AddSpeed(float amount)
//{
//    rb.AddForce(amount * transform.forward, ForceMode.VelocityChange);

//}

//public void AddJumpBoost(float amount)
//{
//    rb.AddForce(amount * transform.up, ForceMode.VelocityChange);

//}
#endregion

#region New
//Deze klas niks mee doen
public abstract class MainBoostClass : MonoBehaviour
{
    public PlayerBoost m_PlayerBooster;
    public abstract void UseBoost(GameObject EliminationCar);
    public void AddCurrentBoostHolder(PlayerBoost Booster)
    {
        m_PlayerBooster = Booster;
    }
}
//Manager op iets zetten
public class BoostManager : MonoBehaviour
{
    public static BoostManager m_singleton;

    [Header("Rocket")]
    public GameObject m_RocketPrefab;
    public float m_RocketSpeed;

    [Header("SpeedBoost")]
    public float m_SpeedBoostSpeed;
    public void Awake()
    {
        m_singleton = this;
    }
}
//zet op elke speler
public class PlayerBoost :MonoBehaviour
{
    public MainBoostClass m_CurrentBoost;

    public bool AddBoost(MainBoostClass Boost)
    {
        if(m_CurrentBoost != null)
        {
            return false;
        }
        else
        {
            m_CurrentBoost = Boost;
            m_CurrentBoost.AddCurrentBoostHolder(this);
            return true;
        }
    }
    public void RemoveBoost()
    {
        m_CurrentBoost = null;
    }
}
#endregion
