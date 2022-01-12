using Andrich.UtilityScripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager Instance { get; private set; }

    [SerializeField] private Vector3 m_OffSet = new Vector3(0, 100f, 0);
    [SerializeField] private float m_MiniMapSize = 20f;

    [SerializeField] private Camera m_MiniMapCamera;
    private Transform m_CameraTarget;

    [SerializeField] private PoolAbleObject m_Arrow;
    private List<MiniMapItem> m_Arrows = new List<MiniMapItem>();
    private List<Transform> m_PlayerTargets = new List<Transform>();

    private List<Transform> m_StaticTargets = new List<Transform>();

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

    private void LateUpdate()
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

    public void AddStaticTargetTransform(Transform itemTarget)
    {

    }

    public void AddPlayerTransform(Transform playerTarget, Player player)
    {
        if(player == PhotonNetwork.LocalPlayer)
        {
            m_CameraTarget = playerTarget;
        }

        MiniMapItem arrow = PoolManager.Instance.GetObjectFromPool(m_Arrow).GetComponent<MiniMapItem>();
        arrow.SetUp(player);

        m_PlayerTargets.Add(playerTarget);
        m_Arrows.Add(arrow);
    }

    public void RemovePlayerTransform(Transform playerTarget)
    {
        if(!m_PlayerTargets.Contains(playerTarget))
        {
            Debug.LogWarning(playerTarget.name + " isn't in the list!");

            return;
        }

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

            Vector3 centerPosition = m_MiniMapCamera.transform.position;
            centerPosition.y = 0f;

            Vector3 playerControllerPosition = m_PlayerTargets[i].position;
            playerControllerPosition.y = 0f;

            float distance = Vector3.Distance(playerControllerPosition, centerPosition);

            if(distance > m_MiniMapSize)
            {
                Vector3 fromCenterToPlayer = playerControllerPosition - centerPosition;
                fromCenterToPlayer *= m_MiniMapSize / distance;

                Vector3 newArrowPosition = centerPosition + fromCenterToPlayer;
                newArrowPosition.y = m_Arrows[i].transform.position.y;

                m_Arrows[i].transform.position = newArrowPosition;
                m_Arrows[i].SetIfOutOfRange(true);
            }
            else
            {
                Vector3 newArrowPosition = playerControllerPosition;
                newArrowPosition.y = m_Arrows[i].transform.position.y;

                m_Arrows[i].transform.position = newArrowPosition;

                m_Arrows[i].transform.forward = m_PlayerTargets[i].forward;

                Quaternion newArrowRotation = m_Arrows[i].transform.rotation;
                newArrowRotation.x = 0f;
                newArrowRotation.z = 0f;

                m_Arrows[i].transform.rotation = newArrowRotation;
                m_Arrows[i].SetIfOutOfRange(false);
            }

            // Square
            //float clampedPositionX = Mathf.Clamp(playerControllerPosition.x, m_MiniMapCamera.transform.position.x - m_MiniMapSize, m_MiniMapCamera.transform.position.x + m_MiniMapSize);
            //float clampedPositionZ = Mathf.Clamp(playerControllerPosition.z, m_MiniMapCamera.transform.position.z - m_MiniMapSize, m_MiniMapCamera.transform.position.z + m_MiniMapSize);
            //playerControllerPosition = new Vector3(clampedPositionX, playerControllerPosition.y, clampedPositionZ);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.CheckIfGameHasBeenWonEventCode:

                break;

            case PhotonEventCodes.AddPlayerRespawnedToUIEventCode:

                break;
            case PhotonEventCodes.AddPlayerGotEliminatedToUIEventCode:

                break;

            default:
                break;
        }
    }
}
