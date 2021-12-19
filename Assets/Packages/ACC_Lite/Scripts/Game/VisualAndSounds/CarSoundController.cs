using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Car sound controller, for play car sound effects
/// </summary>

[RequireComponent (typeof (CarController))]
public class CarSoundController :MonoBehaviour
{
	[SerializeField] private PhotonView m_PhotonView;

	[Header("Engine Sounds")]
	[SerializeField] private AudioKey m_EngineBackFireKey;
	//[SerializeField] AudioClip EngineBackFireClip;
	[SerializeField] float PitchOffset = 0.5f;
	[SerializeField] AudioSource EngineSource;

	[Header("Slip Sounds")]
	[SerializeField] AudioSource SlipSource;
	[SerializeField] float MinSlipSound = 0.15f;
	[SerializeField] float MaxSlipForSound = 1f;

	CarController CarController;

	float MaxRPM { get { return CarController.GetMaxRPM; } }
	float EngineRPM { get { return CarController.EngineRPM; } }

	private void Awake ()
	{
		if (!m_PhotonView.IsMine)
		{
			SlipSource.Stop();
			return;
		}

		CarController = GetComponent<CarController> ();
		CarController.BackFireAction += PlayBackfire;
	}

	void Update ()
	{
		if (!m_PhotonView.IsMine)
		{
			return;
		}

		//Engine PRM sound
		EngineSource.pitch = (EngineRPM / MaxRPM) + PitchOffset;

		//Slip sound logic
		if (CarController.CurrentMaxSlip > MinSlipSound)
		{
			if (!SlipSource.isPlaying)
			{
				SlipSource.Play ();
			}

			float slipVolumeProcent = CarController.CurrentMaxSlip / MaxSlipForSound;
			SlipSource.volume = slipVolumeProcent * 0.5f;
			SlipSource.pitch = Mathf.Clamp (slipVolumeProcent, 0.75f, 1);
		}
		else
		{
			SlipSource.Stop ();
		}
	}

	void PlayBackfire ()
	{
		EngineSource.PlayOneShot(Andrich.UtilityScripts.AudioManager.Instance.GetAudioClip(m_EngineBackFireKey));
	}
}
