using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- FlashText --
// Allows for text to be flashed on and off every 5 seconds.
//////////////////////////////////////////////////////////
public class FlashText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI creatorText;
    private bool textBlink = true;

    void Start()
    {
        StartCoroutine("Flash");        
    }

    /**
    * Flash()
    * Flashes text on and off every 5 seconds.
    */
    public IEnumerator Flash()
    {
        while (textBlink)
        {
            // IEnumerator's yield return allows for previous state to be "saved",
            // continuing from where it yielded.
            text.enabled = false;
            creatorText.text = "Sean the Builder";
            yield return new WaitForSeconds(.5f);
            text.enabled = true;
            creatorText.text = "Wira the AI God";
            yield return new WaitForSeconds(.5f);
            text.enabled = false;
            creatorText.text = "Xu Hui is Singaporean";
            yield return new WaitForSeconds(.5f);
            text.enabled = true;
            creatorText.text = "Still Rakesh's fault";
            yield return new WaitForSeconds(.5f);
            text.enabled = false;
            creatorText.text = "Maybe Renzo can sleep";
            yield return new WaitForSeconds(.5f);
            text.enabled = true;
        }
    }
}

