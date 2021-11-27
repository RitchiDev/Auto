using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using BoostList = System.Collections.Generic.List<IBoostClass>;
public class ItemBoost : MonoBehaviour
{
    [Tooltip("Creates an Image of this item for UI Purposes")]
    public GameObject PrefabItem;

    [Header("Attributes")]
    private Car_Controller m_Controller;
    public BoostList CurrentBoosts = new BoostList();

    private void Awake()
    {
        //Get Current Controller
        //m_Controller = ?? 
    }
    public void OnBoostPressed()
    {
        foreach (var item in CurrentBoosts)
        {
            item.Context(new BoostContext(m_Controller, transform.position));
            item.OnBoostButtonPressed();
        }
    }

}
public interface IBoostClass
{
    void Start(GameObject Prefab);
    void OnBoostButtonPressed();
    void Context(BoostContext NewContext);
}
public abstract class BoostHeadClass : IBoostClass
{
    private Texture m_PrefabTexture;
    public BoostContext m_BoostContext;
    public virtual void Start(GameObject Prefab)
    {
        m_PrefabTexture = AssetPreview.GetAssetPreview(Prefab);
    }
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
    public SpeedBoost(float BoostAmount)
    {
        m_Speed = BoostAmount;
    }
    public override void OnBoostButtonPressed()
    {
        //m_BoostContext.m_Controller.AddSpeed(m_Speed);
        
    }
}

#region AddtoController
//public void AddSpeed(float amount)
//{
//    rb.AddForce(amount * transform.forward, ForceMode.VelocityChange);

//}
#endregion