using Photon.Pun;
using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class PlayerManager : MonoBehaviourPunCallbacks
{
    private PhotonView m_PhotonView;
    public PhotonView PhotonView => m_PhotonView;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (m_PhotonView.IsMine)
        {
            CreatePlayer();
        }
    }

    public abstract void CreatePlayer();

    public abstract void RespawnPlayer();

    public virtual void AddDeathToUI(string name)
    {
        Debug.Log("Added death to UI");
    }

    public virtual void ReturnToTitleScreen()
    {
        StartCoroutine(LeaveAndLoad());
    }

    private IEnumerator LeaveAndLoad()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        while (!PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0); // Titlescreen
    }
}
