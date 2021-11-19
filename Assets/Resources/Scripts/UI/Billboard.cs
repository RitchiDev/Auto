using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera m_Camera;

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
        transform.Rotate(Vector3.up * 100);
    }
}
