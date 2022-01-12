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
            Camera cameraToFind = FindObjectOfType<Camera>();
            if(cameraToFind)
            {
                if(!cameraToFind.GetComponent<IgnoreCamera>() && cameraToFind.gameObject.activeInHierarchy)
                {
                    m_Camera = cameraToFind;
                }
            }
        }

        if(m_Camera == null)
        {
            return;
        }

        transform.LookAt(m_Camera.transform);
        transform.Rotate(Vector3.up * m_Rotation);
    }
}
