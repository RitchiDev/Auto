using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using BoostList = System.Collections.Generic.List<IBoostClass>;
public class ItemBoostManager : MonoBehaviour
{
    [Header("Attributes")]
    public static ItemBoostManager m_singleton;
    private Car_Controller m_Controller;
    
    [Header("BoostsAttributes")]
    [Tooltip("Select based on number. " +
        "\n 0 = Speed " +
        "\n 1 = Jump " +
        "\n 2 = Fire Projectiles?")]
    public List<BoostList> Boosts = new List<BoostList>();
    private int m_SelectedBoost;

    private void Awake()
    {
        if (m_singleton == null)
        {
            m_singleton = this;
        }
        //Get Current Controller
        //m_Controller = ?? 

    }
    public void OnBoostPressed()
    {
        for (int i = 0; i < Boosts[m_SelectedBoost].Count; i++)
        {
            Boosts[m_SelectedBoost][i].OnBoostButtonPressed();
        }
    }

}
public class Boost : MonoBehaviour
{
    [Tooltip("Creates an Image of this item for UI Purposes")]
    public GameObject PrefabItem;
    [Tooltip("Select based on number. " +
       "\n 0 = Speed " +
       "\n 1 = Jump " +
       "\n 2 = Fire Projectiles?")]
    [Range(0,2)]
    public int m_BoostType;

    public int m_AmountBoost;
    private void OnTriggerEnter(Collider other)
    {
        switch (m_BoostType)
        {
            case 0:
                ItemBoostManager.m_singleton.Boosts[m_BoostType].Add(new SpeedBoost(m_AmountBoost));
                break;
        }

        gameObject.SetActive(false);
        //Return To pool here <-- 
    }
}
public interface IBoostClass
{   
    void OnBoostButtonPressed();
    void Context(BoostContext NewContext);
}
public abstract class BoostHeadClass : IBoostClass
{
   
    public BoostContext m_BoostContext;
    public void Context(BoostContext NewContext)
    {
        m_BoostContext = NewContext;
    }
    public abstract void OnBoostButtonPressed();
}
public class BoostContext
{
    public BoostContext(Car_Controller Controller, Vector3 CurrentPosition)
    {
        m_Controller = Controller;
        m_CurrentPlayerPosition = CurrentPosition;
    }
    public Car_Controller m_Controller;
    public Vector3 m_CurrentPlayerPosition;
   
}
public class SpeedBoost : BoostHeadClass
{
    private float m_Speed;
    private Texture m_PrefabTexture;
    public SpeedBoost(float BoostAmount, GameObject Prefab)
    {
        m_Speed = BoostAmount;
        m_PrefabTexture = AssetPreview.GetAssetPreview(Prefab);
    }
    public override void OnBoostButtonPressed()
    {
        //m_BoostContext.m_Controller.AddSpeed(m_Speed);
        
    }
    public Texture ReturnIcon()
    {
        return m_PrefabTexture;
    }
}

#region AddtoController
//public void AddSpeed(float amount)
//{
//    rb.AddForce(amount * transform.forward, ForceMode.VelocityChange);

//}
#endregion