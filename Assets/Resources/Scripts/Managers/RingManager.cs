using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

            PlaceholderRing[] placeholderRings = GetComponentsInChildren<PlaceholderRing>();

            for (int i = 0; i < placeholderRings.Length; i++)
            {
                string path1 = "Photon Prefabs";
                string path2 = "Ring " + placeholderRings[i].Worth;

                if (placeholderRings[i].Worth >= 500)
                {
                    GameObject ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), placeholderRings[i].transform.position, placeholderRings[i].transform.rotation);

                    m_RingsWorth500.Add(ringToAdd.GetComponent<Ring>());
                }
                else
                {
                    GameObject ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), placeholderRings[i].transform.position, placeholderRings[i].transform.rotation);
                    
                    m_Rings.Add(ringToAdd.GetComponent<Ring>());
                }

                Destroy(placeholderRings[i].gameObject);
            }

            DeactiveAllRings();
        }
    }

    public void ActivateAllRings()
    {
        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Activate();
        }

        SetNew500RingActive();
    }

    public void DeactiveAllRings()
    {
        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Deactivate(false);
        }

        for (int i = 0; i < m_RingsWorth500.Count; i++)
        {
            m_RingsWorth500[i].Deactivate(false);
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
                //Debug.Log("New Ring Activated Succesfully!");

                newRingHasBeenActivated = true;
            }
        }

        m_RingsWorth500[index].Activate();
    }
}
