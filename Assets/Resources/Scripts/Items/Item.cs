using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public enum Type
    {
        notSet = 0,
        rocket = 1,
        shield = 2,
    }

    [SerializeField] protected Type m_ItemType;
    public Type ItemType => m_ItemType;

    protected GameObject m_Owner;
    protected ItemController m_OwnerItemController;

    public abstract void Use();

    public virtual void SetOwner(GameObject owner)
    {
        m_Owner = owner;
    }

    public virtual void SetOwner(GameObject owner, ItemController itemController)
    {
        m_Owner = owner;
        m_OwnerItemController = itemController;
    }
}
