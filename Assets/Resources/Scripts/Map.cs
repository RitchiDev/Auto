using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Map : MonoBehaviour
{
    [SerializeField] private int m_MapNumber;
    [SerializeField] private int m_SceneIndex;

    public void SelectMap()
    {
        GameModeManager.Instance.SelectMap(m_SceneIndex, m_MapNumber);
    }
}
