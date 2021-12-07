using UnityEngine;
using System.Collections.Generic;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask m_LayerMask;
    private Vector3 m_PlayerDetectionSize = new Vector3(16f, 10f, 16f);
    private List<Spawnpoint> m_Spawnpoints  = new List<Spawnpoint>();
    private List<Spawnpoint> m_AvailebleSpawnPoints = new List<Spawnpoint>();
    private int m_PreviousIndex;
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
            
            Spawnpoint[] spawnpoints = GetComponentsInChildren<Spawnpoint>();

            foreach (Spawnpoint spawnpoint in spawnpoints)
            {
                m_Spawnpoints.Add(spawnpoint);
            }

            m_AvailebleSpawnPoints = m_Spawnpoints;
        }
    }

    public Transform GetSpawnPoint()
    {
        int index = Random.Range(0, m_Spawnpoints.Count- 1);

        while (index == m_PreviousIndex)
        {
            index = Random.Range(0, m_Spawnpoints.Count - 1);
        }

        m_PreviousIndex = index;
        return m_Spawnpoints[index].transform;
    }

    public Transform GetUntakenSpawnpoints()
    {
        if (m_AvailebleSpawnPoints.Count == 0)
            m_AvailebleSpawnPoints = m_Spawnpoints;

        int index = Random.Range(0, m_AvailebleSpawnPoints.Count - 1);
        Transform Location = m_AvailebleSpawnPoints[index].transform;
        m_AvailebleSpawnPoints.RemoveAt(index);
        return Location;
    }

    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, m_Spawnpoints.Count - 1);
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

            if (Physics.OverlapBox(m_Spawnpoints[index].transform.position, m_PlayerDetectionSize, Quaternion.identity, m_LayerMask) == null)
            {
                Debug.Log("Placed Succesfully!");

                placed = true;
            }
        }
        return m_Spawnpoints[index].transform;
    }
}
