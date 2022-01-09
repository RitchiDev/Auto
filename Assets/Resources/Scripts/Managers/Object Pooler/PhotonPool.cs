using Photon.Pun;
using System.IO;
using UnityEngine;

public static class PhotonPool
{
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
        string path2 = prefabName;
        string pathKey = Path.Combine(path1, path2);

        return PhotonNetwork.Instantiate(pathKey, position, rotation, group, data);
    }

    public static GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        string path1 = PathProperties.PhotonPrefabsFolderNameProperty;
        string path2 = prefabName;
        string pathKey = Path.Combine(path1, path2);

        return PhotonNetwork.InstantiateRoomObject(pathKey, position, rotation, group, data);
    }
}
