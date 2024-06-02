using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeManager : MonoBehaviour
{

    public bool gameActive = true;

    [SerializeField]
    private SpriteRenderer greyOut;
    [SerializeField]
    Canvas PauseMenu;

    [SerializeField]
    BoidController gameplayController;

    void Start(){
        ResumeGame();
    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.Escape)){
            PauseResume();
        }
        
    }

    public void PauseGame(){
        greyOut.enabled = true;
        PauseMenu.enabled = true;
        gameplayController.active = false;
    }

    public void ResumeGame(){
        greyOut.enabled = false;
        PauseMenu.enabled = false;
        gameplayController.active = true;
    }

    public void PauseResume(){
        gameActive ^= true;

        if (gameActive){
            ResumeGame();
        }
        else{
            PauseGame();
        }

    
    }

    public void ExitGame(){
        Application.Quit();
    }

}
