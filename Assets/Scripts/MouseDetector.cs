using UnityEngine;


public class MouseDetector : MonoBehaviour{

    public bool mouseOver = false;

    void Start(){

    }

    void Update(){


    }

    void OnMouseOver(){
        mouseOver = true;
    }

    void OnMouseExit(){
        mouseOver = false;
    }

}