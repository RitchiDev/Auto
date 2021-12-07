using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    private List<Ring> m_Rings = new List<Ring>();
    private List<Ring> m_RingsWorth500 = new List<Ring>();
    public static RingManager Instance { get; private set; }
    private Ring m_PreviousRing;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;

            Ring[] rings = GetComponentsInChildren<Ring>();

            foreach (Ring ring in rings)
            {
                if(ring.Worth >= 500)
                {
                    m_RingsWorth500.Add(ring);
                }
                else
                {
                    m_Rings.Add(ring);
                    ring.transform.parent.gameObject.SetActive(false);
                    SetNew500RingActive();
                }
            }
        }
    }

    public void SetNew500RingActive()
    {
        for (int i = 0; i < m_RingsWorth500.Count; i++)
        {
            Ring ring = m_RingsWorth500[i];
            if(!ring.transform.parent.gameObject.activeInHierarchy)
            {

            }
        }
    }
}
