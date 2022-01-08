using UnityEngine;
using System.Collections.Generic;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private Vector3 m_PlayerDetectionSize = new Vector3(6f, 6f, 6f);
    private List<Spawnpoint> m_Spawnpoints  = new List<Spawnpoint>();
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Spawnpoint[] spawnpoints = GetComponentsInChildren<Spawnpoint>();

        foreach (Spawnpoint spawnpoint in spawnpoints)
        {
            m_Spawnpoints.Add(spawnpoint);
        }
    }

    public Transform GetSpawnPoint(int index)
    {
        if(index > m_Spawnpoints.Count || index < 0)
        {
            Debug.LogWarning("Index is out of range!");
            return GetRandomSpawnPoint();
        }

        return m_Spawnpoints[index].transform;
    }

    public Transform GetRandomSpawnPoint()
    {
        int index = 0;
        bool placed = false;
        int stopCount = 0;

        while (!placed)
        {
            index = Random.Range(0, m_Spawnpoints.Count - 1);

            if (stopCount++ > 1000)
            {
                Debug.Log("Reached Stop Count");
                index = 0;
                break;
            }

            Collider[] hitColliders = Physics.OverlapBox(m_Spawnpoints[index].transform.position, m_PlayerDetectionSize / 2, Quaternion.identity, m_LayerMask);

            //if (Physics.OverlapBox(m_Spawnpoints[index].transform.position, m_PlayerDetectionSize, Quaternion.identity, m_LayerMask) == null)
            if (hitColliders.Length <= 0)
            {
                //Debug.Log("Placed Succesfully!");

                placed = true;
            }
        }

        return m_Spawnpoints[index].transform;
    }
}
