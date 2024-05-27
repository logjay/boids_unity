using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class BoidManager : MonoBehaviour{

    public GameObject BoidTemplate;

    public TextMeshProUGUI DebugBox1;

    public int numBoids = 0;

    private Vector3 SpawnPosition = new Vector3 (0,0,0);

    private List<GameObject> BoidsList = new List<GameObject>();
    
    private Dictionary<int, Vector3> ASCVectors = new Dictionary<int, Vector3>();
    private Dictionary<int, float> ASCRotations = new Dictionary<int, float>();

    public float BoidAttnRadius = 2.5f;

    private float SeparationMinimumDist = 0.0f;

    public float A = 0.1f;
    public float C = 0.1f;
    public float S = 0.1f;

    public float spawn_boundary = 0.5f;
    private RectBounds spawnBounds = new RectBounds();

    [SerializeField]
    private float DestAssignRate = .75f;
    private float newDestCounter;

    public bool wrapAtBounds = true;

    public bool active = true;
    
    void Start(){
        newDestCounter = DestAssignRate;
        SliderSet();
    }

    private RectBounds spawnRectBounds{
        get{
            GameObject GameplayManagers = GameObject.FindGameObjectWithTag("GameplayManagers");
            CameraBounds cameraBounds = GameplayManagers.GetComponentInChildren<CameraBounds>();

            if (spawnBounds.Equals(new RectBounds())){
                spawnBounds = cameraBounds.rectBounds;
                spawnBounds.lower += spawn_boundary;
                spawnBounds.upper -= spawn_boundary;
                spawnBounds.left += spawn_boundary;
                spawnBounds.right -= spawn_boundary;
            }

            return spawnBounds;
        }
        set{}
    }

    GameObject CreateNewBoid(){

        Vector3 thisSpawn;

        if (SpawnPosition == null){
            thisSpawn = BoidTemplate.transform.position;
        }
        else{
            thisSpawn = SpawnPosition;
        }

        GameObject newBoid = Instantiate(BoidTemplate, gameObject.transform);
        // newBoid.transform.position = thisSpawn;
        Boid boidScript = newBoid.GetComponent<Boid>();
        boidScript.rectBounds = spawnRectBounds;
        boidScript.active = true;
        boidScript.wrapAtBounds = wrapAtBounds;
        newBoid.transform.position = boidScript.GetRandomLocInBounds();


        BoidsList.Add(newBoid);
        return newBoid;

    }

    void RemoveBoids(int n=1){
        for (int i=0; i<n ;i++){
            Destroy(BoidsList[0]);
            BoidsList.RemoveAt(0);
        }
    }

    public void SliderSet(){
        numBoids = (int)GameObject.Find("Boid Slider").GetComponent<Slider>().value;
    }

    void CalcASC(int primary, List<int> nearby){
        //Alignment, Separation, Cohesion

        Vector3 primLoc = BoidsList[primary].transform.position;
        Vector3 weighted_pos_vector = Vector3.zero;
        Vector3 average_neighbor_dest = Vector3.zero;
        float avg_rotation = 0f;
        for(int nearIdx=0; nearIdx < nearby.Count; nearIdx++){
            //C
            average_neighbor_dest += BoidsList[nearby[nearIdx]].transform.up;

            //S
            Vector3 pos_delta = BoidsList[nearby[nearIdx]].transform.position - primLoc;
            float pos_delta_dist = pos_delta.magnitude;
            weighted_pos_vector += pos_delta * (BoidAttnRadius - pos_delta_dist + SeparationMinimumDist);

            //A
            float thisRot = Quaternion.Angle(BoidsList[primary].transform.rotation, BoidsList[nearby[nearIdx]].transform.rotation);
            if (thisRot >= 180){
                thisRot -= 360;
            }
            avg_rotation += thisRot / nearby.Count;

        }

        Vector3 avg_separational_weight = weighted_pos_vector / (nearby.Count * 0.5f * BoidAttnRadius);
        // Vector3 avg_other_pos = (average_neighbor_pos - weight_sum) / math.max(nearby.Count, 1);
        Vector3 avg_other_dest = (average_neighbor_dest - (nearby.Count * primLoc)) / math.max(nearby.Count, 1);


        ASCVectors[primary] = avg_separational_weight * S; // separation
        ASCVectors[primary] += avg_other_dest * C; // cohesion

        ASCRotations[primary] = avg_rotation * A;
    }

    void AdjustBoidPathing(){

        for(int primBoid=0; primBoid < BoidsList.Count; primBoid++){
            Boid boidScript = BoidsList[primBoid].GetComponent<Boid>();

            List<int> nearbyBoidIdxs = new List<int>();
            for (int secBoid = 0; secBoid < BoidsList.Count; secBoid++){
                if (secBoid == primBoid){
                    continue;}

                //TODO Track all nearby boids and swap them in and out of lists (or use chunks)
                if (Vector3.Distance(BoidsList[secBoid].transform.up, BoidsList[primBoid].transform.up) < BoidAttnRadius){
                    nearbyBoidIdxs.Add(secBoid);
                }
            }

            CalcASC(primBoid, nearbyBoidIdxs);
            boidScript.ASCVector = ASCVectors[primBoid];
            boidScript.ASCRotation = ASCRotations[primBoid];
            
        }
    }
    

    void AssignNewDestToBoids(){
        for(int i=0; i < BoidsList.Count; i++){
            Boid boidScript = BoidsList[i].GetComponent<Boid>();
            boidScript.CurrentDestination = boidScript.GetRandomLocInBounds();
        }
    }

    void ManageBoidCount(){
        while( BoidsList.Count < numBoids){
            CreateNewBoid();
        }
        RemoveBoids(BoidsList.Count - numBoids);
    }

    void Update(){

        if (!active){
            return;
        }

        ManageBoidCount();

        AdjustBoidPathing();

        if (newDestCounter <= 0){
            // AssignNewDestToBoids();
            newDestCounter = DestAssignRate;
        }
        else{
            newDestCounter -= Time.deltaTime;
        }

        DebugBox1.GetComponent<TextMeshProUGUI>().text = BoidsList.Count.ToString("D");

    }
}
