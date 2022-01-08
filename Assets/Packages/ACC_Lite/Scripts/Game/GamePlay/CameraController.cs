using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Move and rotation camera controller
/// </summary>

public class CameraController : MonoBehaviour
{
	[SerializeField] private PhotonView m_PhotonView;
	[SerializeField] KeyCode SetCameraKey = KeyCode.C;                              //Set next camore on PC hotkey.
	[SerializeField] UnityEngine.UI.Button NextCameraButton;
	[SerializeField] List<CameraPreset> CamerasPreset = new List<CameraPreset>();   //Camera presets
	[SerializeField] private CarSetUp m_CarSetUp;
	[SerializeField] private CarController m_TargetCar;
	int ActivePresetIndex = 0;
	CameraPreset ActivePreset;

	float SqrMinDistance;
	
	private void Awake()
	{
		if (transform.parent)
		{
			transform.SetParent(null);
		}

		if (!m_PhotonView.IsMine)
		{
			gameObject.SetActive(false);
			return;
		}

		CamerasPreset.ForEach (c => c.CameraHolder.SetActive (false));

		UpdateActiveCamera();

		if (NextCameraButton)
		{
			NextCameraButton.onClick.AddListener (SetNextCamera);
		}
	}


	private void Update()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		if (ActivePreset.EnableRotation && (TargetPoint() - transform.position).sqrMagnitude >= SqrMinDistance)
		{
			UpdateRotation();
		}

		transform.position = Vector3.LerpUnclamped (transform.position, TargetPoint(), Time.deltaTime * ActivePreset.SetPositionSpeed);

		if (Input.GetKeyDown (SetCameraKey))
		{
			SetNextCamera ();
		}
	}

	public void FreezeCameras(bool doFreeze)
	{
		for (int i = 0; i < CamerasPreset.Count; i++)
		{
			CamerasPreset[i].CameraHolder.gameObject.isStatic = doFreeze;
		}

		if(!doFreeze)
		{
			UpdateRotation();
		}
	}

	public void SetNextCamera()
	{
		ActivePresetIndex = MathExtentions.LoopClamp (ActivePresetIndex + 1, 0, CamerasPreset.Count);
		UpdateActiveCamera();
	}

	public void UpdateActiveCamera ()
	{
		if (ActivePreset != null)
		{
			ActivePreset.CameraHolder.SetActive (false);
		}

		ActivePreset = CamerasPreset[ActivePresetIndex];
		ActivePreset.CameraHolder.SetActive (true);

		SqrMinDistance = ActivePreset.MinDistanceForRotation;

		if (ActivePreset.EnableRotation && (TargetPoint() - transform.position).sqrMagnitude >= SqrMinDistance)
		{
			UpdateRotation();
		}
	}

	private void UpdateRotation()
	{
		Quaternion rotation = Quaternion.LookRotation(TargetPoint() - transform.position, Vector3.up);
		rotation.z = 0;
		rotation.x = 0;
		ActivePreset.CameraHolder.rotation = Quaternion.Lerp(ActivePreset.CameraHolder.rotation, rotation, Time.deltaTime * ActivePreset.SetRotationSpeed);
	}

	//The target point is calculated from the velocity of the car.
	private Vector3 TargetPoint()
	{
		if (m_CarSetUp == null || m_TargetCar == null)
		{
			return transform.position;
		}

		Vector3 result = m_TargetCar.RB.velocity * ActivePreset.VelocityMultiplier;
		result += m_TargetCar.transform.position;
		//result.y = 0;
		return result;
	}

	[System.Serializable]
	class CameraPreset
	{
		public Transform CameraHolder;                  //Parent for camera.
		public float SetPositionSpeed = 1;              //Change position speed.
		public float VelocityMultiplier;                //Velocity of car multiplier.

		public bool EnableRotation;
		public float MinDistanceForRotation = 0.1f;     //Min distance for rotation, To avoid uncontrolled rotation.
		public float SetRotationSpeed = 1;              //Change rotation speed.
	}
}
