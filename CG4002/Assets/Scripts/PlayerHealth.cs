using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////////////////////////
// -- PlayerHealth -- 
// LOCAL GAME ENGINE: Stores & player health on shoot/grenade/shield actions.
//
// NOTE: LOCAL GAME ENGINE methods are for developer testing only.
//////////////////////////////////////////////////////////
public class PlayerHealth : MonoBehaviour 
{
	public int health; // To keep after full MQTT implementation.
	public int deathCount; // To keep after full MQTT implementation.
	bool p1ShieldOn;
	bool p2ShieldOn;
	private PlayerUtility Player1Utility;
	private PlayerUtility Player2Utility;
	private P1GrenadeMovement Player1Grenade;
	private P2GrenadeMovement Player2Grenade;
	private P1LaserMovement Player1Laser;
	private P2LaserMovement Player2Laser;

	void Start()
	{
		Player1Utility = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerUtility>();
		Player1Grenade = GameObject.FindGameObjectWithTag("Player1").GetComponent<P1GrenadeMovement>();
		Player1Laser = GameObject.FindGameObjectWithTag("Player1").GetComponent<P1LaserMovement>();

		Player2Utility = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerUtility>();
		Player2Grenade = GameObject.FindGameObjectWithTag("Player2").GetComponent<P2GrenadeMovement>();
		Player2Laser = GameObject.FindGameObjectWithTag("Player2").GetComponent<P2LaserMovement>();
	}

	void Update()
	{
		p1ShieldOn = Player1Utility.shieldOn;
		p2ShieldOn = Player2Utility.shieldOn;
	}

    /**
    * ShootP1() - LOCAL GAME ENGINE
    * Player2 shoots Player1.
    */  
	public void ShootP1()
	{
		// Decrease Player2 ammo.
		Player2Utility.Shoot();

		Player1Laser.run();

		// Update Player1 health.
		if (p1ShieldOn)
		{
			Player1Utility.shieldHealth -= 10;
		}
		else 
		{
			if (health > 0)
			{
			health -= 10;
			Debug.Log("Health = " + health.ToString());
			}

			if (health <= 0)
			{
				health = 100;
				deathCount++;
			}
		}
	}

    /**
    * ShootP2() - LOCAL GAME ENGINE
    * Player1 shoots Player2.
    */  
	public void ShootP2()
	{
		// Decrease Player1 ammo.
		Player1Utility.Shoot();

		Player2Laser.run();

		// Update Player2 health.
		if (p2ShieldOn)
		{
			Player2Utility.shieldHealth -= 10;
		}
		else 
		{
			if (health > 0)
			{
			health -= 10;
			Debug.Log("Health = " + health.ToString());
			}

			if (health <= 0)
			{
				health = 100;
				deathCount++;
			}
		}
	}

    /**
    * ThrowGrenadeP1() - LOCAL GAME ENGINE
    * Player2 throws a grenade at Player1.
    */  
	public void ThrowGrenadeP1()
	{
		if (Player2Utility.grenadeCount > 0 && !Player1Grenade.isEnabled) 
		{
			// Decrease Player2 grenade.
			Player2Utility.Grenade();

			// Throw grenade at Player1.
			Player1Grenade.run();
		}

	}

    /**
    * HitGrenadeP1() - LOCAL GAME ENGINE
    * Updates Player1 health if Player2's grenade hits.
    */  
	public void HitGrenadeP1()
	{
		int excessDamage = 0;

		if (p1ShieldOn)
		{
			if (Player1Utility.shieldHealth >= 20)
			{
				Player1Utility.shieldHealth -= 20;
			}
			else if (Player1Utility.shieldHealth < 20)
			{
				excessDamage = 20 - Player1Utility.shieldHealth;
				Player1Utility.shieldHealth -= Player1Utility.shieldHealth;
				health -= excessDamage;

				if (health <= 0)
				{
					health = 100;
					deathCount++;
				}				
			}
		}
		else
		{
			if (health > 0)
			{
			health -= 20;
			Debug.Log("Health = " + health.ToString());
			}

			if (health <= 0)
			{
				health = 100;
				deathCount++;
			}			
		}
	}	

    /**
    * ThrowGrenadeP2() - LOCAL GAME ENGINE
    * Player1 throws a grenade at Player2.
    */  
	public void ThrowGrenadeP2()
	{
		if (Player1Utility.grenadeCount > 0 && !Player2Grenade.isEnabled) 
		{
			// Decrease Player1 grenade.
			Player1Utility.Grenade();

			// Throw grenade at Player2.
			Player2Grenade.run();
		}

	}

    /**
    * HitGrenadeP2() - LOCAL GAME ENGINE
    * Updates Player2 health if Player1's grenade hits.
    */  
	public void HitGrenadeP2()
	{
		int excessDamage = 0;

		if (p2ShieldOn)
		{
			if (Player2Utility.shieldHealth >= 20)
			{
				Player2Utility.shieldHealth -= 20;
			}
			else if (Player2Utility.shieldHealth < 20)
			{
				excessDamage = 20 - Player2Utility.shieldHealth;
				Player2Utility.shieldHealth -= Player2Utility.shieldHealth;
				health -= excessDamage;

				if (health <= 0)
				{
					health = 100;
					deathCount++;
				}				
			}
		}
		else
		{
			if (health > 0)
			{
			health -= 20;
			Debug.Log("Health = " + health.ToString());
			}

			if (health <= 0)
			{
				health = 100;
				deathCount++;
			}			
		}
	}	
}
