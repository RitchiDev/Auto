using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class CarMaterialSetUp : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private EliminationPlayerController m_PlayerController;
    [SerializeField] private MeshRenderer m_MeshRenderer;

    private void Start()
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        if(m_PhotonView.IsMine)
        {
            byte primaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);
            byte secondaryIndex = (byte)PlayerPrefs.GetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty, SettingsProperties.DefaultCarColorIndex);

            m_PhotonView.RPC("RPC_SetUp", RpcTarget.AllBuffered, primaryIndex, secondaryIndex);
        }
    }

    [PunRPC]
    private void RPC_SetUp(byte primaryIndex, byte secondaryIndex)
    {
        Material[] newMaterials = m_MeshRenderer.materials;
        newMaterials[1] = CarMaterialManager.Instance.GetSelectedPrimaryMaterial(primaryIndex);
        newMaterials[2] = CarMaterialManager.Instance.GetSelectedSecondaryMaterial(secondaryIndex);
        m_MeshRenderer.materials = newMaterials;
    }
}
