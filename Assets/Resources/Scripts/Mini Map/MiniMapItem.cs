using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapItem : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_DefaultIcon;
    [SerializeField] private MeshRenderer m_OutOfRangeIcon;

    [SerializeField] private Material m_IsMyArrowMaterial;
    [SerializeField] private Material m_IsNotMyArrowMaterial;

    public void SetUp(Player player)
    {
        bool isMyPlayer = PhotonNetwork.LocalPlayer == player;

        Material[] newArrowMaterials = m_DefaultIcon.materials;
        newArrowMaterials[0] = isMyPlayer ? m_IsMyArrowMaterial : m_IsNotMyArrowMaterial;
        m_DefaultIcon.materials = newArrowMaterials;

        Material[] newCircleMaterials = m_OutOfRangeIcon.materials;
        newCircleMaterials[0] = isMyPlayer ? m_IsMyArrowMaterial : m_IsNotMyArrowMaterial;
        m_OutOfRangeIcon.materials = newCircleMaterials;
    }

    public void SetIfOutOfRange(bool isOutOfRange)
    {
        if(isOutOfRange)
        {
            if(m_OutOfRangeIcon)
            {
                if(!m_OutOfRangeIcon.gameObject.activeInHierarchy)
                {
                    m_OutOfRangeIcon.gameObject.SetActive(true);
                }
            }

            if(m_DefaultIcon)
            {
                if(m_DefaultIcon.gameObject.activeInHierarchy)
                {
                    m_DefaultIcon.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if(m_DefaultIcon)
            {
                if(!m_DefaultIcon.gameObject.activeInHierarchy)
                {
                    m_DefaultIcon.gameObject.SetActive(true);
                }
            }

            if(m_OutOfRangeIcon)
            {
                if (m_OutOfRangeIcon.gameObject.activeInHierarchy)
                {
                    m_OutOfRangeIcon.gameObject.SetActive(false);
                }
            }
        }
    }
}
