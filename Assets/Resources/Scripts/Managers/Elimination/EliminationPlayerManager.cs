using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using UnityEngine;
using Andrich.UtilityScripts;

public class EliminationPlayerManager : PlayerManager
{
    [Header("Components")]
    private GameObject m_PlayerGameObject;
    private PhotonView m_PhotonView;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        //Restart();
    }

    private void Start()
    {
        Restart();
    }

    public override void Restart()
    {
        if (m_PhotonView.IsMine)
        {
            if (m_PlayerGameObject)
            {
                PhotonNetwork.Destroy(m_PlayerGameObject);
            }

            Player player = PhotonNetwork.LocalPlayer;
            player.SetScore(0);
            player.SetDeaths(0);
            player.SetKOs(0);
            player.SetReadyState(false);
            //player.SetVotedRematchState(false);
            player.SetIfEliminated(false);
            player.SetLoadedAndReadyState(true);
            CreatePlayerController();
        }
    }

    public override void CreatePlayerController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(PhotonNetwork.LocalPlayer.ActorNumber - 1);

        object[] data = new object[] { m_PhotonView.ViewID };
        byte group = 0;
        //string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
        //string path2 = RoomManager.Instance.GameModeSettings.PlayerControllerString;
        //m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

        string prefabName = RoomManager.Instance.GameModeSettings.PlayerControllerString;
        m_PlayerGameObject = PhotonPool.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation, group, data);

        DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
    }

    public override void CreatePlayerSpectator()
    {
        Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();

        object[] data = new object[] { m_PhotonView.ViewID };
        byte group = 0;
        //string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
        //string path2 = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
        //m_PlayerGameObject = PhotonNetwork.Instantiate(Path.Combine(path1, path2), spawnPoint.position, spawnPoint.rotation, group, data);

        string prefabName = RoomManager.Instance.GameModeSettings.PlayerSpectatorString;
        m_PlayerGameObject = PhotonPool.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation, group, data);

        DisconnectPlayerManager.Instance.SetPlayerGameObject(m_PlayerGameObject);
    }

    public override void RespawnPlayer()
    {
        if (m_PhotonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.AddDeath(1);
            PhotonNetwork.LocalPlayer.AddScore(-250);

            //AddRespawnToUI(PhotonNetwork.LocalPlayer.NickName, deathCause, afterKO);

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

    public override void RespawnPlayerAsSpectator()
    {
        if (m_PhotonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.AddDeath(1);

            //AddEliminateToUI(PhotonNetwork.LocalPlayer.NickName);

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
