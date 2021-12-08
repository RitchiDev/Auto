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

            RingCollision[] ringCollisions = GetComponentsInChildren<RingCollision>();

            foreach (RingCollision ringCollision in ringCollisions)
            {
                if(ringCollision.Worth >= 500)
                {
                    m_RingsWorth500.Add(ringCollision);
                }
                else
                {
                    m_Rings.Add(ringCollision);
                }
            }
        }
    }

    public void ActivateAllRings()
    {
        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Ring.Activate();
        }

        SetNew500RingActive();
    }

    public void DeactiveAllRings()
    {
        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Ring.Deactivate(false);
        }

        for (int i = 0; i < m_RingsWorth500.Count; i++)
        {
            m_RingsWorth500[i].Ring.Deactivate(false);
        }
    }

    public void SetNew500RingActive()
    {
        if(m_RingsWorth500.Count <= 0)
        {
            Debug.Log("There are no rings worth 500 in the scene!");
            return;
        }

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

            if (m_RingsWorth500[index] != m_PreviousRing)
            {
                Debug.Log("New Ring Activated Succesfully!");

                newRingHasBeenActivated = true;
            }
        }

        m_RingsWorth500[index].Ring.Activate();
    }
}
