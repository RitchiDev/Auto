using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public abstract class Item : MonoBehaviour
{
    public enum Type
    {
        notSet = 0,
        rocket = 1,
        shield = 2,
        demolitionAura = 3,
        fakeItemBox = 4,
    }

    [SerializeField] private ItemData m_ItemData;
    public ItemData ItemData => m_ItemData;

    protected Player m_Owner;
    public Player Owner => m_Owner;

    protected ItemController m_OwnerItemController;
    public ItemController OwnerItemController => m_OwnerItemController;

    protected PhotonView m_ChildPhotonView;

    public abstract void Use();

    public virtual void SetOwner(Player owner)
    {
        m_Owner = owner;
    }

    public virtual void SetOwner(Player owner, ItemController itemController)
    {
        m_Owner = owner;
        m_OwnerItemController = itemController;
        m_ChildPhotonView = itemController.ControllerPhotonView;
    }
}
