using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{

    public GameObject BoidTemplate;

    // private GameObject GameplayManagers;
    // private CameraBounds cameraBounds;

    private Vector3 SpawnPosition;

    private List<GameObject> BoidsList = new List<GameObject>();

    public float spawn_boundary = 0.5f;
    private RectBounds spawnRectBounds;
    public float DestAssignRate = .75f;
    private float newDestCounter;
    
    void Start(){
        CreateBounds();

        newDestCounter = DestAssignRate;
        
        CreateNewBoid();
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
        newBoid.transform.position = thisSpawn;
        Boid boidScript = newBoid.GetComponent<Boid>();
        boidScript.active = true;
        boidScript.rectBounds = spawnRectBounds;


        BoidsList.Add(newBoid);
        return newBoid;

    }
    
    void CreateBounds(){
        GameObject GameplayManagers = GameObject.FindGameObjectWithTag("GameplayManagers");
        CameraBounds cameraBounds = GameplayManagers.GetComponentInChildren<CameraBounds>();
        spawnRectBounds = cameraBounds.rectBounds;

        spawnRectBounds.lower += spawn_boundary;
        spawnRectBounds.upper -= spawn_boundary;
        spawnRectBounds.left += spawn_boundary;
        spawnRectBounds.right -= spawn_boundary;

    }

    void AssignNewDestToBoids(){
        for(int i=0; i < BoidsList.Count; i++){
            Boid boidScript = BoidsList[i].GetComponent<Boid>();

            // boidScript.GetRandomLocInBounds(new Vector2(cameraBounds.left, cameraBounds.lower), 
            //                                 new Vector2(cameraBounds.right, cameraBounds.upper));

            boidScript.GetRandomLocInBounds();

        }
    }

    void Update(){
        if (Input.GetKeyDown("space")){
            CreateNewBoid();
        }


        if (newDestCounter <= 0){
            AssignNewDestToBoids();
            newDestCounter = DestAssignRate;
        }
        else{
            newDestCounter -= Time.deltaTime;
        }

    }
}
