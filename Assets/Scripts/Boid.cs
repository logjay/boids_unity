using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;

public class Boid : MonoBehaviour
{
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

    public Vector3 CurrentDestination;

    void Start(){
    }

    Vector3 ClampToBounds(Vector3 loc){
        Vector3 newPos = loc;
        newPos.x = math.clamp(loc.x, rectBounds.left, rectBounds.right);
        newPos.y = math.clamp(loc.y, rectBounds.lower, rectBounds.upper);

        return newPos;
    }

    void MoveObject(Vector3 target_loc){

        Vector3 median_target = Vector3.Slerp(gameObject.transform.up, target_loc, DirectionGradient);

        Vector3 direction =  median_target * (currentSpeed() * Time.deltaTime);
        Vector3 newPos = gameObject.transform.position + direction;

        gameObject.transform.position = ClampToBounds(newPos);

        // gameObject.transform.position += direction;

        float max_rot = max_rotation_hz * 360 * Time.deltaTime;

        Quaternion rotation_delta = Quaternion.FromToRotation(gameObject.transform.up, target_loc.normalized);
        float req_rotation = rotation_delta.eulerAngles.z;

        if (req_rotation >= 180){
            req_rotation -= 360;
        }
        req_rotation = Mathf.Lerp(req_rotation, last_req_rotation, RotationGradient);
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
        MoveObject(towardsPoint);
    }

    void MoveTowardsCursor(){
        Vector2 mousePos = CurrentMousePos();
        MoveTowardsPoint(mousePos);
    }
    Vector2 CurrentMousePos(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePos.x, mousePos.y);
    }

    void MoveRandomly(){
        // get bounds of 'arena'
        // get nearby boids (or all boids)
        // choose a direction to fly in to address that
    }

    public void GetRandomLocInBounds(){
        float x = UnityEngine.Random.Range(rectBounds.left, rectBounds.right);
        float y = UnityEngine.Random.Range(rectBounds.lower, rectBounds.upper);


        CurrentDestination = new Vector3(x, y, gameObject.transform.position.z);
    }

    void Update()
    {
        if (!active){
            return;
        }

        if (CurrentDestination != null){
            MoveTowardsPoint(CurrentDestination);
            if (Vector3.Distance(gameObject.transform.position,CurrentDestination) < 0.1f){
                //TODO assign new CurrentDest
            }
        }


        if (Input.GetMouseButton(0)){
            MoveTowardsCursor();

        //     DebugBox1.GetComponent<TextMeshProUGUI>().text = "Requested Angle:\n" + last_req_rotation.ToString("F1");
        //     // DebugBox2.GetComponent<TextMeshProUGUI>().text = "Max Angle:\n" + max_rot.ToString("F1");
        }
        
    }
}
