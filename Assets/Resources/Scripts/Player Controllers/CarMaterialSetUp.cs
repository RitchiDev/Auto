using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class CarMaterialSetUp : MonoBehaviour
{
    [SerializeField] private EliminationPlayerController m_EliminationPlayerController;
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private MeshRenderer m_MeshRenderer;

    private void Start()
    {
        if(m_PhotonView.IsMine)
        {
            m_PhotonView.RPC("RPC_SetUp", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_SetUp()
    {
        Material[] newMaterials = m_MeshRenderer.materials;

        newMaterials[1] = CarMaterialManager.Instance.GetSelectedPrimaryMaterial(m_EliminationPlayerController.Player.GetPrimaryMaterialIndex());
        newMaterials[2] = CarMaterialManager.Instance.GetSelectedSecondaryMaterial(m_EliminationPlayerController.Player.GetSecondaryMaterialIndex());

        m_MeshRenderer.materials = newMaterials;
    }
}
