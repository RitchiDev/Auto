using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpectateCameraController : MonoBehaviour
{
    private PhotonView m_PhotonView;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_CameraSensitivity = 90;
    [SerializeField] private float m_ClimbSpeed = 4;
    [SerializeField] private float m_NormalMoveSpeed = 10;
    [SerializeField] private float m_SlowMoveFactor = 0.25f;
    [SerializeField] private float m_FastMoveFactor = 3;

    private float m_RotationX = 0.0f;
    private float m_RotationY = 0.0f;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        if(m_PhotonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Destroy(m_Camera);
        }
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        m_RotationX += Input.GetAxis("Mouse X") * m_CameraSensitivity * Time.deltaTime;
        m_RotationY += Input.GetAxis("Mouse Y") * m_CameraSensitivity * Time.deltaTime;
        m_RotationY = Mathf.Clamp(m_RotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(m_RotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(m_RotationY, Vector3.left);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (m_NormalMoveSpeed * m_FastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (m_NormalMoveSpeed * m_FastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (m_NormalMoveSpeed * m_SlowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (m_NormalMoveSpeed * m_SlowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * m_NormalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * m_NormalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q))
        { 
            transform.position += transform.up * m_ClimbSpeed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= transform.up * m_ClimbSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.Locked : CursorLockMode.Confined;
        }
    }
}
