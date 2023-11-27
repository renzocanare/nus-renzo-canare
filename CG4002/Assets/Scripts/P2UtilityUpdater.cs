using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- P2UtilityUpdater -- 
// Updates utility information (ammo count, grenade count, shield count) of Player 2.
//////////////////////////////////////////////////////////
public class P2UtilityUpdater : MonoBehaviour 
{
	public TextMeshProUGUI ammoNumber;
	public TextMeshProUGUI grenadeNumber;
	public TextMeshProUGUI shieldNumber;
	public TextMeshProUGUI shieldHealthHeader;
	public TextMeshProUGUI shieldHealth;
	public Image shieldTimeImage;
	public TextMeshProUGUI shieldTimeNumber;
	private PlayerUtility p2Utility;

	void Start() 
	{
		p2Utility = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerUtility>();

		shieldHealthHeader.enabled = false;
		shieldHealth.enabled = false;
		shieldTimeImage.enabled = false;
		shieldTimeNumber.enabled = false;
	}

	void Update() 
	{
		// Update text boxes with Player2 utility information.
		ammoNumber.text = p2Utility.ammoCount.ToString();
		grenadeNumber.text = p2Utility.grenadeCount.ToString();
		shieldNumber.text = p2Utility.shieldCount.ToString();
		shieldHealth.text = p2Utility.shieldHealth.ToString();
		int shieldTimeCount = (int)(10 - p2Utility.onTimer);
		shieldTimeNumber.text = shieldTimeCount.ToString();
		
		// Update ammo color indicator.
		if (p2Utility.ammoCount <= 3 && p2Utility.ammoCount > 0)
		{
			ammoNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p2Utility.ammoCount == 0)
		{
			ammoNumber.text = "E";
			ammoNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			ammoNumber.color = Color.white;
		}
		
		// Update grenade color indicator.
		if (p2Utility.grenadeCount <= 1 && p2Utility.grenadeCount > 0)
		{
			grenadeNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p2Utility.grenadeCount == 0)
		{
			grenadeNumber.text = "E";
			grenadeNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			grenadeNumber.color = Color.white;
		}

		// Update shield color indicator.
		if (p2Utility.shieldCount <= 2 && p2Utility.shieldCount > 0)
		{
			shieldNumber.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
		}
		else if (p2Utility.shieldCount == 0)
		{
			shieldNumber.text = "E";
			shieldNumber.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1.0f));
		}
		else
		{
			shieldNumber.color = Color.white;
		}

		// Display shield activated text.
		if (p2Utility.shieldOn)
		{
			shieldHealthHeader.enabled = true;
			shieldHealth.enabled = true;
			shieldTimeImage.enabled = true;
			shieldTimeNumber.enabled = true;

			shieldTimeImage.fillAmount = (10.0f - p2Utility.onTimer) / 10.0f;

			if (p2Utility.shieldHealth <= 10)
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
