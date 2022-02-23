using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Andrich.UtilityScripts;
using Photon.Pun;

public class RocketProjectile : Projectile
{
    [Header("Prefabs")]
    [SerializeField] private PoolAbleObject m_SmallExplosionArea;

    [Header("Effects")]
    [SerializeField] private PoolAbleObject m_HitEffect;

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 launchDirection, float LaunchPower)
    {
        m_Rigidbody.AddForce(transform.forward * LaunchPower, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonView otherPhotonView = other.GetComponent<PhotonView>();
        if (otherPhotonView)
        {
            if (otherPhotonView.IsMine)
            {
                return;
            }
        }

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
        m_PhotonView.RPC("RPC_Deactivate", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Deactivate()
    {
        GameObject hitEffect = PoolManager.Instance.GetObjectFromPool(m_HitEffect);
        hitEffect.transform.position = transform.position;
        hitEffect.transform.rotation = transform.rotation;

        ExplosionArea explosionArea = PoolManager.Instance.GetObjectFromPool(m_SmallExplosionArea).GetComponent<ExplosionArea>();
        explosionArea.transform.position = transform.position;
        explosionArea.transform.rotation = transform.rotation;
        explosionArea.SetOwner(m_Owner);

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

    private bool InteractingWithItemController(ItemController itemController)
    {
        //Debug.Log("hoi");

        if (itemController)
        {
            if(m_Owner == itemController.Owner)
            {
                return true;
            }

            //Debug.Log(itemController.Owner.NickName);
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
