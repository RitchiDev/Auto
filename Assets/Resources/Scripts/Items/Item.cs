using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public abstract class Item : MonoBehaviour
{
    public enum Type
    {
        notSet = 0,
        rocket = 1,
        shield = 2,
        demolitionAura = 3,

    }

    [SerializeField] protected Type m_ItemType;
    public Type ItemType => m_ItemType;

    protected Player m_Owner;
    public Player Owner => m_Owner;

    protected ItemController m_OwnerItemController;
    public ItemController OwnerItemController => m_OwnerItemController;

    public abstract void Use();

    public virtual void SetOwner(Player owner)
    {
        m_Owner = owner;
    }

    public virtual void SetOwner(Player owner, ItemController itemController)
    {
        m_Owner = owner;
        m_OwnerItemController = itemController;
    }
}
