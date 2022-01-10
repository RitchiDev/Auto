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

        //m_PrimaryColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(index);
        //m_PrimarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(index);
        PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex(index);
    }

    public void ChangeSecondaryCarMaterial(int amount)
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        int index = PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        //m_SecondaryColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(index);
        //m_SecondarySpriteColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(index);
        PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex(index);
    }
}
