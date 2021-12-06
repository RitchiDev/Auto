using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode.Elimination
{
    public class EliminationPlayerManager : PlayerManager
    {
        private GameObject m_PlayerGameObject;
        [SerializeField] private GameObject m_PlayerDiedItemPrefab;
        [SerializeField] private Transform m_DeadPlayerListContainer;
        private PhotonView m_PhotonView;

        private void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (m_PhotonView.IsMine)
            {
                CreatePlayerController();
            }
        }


        public override void OnLeftRoom()
        {
            Debug.Log("Left Room");

            PhotonNetwork.LoadLevel(0); // Titlescreen

            base.OnLeftRoom();
        }

        public override void CreatePlayerController()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);
        }

        public override void CreatePlayerSpectator()
        {
            AddDeathToUI(PhotonNetwork.LocalPlayer.NickName);

            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            //string path2 = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);
            //m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), Vector3.zero, Quaternion.identity, group, data);
        }

        public override void ReturnToTitlescreen()
        {
            StartCoroutine(LeaveAndLoad());
        }

        private IEnumerator LeaveAndLoad()
        {
            PhotonNetwork.Destroy(m_PlayerGameObject);

            PhotonNetwork.LeaveRoom();

            while (PhotonNetwork.InRoom)
            {
                Debug.Log("In Room");

                yield return null;
            }
            while (!PhotonNetwork.IsConnected)
            {
                Debug.Log("Still Connected");

                yield return null;
            }

            Destroy(RoomManager.Instance.gameObject);

            Debug.Log("Leaving");
        }

        public override void AddDeathToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddDeathToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddDeathToUI(string name)
        {
            Instantiate(m_PlayerDiedItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name);
        }

        public override void RespawnPlayer()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.Destroy(m_PlayerGameObject);

                CreatePlayerController();
            }
        }

        public override void RespawnPlayerAsSpectator()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.Destroy(m_PlayerGameObject);

                CreatePlayerSpectator();
            }
        }
    }
}
