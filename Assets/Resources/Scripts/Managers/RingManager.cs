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

        switch (eventCode)
        {
            case PhotonEventCodes.ActivateAllRingsEventCode:

                ActivateAllRings();

                break;

            case PhotonEventCodes.DeactivateAllRingsEventCode:

                DeactiveAllRings();

                break;

            case PhotonEventCodes.ActivateNew500RingEventCode:

                object[] ringIndexData = (object[])photonEvent.CustomData;

                ActivateNew500Ring((int)ringIndexData[0]);

                break;
            default:
                break;
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

    private void ActivateAllRings()
    {
        //Debug.Log("Activating all rings!");

        for (int i = 0; i < m_Rings.Count; i++)
        {
            m_Rings[i].Activate();
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonEvents.RaiseActivateNew500RingEvent(GetNew500RingListIndex());
        }
    }

    private void DeactiveAllRings()
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

    public int GetNew500RingListIndex()
    {
        if (m_RingsWorth500.Count <= 0)
        {
            Debug.LogError("There are no rings worth 500 in the scene!");
            return 0;
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
                newRingHasBeenActivated = true;
            }
        }

        return index;
    }

    private void ActivateNew500Ring(int index)
    {
        //Debug.Log("Activating new 500 ring!");

        if (m_RingsWorth500.Count <= 0)
        {
            Debug.Log("There are no rings worth 500 in the scene!");
            return;
        }

        m_RingsWorth500[index].Activate();
    }
}
