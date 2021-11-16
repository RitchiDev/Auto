using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Spawnpoint[] m_Spawnpoints;
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
            m_Spawnpoints = GetComponentsInChildren<Spawnpoint>();
            m_PreviousIndex = -1;
        }
    }

    public Transform GetSpawnPoint()
    {
        int index = Random.Range(0, m_Spawnpoints.Length - 1);

        while (index == m_PreviousIndex)
        {
            index = Random.Range(0, m_Spawnpoints.Length - 1);
        }

        m_PreviousIndex = index;
        return m_Spawnpoints[index].transform;
    }
}
