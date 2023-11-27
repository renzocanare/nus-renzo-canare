using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- P1UtilityUpdater -- 
// Updates utility information (ammo count, grenade count, shield count) of Player 1.
//////////////////////////////////////////////////////////
public class P1UtilityUpdater : MonoBehaviour 
{
	public TextMeshProUGUI ammoNumber;
	public TextMeshProUGUI grenadeNumber;
	public TextMeshProUGUI shieldNumber;
	public TextMeshProUGUI shieldHealthHeader;
	public TextMeshProUGUI shieldHealth;
	public Image shieldTimeImage;
	public TextMeshProUGUI shieldTimeNumber;
	private PlayerUtility p1Utility;

	void Start() 
	{
		p1Utility = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerUtility>();

		shieldHealthHeader.enabled = false;
		shieldHealth.enabled = false;
		shieldTimeImage.enabled = false;
		shieldTimeNumber.enabled = false;
	}

	void Update() 
	{
		// Update text boxes with Player1 utility information.
		ammoNumber.text = p1Utility.ammoCount.ToString();
		grenadeNumber.text = p1Utility.grenadeCount.ToString();
		shieldNumber.text = p1Utility.shieldCount.ToString();
		shieldHealth.text = p1Utility.shieldHealth.ToString();
		int shieldTimeCount = (int)(10 - p1Utility.onTimer);
		shieldTimeNumber.text = shieldTimeCount.ToString();

		// Update ammo color indicator.
		if (p1Utility.ammoCount <= 3 && p1Utility.ammoCount > 0)
		{
			ammoNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p1Utility.ammoCount == 0)
		{
			ammoNumber.text = "E";
			ammoNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			ammoNumber.color = Color.white;
		}

		// Update grenade color indicator.
		if (p1Utility.grenadeCount <= 1 && p1Utility.grenadeCount > 0)
		{
			grenadeNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p1Utility.grenadeCount == 0)
		{
			grenadeNumber.text = "E";
			grenadeNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			grenadeNumber.color = Color.white;
		}

		// Update shield color indicator.
		if (p1Utility.shieldCount <= 2 && p1Utility.shieldCount > 0)
		{
			shieldNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p1Utility.shieldCount == 0)
		{
			shieldNumber.text = "E";
			shieldNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			shieldNumber.color = Color.white;
		}

		// Display shield activated text.
		if (p1Utility.shieldOn)
		{
			shieldHealthHeader.enabled = true;
			shieldHealth.enabled = true;
			shieldTimeImage.enabled = true;
			shieldTimeNumber.enabled = true;

			shieldTimeImage.fillAmount = (10.0f - p1Utility.onTimer) / 10.0f;

			if (p1Utility.shieldHealth <= 10)
			{
				shieldHealth.color = Color.Lerp(Color.black, Color.clear, Mathf.PingPong(Time.time, 1.0f));
			}
			else
			{
				shieldHealth.color = Color.black;
			}
		}
		else
		{
			shieldHealthHeader.enabled = false;
			shieldHealth.enabled = false;
			shieldTimeImage.enabled = false;
			shieldTimeNumber.enabled = false;				
		}

	}
}
