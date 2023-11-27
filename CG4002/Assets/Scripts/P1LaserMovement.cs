using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////
// -- P1LaserMovement -- 
// Enables laser movement & animation from PLAYER 2 to PLAYER 1.
//////////////////////////////////////////////////////////
public class P1LaserMovement : MonoBehaviour
{
    // Declarations for laser movement and function.
    public GameObject target;
    public GameObject Laser;
    public GameObject ARCamera;
    public bool isEnabled = false;
    public bool isTargetFound = false;
    private bool inFunction = false; // Stops updating isTargetFound if executing grenade throw/miss to allow full completion of action.
    private Vector3 targetPosition;
    private float journeyTime = 0.5f;
    private float currTime = 0.0f;
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        // Turn off Laser asset.
        Laser.SetActive(false);
    }

    /**
    * run()
    * Start shooting Laser from Player2 to Player1.
    */  
    public void run()
    {
        // Turn on Laser asset.
        Laser.SetActive(true);
        Laser.transform.position = target.transform.position;
        
        // Get points start and end.
        startPos = target.transform.position;
        endPos = ARCamera.transform.position;
        
        // Start moving.
        isEnabled = true;
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


    void Update()
    {
        // If run() and there is a target, move laser to target.
        if (isEnabled && isTargetFound)
        {
            inFunction = true;
            
            currTime += Time.deltaTime;

            // Update laser position over the path.
            Laser.transform.position = Vector3.Lerp(startPos, endPos, currTime/journeyTime );

            Debug.Log("Hit Shoot Time Elapsed:" + currTime);
           
            // Check if the position of the Laser and target are approximately equal.
            if (Vector3.Distance(Laser.transform.position, endPos) < 0.1f)
            {
                // Reset and disable laser from view.
                //displayBoom();
                Laser.transform.position = target.transform.position;
                Laser.SetActive(false);
                isEnabled = false;
                currTime = 0.0f; 
                inFunction = false;

                Debug.Log("Player2 Hit & Impact by Player1 laser!");
            }
        }

        // If run() and there is no target on screen, miss.
        else if (isEnabled && !isTargetFound)
        {
            inFunction = true;

            Laser.transform.position = target.transform.position;
            Laser.SetActive(false);
            isEnabled = false;
            currTime = 0.0f; 
            inFunction = false;

        }
    }
}
