using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameMode.Elimination
{
    public class EliminationPlayerManager : PlayerManager
    {
        private GameObject m_PlayerController;
        public GameObject PlayerController => m_PlayerController;
        [SerializeField] private GameObject m_PlayerDiedListItemPrefab;
        [SerializeField] private Transform m_DeadPlayerListContent;
        private float m_DeactivateDeadPlayerPopupTimer;
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
            m_PlayerController = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);
        }

        public override void CreatePlayerSpectator()
        {
            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
            m_PlayerController = PhotonNetwork.Instantiate(Path.Combine(path1, path2), Vector3.zero, Quaternion.identity, group, data);
        }

        public override void ReturnToTitlescreen(GameObject gameObjectToDestroy)
        {
            PhotonNetwork.Destroy(gameObjectToDestroy);
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
        }

        public override void AddDeathToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddDeathToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddDeathToUI(string name)
        {
            Debug.Log("Added player death UI");
            m_DeactivateDeadPlayerPopupTimer = Mathf.Clamp(m_DeactivateDeadPlayerPopupTimer + 5f, 0.0f, 6f);
            //Object.Instantiate<GameObject>(this.m_PlayerDiedListItemPrefab, this.m_DeadPlayerListContent).GetComponent<PlayerListItem>().SetUp(name);
        }

        public override void RespawnPlayer()
        {
            if(m_PhotonView.IsMine)
            {
                PhotonNetwork.Destroy(m_PlayerController);

                CreatePlayerController();
            }
        }

        public override void RespawnPlayerAsSpectator()
        {
            //if(m_PlayerController)
            //{
            //    PhotonNetwork.Destroy(m_PlayerController);
            //}

            //CreatePlayerSpectator();
        }
    }
}
