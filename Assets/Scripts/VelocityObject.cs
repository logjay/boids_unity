using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;


/*goals for class:

control movement
    have target destination
    fetch new destination if prev is reached

    control bounds of movement
    control speed

    accept instructions (pull/push on next instruction)


*/

public class VelocityObject : MonoBehaviour{


    public float max_accel = 2f;
    [SerializeField]
    private Vector3 acceleration = Vector3.zero;

    public float max_speed = 3f;
    [SerializeField]
    public Vector3 velocity = Vector3.zero;

    public Vector3 iter_force = Vector3.zero;
    // public float rotAcceleration = 0f;
    // private float rotVelocity = 0f;

    public float desiredRot = 0;

    [SerializeField]
    private float rotational_cap = 360;
    // [SerializeField]
    // float RotationGradient = 0.5f;

    public RectBounds ValidArea;
    public bool wrapAtBounds = true;

    private float fixedZ = 0.05f;

    public bool active = false;

    // private float prevRotation;
    
    
    public Vector3 GetRandomLocInBounds(){
        float x = UnityEngine.Random.Range(ValidArea.left, ValidArea.right);
        float y = UnityEngine.Random.Range(ValidArea.lower, ValidArea.upper);

        return new Vector3(x, y, fixedZ);
    }

    Vector3 BoundaryClamp(Vector3 pos){
        Vector3 newPos = pos;
        newPos.x = math.clamp(pos.x, ValidArea.left, ValidArea.right);
        newPos.y = math.clamp(pos.y, ValidArea.lower, ValidArea.upper);
        newPos.z = fixedZ;

        return newPos;
    }

    Vector3 BoundaryWrap(Vector3 pos){
        Vector3 newPos = pos;

        while(newPos.x < ValidArea.left || newPos.x > ValidArea.right){
            if (pos.x < ValidArea.left){
                newPos.x = ValidArea.right - (ValidArea.left - pos.x);
            }
            else if (pos.x > ValidArea.right){
                newPos.x = ValidArea.left + (pos.x - ValidArea.right);
            }
        } 

        while(newPos.y < ValidArea.lower || newPos.y > ValidArea.upper){
            if (pos.y < ValidArea.lower){
                newPos.y = ValidArea.upper - (ValidArea.lower - pos.y);
            }
            else if (pos.y > ValidArea.upper){
                newPos.y = ValidArea.lower + (pos.y - ValidArea.upper);
            }
        }

        newPos.z = fixedZ;

        return newPos;
    }

    public Vector3 GetMoveByPhysics(){

        acceleration += iter_force;
        acceleration = Vector3.ClampMagnitude(acceleration, max_accel);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, max_speed);

        return velocity * Time.deltaTime;
    }

    public void RotateInDirection(float rotation_in_degrees){

        if (rotation_in_degrees >= 180){
            rotation_in_degrees -= 360;
        }

        float maxRotationPerTick = rotational_cap * Time.deltaTime;
        rotation_in_degrees = math.clamp(rotation_in_degrees, -maxRotationPerTick, maxRotationPerTick);

        gameObject.transform.Rotate(0f, 0f, rotation_in_degrees);

    }

    public void MoveObjectForward(Vector3 movement){

        Vector3 newLoc = movement + gameObject.transform.position;

        if (wrapAtBounds){
            newLoc = BoundaryWrap(newLoc);
        }
        else{
            newLoc = BoundaryClamp(newLoc);
        }

        newLoc.z = fixedZ;

        gameObject.transform.position = newLoc;
    }

    public void StepObject(){
        Vector3 stepMovement = GetMoveByPhysics();
        iter_force = Vector3.zero; //rest after using the force

        MoveObjectForward(stepMovement);

        float rotation = Quaternion.FromToRotation(gameObject.transform.up,stepMovement).eulerAngles.z;

        rotation = Mathf.Lerp(rotation, desiredRot, 0.2f);

        desiredRot = 0f; //rest after using the force
        // gameObject.transform.Rotate(new Vector3(0, 0, rotation));
        RotateInDirection(rotation);
        //add rotation to follow movement?

        
    }

    void Start(){

        gameObject.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0,360));
    }
    void Update(){

        if (!active){
            return;}

        // MoveObject();

    }
}