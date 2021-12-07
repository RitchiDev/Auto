using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    private List<RingCollision> m_Rings = new List<RingCollision>();
    private List<RingCollision> m_RingsWorth500 = new List<RingCollision>();
    public static RingManager Instance { get; private set; }
    private RingCollision m_PreviousRing;

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

            RingCollision[] rings = GetComponentsInChildren<RingCollision>();

            foreach (RingCollision ring in rings)
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
        int index = Random.Range(0, m_RingsWorth500.Count - 1);
        bool newRingHasBeenActivated = false;
        int stopCount = 0;

        while (!newRingHasBeenActivated)
        {
            index = Random.Range(0, m_RingsWorth500.Count - 1);

            if (stopCount++ > 1000)
            {
                Debug.Log("Reached Stop Count");
                index = 0;
                break;
            }

            if (m_RingsWorth500[index] != m_PreviousRing && !m_RingsWorth500[index].transform.parent.gameObject.activeInHierarchy)
            {
                Debug.Log("New Ring Activated Succesfully!");

                newRingHasBeenActivated = true;
            }
        }

        m_RingsWorth500[index].transform.parent.gameObject.SetActive(true);
    }
}
