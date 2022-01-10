using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using Andrich.UtilityScripts;

public class RingManager : MonoBehaviour, IOnEventCallback
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
        }

        SetRings();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.ActivateAllRingsEventCode)
        {
            ActivateAllRings();
        }

        if(eventCode == EventCodes.DeactivateAllRingsEventCode)
        {
            DeactiveAllRings();
        }

        if(eventCode == EventCodes.ActivateNew500RingEventCode)
        {
            ActivateNew500Ring();
        }
    }

    private void SetRings()
    {
        Ring[] rings = GetComponentsInChildren<Ring>();

        for (int i = 0; i < rings.Length; i++)
        {
            if (rings[i].Worth >= 500)
            {
                m_RingsWorth500.Add(rings[i]);
            }
            else
            {
                m_Rings.Add(rings[i]);
            }
        }

        //DeactiveAllRings();
    }

    public void ActivateAllRings()
    {
        Debug.Log("Activating all rings!");

        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Activate();
        }

        ActivateNew500Ring();
    }

    public void DeactiveAllRings()
    {
        //Debug.Log("Deactivating all rings!");

        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Deactivate(false);
        }

        for (int i = 0; i < m_RingsWorth500.Count; i++)
        {
            m_RingsWorth500[i].Deactivate(false);
        }
    }

    public void ActivateNew500Ring()
    {
        //Debug.Log("Activating new 500 ring!");

        if (m_RingsWorth500.Count <= 0)
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
