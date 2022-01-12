using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FakeItemBox : Projectile
{
    [Header("Effects")]
    [SerializeField] private PoolAbleObject m_HitEffect;
    //[SerializeField] private GameObject m_HitEffectPrefab;

    private void Deactivate()
    {
        m_PhotonView.RPC("RPC_Deactivate", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Deactivate()
    {
        GameObject hitEffect = PoolManager.Instance.GetObjectFromPool(m_HitEffect);
        hitEffect.transform.position = transform.position;
        hitEffect.transform.rotation = transform.rotation;

        if (m_PhotonView.IsMine)
        {
            DestroyGlobally();
        }
        else
        {
            DestroyLocally();
        }
    }

    private void DestroyGlobally()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void DestroyLocally()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(!m_PhotonView.IsMine)
        //{
        //    return;
        //}

        PhotonView otherPhotonView = other.GetComponent<PhotonView>();
        if (otherPhotonView)
        {
            if (!otherPhotonView.IsMine)
            {
                return;
            }
        }

        ItemController itemController = other.GetComponent<ItemController>();
        if (InteractingWithItemController(itemController))
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
}
