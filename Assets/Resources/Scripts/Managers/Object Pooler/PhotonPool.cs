using Photon.Pun;
using System.IO;
using UnityEngine;

public static class PhotonPool
{
    /// <summary>
    /// Photon Prefabs folder name (set in PathProperties) is included when instantiating
    /// </summary>
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
        string path2 = prefabName;
        string pathKey = Path.Combine(path1, path2);

        return PhotonNetwork.Instantiate(pathKey, position, rotation, group, data);
    }
}
