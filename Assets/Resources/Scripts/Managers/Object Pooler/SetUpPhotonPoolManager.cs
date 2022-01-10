using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;

public class SetUpPhotonPoolManager : MonoBehaviour
{
    public static SetUpPhotonPoolManager Instance { get; private set; }
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

        AddToPool();
    }

    private void AddToPool()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null)
        {
            for (int i = 0; i < m_Prefabs.Count; i++)
            {
                GameObject prefab = m_Prefabs[i];

                string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
                string path2 = prefab.name;
                string pathKey = Path.Combine(path1, path2);

                if(pool.ResourceCache.ContainsKey(pathKey))
                {
                    return;
                }

                pool.ResourceCache.Add(pathKey, prefab);
            }
        }
    }
}
