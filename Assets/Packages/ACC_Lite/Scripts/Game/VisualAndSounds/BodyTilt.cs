using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// To tilt the car body (Visually only).
/// </summary>

[RequireComponent (typeof (CarController))]
public class BodyTilt : MonoBehaviour
{
	[SerializeField] private PhotonView m_PhotonView;

	[SerializeField] Transform Body;                                //Link to car body.
	[SerializeField] float MaxAngle = 10;                           //Max tilt angle of car body.
	[SerializeField] float AngleVelocityMultiplayer = 0.2f;         //Rotation angle multiplier when moving forward.
	[SerializeField] float RearAngleVelocityMultiplayer = 0.4f;     //Rotation angle multiplier when moving backwards.
	[SerializeField] float MaxTiltOnSpeed = 60;                     //The speed at which the maximum tilt is reached.

	CarController Car;
	float Angle;

	private void Awake ()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		Car = GetComponent<CarController> ();
	}

	private void Update ()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		if (Car.CarDirection == 1)
			Angle = -Car.VelocityAngle * AngleVelocityMultiplayer;
		else if (Car.CarDirection == -1)
		{
			Angle = MathExtentions.LoopClamp (Car.VelocityAngle + 180, -180, 180) * RearAngleVelocityMultiplayer;
		}
		else
		{
			Angle = 0;
		}

		Angle *= Mathf.Clamp01 (Car.SpeedInHour / MaxTiltOnSpeed);
		Angle = Mathf.Clamp (Angle, -MaxAngle, MaxAngle);
		Body.localRotation = Quaternion.AngleAxis (Angle, Vector3.forward);
	}
}
