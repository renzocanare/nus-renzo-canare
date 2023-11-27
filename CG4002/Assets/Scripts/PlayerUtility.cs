using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//////////////////////////////////////////////////////////
// -- PlayerUtility -- 
// Stores & updates player ammo/grenade/shield utility.
//
// NOTE: LOCAL GAME ENGINE methods are for developer testing only.
//////////////////////////////////////////////////////////
public class PlayerUtility : MonoBehaviour 
{
	// To keep after full MQTT implementation.
	public int ammoCount; 
	public int grenadeCount;
	public int shieldCount;
	public int shieldHealth;
	public bool shieldOn = false;
	public bool shieldWait = false;
	private bool startWait = false;
	public float onTimer = 0.0f;
	private float offTimer = 0.0f;
	public GameObject ShieldPlane;
	public TextMeshProUGUI destroyedText;
	private mqttController controller;

	void Start()
	{
		controller = GameObject.FindGameObjectWithTag("MQTTController").GetComponent<mqttController>();
		ShieldPlane.SetActive(false);
		destroyedText.enabled = false;
	}

	void disableText()
	{
		destroyedText.enabled = false;
	}


	/**
    * Update() - DO NOT DELETE
    * Start shield animation.
	* Shield can be activated anywhere by:
	* 	if (shieldOn == false && shieldCount > 0 && shieldWait == false)
	*	{
	*	shieldOn = true;
	*	}
    */  
	void Update()
	{
		// Turn off shield if shieldHealth reaches 0 & reset shield health.
		if (shieldOn && shieldHealth <= 0 && onTimer < 10.0f)
		{
			onTimer = 10.0f;
			shieldHealth = 30;

			// if (!controller.p1_corr_state && !controller.p2_corr_state)
			// {
			// 	destroyedText.enabled = true;
			// 	Invoke("disableText", 3.0f);
			// }
		}

		// Turn on shield for 10 seconds.
		if (shieldOn && onTimer < 10.0f)
		{
			ShieldPlane.SetActive(true);
			onTimer += Time.deltaTime;
			startWait = true;
			Debug.Log("On Timer = " + onTimer.ToString());
		}
		// Turn off shield after 10 seconds
		else if (onTimer >= 10.0f)
		{
			ShieldPlane.SetActive(false);
			shieldOn = false;
			onTimer = 0.0f;
			shieldHealth = 30;
		}

		if (startWait)
		{
			if (offTimer < 10.0f)
			{
				shieldWait = true;
				offTimer += Time.deltaTime;
			}
			else
			{
				shieldWait = false;
				startWait = false;
				offTimer = 0.0f;
			}

			Debug.Log("Off Timer = " + offTimer.ToString());			
		}


		// // Wait for 10 seconds before next shield on.
		// if (shieldWait && offTimer < 10.0f)
		// {
		// 	offTimer += Time.deltaTime;
		// 	Debug.Log("Off Timer = " + offTimer.ToString());
		// }
		// else if (offTimer >= 10.0f)
		// {
		// 	shieldWait = false;
		// 	offTimer = 0.0f;
		// }
	}

    /**
    * Shoot() - LOCAL GAME ENGINE
    * Decrease Player ammo count.
    */  
	public void Shoot()
	{
		ammoCount -=1;
		Debug.Log("Ammo = " + ammoCount.ToString());

		if (ammoCount < 0)
		{
			ammoCount = 6;
		}

	}

    /**
    * Grenade() - LOCAL GAME ENGINE
    * Decrease Player grenade count.
    */  
	public void Grenade()
	{
		if (grenadeCount > 0)
		{
			grenadeCount -=1;
		}
	}

    /**
    * Shield() - LOCAL GAME ENGINE
    * Enable shield and decrease shield count.
    */  
	public void Shield()
	{
		if (shieldOn == false && shieldCount > 0 && shieldWait == false)
		{
		shieldOn = true;

		shieldCount -=1;

		Debug.Log("Shield = " + shieldCount.ToString());
		}
	}
}
