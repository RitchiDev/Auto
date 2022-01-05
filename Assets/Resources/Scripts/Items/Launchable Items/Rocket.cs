using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : LaunchableItem
{
    public override void Use()
    {
        throw new System.NotImplementedException();
    }


    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.GetComponent<ItemController>();

        if (itemController == m_OwnerItemController)
        {
            return;
        }

        if (InteractingWithItemController(itemController))
        {
            return;
        }

        if(InteractingWithShield(other))
        {
            // Voor als er meer hieronder komt
            return;
        }
    }

    private bool InteractingWithItemController(ItemController itemController)
    {
        if (itemController)
        {
            // Do Item Things
            gameObject.SetActive(false);

            return true;
        }

        return false;
    }

    private bool InteractingWithShield(Collider other)
    {
        Shield shield = other.GetComponent<Shield>();

        if (shield)
        {
            // Do Item Things
            gameObject.SetActive(false);

            return true;
        }

        return false;
    }
}
