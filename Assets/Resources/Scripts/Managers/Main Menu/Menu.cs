using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private MenuName m_MenuName;
    private bool m_IsOpen;

    public MenuName MenuName => m_MenuName;
    public bool IsOpen => m_IsOpen;

    private void Awake()
    {
        m_IsOpen = gameObject.activeInHierarchy;
    }

    public void Open()
    {
        m_IsOpen = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        m_IsOpen = false;
        gameObject.SetActive(false);
    }
}
