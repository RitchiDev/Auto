using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    public GameModeSettings GameModeSettings { get; private set; }

    private PhotonView m_PhotonView;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            m_PhotonView = GetComponent<PhotonView>();
        }
    }

    public void SetUp(GameModeSettings settings)
    {
        GameModeSettings = settings;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            return;
        }

        //Debug.Log(GameModeSettings.PhotonPrefabsFolder);
        //Debug.Log(Path.Combine(path1, path2));

        string path1 = GameModeSettings.PhotonPrefabsFolder;
        string path2 = GameModeSettings.PlayerManagerString;
        PhotonNetwork.Instantiate(Path.Combine(path1, path2), Vector3.zero, Quaternion.identity);
    }
}
