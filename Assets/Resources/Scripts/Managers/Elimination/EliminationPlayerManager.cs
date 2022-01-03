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
        [SerializeField] private Transform m_DeadPlayerListContainer;
        [SerializeField] private GameObject m_EliminatedPlayerItemPrefab;
        [SerializeField] private GameObject m_RespawnedPlayerItemPrefab;
        [SerializeField] private GameObject m_KOdPlayerItemPrefab;

        private PhotonView m_PhotonView;

        private void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            Restart();
        }

        private void Start()
        {
            //Restart();   
        }

        public override void Restart()
        {
            //if(m_PlayerGameObject)
            //{
            //    EliminationPlayerController playerEliminationController = m_PlayerGameObject.GetComponent<EliminationPlayerController>();
            //    if (playerEliminationController)
            //    {
            //        EliminationGameManager.Instance.RemoveAlivePlayer(playerEliminationController);
            //    }
            //}

            if (m_PhotonView.IsMine)
            {
                if (m_PlayerGameObject)
                {
                    PhotonNetwork.Destroy(m_PlayerGameObject);
                }

                Player player = PhotonNetwork.LocalPlayer;
                player.SetScore(0);
                player.SetDeaths(0);
                player.SetReadyState(false);
                player.SetVotedRematchState(false);
                player.SetIfEliminated(false);
                player.SetLoadedAndReadyState(true);
                CreatePlayerController();
            }
        }

        public override void CreatePlayerController()
        {
            Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

            DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
        }

        public override void CreatePlayerSpectator()
        {
            Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();

            object[] data = new object[] { m_PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
            m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

            DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
        }

        public override void AddEliminateToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddEliminateToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddEliminateToUI(string name)
        {
            Instantiate(m_EliminatedPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name);
        }

        public override void AddRespawnToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddRespawnToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddRespawnToUI(string name)
        {
            Instantiate(m_RespawnedPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name);
        }

        public override void AddKOToUI(string name)
        {
            m_PhotonView.RPC("RPC_AddKOToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddKOToUI(string name)
        {
            Instantiate(m_KOdPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name);
        }

        public override void RespawnPlayer()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.LocalPlayer.AddDeath(1);
                PhotonNetwork.LocalPlayer.AddScore(-250);

                AddRespawnToUI(PhotonNetwork.LocalPlayer.NickName);

                Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();

                m_PlayerGameObject.transform.position = spawnPoint.position;
                m_PlayerGameObject.transform.rotation = spawnPoint.rotation;

                Rigidbody rigidbody = m_PlayerGameObject.GetComponent<Rigidbody>();
                if(rigidbody)
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
        }

        public override void RespawnPlayerAfterKO()
        {

        }

        public override void RespawnPlayerAsSpectator()
        {
            if (m_PhotonView.IsMine)
            {
                PhotonNetwork.LocalPlayer.AddDeath(1);

                AddEliminateToUI(PhotonNetwork.LocalPlayer.NickName);

                PhotonNetwork.Destroy(m_PlayerGameObject);

                CreatePlayerSpectator();
            }
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
