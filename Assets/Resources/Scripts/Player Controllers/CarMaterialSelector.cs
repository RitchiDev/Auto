using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Andrich.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CarMaterialSelector : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Image m_PrimaryColorImage;
    [SerializeField] private Image m_PrimarySpriteColorImage;
    [SerializeField] private Image m_SecondaryColorImage;
    [SerializeField] private Image m_SecondarySpriteColorImage;

    private Player m_Player;
    public Player Player => m_Player;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetUp(Player player)
    {
        m_Player = player;

        if(player == PhotonNetwork.LocalPlayer)
        {
            byte primaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
            //PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex(primaryIndex);

            byte secondaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
            //PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex(secondaryIndex);

            //PhotonEvents.RaisePlayerEditedPrimaryCarControllerEvent(primaryIndex, secondaryIndex, player);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.RaisePlayerEditedCarColorEventCode:

                object[] currentData = (object[])photonEvent.CustomData;

                UpdateCarSprite((byte)currentData[0], (byte)currentData[1], (Player)currentData[2]);

                break;

            default:
                break;
        }
    }

    private void UpdateCarSprite(int primaryIndex, int secondaryIndex, Player player)
    {
        if (player == m_Player)
        {
            m_PrimaryColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(player.GetPrimaryMaterialIndex());
            m_PrimarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(player.GetPrimaryMaterialIndex());

            m_SecondaryColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(player.GetSecondaryMaterialIndex());
            m_SecondarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(player.GetSecondaryMaterialIndex());
        }
    }

    public void ChangePrimaryCarMaterial(int amount)
    {
        if(!CarMaterialManager.Instance)
        {
            return;
        }

        int index = PhotonNetwork.LocalPlayer.GetPrimaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxPrimaryIndex);

        //PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex((byte)index);

        PlayerPrefs.SetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, index);

        //PhotonEvents.RaisePlayerEditedCarControllerEvent(primaryIndex, secondaryIndex, player);
    }

    public void ChangeSecondaryCarMaterial(int amount)
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        int index = PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        //PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex((byte)index);
        PlayerPrefs.SetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, index);
    }
}
