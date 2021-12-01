using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveGameManager : MonoBehaviour
{
    public static LeaveGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void DestroyPlayerAndLeave(GameObject playerController)
    {
        PhotonNetwork.Destroy(playerController);
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
