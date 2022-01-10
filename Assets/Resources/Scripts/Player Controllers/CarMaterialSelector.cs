using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Andrich.UtilityScripts;
using Photon.Realtime;

public class CarMaterialSelector : MonoBehaviour
{
    [SerializeField] private Image m_PrimaryColorImage;
    [SerializeField] private Image m_PrimarySpriteColorImage;
    [SerializeField] private Image m_SecondaryColorImage;
    [SerializeField] private Image m_SecondarySpriteColorImage;

    private Player m_Player;
    public Player Player => m_Player;

    public void SetUp(Player player)
    {
        m_Player = player;

        if(player == PhotonNetwork.LocalPlayer)
        {
            byte primaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
            PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex(primaryIndex);

            byte secondaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
            PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex(secondaryIndex);
        }

        UpdateCarSprite(player);

    }

    public void UpdateCarSprite(Player player)
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

        PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex((byte)index);

        PlayerPrefs.SetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, index);
    }

    public void ChangeSecondaryCarMaterial(int amount)
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        int index = PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex((byte)index);

        PlayerPrefs.SetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, index);
    }
}
