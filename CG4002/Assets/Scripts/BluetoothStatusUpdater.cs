using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BluetoothStatusUpdater : MonoBehaviour
{
    private mqttBTController controller;
    private MenuPopUp menu;
    public GameObject BluetoothMenu;
    public TextMeshProUGUI p1_gun_text;
    public TextMeshProUGUI p1_IMU_text;
    public TextMeshProUGUI p1_vest_text;
    public TextMeshProUGUI p2_gun_text;
    public TextMeshProUGUI p2_IMU_text;
    public TextMeshProUGUI p2_vest_text;
    public bool isBluetoothMenuOn = true;
    private bool isAnyOff;


    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("MQTTController2").GetComponent<mqttBTController>();
        menu = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuPopUp>();
    }


    void Update()
    {
        isBluetoothMenuOn = menu.bluetoothMenuOn;

        isAnyOff = !controller.p1_bt_gun || !controller.p1_bt_IMU || !controller.p1_bt_vest ||
                        !controller.p2_bt_gun || !controller.p2_bt_IMU || !controller.p2_bt_vest;
        
        if (isAnyOff && isBluetoothMenuOn)
        {
            BluetoothMenu.SetActive(true);

            // Player 1 Bluetooth connectivity.
            if (!controller.p1_bt_gun)
            {
                p1_gun_text.text = "P1 Gun Disconnected";
                p1_gun_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p1_gun_text.text = "P1 Gun Connected";
                p1_gun_text.color = Color.green;
            }
            
            if (!controller.p1_bt_IMU)
            {
                p1_IMU_text.text = "P1 IMU Disconnected";
                p1_IMU_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p1_IMU_text.text = "P1 IMU Connected";
                p1_IMU_text.color = Color.green;
            }

            if (!controller.p1_bt_vest)
            {
                p1_vest_text.text = "P1 Vest Disconnected";
                p1_vest_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p1_vest_text.text = "P1 Vest Connected";
                p1_vest_text.color = Color.green;
            }

            // Player 2 Bluetooth connectivity.
            if (!controller.p2_bt_gun)
            {
                p2_gun_text.text = "P2 Gun Disconnected";
                p2_gun_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p2_gun_text.text = "P2 Gun Connected";
                p2_gun_text.color = Color.green;
            }
            
            if (!controller.p2_bt_IMU)
            {
                p2_IMU_text.text = "P2 IMU Disconnected";
                p2_IMU_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p2_IMU_text.text = "P2 IMU Connected";
                p2_IMU_text.color = Color.green;
            }

            if (!controller.p2_bt_vest)
            {
                p2_vest_text.text = "P2 Vest Disconnected";
                p2_vest_text.color = Color.Lerp(Color.clear, Color.red, Mathf.PingPong(Time.time, 1.0f));
            }
            else
            {
                p2_vest_text.text = "P2 Vest Connected";
                p2_vest_text.color = Color.green;
            }                           
        }
        else
        {
            BluetoothMenu.SetActive(false);
        }    
    }
}
