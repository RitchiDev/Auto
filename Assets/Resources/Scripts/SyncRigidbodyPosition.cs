using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SyncRigidbodyPosition : MonoBehaviour, IPunObservable
{
    private Vector3 m_NetworkPosition;
    private Quaternion m_NetworkRotation;
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_Rigidbody.position = Vector3.MoveTowards(m_Rigidbody.position, m_NetworkPosition, Time.fixedDeltaTime);
        m_Rigidbody.rotation = Quaternion.RotateTowards(m_Rigidbody.rotation, m_NetworkRotation, Time.fixedDeltaTime * 100.0f);
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
            m_NetworkPosition = (Vector3)stream.ReceiveNext();
            m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            m_Rigidbody.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTimestamp));
            m_NetworkPosition += (this.m_Rigidbody.velocity * lag);
        }
    }
}
