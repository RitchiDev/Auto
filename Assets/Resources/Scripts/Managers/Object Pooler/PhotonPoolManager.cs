using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;

public class PhotonPoolManager : MonoBehaviour
{
    public static PhotonPoolManager Instance { get; private set; }
    [SerializeField] private List<GameObject> m_Prefabs = new List<GameObject>();

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + Instance.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if(m_Prefabs.Count < 0)
        {
            Debug.Log("There are no prefabs");
            return;
        }

        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if(pool != null)
        {
            for (int i = 0; i < m_Prefabs.Count; i++)
            {
                GameObject prefab = m_Prefabs[i];

                string path1 = RoomManager.Instance.GameModeSettings.PhotonPrefabsFolder;
                string path2 = prefab.name;
                string pathKey = Path.Combine(path1, path2);

                pool.ResourceCache.Add(pathKey, prefab);
            }
        }
    }

    public GameObject NetworkInstantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
        string path2 = prefabName;
        string pathKey = Path.Combine(path1, path2);

        return PhotonNetwork.Instantiate(pathKey, position, rotation, group, data);
    }

    public GameObject NetworkRoomInstantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
        string path2 = prefabName;
        string pathKey = Path.Combine(path1, path2);

        return PhotonNetwork.InstantiateRoomObject(pathKey, position, rotation, group, data);
    }
}
