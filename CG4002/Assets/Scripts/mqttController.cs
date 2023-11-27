using System.Collections;
using System.Collections.Generic;
//using System.Text.Json;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////
// -- mqttController -- 
// Parses inputs from MQTT broker, updates gamestates
// and executes actions (shoot, shield, grenade).
//////////////////////////////////////////////////////////
public class mqttController : MonoBehaviour
{
    public string nameController = "mqttController";
    public mqttReceiver _eventSender;
    
    public int MainPlayer = 0; // 1 for Player 1, 2 for Player 2.

    public Image crosshair;

    public Player player;
    
    public int p1_HP;
    public string p1_action;
    public bool p1_util_empty;
    public int p1_bullets;
    public int p1_grenades;
    public int p1_shield_time;
    public int p1_shield_health;
    public int p1_num_deaths;
    public int p1_num_shield;

    public int p2_HP;
    public string p2_action;
    public bool p2_util_empty;
    public int p2_bullets;
    public int p2_grenades;
    public int p2_shield_time;
    public int p2_shield_health;
    public int p2_num_deaths;
    public int p2_num_shield;

    private PlayerHealth p1Health;
    private PlayerUtility p1Utility;
    private P1GrenadeMovement p1Grenade;
    private P1LaserMovement p1Shoot;

    private PlayerHealth p2Health;
    private PlayerUtility p2Utility;
    private P2GrenadeMovement p2Grenade;
    private P2LaserMovement p2Shoot;

    private MenuPopUp Menu;

    /**
    * -- START CLASS DECLARATION FOR JSON PARSING --
    * Structure: JSON message is a singular object containing two main properties,
    * where each property represents two different players with their own properties.
    * Hence, two different classes are required to contain the two main properties.
    */ 
    [System.Serializable]
    public class P1
    {
        [SerializeField]
        public int hp;

        [SerializeField]
        public string action;

        [SerializeField]
        public bool util_empty;

        [SerializeField]
        public int bullets;

        [SerializeField]
        public int grenades;

        [SerializeField]
        public int shield_time;

        [SerializeField]
        public int shield_health;

        [SerializeField]
        public int num_deaths;

        [SerializeField]
        public int num_shield;     

        public P1(int hp, string action, bool util_empty, 
                int bullets, int grenades, int shield_time, 
                int shield_health, int num_deaths, int num_shield)
        {
            this.hp = hp;
            this.action = action;
            this.util_empty = util_empty;
            this.bullets = bullets;
            this.grenades = grenades;
            this.shield_time = shield_time;
            this.shield_health = shield_health;
            this.num_deaths = num_deaths;
            this.num_shield = num_shield;
        }
    }

    [System.Serializable]
    public class P2
    {
        [SerializeField]
        public int hp;

        [SerializeField]
        public string action;

        [SerializeField]
        public bool util_empty;

        [SerializeField]
        public int bullets;

        [SerializeField]
        public int grenades;

        [SerializeField]
        public int shield_time;

        [SerializeField]
        public int shield_health;

        [SerializeField]
        public int num_deaths;

        [SerializeField]
        public int num_shield;         

        public P2(int hp, string action, bool util_empty, 
                int bullets, int grenades, int shield_time, 
                int shield_health, int num_deaths, int num_shield)
        {
            this.hp = hp;
            this.action = action;
            this.util_empty = util_empty;
            this.bullets = bullets;            
            this.grenades = grenades;
            this.shield_time = shield_time;
            this.shield_health = shield_health;
            this.num_deaths = num_deaths;
            this.num_shield = num_shield;
        }
    }

    [System.Serializable]
    public class Player
    {
        [SerializeField]
        public P1 p1;

        [SerializeField]
        public P2 p2;

        public Player(P1 p1, P2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    /**
    * -- END CLASS DECLARATION FOR JSON PARSING -- 
    */

    void Start()
    {
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;

        p1Health = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerHealth>();
        p1Utility = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerUtility>();
        p1Grenade = GameObject.FindGameObjectWithTag("Player1").GetComponent<P1GrenadeMovement>();
        p1Shoot = GameObject.FindGameObjectWithTag("Player1").GetComponent<P1LaserMovement>();

        p2Health = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerHealth>();
        p2Utility = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerUtility>();        
        p2Grenade = GameObject.FindGameObjectWithTag("Player2").GetComponent<P2GrenadeMovement>();
        p2Shoot = GameObject.FindGameObjectWithTag("Player2").GetComponent<P2LaserMovement>();

        Menu = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuPopUp>();
    }

    /**
    * OnMessageArrivedHandler(string)
    * Parses JSON message from MQTT broker, updates
    * game state and execute player actions.
    *
    * NOTE: Actions will not execute again if there is no change in the JSON message.     
    */    
    private void OnMessageArrivedHandler(string newMsg)
    {
        // Parse JSON message and store in class Player.P1/P2.
        // JsonUtility.FromJson() is built-in to Unity and uses the Unity Serializer.
        player = JsonUtility.FromJson<Player>(newMsg);
        
        // Update game state.
        updateP1Local();
        updateP2Local();
        updateGameState();
        runShoot();
        runShield();
        runGrenade();
        runLogout(); // Always logout, especially for corrected state.

        Debug.Log("Message confirmed received. The message to controller, " + nameController + ", is = " + newMsg);
    }

    /**
    * updateP1Local()
    * Store Player 1 JSON state into mqttController variables.
    */  
    private void updateP1Local()
    {
        p1_HP = player.p1.hp;
        p1_action = player.p1.action;
        p1_util_empty = player.p1.util_empty;
        p1_bullets = player.p1.bullets;
        p1_grenades = player.p1.grenades;
        p1_shield_time = player.p1.shield_time;
        p1_shield_health = player.p1.shield_health;
        p1_num_deaths = player.p1.num_deaths;
        p1_num_shield = player.p1.num_shield;
    }

    /**
    * updateP2Local()
    * Store Player 2 JSON state into mqttController variables.
    */  
    private void updateP2Local()
    {       
        p2_HP = player.p2.hp;
        p2_action = player.p2.action;
        p2_util_empty = player.p2.util_empty;
        p2_bullets = player.p2.bullets;
        p2_grenades = player.p2.grenades;
        p2_shield_time = player.p2.shield_time;
        p2_shield_health = player.p2.shield_health;
        p2_num_deaths = player.p2.num_deaths;
        p2_num_shield = player.p2.num_shield;
    }

    /**
    * runShoot()
    * Player shoots at opponent indicated in JSON message.
    */  
    private void runShoot()
    {
        if (MainPlayer == 1)
        {
            // Player 1 shoots Player 2.
            if (string.Equals(p1_action, "shoot"))
            {
                if (!p1_util_empty)
                {
                    p2Shoot.run();
                }
            }

            // Player 2 shoots Player 1.
            if (string.Equals(p2_action, "shoot"))
            {
                if (!p2_util_empty)
                {                
                    p1Shoot.run();
                }
            }
        }
        if (MainPlayer == 2)
        {
            // Player 1 shoots Player 2.
            if (string.Equals(p1_action, "shoot"))
            {
                if (!p1_util_empty)
                {
                    p1Shoot.run();
                }
            }
            // Player 2 shoots Player 1.
            if (string.Equals(p2_action, "shoot"))
            {
                if (!p2_util_empty)
                {                
                    p2Shoot.run();
                }
            }
        }        

    }

    /**
    * runShield()
    * Turn on player's shield indicated in JSON message.
    */  
    private void runShield()
    {
        if (MainPlayer == 1)
        {
            if (string.Equals(p1_action, "shield"))
            {
                // Turn on Player 1 shield if have shield ammo.
                if (p1Utility.shieldWait == false && !p1_util_empty)
                {
                    p1Utility.shieldOn = true;             
                }
            }

            if (string.Equals(p2_action, "shield"))
            {
                // Turn on Player 1 shield if have shield ammo.
                if (p2Utility.shieldWait == false && !p2_util_empty)
                {
                    p2Utility.shieldOn = true;            
                }
            }      
        }
        else if (MainPlayer == 2)
        {
            if (string.Equals(p1_action, "shield"))
            {
                // Turn on Player 1 shield if have shield ammo.
                if (p1Utility.shieldWait == false && !p1_util_empty)
                {
                    p1Utility.shieldOn = true;            
                }
            }

            if (string.Equals(p2_action, "shield"))
            {
                // Turn on Player 1 shield if have shield ammo.
                if (p2Utility.shieldWait == false && !p2_util_empty)
                {
                    p2Utility.shieldOn = true;           
                }
            }                
        }
 
    }

    /**
    * runGrenade()
    * Throw player's grenade at opponent indicated in JSON message.
    */  
    private void runGrenade()
    {
        if (MainPlayer == 1)
        {
            // Throw Player 1 grenade towards Player 2.
            if (string.Equals(p1_action, "grenade"))
            {
                if (!p1_util_empty)
                {
                    p2Grenade.run();
                }
            }

            // Throw Player 2 grenade towards Player 1.
            if (string.Equals(p2_action, "grenade"))
            {
                if (!p2_util_empty)
                {
                    p1Grenade.run();
                }
            }   
        }
        else if (MainPlayer == 2)
        {
            // Throw Player 1 grenade towards Player 2.
            if (string.Equals(p1_action, "grenade"))
            {
                if (!p1_util_empty)
                {
                    p1Grenade.run();
                }       
            }
 
            // Throw Player 2 grenade towards Player 1.
            if (string.Equals(p2_action, "grenade"))
            {
                if (!p2_util_empty)
                {
                    p2Grenade.run();
                }               
            }   
        }
    }


    /**
    * runLogout()
    * Turn on end game screen.
    */  
    private void runLogout()
    {
        if (string.Equals(p1_action, "logout") || string.Equals(p2_action, "logout"))
        {
            Menu.endGame();
        }
    }

    /**
    * updateGameState()
    * Update both players' game state.
    */  
    private void updateGameState()
    {
        p1Health.health = p1_HP;
        p1Utility.ammoCount = p1_bullets;
        p1Utility.grenadeCount = p1_grenades;
        p1Utility.onTimer = 10 - p1_shield_time; // Inversed as shield in PlayerUtility counts up.
        p1Utility.shieldHealth = p1_shield_health;
        p1Health.deathCount = p1_num_deaths;
        p1Utility.shieldCount = p1_num_shield;

        p2Health.health = p2_HP;
        p2Utility.ammoCount = p2_bullets;
        p2Utility.grenadeCount = p2_grenades;
        p2Utility.onTimer = 10 - p2_shield_time; // Inversed as shield in PlayerUtility counts up.
        p2Utility.shieldHealth = p2_shield_health;
        p2Health.deathCount = p2_num_deaths;
        p2Utility.shieldCount = p2_num_shield;
    }

    /**
    * p1GrenadeStatus()
    * Sends message back to MQTT broker to update if Player 2's grenade 
    * has hit or missed Player 1.
    */  
    public void p1GrenadeStatus(bool hit)
    {
        if (hit)
        {
            _eventSender.CustomPublish("P2toP1_grenade_hit");
        }
        else
        {
            _eventSender.CustomPublish("P2toP1_grenade_miss");
        }
    }

    /**
    * p2GrenadeStatus()
    * Sends message back to MQTT broker to update if Player 1's grenade 
    * has hit or missed Player 2.
    */  
    public void p2GrenadeStatus(bool hit)
    {
        if (hit)
        {
            _eventSender.CustomPublish("P1toP2_grenade_hit");
        }
        else
        {
            _eventSender.CustomPublish("P1toP2_grenade_miss");
        }
    }

    /**
    * playerOnScreen()
    * Sends message back to MQTT broker to update if Opponent is on screen.
    */  
    public void playerOnScreen()
    {
        if (MainPlayer == 1)
        {
            _eventSender.CustomPublish("P2_on_P1screen");
        }
        else if (MainPlayer == 2)
        {
            _eventSender.CustomPublish("P1_on_P2screen");
        }

        //crosshair.enabled = true;
    }

    /**
    * playerOffScreen()
    * Sends message back to MQTT broker to update if Opponent is off screen.
    */  
    public void playerOffScreen()
    {
        if (MainPlayer == 1)
        {
            _eventSender.CustomPublish("P2_off_P1screen");
        }
        else if (MainPlayer == 2)
        {
            _eventSender.CustomPublish("P1_off_P2screen");
        }

        //crosshair.enabled = false;
    }

}

