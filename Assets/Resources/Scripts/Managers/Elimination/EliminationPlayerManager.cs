using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using UnityEngine;
using Andrich.UtilityScripts;

namespace GameMode.Elimination
{
    public class EliminationPlayerManager : PlayerManager
    {
        private GameObject m_PlayerGameObject;
        [SerializeField] private GameObject m_PlayerDiedItemPrefab;
        [SerializeField] private Transform m_DeadPlayerListContainer;
        [SerializeField] private GameObject m_RespawnedPlayerItemPrefab;
        private PhotonView m_PhotonView;

        private void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.LocalPlayer.SetScore(0);
                PhotonNetwork.LocalPlayer.SetDeaths(0);
                CreatePlayerController();
            }
        }

        public override void CreatePlayerController()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

            DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
        }

        public override void CreatePlayerSpectator()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

            DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
        }

        public override void AddDeathToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddDeathToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddDeathToUI(string name)
        {
            Instantiate(m_RespawnedPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name);
        }

        public override void RespawnPlayer()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.LocalPlayer.AddDeath(1);

                AddDeathToUI(PhotonNetwork.LocalPlayer.NickName);

                PhotonNetwork.Destroy(m_PlayerGameObject);

                CreatePlayerController();
            }
        }

        public override void RespawnPlayerAsSpectator()
        {
            if (m_PhotonView.IsMine)
            {
                Debug.Log("View Is Mine");
                PhotonNetwork.LocalPlayer.AddDeath(1);

                AddDeathToUI(PhotonNetwork.LocalPlayer.NickName);

                PhotonNetwork.Destroy(m_PlayerGameObject);

                CreatePlayerSpectator();
            }

            Debug.Log("View Is NOT Mine!");

        }

        public void ReturnToTitlescreen()
        {
            StartCoroutine(LeaveAndLoad());
        }

        private IEnumerator LeaveAndLoad()
        {
            if (m_PlayerGameObject)
            {
                PhotonNetwork.Destroy(m_PlayerGameObject);
            }

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

            PhotonNetwork.Disconnect();

            Debug.Log("Leaving");
        }
    }
}
