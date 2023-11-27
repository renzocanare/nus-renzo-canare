using System.Collections;
using System.Collections.Generic;
//using System.Text.Json;
using Newtonsoft.Json;
using UnityEngine;

//////////////////////////////////////////////////////////
// -- mqttBTController -- 
// Get input from Bluetooth Topic and update bluetooth
// information.
//////////////////////////////////////////////////////////
public class mqttBTController : MonoBehaviour
{
    public string nameController = "mqttBTController";
    public mqttReceiver _eventSender;

    public Status msg; 
    
    public bool p1_bt_vest = false;
    public bool p1_bt_IMU = false;
    public bool p1_bt_gun = false;
    public bool p2_bt_vest = false;
    public bool p2_bt_IMU = false;
    public bool p2_bt_gun = false;

    [System.Serializable]
    public class Status
    {
        [SerializeField]
        public bool p1_vest_on;
        
        [SerializeField]
        public bool p1_IMU_on;
        
        [SerializeField]
        public bool p1_gun_on;
        
        [SerializeField]
        public bool p2_vest_on;
        
        [SerializeField]
        public bool p2_IMU_on;
        
        [SerializeField]
        public bool p2_gun_on;

        public Status(bool p1_vest_on, bool p1_IMU_on, bool p1_gun_on,
                    bool p2_vest_on, bool p2_IMU_on, bool p2_gun_on)
        {
            this.p1_vest_on = p1_vest_on;
            this.p1_IMU_on = p1_IMU_on;
            this.p1_gun_on = p1_gun_on;
            this.p2_vest_on = p2_vest_on;
            this.p2_IMU_on = p2_IMU_on;
            this.p2_gun_on = p2_gun_on;
        }
    }

    void Start()
    {
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
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
        
        Debug.Log("Message confirmed received. The message to controller, " + nameController + ", is = " + newMsg);
    }

    private void updateLocal()
    {
        p1_bt_vest = msg.p1_vest_on;
        p1_bt_IMU = msg.p1_IMU_on;
        p1_bt_gun = msg.p1_gun_on;
        p2_bt_vest = msg.p2_vest_on;
        p2_bt_IMU = msg.p2_IMU_on;
        p2_bt_gun = msg.p2_gun_on;
    }
}

