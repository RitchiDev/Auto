using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask m_LayerMask;
    private Vector3 m_PlayerDetectionSize = new Vector3(16f, 10f, 16f);
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

    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, m_Spawnpoints.Length - 1);
        bool placed = false;
        int stopCount = 0;

        while (!placed)
        {
            if (stopCount++ > 1000)
            {
                Debug.Log("Reached Stop Count");
                index = 0;
                break;
            }

            //if (Physics.OverlapBox(m_Spawnpoints[index].transform.position, m_PlayerDetectionSize, Quaternion.identity, m_LayerMask) == null)
            if (Physics.OverlapBox(m_Spawnpoints[index].transform.position, m_PlayerDetectionSize, Quaternion.identity, m_LayerMask) == null)
            {
                Debug.Log("Placed Succesfully!");

                placed = true;
            }
        }

        return m_Spawnpoints[index].transform;
    }
}
