using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_IgnoreOwnerDelay = 0.2f;
    [SerializeField] protected PhotonView m_PhotonView;
    [SerializeField] protected Item.Type m_ItemType;
    public Item.Type ItemType => m_ItemType;

    protected Player m_Owner;
    public Player Owner => m_Owner;

    protected bool m_IgnoreOwner;

    private void OnEnable()
    {
        if (!m_PhotonView.IsMine)
        {
            //m_PhotonView.TransferOwnership(PhotonNetwork.MasterClient);
            m_Owner = (Player)m_PhotonView.InstantiationData[0];
        }
    }

    public void SetOwner(Player owner)
    {
        m_Owner = owner;

        if (m_PhotonView.IsMine)
        {
            m_PhotonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

    protected IEnumerator IgnoreOwnerDelay()
    {
        float totalTime = m_IgnoreOwnerDelay;
        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            yield return null;
        }
    }
}
