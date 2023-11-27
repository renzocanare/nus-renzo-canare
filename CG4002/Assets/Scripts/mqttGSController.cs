using System.Collections;
using System.Collections.Generic;
//using System.Text.Json;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- mqttGSController -- 
// Get input from GS Topic and update bluetooth
// information.
//////////////////////////////////////////////////////////
public class mqttGSController : MonoBehaviour
{
    public string nameController = "mqttGSController";
    public mqttReceiver _eventSender;

    public Status msg; 

    public bool p1_bt_turn;
    public string p1_bt_action;
    public bool p2_bt_turn;
    public string p2_bt_action;

	public Image checkMarkNear;
	public Image checkMarkFar;
    public TextMeshProUGUI gameStatusText;

    private string statusText = "No Action";
    private mqttController controller;

    [System.Serializable]
    public class Status
    {
        [SerializeField]
        public bool p1_turn_complete;
        
        [SerializeField]
        public string p1_action;
        
        [SerializeField]
        public bool p2_turn_complete;
        
        [SerializeField]
        public string p2_action;
        

        public Status(bool p1_turn_complete, string p1_action, bool p2_turn_complete,
                    string p2_action)
        {
            this.p1_turn_complete = p1_turn_complete;
            this.p1_action = p1_action;
            this.p2_turn_complete = p2_turn_complete;
            this.p2_action = p2_action;
  
        }
    }

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("MQTTController").GetComponent<mqttController>();
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
        checkMarkNear.enabled = false;
		checkMarkFar.enabled = false;
        gameStatusText.enabled = true;
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
        msg = JsonUtility.FromJson<Status>(newMsg);
        
        updateLocal();
        updateStatusText();
        updateCheckMark();
        
        Debug.Log("Message confirmed received. The message to controller, " + nameController + ", is = " + newMsg);
    }

    private void updateLocal()
    {
        p1_bt_turn = msg.p1_turn_complete;
        p1_bt_action = msg.p1_action;
        p2_bt_turn = msg.p2_turn_complete;
        p2_bt_action = msg.p2_action;

    }

    public void updateStatusText()
    {
        if (controller.MainPlayer == 1)
        {
            if (string.Equals(p1_bt_action, "shoot"))
            {
                statusText = "P1 Shoot";
            }
            else if (string.Equals(p1_bt_action, "shield"))
            {
                statusText = "P1 Shield";
            }
            else if (string.Equals(p1_bt_action, "grenade"))
            {
                statusText = "P1 Grenade";
            }        
            else if (string.Equals(p1_bt_action, "reload"))
            {
                statusText = "P1 Reload";
            }
            // else if (string.Equals(p1_bt_action, "logout"))
            // {
            //     gameStatusText.enabled = false;
            // }
        }
        else if (controller.MainPlayer == 2)
        {
            if (string.Equals(p2_bt_action, "shoot"))
            {
                statusText = "P2 Shoot";
            }
            else if (string.Equals(p2_bt_action, "shield"))
            {
                statusText = "P2 Shield";
            }
            else if (string.Equals(p2_bt_action, "grenade"))
            {
                statusText = "P2 Grenade";
            }        
            else if (string.Equals(p2_bt_action, "reload"))
            {
                statusText = "P2 Reload";
            }
            // else if (string.Equals(p2_bt_action, "logout"))
            // {
            //     gameStatusText.enabled = false;
            // }
        }   
    }
    
    private void disable()
    {
        checkMarkFar.enabled = false;
        checkMarkNear.enabled = false;
    }

    private void updateCheckMark()
    {
        if (controller.MainPlayer == 1)
        {
            if (p1_bt_turn)
            {
                checkMarkNear.enabled = true;
            }
            else
            {
                checkMarkNear.enabled = false;
            }

            if (p2_bt_turn)
            {
                checkMarkFar.enabled = true;
            }
            else
            {
                checkMarkFar.enabled = false;
            }            
        }
        else if (controller.MainPlayer == 2)
        {
            if (p1_bt_turn)
            {
                checkMarkFar.enabled = true;
            }
            else
            {
                checkMarkFar.enabled = false;
            }

            if (p2_bt_turn)
            {
                checkMarkNear.enabled = true;
            }
            else
            {
                checkMarkNear.enabled = false;
            }            
        }

        if (p1_bt_turn && p2_bt_turn)
        {
            Invoke("disable", 1.0f);
        }
	} 

    void Update() 
	{
		gameStatusText.text = statusText;
    }
}

