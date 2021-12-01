using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun.UtilityScripts;

namespace GameMode.Elimination
{
    public class EliminationPlayerManager : PlayerManager
    {
        private GameObject m_PlayerController;
        public GameObject PlayerController => m_PlayerController;
        [SerializeField] private GameObject m_PlayerDiedListItemPrefab;
        [SerializeField] private Transform m_DeadPlayerListContent;
        private float m_DeactivateDeadPlayerPopupTimer;

        private List<GameObject> m_AlivePlayers = new List<GameObject>();

        private float m_MaxEliminationTime = 30f;
        private float m_TimeUntillElimination;

        private void Start()
        {
            m_TimeUntillElimination = m_MaxEliminationTime;

            if (PhotonView.IsMine)
            {
                CreatePlayer();
            }
        }

        private void Update()
        {
            if(m_AlivePlayers.Count > 0)
            {
                EliminationTimer();
            }
        }

        private void EliminationTimer()
        {
            if(m_TimeUntillElimination > 0)
            {
                m_TimeUntillElimination -= Time.deltaTime;
            }

            else if(m_TimeUntillElimination < 0)
            {
                m_TimeUntillElimination = m_MaxEliminationTime;
                EliminationPlayerController playerWithLowestScore = m_AlivePlayers[0].GetComponent<EliminationPlayerController>();
                int playerWithLowestScoreIndex = 0;

                for (int i = 0; i < m_AlivePlayers.Count; i++)
                {
                    EliminationPlayerController currentCheckedPlayer = m_AlivePlayers[i].GetComponent<EliminationPlayerController>();

                    if(currentCheckedPlayer.Player.GetScore() < playerWithLowestScore.Player.GetScore())
                    {
                        playerWithLowestScore = currentCheckedPlayer;
                        playerWithLowestScoreIndex = i;
                    }

                    if(i >= m_AlivePlayers.Count)
                    {
                        m_AlivePlayers.RemoveAt(playerWithLowestScoreIndex);
                        playerWithLowestScore.Eliminate();
                    }
                }
            }
        }

        public override void CreatePlayer()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

            object[] data = new object[] { PhotonView.ViewID };
            byte group = 0;
            string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
            string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
            m_PlayerController = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

            if(InGameUIManager.Instance)
            {
                //Debug.Log(m_PlayerController);
                InGameUIManager.Instance.SetCurrentPlayerController(m_PlayerController);
            }

            PhotonView.RPC("RPC_AddAlivePlayer", RpcTarget.AllBuffered, m_PlayerController);
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Destroy(m_PlayerController);
        }

        public override void RespawnPlayer()
        {
            PhotonNetwork.Destroy(m_PlayerController);
            CreatePlayer();
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

        [PunRPC]
        private void RPC_AddAlivePlayer(GameObject playerController)
        {
            m_AlivePlayers.Add(playerController);
        }
    }
}
