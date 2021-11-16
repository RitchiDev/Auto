using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject m_Shape;

    private void Awake()
    {
        m_Shape.SetActive(false);
    }
}
