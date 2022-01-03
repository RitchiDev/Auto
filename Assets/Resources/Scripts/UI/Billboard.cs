using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private float m_Rotation = 180f;

    private void Update()
    {
        if(m_Camera == null)
        {
            m_Camera = FindObjectOfType<Camera>();
        }

        if(m_Camera == null)
        {
            return;
        }

        transform.LookAt(m_Camera.transform);
        transform.Rotate(Vector3.up * m_Rotation);
    }
}
