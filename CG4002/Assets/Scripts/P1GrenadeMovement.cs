using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////
// -- P1GrenadeMovement -- 
// Enables Grenade movement & animation from PLAYER 2 to PLAYER 1.
// NOTE: No action from Player 2 if cannot be seen on visualizer.
//////////////////////////////////////////////////////////
public class P1GrenadeMovement : MonoBehaviour
{
    // Declarations for grenade movement and function.
    public GameObject target;
    public GameObject Grenade;
    public GameObject ARCamera;
    public GameObject explodeImage;
    public bool isEnabled = false;
    public bool isTargetFound = false;
    private bool isSent = false;
    private bool inFunction = false; // Stops updating isTargetFound if executing grenade throw/miss to allow full completion of action.
    private Vector3 targetPosition;
    private float journeyTime = 2.0f;
    private float currTime = 0.0f;
    private PlayerHealth player1Health;
    private Vector3 center;
    private Vector3 startPos;
    private Vector3 endPos;

    // Declarations to send information back to MQTT broker.
    private mqttController mqttController;
    private mqttReceiver mqttReceiver;

    // Declaration for developer damage boolean.
    private MenuPopUp Menu;

    void Start()
    {
        player1Health = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerHealth>();
        mqttController = GameObject.FindGameObjectWithTag("MQTTController").GetComponent<mqttController>();
        mqttReceiver = GameObject.FindGameObjectWithTag("MQTTReceiver").GetComponent<mqttReceiver>();
        Menu = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuPopUp>();

        // Turn off Grenade asset.
        Grenade.SetActive(false);
        explodeImage.SetActive(false);
    }

    /**
    * run()
    * Start throwing grenade from Player2 to Player1.
    */  
    public void run()
    {
        // Turn on Grenade asset.
        Grenade.SetActive(true);
        Grenade.transform.position = target.transform.position;
        
        // Get points for arc calculation and grenade movement.
        center = ARCamera.transform.position + (target.transform.position - ARCamera.transform.position)/2;
        center += new Vector3(0, 200, 0); 
        startPos = Grenade.transform.position;
        endPos = ARCamera.transform.position;
        
        // Start moving.
        isEnabled = true;   
        isSent = false;
        inFunction = false;
    }

    /**
    * targetFound()
    * Set isTargetFound to TRUE if target is found.
    */  
    public void targetFound()
    {
        if (!inFunction) {
            isTargetFound = true;
        }
    }

    /**
    * targetLost()
    * Set isTargetFound to FALSE if target is lost.
    */  
    public void targetLost()
    {
        if (!inFunction) {
            isTargetFound = false;
        }
    }

    /**
    * displayBoom()
    * Displays explosion animation on grenade impact.
    */ 
    public void displayBoom()
    {
        explodeImage.SetActive(true);

        // Invoke disableBoom() after 2 seconds.
        Invoke ("disableBoom", 2.0f);
    }

    /**
    * disableBoom()
    * Turns off explosion animation.
    */ 
    public void disableBoom()
    {
        explodeImage.SetActive(false);
    }

    void Update()
    {
        // If run() and there is a target, throw grenade to target.
        if (isEnabled && isTargetFound)
        {
            inFunction = true;

            // Immediately send ONE message to MQTT broker that grenade from Player2 to Player1 will HIT.
            if (!isSent)
            {
                if (Menu.developerDamage)
                {
                    player1Health.HitGrenadeP1(); // Reduce P1 health (to be removed later, used currently for dev testing).
                }
                
                // if (mqttReceiver.isConnected)
                // {
                //     mqttController.p1GrenadeStatus(true);
                // }
                
                isSent = true; 

                Debug.Log("MQTT sent for Player1 HIT by Player2 grenade!");  
            }

            // Using Bezier Curves, calculate two vectors and interpolate an arced path between them.
            currTime += Time.deltaTime;
            Vector3 m1 = Vector3.Lerp( startPos, center, currTime/journeyTime );
            Vector3 m2 = Vector3.Lerp( center, ARCamera.transform.position, currTime/journeyTime );

            // Update grenade position over the interpolated arced path.
            Grenade.transform.position = Vector3.Lerp(m1, m2, currTime/journeyTime );

            // Rotate grenade to look like it is naturally being thrown in the air.
            Grenade.transform.Rotate(0, 0, Time.deltaTime * 900.0f);

            Debug.Log("Grenade Time Elapsed:" + currTime);
           
            // Check if the position of the Grenade and target are approximately equal.
            if (Vector3.Distance(Grenade.transform.position, ARCamera.transform.position) < 0.1f)
            {
                // Explode, reset and disable grenade from view.
                displayBoom();
                Grenade.transform.position = target.transform.position;
                Grenade.SetActive(false);
                isEnabled = false;
                currTime = 0.0f;
                isSent = false;
                inFunction = false;

                Debug.Log("Player1 Hit & Impact by Player2 grenade!");  
            }
        }

        // If run() and there is no target on screen, send miss to MQTT broker.
        else if (isEnabled && !isTargetFound)
        {
            inFunction = true;

            // Immediately send ONE message to MQTT broker that grenade from Player2 to Player1 will MISS.
            if (!isSent)
            {
                // if (mqttReceiver.isConnected)
                // {
                //     mqttController.p1GrenadeStatus(false);
                // }
                
                isSent = true; // Set to true to prevent message from being sent more than once.

                Debug.Log("MQTT sent for Player1 MISS by Player2 grenade!"); 
            }

            // Reset grenade and disable from view.
            Grenade.transform.position = target.transform.position;
            Grenade.SetActive(false);
            isEnabled = false;
            inFunction = false;
            isSent = false;
            
            Debug.Log("Player1 Miss by Player2 grenade!");    
        }
    }
}
