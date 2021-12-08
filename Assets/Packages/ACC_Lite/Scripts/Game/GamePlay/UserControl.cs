using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class UserControl : MonoBehaviour
{
	[SerializeField] private PhotonView m_PhotonView;
	CarController ControlledCar;

	public float Horizontal { get; private set; }
	public float Vertical { get; private set; }
	public bool Brake { get; private set; }

	public static MobileControlUI CurrentUIControl { get; set; }

	private void Awake ()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		ControlledCar = GetComponent<CarController> ();
		CurrentUIControl = FindObjectOfType<MobileControlUI> ();
	}

	void Update ()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		if (CurrentUIControl != null && CurrentUIControl.ControlInUse)
		{
			//Mobile control.
			Horizontal = CurrentUIControl.GetHorizontalAxis;
			Vertical = CurrentUIControl.GetVerticalAxis;
		}
		else
		{
			//Standart input control (Keyboard or gamepad).
			Horizontal = Input.GetAxis ("Horizontal");
			Vertical = Input.GetAxis ("Vertical");
			Brake = Input.GetButton ("Jump");
		}

		//Apply control for controlled car.
		ControlledCar.UpdateControls (Horizontal, Vertical, Brake);
	}
}
