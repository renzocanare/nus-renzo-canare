using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainP2Init : MonoBehaviour
{
    private mqttController mqttController;  

    void Start()
    {
        mqttController = GameObject.FindGameObjectWithTag("MQTTController").GetComponent<mqttController>();

        mqttController.MainPlayer = 2;

    }
}
