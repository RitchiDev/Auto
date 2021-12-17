using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Andrich.UtilityScripts;
using Photon.Realtime;

public class CarMaterialSelector : MonoBehaviour
{
    [SerializeField] private PlayerInRoomItem m_PlayerItem;
    [SerializeField] private Image m_PrimaryColorImage;
    [SerializeField] private Image m_SecondaryColorImage;

    private Player m_Player;
    public Player Player => m_Player;

    public void SetUp(Player player)
    {
        m_Player = player;

        if (PhotonNetwork.LocalPlayer == player)
        {
            ChangePrimaryCarMaterial(PhotonNetwork.LocalPlayer.GetPrimaryMaterialIndex());
            ChangeSecondaryCarMaterial(PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex());
        }
    }

    public void ChangePrimaryCarMaterial(int amount)
    {
        int index = PhotonNetwork.LocalPlayer.GetPrimaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxPrimaryIndex);

        m_PrimaryColorImage.color = CarMaterialManager.Instance.GetSelectedPrimaryColor(index);
        PhotonNetwork.LocalPlayer.SetPrimaryMaterialIndex(index);
    }

    public void ChangeSecondaryCarMaterial(int amount)
    {
        int index = PhotonNetwork.LocalPlayer.GetSecondaryMaterialIndex();
        index = Mathf.Clamp(index + amount, 0, CarMaterialManager.Instance.MaxSecondaryIndex);

        m_SecondaryColorImage.color = CarMaterialManager.Instance.GetSelectedSecondaryColor(index);
        PhotonNetwork.LocalPlayer.SetSecondaryMaterialIndex(index);
    }
}
