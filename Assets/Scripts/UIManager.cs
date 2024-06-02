using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    void Start(){
        
    }

    void Update(){
        
    }

    public void StartGame(){
        SceneManager.LoadScene("Assets/Scenes/BoidsPlayer.unity");

    }

    public void Exit(){
        Application.Quit();
    }
}
