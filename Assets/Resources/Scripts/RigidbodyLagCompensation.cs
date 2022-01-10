using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RigidbodyLagCompensation : MonoBehaviour, IPunObservable
{
    private PhotonView m_Photonview;
    private Vector3 m_NetworkPosition;
    private Quaternion m_NetworkRotation;
    private Rigidbody m_Rigidbody;
    private float m_Lag;

    private void Awake()
    {
        m_Photonview = GetComponent<PhotonView>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.m_Rigidbody.position);
            stream.SendNext(this.m_Rigidbody.rotation);
            stream.SendNext(this.m_Rigidbody.velocity);
        }
        else
        {
            m_Rigidbody.position = (Vector3)stream.ReceiveNext();
            m_Rigidbody.rotation = (Quaternion)stream.ReceiveNext();
            m_Rigidbody.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            lag *= 1.05f;
            m_Rigidbody.position += m_Rigidbody.velocity * lag;
        }
    }

}
