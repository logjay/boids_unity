using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;

public class Boid : MonoBehaviour{

    public TextMeshProUGUI DebugBox1;
    public TextMeshProUGUI DebugBox2;

    // public GameObject CameraManager;
    // private CameraBounds cameraBounds;

    public RectBounds rectBounds = new RectBounds();
    
    public bool active = true;
    public float maxspeed = 5.5f;
    public float max_rotation_hz = .75f;
    private float last_req_rotation = 69.69f;

    public float DirectionGradient = 0.1f;
    public float RotationGradient = 0.5f;

    public float ASCGradient = 0.1f;
    public Vector3 ASCVector = Vector3.zero;
    public float ASCRotation = 0f;

    public Vector3 CurrentDestination;

    public bool wrapAtBounds = true;

    private float fixedZ = 0f;

    void Start(){
    }

    Vector3 ClampToBounds(Vector3 pos){
        Vector3 newPos = pos;
        newPos.x = math.clamp(pos.x, rectBounds.left, rectBounds.right);
        newPos.y = math.clamp(pos.y, rectBounds.lower, rectBounds.upper);
        newPos.z = fixedZ;

        return newPos;
    }

    Vector3 WrapOnBounds(Vector3 pos){
        Vector3 newPos = pos;
        
        if (pos.x < rectBounds.left){
            newPos.x = rectBounds.right - (rectBounds.left - pos.x);
        }
        else if (pos.x > rectBounds.right){
            newPos.x = rectBounds.left + (pos.x - rectBounds.right);
        }

        if (pos.y < rectBounds.lower){
            newPos.y = rectBounds.upper - (rectBounds.lower - pos.y);
        }
        else if (pos.y > rectBounds.upper){
            newPos.y = rectBounds.lower + (pos.y - rectBounds.upper);
        }

        newPos.z = fixedZ;

        return newPos;
    }

    void MoveObject(Vector3 target_dir){

        Vector3 median_target = Vector3.Slerp(gameObject.transform.up, target_dir, DirectionGradient);
        median_target = Vector3.Slerp(median_target, ASCVector, ASCGradient);


        Vector3 direction =  median_target.normalized * (currentSpeed() * Time.deltaTime);
        Vector3 newPos = gameObject.transform.position + direction;


        if (wrapAtBounds){
            gameObject.transform.position = WrapOnBounds(newPos);
        }
        else{
            gameObject.transform.position = ClampToBounds(newPos);
        }
        

        // gameObject.transform.position += direction;

        float max_rot = max_rotation_hz * 360 * Time.deltaTime;

        // Quaternion rotation_delta = Quaternion.FromToRotation(gameObject.transform.up, target_dir.normalized);
        Quaternion rotation_delta = Quaternion.FromToRotation(gameObject.transform.up, direction);
        float req_rotation = rotation_delta.eulerAngles.z;

        if (req_rotation >= 180){
            req_rotation -= 360;
        }
        req_rotation = Mathf.Lerp(req_rotation, last_req_rotation, RotationGradient);
        req_rotation = Mathf.Lerp(req_rotation, ASCRotation, ASCGradient);

        req_rotation = math.clamp(req_rotation, -max_rot, max_rot);

        gameObject.transform.Rotate(0f, 0f, req_rotation);


        last_req_rotation = req_rotation;
    }

    private float currentSpeed(){
        return maxspeed;
    }

    public void MoveTowardsPoint(Vector2 targetPos){
        targetPos = ClampToBounds(targetPos);

        Vector3 towardsPoint = new Vector3(targetPos.x, targetPos.y, gameObject.transform.position.z) - gameObject.transform.position;
        MoveObject(towardsPoint.normalized);
    }

    void MoveTowardsCursor(){
        Vector2 mousePos = CurrentMousePos();
        MoveTowardsPoint(mousePos);
    }
    
    Vector2 CurrentMousePos(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePos.x, mousePos.y);
    }

    public Vector3 GetRandomLocInBounds(){
        float x = UnityEngine.Random.Range(rectBounds.left, rectBounds.right);
        float y = UnityEngine.Random.Range(rectBounds.lower, rectBounds.upper);


        return new Vector3(x, y, gameObject.transform.position.z);
    }

    void Update()
    {
        if (!active){
            return;
        }

        // if (Input.GetMouseButton(0)){
        //     MoveTowardsCursor();
        //     return;
        // }

        if (CurrentDestination != default(Vector3)){
            MoveTowardsPoint(CurrentDestination);
            if (Vector3.Distance(gameObject.transform.position,CurrentDestination) < 0.1f){
                //TODO assign new CurrentDest
            }
        }
        else{
            MoveTowardsPoint(gameObject.transform.position + gameObject.transform.up);
        }



        
    }

}
