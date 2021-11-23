using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace EditedCarController
{

    public class Minimap_Cam : MonoBehaviour
    {
        [SerializeField] private PhotonView m_PhotonView;

        [Header("Objects Settings")]
        public Transform Target; //Target (the car)
        public Camera Camera; //The camera (the one this is assigned to)

        [Header("Car_Icon")]
        public Transform Icon; //The icon of the car
        public Vector3 Icon_Rotation_Offset; //The icon rotation offset
        public float Height_Offset; //How high the camera is

        [Header("Settings")]
        public Vector3 Offset_Pos; //The position offset
        public float Cam_Size; //The camera size

        private void Start()
        {
            if(!m_PhotonView.IsMine)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update(){
            //Setting Camera Position To Target's Position + Offset
            transform.position = Target.position + Offset_Pos;

            //Setting Camera Size to Cam_Size
            Camera.orthographicSize = Cam_Size;

            //Setting the car's rotation and position to the Car Icon
            Icon.rotation = Target.rotation * Quaternion.Euler(Icon_Rotation_Offset);
            Icon.position = Target.position + new Vector3(0, Height_Offset, 0);
        }
    }
}
