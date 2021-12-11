using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

            for (int i = 0; i < ringCollisions.Length; i++)
            {
                string path1 = "Photon Prefabs";
                string path2 = "Ring " + ringCollisions[i].Worth;
                //string path2 = ringCollisions[i].transform.parent.name;

                if (ringCollisions[i].Worth >= 500)
                {
                    GameObject ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), ringCollisions[i].transform.position, ringCollisions[i].transform.rotation);

                    //RingCollision ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), ringCollisions[i].transform.position, ringCollisions[i].transform.rotation, group, data).transform.parent.GetComponentInChildren<RingCollision>();
                    m_RingsWorth500.Add(ringToAdd.GetComponentInChildren<RingCollision>());
                }
                else
                {
                    GameObject ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), ringCollisions[i].transform.position, ringCollisions[i].transform.rotation);
                    
                    //RingCollision ringToAdd = PhotonNetwork.Instantiate(Path.Combine(path1, path2), ringCollisions[i].transform.position, ringCollisions[i].transform.rotation, group, data).transform.parent.GetComponentInChildren<RingCollision>();
                    m_Rings.Add(ringToAdd.GetComponentInChildren<RingCollision>());
                }

                Destroy(ringCollisions[i].transform.parent.gameObject);
            }

            //foreach (RingCollision ringCollision in ringCollisions)
            //{
            //    if (ringCollision.Worth >= 500)
            //    {
            //        m_RingsWorth500.Add(ringCollision);
            //    }
            //    else
            //    {
            //        m_Rings.Add(ringCollision);
            //    }
            //}
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
