using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;
using TMPro; 

//////////////////////////////////////////////////////////
// -- MenuPopUp --
// Controls pop-up menu for MQTT settings and developer options.
//////////////////////////////////////////////////////////
public class MenuPopUp: MonoBehaviour {

    public GameObject Menu;
    public GameObject EndGame;
    public mqttReceiver mqttReceiver;
    public Toggle autoReconnect; // Toggle for force auto reconnect button.
    public Toggle devDamage; // Toggle for developer damage.
    public Toggle bluetoothMenu; // Toggle for bluetooth menu.
    public TextMeshProUGUI statusText; // MQTT status information.
    public bool developerDamage = false;
    public bool bluetoothMenuOn = true;

    // For turning off DevButtons.
    public GameObject DevButton1;
    bool devButton1Status;
    public GameObject DevButton2;
    bool devButton2Status;
    public GameObject DevButton3;
    bool devButton3Status;
    public GameObject DevButton4;
    bool devButton4Status;
    public GameObject DevButton5;
    bool devButton5Status;
    public GameObject DevButton6;
    bool devButton6Status;

    void Start()
    {
        Menu.SetActive(false);
        EndGame.SetActive(false);

        // Get status of developer buttons.
        devButton1Status = DevButton1.activeSelf;
        devButton2Status = DevButton2.activeSelf;
        devButton3Status = DevButton3.activeSelf;
        devButton4Status = DevButton4.activeSelf;
        devButton5Status = DevButton5.activeSelf;
        devButton6Status = DevButton6.activeSelf;
    }

    void Update()
    {
        // Set force auto reconnect via toggle button.
        if (autoReconnect.isOn)
        {
            mqttReceiver.reconnectAutomatically = true;
        }
        else
        {
            mqttReceiver.reconnectAutomatically = false;
        }

        // Set developer damage via toggle button.
        if (devDamage.isOn)
        {
            developerDamage = true;
        }
        else
        {
            developerDamage = false;
        }        

        // Set bluetooth menu via toggle button.
        if (bluetoothMenu.isOn)
        {
            bluetoothMenuOn = true;
        }
        else
        {
            bluetoothMenuOn = false;
        }      

        // Set text and color of MQTT status information.
        if (mqttReceiver.isConnected)
        {
            statusText.text = "Connected";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "Disconnected";
            statusText.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));           
        }
    }

    /**
    * OpenMenu()
    * Opens the pop-up menu.
    */
    public void OpenMenu() 
    {  
        Menu.SetActive(true);
    }

    /**
    * CloseMenu()
    * Closes the pop-up menu.
    */
    public void CloseMenu()
    {
        Menu.SetActive(false);
    }  

    /**
    * endGame()
    * Opens the endgame menu.
    */
    public void endGame()
    {
        EndGame.SetActive(true);
    }

    /**
    * ToggleDevButtons()
    * Shows/hides action (shoot, grenade, shield) buttons on player screen.
    */
    public void ToggleDevButtons()
    {
        devButton1Status = !devButton1Status;
        DevButton1.SetActive(devButton1Status);

        devButton2Status = !devButton2Status;
        DevButton2.SetActive(devButton2Status);

        devButton3Status = !devButton3Status;
        DevButton3.SetActive(devButton3Status);

        devButton4Status = !devButton4Status;
        DevButton4.SetActive(devButton4Status);

        devButton5Status = !devButton5Status;
        DevButton5.SetActive(devButton5Status);

        devButton6Status = !devButton6Status;
        DevButton6.SetActive(devButton6Status);
    }
}   