using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using Andrich.UtilityScripts;

public class ItemBoxManager : MonoBehaviour, IOnEventCallback
{
    public static ItemBoxManager Instance { get; private set; }
    private List<ItemBox> m_ItemBoxes = new List<ItemBox>();

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

        SetItemBoxes();
    }

    private void SetItemBoxes()
    {
        ItemBox[] itemBoxes = GetComponentsInChildren<ItemBox>();

        for (int i = 0; i < itemBoxes.Length; i++)
        {
            m_ItemBoxes.Add(itemBoxes[i]);
        }

        DeactivateAllItemBoxes();
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
            case PhotonEventCodes.ActivateAllItemBoxesEventCode:

                ActivateAllItemBoxes();

                break;

            case PhotonEventCodes.DeactivateAllItemBoxesEventCode:

                DeactivateAllItemBoxes();

                break;

            default:
                break;
        }
    }

    private void ActivateAllItemBoxes()
    {
        for (int i = 0; i < m_ItemBoxes.Count; i++)
        {
            m_ItemBoxes[i].Activate();
        }
    }

    private void DeactivateAllItemBoxes()
    {
        for (int i = 0; i < m_ItemBoxes.Count; i++)
        {
            m_ItemBoxes[i].Deactivate(false);
        }
    }
}
