using Photon.Pun;
using System.IO;
using UnityEngine;

namespace GameMode.Elemination
{
    public class EleminationPlayerManager : PlayerManager
    {
        private GameObject m_Player;
        [SerializeField] private GameObject m_PlayerDiedListItemPrefab;
        [SerializeField] private Transform m_DeadPlayerListContent;
        private float m_DeactivateDeadPlayerPopupTimer;

        private void Start()
        {
            if (PhotonView.IsMine)
            {
                CreatePlayer();
            }
        }

        public override void CreatePlayer()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            m_Player = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);
        }

        public override void RespawnPlayer()
        {
            PhotonNetwork.Destroy(m_Player);
            CreatePlayer();
        }

        public override void ReturnToTitleScreen()
        {
            PhotonNetwork.Destroy(m_Player);
            base.ReturnToTitleScreen();
        }

        public override void AddDeathToUI(string name)
        {
            PhotonView.RPC("RPC_AddDeathToUI", RpcTarget.All, name);
        }

        [PunRPC]
        private void RPC_AddDeathToUI(string name)
        {
            Debug.Log("Added player death UI");
            m_DeactivateDeadPlayerPopupTimer = Mathf.Clamp(m_DeactivateDeadPlayerPopupTimer + 5f, 0.0f, 6f);
            //Object.Instantiate<GameObject>(this.m_PlayerDiedListItemPrefab, this.m_DeadPlayerListContent).GetComponent<PlayerListItem>().SetUp(name);
        }
    }
}
