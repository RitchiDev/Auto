using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected Item.Type m_ItemType;
    public Item.Type ItemType => m_ItemType;

    protected Player m_Owner;
    public Player Owner => m_Owner;

    public void SetOwner(Player owner)
    {
        m_Owner = owner;
    }
}
