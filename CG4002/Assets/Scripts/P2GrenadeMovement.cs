using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////
// -- P2GrenadeMovement -- 
// Enables Grenade movement & animation from PLAYER 1 to PLAYER 2.
//////////////////////////////////////////////////////////
public class P2GrenadeMovement : MonoBehaviour
{
    // Declarations for grenade movement and function.
    public GameObject target;
    public GameObject Grenade;
    public GameObject ARCamera;
    public GameObject explodeImage;
    public GameObject missTarget;
    public bool isEnabled = false;
    public bool isTargetFound = false;
    private bool isSent = false;
    private bool inFunction = false; // Stops updating isTargetFound if executing grenade throw/miss to allow full completion of action.
    private Vector3 targetPosition;
    private float journeyTime = 2.0f;
    private float currTime = 0.0f;
    private PlayerHealth player2Health;
    private Vector3 centerHit;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 centerMiss;
    private Vector3 missPos;

    // Declarations to send information back to MQTT broker.
    private mqttController mqttController;
    private mqttReceiver mqttReceiver;

    // Declaration for developer damage boolean.
    private MenuPopUp Menu;
    
    void Start()
    {
        player2Health = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerHealth>();
        mqttController = GameObject.FindGameObjectWithTag("MQTTController").GetComponent<mqttController>();
        mqttReceiver = GameObject.FindGameObjectWithTag("MQTTReceiver").GetComponent<mqttReceiver>();
        Menu = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuPopUp>();
        
        // Turn off Grenade asset.
        Grenade.SetActive(false);
        explodeImage.SetActive(false);
    }

    /**
    * run()
    * Start throwing grenade from Player1 to Player2.
    */  
    public void run()
    {
        // Turn on Grenade asset.
        Grenade.SetActive(true);
        Grenade.transform.position = ARCamera.transform.position;
        
        // Get points for hit arc calculation and grenade movement.
        centerHit = Grenade.transform.position + (target.transform.position - Grenade.transform.position)/2;
        centerHit += new Vector3(0, 200, 0); 
        startPos = Grenade.transform.position;
        endPos = target.transform.position;

        // Get points for miss arc calculation and grenade movement.
        missPos = missTarget.transform.position;
        centerMiss = Grenade.transform.position + (missPos - Grenade.transform.position)/2;
        centerMiss += new Vector3(0, 200, 0); 
        
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

            // Immediately send ONE message to MQTT broker that grenade from Player1 to Player2 will HIT.
            if (!isSent) 
            {
                if (Menu.developerDamage)
                {
                    player2Health.HitGrenadeP2(); // Reduce P2 health.
                }
                
                // if (mqttReceiver.isConnected)
                // {
                //     mqttController.p2GrenadeStatus(true);
                // }

                isSent = true;

                Debug.Log("MQTT sent for Player2 HIT by Player1 grenade!"); 
            }

            // Using Bezier Curves, calculate two vectors and interpolate an arced path between them.
            currTime += Time.deltaTime;
            Vector3 m1 = Vector3.Lerp( startPos, centerHit, currTime/journeyTime );
            Vector3 m2 = Vector3.Lerp( centerHit, target.transform.position, currTime/journeyTime );

            // Update grenade position over the interpolated arced path.
            Grenade.transform.position = Vector3.Lerp(m1, m2, currTime/journeyTime );

            // Rotate grenade to look like it is naturally being thrown in the air.
            Grenade.transform.Rotate(0, 0, Time.deltaTime * 900.0f);   

            Debug.Log("Hit Grenade Time Elapsed:" + currTime);
           
            // Check if the position of the Grenade and target are approximately equal.
            if (Vector3.Distance(Grenade.transform.position, target.transform.position) < 0.1f)
            {
                // Explode, reset and disable grenade from view.
                displayBoom();
                Grenade.transform.position = ARCamera.transform.position;
                Grenade.SetActive(false);
                isEnabled = false;
                currTime = 0.0f; 
                isSent = false;
                inFunction = false;

                Debug.Log("Player2 Hit & Impact by Player1 grenade!");
            }
        }

        // If run() and there is no target on screen, send miss to MQTT broker and throw non-exploding grenade to middle of screen.
        else if (isEnabled && !isTargetFound)
        {
            inFunction = true;

            // Immediately send ONE message to MQTT broker that grenade from Player1 to Player2 will MISS.
            if (!isSent) 
            {
                // if (mqttReceiver.isConnected)
                // {
                //     mqttController.p2GrenadeStatus(false);
                // }
                
                isSent = true; // Set to true to prevent message from being sent more than once.

                Debug.Log("MQTT sent for Player2 MISS by Player1 grenade!"); 
            }

            // Using Bezier Curves, calculate two vectors and interpolate an arced path between them.
            currTime += Time.deltaTime;
            Vector3 m1 = Vector3.Lerp( startPos, centerMiss, currTime/journeyTime );
            Vector3 m2 = Vector3.Lerp( centerMiss, missPos, currTime/journeyTime );
            
            // Update grenade position over the interpolated arced path.
            Grenade.transform.position = Vector3.Lerp(m1, m2, currTime/journeyTime );

            // Rotate grenade to look like it is naturally being thrown in the air.
            Grenade.transform.Rotate(0, 0, Time.deltaTime * 900.0f); 

            Debug.Log("Miss Grenade Time Elapsed:" + currTime);
           
            // Check if the position of the Grenade and target are approximately equal.
            if (Vector3.Distance(Grenade.transform.position, missPos) < 0.1f)
            {
                // Reset and disable grenade from view.
                Grenade.transform.position = ARCamera.transform.position;
                Grenade.SetActive(false);
                isEnabled = false;
                currTime = 0.0f; 
                isSent = false;
                inFunction = false;

                Debug.Log("Player2 Miss by Player1 grenade!");   
            }
        }
    }
}
