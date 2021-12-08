using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// Base class game controller.
/// </summary>
public class CarSetUp :MonoBehaviour
{
	[SerializeField] private PhotonView m_PhotonView;

	[SerializeField] KeyCode NextCarKey = KeyCode.N;
	[SerializeField] UnityEngine.UI.Button NextCarButton;
	public static bool RaceIsStarted { get { return true; } }
	public static bool RaceIsEnded { get { return false; } }

	CarController m_PlayerCar;
	[SerializeField] private List<CarController> m_Cars = new List<CarController>();
	int CurrentCarIndex = 0;

	private void Awake()
	{
		if(!m_PhotonView.IsMine)
		{
			return;
		}

		//Find all cars in current game.
		//m_Cars.AddRange (GameObject.FindObjectsOfType<CarController> ());
		m_Cars = m_Cars.OrderBy (c => c.name).ToList();

		foreach (var car in m_Cars)
		{
			var userControl = car.GetComponent<UserControl>();
			var audioListener = car.GetComponent<AudioListener>();

			if (userControl == null)
			{
				userControl = car.gameObject.AddComponent<UserControl> ();
			}

			if (audioListener == null)
			{
				audioListener = car.gameObject.AddComponent<AudioListener> ();
			}

			userControl.enabled = false;
			audioListener.enabled = false;
		}

		m_PlayerCar = m_Cars[0];
		m_PlayerCar.GetComponent<UserControl> ().enabled = true;
		m_PlayerCar.GetComponent<AudioListener> ().enabled = true;

		if (NextCarButton)
        {
			NextCarButton.onClick.AddListener (NextCar);
		}
	}

	void Update () 
	{ 
		if(!m_PhotonView.IsMine)
		{
			return;
		}

		if (Input.GetKeyDown (NextCarKey))
		{
			NextCar ();
		}
	}

	private void NextCar ()
	{
		m_PlayerCar.GetComponent<UserControl> ().enabled = false;
		m_PlayerCar.GetComponent<AudioListener> ().enabled = false;

		CurrentCarIndex = MathExtentions.LoopClamp (CurrentCarIndex + 1, 0, m_Cars.Count);

		m_PlayerCar = m_Cars[CurrentCarIndex];
		m_PlayerCar.GetComponent<UserControl> ().enabled = true;
		m_PlayerCar.GetComponent<AudioListener> ().enabled = true;
	}
}
