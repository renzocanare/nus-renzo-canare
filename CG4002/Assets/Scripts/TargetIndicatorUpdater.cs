using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////////////////////////
// -- TargetIndicatorUpdater --
// Controls display of target indicator.
//////////////////////////////////////////////////////////
public class TargetIndicatorUpdater : MonoBehaviour
{
    public GameObject TargetIndicator;
    public bool isTargetFound;

    void Start()
    {
        TargetIndicator.SetActive(false);
        isTargetFound = false;
    }

    /**
    * targetFound()
    * Set isTargetFound to TRUE if target is found.
    */  
    public void targetFound()
    {
        isTargetFound = true;
    }

    /**
    * targetLost()
    * Set isTargetFound to FALSE if target is lost.
    */  
    public void targetLost()
    {
        isTargetFound = false;
    }

    void Update()
    {
        if (isTargetFound)
        {
            TargetIndicator.SetActive(true);
        }
        else
        {
            TargetIndicator.SetActive(false);
        }
    }
}
