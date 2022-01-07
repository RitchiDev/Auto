using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Andrich.UtilityScripts;

public class RocketProjectile : MonoBehaviour
{
    [Header("Misc")]
    [SerializeField] private float m_ExplosionRadius = 1;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_ExplosionAreaPrefab;

    [Header("Effects")]
    [SerializeField] private GameObject m_HitEffectPrefab;

    private Rigidbody m_Rigidbody;
    private Player m_Owner;
    public Player Owner => m_Owner;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void SetOwner(Player owner)
    {
        m_Owner = owner;
    }

    public void Launch(Vector3 launchDirection, float LaunchPower)
    {
        m_Rigidbody.AddForce(transform.forward * LaunchPower, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.GetComponent<ItemController>();
        if(InteractingWithItemController(itemController))
        {
            return;
        }

        DemolitionAura demolitionAura = other.GetComponent<DemolitionAura>();
        if(InteractingWithDemolitionAura(demolitionAura))
        {
            return;
        }

        //Debug.Log(other.name);
        Deactivate();
    }

    private void Deactivate()
    {
        Instantiate(m_HitEffectPrefab, transform.position, transform.rotation);

        ExplosionArea explosionArea = Instantiate(m_ExplosionAreaPrefab, transform.position, transform.rotation).GetComponent<ExplosionArea>();
        explosionArea.SetOwner(m_Owner);

        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    private bool InteractingWithItemController(ItemController itemController)
    {
        //Debug.Log("hoi");
        if (itemController)
        {
            if(m_Owner == itemController.Owner)
            {
                return true;
            }

            Deactivate();

            return true;
        }

        return false;
    }

    private bool InteractingWithDemolitionAura(DemolitionAura demolitionAura)
    {
        if (demolitionAura)
        {
            return true;
        }

        return false;
    }
}
