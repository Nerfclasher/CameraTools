﻿using System;
using System.Collections;
using UnityEngine;
namespace CameraTools
{
	public class CTPartAudioController : MonoBehaviour
	{
		Vessel vessel;
		Part part;

		public AudioSource audioSource;

		float origMinDist = 1;
		float origMaxDist = 1;

		float modMinDist = 1;
		float modMaxDist = 1500;

		void Awake()
		{
			part = GetComponentInParent<Part>();
			vessel = part.vessel;

			CamTools.OnResetCTools += OnResetCTools;
		}

		void Start()
		{
			if(!audioSource)
			{
				Destroy(this);
				return;
			}

			origMinDist = audioSource.minDistance;
			origMaxDist = audioSource.maxDistance;
		}

		void FixedUpdate()
		{
			if(!audioSource)
			{
				Destroy(this);
				return;
			}

			float angleToCam = Vector3.Angle(vessel.srf_velocity, FlightCamera.fetch.mainCamera.transform.position - vessel.transform.position);
			angleToCam = Mathf.Clamp(angleToCam, 1, 180);

			float srfSpeed = (float)vessel.srfSpeed;

			float lagAudioFactor = (75000 / (Vector3.Distance(vessel.transform.position, FlightCamera.fetch.mainCamera.transform.position) * srfSpeed * angleToCam / 90));
			lagAudioFactor = Mathf.Clamp(lagAudioFactor * lagAudioFactor * lagAudioFactor, 0, 4);
			lagAudioFactor += srfSpeed / 230;

			float waveFrontFactor = ((3.67f * angleToCam)/srfSpeed);
			waveFrontFactor = Mathf.Clamp(waveFrontFactor * waveFrontFactor * waveFrontFactor, 0, 2);
			if(vessel.srfSpeed > 330)
			{
				waveFrontFactor = (srfSpeed / (angleToCam) < 3.67f) ? srfSpeed / 15 : 0;
			}

			lagAudioFactor *= waveFrontFactor;
		
			audioSource.minDistance = modMinDist * lagAudioFactor;
			audioSource.maxDistance = Mathf.Clamp(modMaxDist * lagAudioFactor, audioSource.minDistance, 3000);
				
		}

		void OnDestroy()
		{
			CamTools.OnResetCTools -= OnResetCTools;
		}

		void OnResetCTools()
		{
			audioSource.minDistance = origMinDist;
			audioSource.maxDistance = origMaxDist;
			Destroy(this);
		}


	}
}

