using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeItemBox : Projectile
{
    [Header("Effects")]
    [SerializeField] private GameObject m_HitEffectPrefab;

    private void Deactivate()
    {
        Instantiate(m_HitEffectPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.GetComponent<ItemController>();
        if (InteractingWithItemController(itemController))
        {
            return;
        }

        DemolitionAura demolitionAura = other.GetComponent<DemolitionAura>();
        if (InteractingWithDemolitionAura(demolitionAura))
        {
            return;
        }

        //Debug.Log(other.name);
        Deactivate();
    }

    private bool InteractingWithItemController(ItemController itemController)
    {
        //Debug.Log("hoi");
        if (itemController)
        {
            itemController.HitByProjectile(this);
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
