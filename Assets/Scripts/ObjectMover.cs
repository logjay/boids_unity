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

public class ObjectMover : MonoBehaviour{

    [SerializeField]
    private Vector3 currTarget;


    public float acceleration = 1f;
    public float max_speed = 5f;
    [SerializeField]
    private float velocity = 0;
    [SerializeField]
    private float rotational_cap = 45;

    [SerializeField]
    private float momentumGradient = 0.85f;
    [SerializeField]
    private float rotGradient = 0.85f;

    public RectBounds ValidArea;
    public bool wrapAtBounds = true;

    [SerializeField]
    private bool RotationBasedMovement = true;

    private float fixedZ = 0.05f;
    private float reachedDestThresh = 0.05f;

    public bool active = false;

    private float prevRotation;
    
    public void GetNewTarget(){
        currTarget = GetRandomLocInBounds();
        return;
    }

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
        return gameObject.transform.up * velocity * Time.deltaTime;
    }

    Vector3 CalculateNewLoc(Vector3 directionVect){
        directionVect = directionVect.normalized;
        directionVect = Vector3.Slerp(directionVect, gameObject.transform.up * velocity, momentumGradient).normalized;

        Vector3 delta = directionVect * math.min(velocity * Time.deltaTime, Vector3.Magnitude(directionVect));
        velocity = math.min(max_speed, velocity + Time.deltaTime * acceleration);

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

    void MoveObjectInDirection(Vector3 directionVect){
        gameObject.transform.position = CalculateNewLoc(directionVect);
    }

    void RotateInDirection(Vector3 pointToward, float? rotationBump = null, float bumpPull = 0.2f){

        Quaternion rotation_delta = Quaternion.FromToRotation(gameObject.transform.up, pointToward - gameObject.transform.position);
        float req_rotation = rotation_delta.eulerAngles.z;

        if (req_rotation >= 180){
            req_rotation -= 360;
        }

        
        req_rotation = Mathf.Lerp(req_rotation, prevRotation, rotGradient);

        if (rotationBump != null){
            req_rotation = Mathf.Lerp(req_rotation, (float)rotationBump, bumpPull);
        }


        float maxRotationPerTick = rotational_cap * Time.deltaTime;
        req_rotation = math.clamp(req_rotation, -maxRotationPerTick, maxRotationPerTick);

    
        gameObject.transform.Rotate(0f, 0f, req_rotation);


        prevRotation = req_rotation;
    }

    public void MoveTowardsTarget(Vector3? bump = null, float? rotBump = null){

        Vector3 directionVect = currTarget - gameObject.transform.position;
        if (bump != null){

            directionVect += (Vector3)bump;
        }

        if (Vector3.Distance(gameObject.transform.position, currTarget) < reachedDestThresh){
            GetNewTarget();
        }

        if (RotationBasedMovement){
            RotateInDirection(directionVect, rotBump);
            MoveObjectInDirection(gameObject.transform.up);
        }
        else{
            // Vector3 newLoc = CalculateNewLoc(directionVect);
            Vector3 newLoc = CalculateNewLoc(gameObject.transform.up);
            RotateInDirection(newLoc);
            gameObject.transform.position = newLoc;
        }
        

    }
    
    void Start(){

    }
    void Update(){

        if (!active){
            return;}

    }
}