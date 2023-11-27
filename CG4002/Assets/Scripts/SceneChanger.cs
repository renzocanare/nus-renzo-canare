using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.SceneManagement; 

//////////////////////////////////////////////////////////
// -- SceneChanger --
// Controls switching between scenes.
//////////////////////////////////////////////////////////
public class SceneChanger: MonoBehaviour {

    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainScreen")); 
    }

    /**
    * Scene(string)
    * Switches to scene of name in string.
    */  
    public void Scene(string sceneName) {  
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Debug.Log("Scene switched!");  
    } 
}   