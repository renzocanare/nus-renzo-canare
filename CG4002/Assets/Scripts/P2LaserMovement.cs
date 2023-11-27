using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////
// -- P2LaserMovement -- 
// Enables laser movement & animation from PLAYER 1 to PLAYER 2.
//////////////////////////////////////////////////////////
public class P2LaserMovement : MonoBehaviour
{
    // Declarations for laser movement and function.
    public GameObject target;
    public GameObject Laser;
    public GameObject ARCamera;
    //public GameObject explodeImage;
    public GameObject missTarget;
    public bool isEnabled = false;
    public bool isTargetFound = false;
    private bool inFunction = false; // Stops updating isTargetFound if executing grenade throw/miss to allow full completion of action.
    private Vector3 targetPosition;
    private float journeyTime = 0.5f;
    private float currTime = 0.0f;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 missPos;

    void Start()
    {
        // Turn off Laser asset.
        Laser.SetActive(false);
        //explodeImage.SetActive(false);
    }

    /**
    * run()
    * Start shooting Laser from Player1 to Player2.
    */  
    public void run()
    {
        // Turn on Laser asset.
        Laser.SetActive(true);
        Laser.transform.position = ARCamera.transform.position;
        
        // Get points start and end.
        startPos = Laser.transform.position;
        endPos = target.transform.position;
        
        // Get points for miss arc calculation and grenade movement.
        missPos = missTarget.transform.position;

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
                Laser.transform.position = ARCamera.transform.position;
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

            currTime += Time.deltaTime;

            // Update laser position over the path.
            Laser.transform.position = Vector3.Lerp(startPos, missPos, currTime/journeyTime );
            
            // Check if the position of the Laser and target are approximately equal.
            if (Vector3.Distance(Laser.transform.position, missPos) < 0.1f)
            {
                // Reset and disable laser from view.
                //displayBoom();
                Laser.transform.position = ARCamera.transform.position;
                Laser.SetActive(false);
                isEnabled = false;
                currTime = 0.0f; 
                inFunction = false;

                Debug.Log("Player2 miss by Player1 laser!");
            }

        }
    }
}
