using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance { get; private set; }

    [SerializeField] private Vector3 m_OffSet = new Vector3(0, 100f, 0);

    [SerializeField] private Camera m_MiniMapCamera;
    private Transform m_CameraTarget;

    [SerializeField] private PoolAbleObject m_Arrow;
    private List<Transform> m_Arrows = new List<Transform>();
    private List<Transform> m_PlayerTargets = new List<Transform>();

    [SerializeField] private Material m_IsMyArrowMaterial;
    [SerializeField] private Material m_IsNotMyArrowMaterial;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Restart()
    {
        for (int i = 0; i < m_Arrows.Count; i++)
        {
            m_Arrows[i].SetActive(false);
        }

        m_Arrows.Clear();
        m_PlayerTargets.Clear();
    }

    private void Update()
    {
        if(m_PlayerTargets.Count > 0 && m_Arrows.Count > 0)
        {
            UpdateArrowPositions();
        }

        if(m_CameraTarget != null)
        {
            UpdateCameraPosition();
        }
    }

    public void AddPlayerTransform(Transform playerTarget, Player player)
    {
        if(player == PhotonNetwork.LocalPlayer)
        {
            m_CameraTarget = playerTarget;
        }

        GameObject arrow = PoolManager.Instance.GetObjectFromPool(m_Arrow);
        MeshRenderer arrowMeshRender = arrow.GetComponentInChildren<MeshRenderer>();
        Material[] newMaterials = arrowMeshRender.materials;
        newMaterials[0] = PhotonNetwork.LocalPlayer == player ? m_IsMyArrowMaterial : m_IsNotMyArrowMaterial;
        arrowMeshRender.materials = newMaterials;

        m_PlayerTargets.Add(playerTarget);
        m_Arrows.Add(arrow.transform);
    }

    public void RemovePlayerTransform(Transform playerTarget)
    {
        for (int i = 0; i < m_PlayerTargets.Count; i++)
        {
            if(m_PlayerTargets[i] == playerTarget)
            {
                m_Arrows[i].SetActive(false);
                m_Arrows.RemoveAt(i);
            }
        }

        m_PlayerTargets.Remove(playerTarget);
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = new Vector3(m_CameraTarget.position.x + m_OffSet.x, m_MiniMapCamera.transform.position.y, m_CameraTarget.position.z + m_OffSet.z);
        m_MiniMapCamera.transform.position = targetPosition;
    }

    private void UpdateArrowPositions()
    {
        for (int i = 0; i < m_PlayerTargets.Count; i++)
        {
            if(m_Arrows[i] == null || m_PlayerTargets[i] == null)
            {
                return;
            }

            Vector3 playerControllerPosition = m_PlayerTargets[i].position;

            Vector3 newArrowPosition = new Vector3(playerControllerPosition.x, m_Arrows[i].position.y, playerControllerPosition.z);

            m_Arrows[i].position = newArrowPosition;

            m_Arrows[i].forward = m_PlayerTargets[i].forward;

            Quaternion newArrowRotation = m_Arrows[i].rotation;
            newArrowRotation.x = 0f;
            newArrowRotation.z = 0f;

            m_Arrows[i].rotation = newArrowRotation;
        }
    }
}
