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


    public Vector3 acceleration = Vector3.zero;
    public float max_speed = 5f;
    [SerializeField]
    private Vector3 velocity = Vector3.zero;

    public float force = 1f;

    public float rotAcceleration = 0f;
    private float rotVelocity = 0f;

    [SerializeField]
    private float rotational_cap = 90;
    [SerializeField]
    float RotationGradient = 0.5f;

    public RectBounds ValidArea;
    public bool wrapAtBounds = true;

    private float fixedZ = 0.05f;

    public bool active = false;

    private float prevRotation;
    
    
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

    public Vector3 NextLocation(){
        return gameObject.transform.up + velocity * Time.deltaTime;
    }

    public Vector3 defaultAccel(){
        // return gameObject.transform.up * force;
        // return gameObject.transform.up * 0.75f;
        return Vector3.zero;
    }

    Vector3 CalculateNextMarch(){

        rotVelocity += Time.deltaTime * rotAcceleration;

        // velocity += Time.deltaTime * acceleration;

        velocity = gameObject.transform.up;

        velocity = velocity.normalized * math.min(velocity.magnitude, max_speed);
        Vector3 delta = velocity * Time.deltaTime;

        Vector3 newLoc = gameObject.transform.position + delta;

        if (wrapAtBounds){
            newLoc = BoundaryWrap(newLoc);
        }
        else{
            newLoc = BoundaryClamp(newLoc);
        }
        newLoc.z = fixedZ;

        return newLoc;
    }

    public void RotateInDirection(float rotation_in_degrees){

        if (rotation_in_degrees >= 180){
            rotation_in_degrees -= 360;
        }

        float maxRotationPerTick = rotational_cap * Time.deltaTime;
        rotation_in_degrees = math.clamp(rotation_in_degrees, -maxRotationPerTick, maxRotationPerTick);

        rotation_in_degrees = Mathf.Lerp(rotation_in_degrees, prevRotation, RotationGradient);

        gameObject.transform.Rotate(0f, 0f, rotation_in_degrees);


        prevRotation = rotation_in_degrees;
    }

    public void MoveObjectForward(float speed = 1f){

        Vector3 delta = gameObject.transform.up.normalized * Time.deltaTime * speed;
        Vector3 newLoc = gameObject.transform.position + delta;

        if (wrapAtBounds){
            newLoc = BoundaryWrap(newLoc);
        }
        else{
            newLoc = BoundaryClamp(newLoc);
        }

        newLoc.z = fixedZ;

        gameObject.transform.position = newLoc;

    }

    public void MoveObject(Vector3 nudgeLoc = default(Vector3), float nudgeStrength = 0f){
        // gameObject.transform.position = CalculateNextMarch();
        // velocity.z = 0;

        // Vector3.Slerp(velocity, gameObject.transform.up, );

        // Quaternion transition_rotation = Quaternion.FromToRotation(velocity, gameObject.transform.up);
        // Quaternion.Slerp(transition_rotation);


        //working is below
        Quaternion rotation = Quaternion.FromToRotation(velocity, gameObject.transform.up);
        gameObject.transform.Rotate(new Vector3(0f, 0f, rotation.eulerAngles.z) * Time.deltaTime);

        gameObject.transform.position = CalculateNextMarch();

        acceleration = defaultAccel();
        
    }
    
    void Start(){

        RotateInDirection(UnityEngine.Random.Range(0,360));
    }
    void Update(){

        if (!active){
            return;}

        // MoveObject();

    }
}