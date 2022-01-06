﻿using System;
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

		UpdateActiveCamera ();

		//CamerasPreset[0].CameraHolder.transform.localPosition = Vector3.zero;
		//Quaternion rotation = Quaternion.LookRotation(TargetPoint - transform.position, Vector3.up);
		//ActivePreset.CameraHolder.rotation = rotation;
		//CamerasPreset[0].CameraHolder.transform.localRotation = new Quaternion(0, -198, 0, 0);

		if (NextCameraButton)
		{
			NextCameraButton.onClick.AddListener (SetNextCamera);
		}
	}


	//The target point is calculated from velocity of car.
	Vector3 TargetPoint
	{
		get
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
	}

	private void Update()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		if (ActivePreset.EnableRotation && (TargetPoint - transform.position).sqrMagnitude >= SqrMinDistance)
		{
			Quaternion rotation = Quaternion.LookRotation (TargetPoint - transform.position, Vector3.up);
			ActivePreset.CameraHolder.rotation = Quaternion.Lerp (ActivePreset.CameraHolder.rotation, rotation, Time.deltaTime * ActivePreset.SetRotationSpeed);
		}

		transform.position = Vector3.LerpUnclamped (transform.position, TargetPoint, Time.deltaTime * ActivePreset.SetPositionSpeed);

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
	}

	public void SetNextCamera ()
	{
		ActivePresetIndex = MathExtentions.LoopClamp (ActivePresetIndex + 1, 0, CamerasPreset.Count);
		UpdateActiveCamera ();
	}

	public void UpdateActiveCamera ()
	{
		if (ActivePreset != null)
		{
			ActivePreset.CameraHolder.SetActive (false);
		}

		ActivePreset = CamerasPreset[ActivePresetIndex];
		ActivePreset.CameraHolder.SetActive (true);

		SqrMinDistance = ActivePreset.MinDistanceForRotation * 2;

		if (ActivePreset.EnableRotation && (TargetPoint - transform.position).sqrMagnitude >= SqrMinDistance)
		{
			Quaternion rotation = Quaternion.LookRotation (TargetPoint - transform.position, Vector3.up);
			ActivePreset.CameraHolder.rotation = rotation;
		}
	}

	[System.Serializable]
	class CameraPreset
	{
		public Transform CameraHolder;                  //Parent fo camera.
		public float SetPositionSpeed = 1;              //Change position speed.
		public float VelocityMultiplier;                //Velocity of car multiplier.

		public bool EnableRotation;
		public float MinDistanceForRotation = 0.1f;     //Min distance for potation, To avoid uncontrolled rotation.
		public float SetRotationSpeed = 1;              //Change rotation speed.
	}
}
