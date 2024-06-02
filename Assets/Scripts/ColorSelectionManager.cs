using UnityEngine;
using System.Collections.Generic;
using System;


public class ColorSelector : MonoBehaviour{

    public Color activeColor = Color.white;
    // public Gradient gradient;

    public GameObject colorBall;
    public GameObject colorBar;
    public GameObject colorSelectIndicator;

    private SpriteRenderer indicatorBall;

    private static MouseDetector ballDetector;
    private static MouseDetector barDetector;

    public List<string> coloredObjectTags = new List<string>();
    

    void Start(){
        ballDetector = colorBall.GetComponent<MouseDetector>();
        barDetector = colorBar.GetComponent<MouseDetector>();
        colorSelectIndicator.transform.Rotate(0f, 0f, 180f);

        coloredObjectTags.Add("Boid");

        indicatorBall = colorSelectIndicator.transform.Find("Indicator").GetComponent<SpriteRenderer>();
    }

    void Update(){
        if (barDetector.mouseOver){
            colorSelectIndicator.SetActive(true);
            Color HoveredColor = GetHoveredColor();
            indicatorBall.color = HoveredColor;
            colorSelectIndicator.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButton(0)){
                
                
                SetActiveColor(HoveredColor);
                
                // colorSelectIndicator.transform.position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                
            }
        }
        else{
            colorSelectIndicator.SetActive(false);
        }

    }

    void SetActiveColor(Color selectedColor){
        activeColor = selectedColor;
        colorBall.GetComponent<SpriteRenderer>().color = selectedColor;
        SetTaggedObjectsToActiveColor();
    }

    void SetTaggedObjectsToActiveColor(){

        List<GameObject> taggedObjects = new List<GameObject>();

        for (int i=0; i < coloredObjectTags.Count; i++){
            taggedObjects.AddRange( GameObject.FindGameObjectsWithTag(coloredObjectTags[i]));
        }

        for (int i=0; i < taggedObjects.Count; i++){
            taggedObjects[i].GetComponent<SpriteRenderer>().color = activeColor;
        }
    }

    Color GetHoveredColor(){
        if (!barDetector.mouseOver){
            return Color.magenta;
        }

        SpriteRenderer barSprtRender = colorBar.GetComponent<SpriteRenderer>();
        
        Vector2 HoveredPix = GetPixelAtPos(barSprtRender, Input.mousePosition);

        HoveredPix -= barSprtRender.sprite.pivot;

        return barSprtRender.sprite.texture.GetPixel((int)HoveredPix.x, (int)HoveredPix.y);
    }

    Vector2Int GetPixelAtPos(SpriteRenderer sprRender, Vector3 CameraPos){

        Transform relativeTransform = colorBar.transform;

        Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(CameraPos));

        // Debug.DrawLine(Vector3.zero, Camera.main.ScreenToViewportPoint(CameraPos), Color.green, 10);

        // var sprite = img.GetComponent<SpriteRenderer>();

        Texture2D texture = sprRender.sprite.texture;
        Plane plane = new Plane(relativeTransform.forward, relativeTransform.position);
        float intersectDist;
        plane.Raycast(ray, out intersectDist);

        Vector3 spritePos = sprRender.worldToLocalMatrix.MultiplyPoint3x4(ray.origin + (ray.direction * intersectDist));

        Rect textureRect = sprRender.sprite.textureRect;

        float pixelsPerUnit = sprRender.sprite.pixelsPerUnit;
        float halfRealTexWidth = texture.width * 0.0f; // use the real texture width here because center is based on this -- probably won't work right for atlases
        float halfRealTexHeight = texture.height * 0.0f;

        // Convert to pixel position, offsetting so 0,0 is in lower left instead of center
        int texPosX = (int)(spritePos.x * pixelsPerUnit + halfRealTexWidth);
        int texPosY = (int)(spritePos.y * pixelsPerUnit + halfRealTexHeight);

        // Check if pixel is within texture
        // if(texPosX < 0 || texPosX < textureRect.x || texPosX >= Mathf.FloorToInt(textureRect.xMax)) return false; // out of bounds
        // if(texPosY < 0 || texPosY < textureRect.y || texPosY >= Mathf.FloorToInt(textureRect.yMax)) return false; // out of bounds

        return new Vector2Int(texPosX, texPosY);
    }
}