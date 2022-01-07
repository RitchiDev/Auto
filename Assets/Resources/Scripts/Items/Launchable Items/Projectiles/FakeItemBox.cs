using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeItemBox : MonoBehaviour
{
    private Player m_Owner;
    public Player Owner => m_Owner;

    [Header("Effects")]
    [SerializeField] private GameObject m_HitEffectPrefab;

    public void SetOwner(Player owner)
    {
        m_Owner = owner;
    }

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
