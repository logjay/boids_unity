using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{

    public bool active = true;

    void Start(){
        
    }

    void Update(){
        if(!active){
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = new Vector2(mousePos.x, mousePos.y);
    }
}
