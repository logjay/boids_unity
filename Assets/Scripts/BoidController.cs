using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoidController : MonoBehaviour{

    public GameObject BoidTemplate;
    public TextMeshProUGUI DebugBox1;

    public int numBoids = 0;
    private List<GameObject> BoidsList = new List<GameObject>();

    public bool wrapAtBounds = true;
    private RectBounds spawnRectBounds;
    private float spawnBoundBuffer = -0.1f;

    private Dictionary<int, Vector3> ASCVectBumps = new Dictionary<int, Vector3>();
    private Dictionary<int, float> ASCRotBumps = new Dictionary<int, float>();

    public float nearRadius = 1f;
    public float farRadius = 2f;

    [SerializeField]
    private float SProxWeight = 1.5f;

    public float A = 1f;
    public float C = 1f;
    public float S = 1f;

    public TextMeshProUGUI ALabel;
    public TextMeshProUGUI SLabel;
    public TextMeshProUGUI CLabel;

    public bool active = true;

    void Start(){
        BoidSliderSet();

        GameObject.Find("ASlider").GetComponent<Slider>().value = A;
        GameObject.Find("SSlider").GetComponent<Slider>().value = S;
        GameObject.Find("CSlider").GetComponent<Slider>().value = C;
        ALabel.GetComponent<TextMeshProUGUI>().text = "A " + A.ToString("F4");
        SLabel.GetComponent<TextMeshProUGUI>().text = "S " + S.ToString("F4");
        CLabel.GetComponent<TextMeshProUGUI>().text = "C " + C.ToString("F4");
    }

    void Update(){
        if (!active){
            return;
        }

        ManageBoidCount();

        MoveBoids();

        DebugBox1.GetComponent<TextMeshProUGUI>().text = numBoids.ToString("D");

    }

    List<int> GetNearby(int primeIdx, float radius=1f){
        List<int> nearbyNodes = new List<int>();

        GameObject prime_boid = BoidsList[primeIdx];

        for(int idx=0; idx < BoidsList.Count; idx++){
            if (idx == primeIdx){
                continue;
            }

            if (Vector3.Distance(prime_boid.transform.position,BoidsList[idx].transform.position) < radius){
                nearbyNodes.Add(idx);
            }
        }

        return nearbyNodes;
    }

    float rotationalJitter(float limit = 60f){

        return UnityEngine.Random.Range(-limit, limit);
    }

    void CalcASC(int prime){
        // List<int> nearbyNodes = new List<int>();

        GameObject prime_boid = BoidsList[prime];
        Vector3 primePos = prime_boid.transform.position;

        int viewedFarBoids = 0, viewedNearBoids=0;
        Vector3 CohesionAveragePos = Vector3.zero;
        Vector3 WeightedSeparationVector = Vector3.zero;

        Vector3 AvgRotationVector = Vector3.zero;

        for(int idx=0; idx < BoidsList.Count; idx++){
            if (idx == prime){
                continue;
            }
            
            Vector3 boidPos = BoidsList[idx].transform.position;
            float distance = Vector3.Distance(primePos, boidPos);


            if (distance <= farRadius){

                Vector3 posDelta = boidPos - primePos;

                //TODO perform check for angle of viewing
                
                //get average position for cohesion
                CohesionAveragePos += posDelta;
                viewedFarBoids++;

                if (distance <= nearRadius){

                    VelocityObject velocityScript = BoidsList[idx].GetComponent<VelocityObject>();

                    // posDelta = boidPos - (primePos + prime_boid.transform.up);

                    //get separation push (do NOT average)
                    WeightedSeparationVector -= posDelta * math.pow(nearRadius - Vector3.Magnitude(posDelta), SProxWeight) / math.pow(nearRadius, SProxWeight);

                    //get alignment
                    // float relRot = Quaternion.FromToRotation(prime_boid.transform.transform.up, BoidsList[idx].transform.up).eulerAngles.z;
                    
                    // relRot -= 360;

                    // if (relRot >= 180){
                    //     relRot -= 360;
                    // }

                    AvgRotationVector += velocityScript.velocity;

                    viewedNearBoids++;
                }

            }
        }

        CohesionAveragePos /= math.max(viewedFarBoids, 1);
        // AvgRotationVector /= math.max(viewedNearBoids, 1);



        // ASCVectBumps[prime] = Vector3.Slerp(WeightedSeparationVector, CohesionAveragePos.normalized, C / (C+S)) * (C+S) / 2;

        ASCVectBumps[prime] = AvgRotationVector.normalized * A;
        ASCVectBumps[prime] += WeightedSeparationVector.normalized * S;
        ASCVectBumps[prime] += CohesionAveragePos.normalized * C;
        // ASCRotBumps[prime] = AvgRotationVector.normalized;
    }

    void GetASCBumps(){
        for(int prime=0; prime < BoidsList.Count; prime++){
            CalcASC(prime);
        }
    }

    void MoveBoids(){

        GetASCBumps();

        for(int primBoid=0; primBoid < BoidsList.Count; primBoid++){


            VelocityObject moveScript = BoidsList[primBoid].GetComponent<VelocityObject>();



            moveScript.iter_force += ASCVectBumps[primBoid];
            // moveScript.desiredRot = ASCRotBumps[primBoid];
            moveScript.StepObject();

            // if (float.IsNaN(ASCVectBumps[primBoid].x)){
            //     moveScript.MoveObjectForward();
            // }
            // else{
            //     Vector3 netVector = Vector3.Slerp(BoidsList[primBoid].transform.up, ASCVectBumps[primBoid], 0.5f);

            //     //rotate
            //     float rotationToTarget = Quaternion.FromToRotation(BoidsList[primBoid].transform.up, netVector).eulerAngles.z;
            //     float rotationToAlignment = (ASCRotBumps[primBoid] - BoidsList[primBoid].transform.eulerAngles.z) * A;
            //     rotationToTarget = math.lerp(rotationToTarget, rotationToAlignment, .5f);
            //     moveScript.RotateInDirection(rotationToTarget + rotationalJitter(5f));


            //     // moveScript.MoveObjectForward(math.min(netVector.magnitude, 1f));
            // }


        }

    }

    private RectBounds ValidArea{
        get{
            // GameObject CameraManager = GameObject.FindGameObjectWithTag("CameraManager");
            GameObject CameraManager = GameObject.Find("CameraManager");
            CameraBounds cameraBounds = CameraManager.GetComponent<CameraBounds>();

            if (spawnRectBounds.Equals(new RectBounds())){
                spawnRectBounds = cameraBounds.rectBounds;
                spawnRectBounds.lower += spawnBoundBuffer;
                spawnRectBounds.upper -= spawnBoundBuffer;
                spawnRectBounds.left += spawnBoundBuffer;
                spawnRectBounds.right -= spawnBoundBuffer;
            }

            return spawnRectBounds;
        }
        set{}
    }

    public void BoidSliderSet(){
        numBoids = (int)GameObject.Find("BoidSlider").GetComponent<Slider>().value;
    }

    public void ASliderSet(){
        A = GameObject.Find("ASlider").GetComponent<Slider>().value;
        ALabel.GetComponent<TextMeshProUGUI>().text = "A " + A.ToString("F4");
    }
    public void SSliderSet(){
        S = GameObject.Find("SSlider").GetComponent<Slider>().value;
        SLabel.GetComponent<TextMeshProUGUI>().text = "S " + S.ToString("F4");
    }
    public void CSliderSet(){
        C = GameObject.Find("CSlider").GetComponent<Slider>().value;
        CLabel.GetComponent<TextMeshProUGUI>().text = "C " + C.ToString("F4");
    }

    void ManageBoidCount(){
        void RemoveBoids(int n=1){
            for (int i=0; i<n ;i++){
                Destroy(BoidsList[0]);
                BoidsList.RemoveAt(0);
            }
        }

        GameObject CreateNewBoid(){

            GameObject newBoid = Instantiate(BoidTemplate, gameObject.transform);
            // ObjectMover objectMoverScript = newBoid.GetComponent<ObjectMover>();
            VelocityObject objectMoverScript = newBoid.GetComponent<VelocityObject>();
            objectMoverScript.active = true;
            objectMoverScript.wrapAtBounds = wrapAtBounds;

            objectMoverScript.ValidArea = ValidArea;
            newBoid.transform.position = objectMoverScript.GetRandomLocInBounds();
            newBoid.transform.rotation = Quaternion.Euler(new Vector3(0,0, UnityEngine.Random.Range(-180,180)));
            // objectMoverScript.RotateInDirection((float)UnityEngine.Random.Range(-180,180));

            BoidsList.Add(newBoid);
            return newBoid;

        }

        while( BoidsList.Count < numBoids){
            CreateNewBoid();
        }
        RemoveBoids(BoidsList.Count - numBoids);
    }


}