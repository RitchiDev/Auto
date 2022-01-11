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

            PhotonEvents.RaisePlayerEditedPrimaryCarControllerEvent(primaryIndex, player);
            PhotonEvents.RaisePlayerEditedSecondaryCarControllerEvent(secondaryIndex, player);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.RaisePlayerEditedPrimaryCarColorEventCode:

                object[] primaryData = (object[])photonEvent.CustomData;

                UpdateCarSpritePrimary((byte)primaryData[0], (Player)primaryData[1]);

                break;

            case PhotonEventCodes.RaisePlayerEditedSecondaryCarColorEventCode:

                object[] secondaryData = (object[])photonEvent.CustomData;

                UpdateCarSpriteSecondary((byte)secondaryData[0], (Player)secondaryData[1]);

                break;

            default:
                break;
        }
    }

    private void UpdateCarSpritePrimary(int primaryIndex, Player player)
    {
        if (player == m_Player)
        {
            m_PrimaryColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(primaryIndex);
            m_PrimarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(primaryIndex);
        }
    }

    private void UpdateCarSpriteSecondary(int secondaryIndex, Player player)
    {
        if (player == m_Player)
        {
            m_SecondaryColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(secondaryIndex);
            m_SecondarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(secondaryIndex);
        }
    }

    public void ChangePrimaryCarMaterial(int amount)
    {
        if(!CarMaterialManager.Instance)
        {
            return;
        }

        //int index = PhotonNetwork.LocalPlayer.GetPrimaryMaterialIndex();
        int index = PlayerPrefs.GetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        //PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex((byte)index);

        PlayerPrefs.SetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, index);

        PhotonEvents.RaisePlayerEditedPrimaryCarControllerEvent((byte)index, PhotonNetwork.LocalPlayer);
    }

    public void ChangeSecondaryCarMaterial(int amount)
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        //int index = PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex();
        int index = PlayerPrefs.GetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        //PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex((byte)index);
        PlayerPrefs.SetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, index);

        PhotonEvents.RaisePlayerEditedSecondaryCarControllerEvent((byte)index, PhotonNetwork.LocalPlayer);
    }
}
