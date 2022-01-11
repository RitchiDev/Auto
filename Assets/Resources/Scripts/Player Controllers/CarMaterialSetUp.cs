using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class CarMaterialSetUp : MonoBehaviour
{
    [SerializeField] private EliminationPlayerController m_PlayerController;
    [SerializeField] private MeshRenderer m_MeshRenderer;

    private void Start()
    {
        if (!CarMaterialManager.Instance)
        {
            return;
        }

        Material[] newMaterials = m_MeshRenderer.materials;

        newMaterials[1] = CarMaterialManager.Instance.GetSelectedPrimaryMaterial(m_PlayerController.Owner.GetPrimaryMaterialIndex());
        newMaterials[2] = CarMaterialManager.Instance.GetSelectedSecondaryMaterial(m_PlayerController.Owner.GetSecondaryMaterialIndex());

        m_MeshRenderer.materials = newMaterials;
    }
}
