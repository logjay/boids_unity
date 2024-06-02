using UnityEngine;
using System.Collections.Generic;


public class ColorGradientPrinter : MonoBehaviour{


    public Gradient gradient;

    Sprite thisObjSprite;

    void Start(){
        thisObjSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        // thisObjSprite.texture = new Texture2D(thisObjSprite.texture.width, thisObjSprite.texture.height, TextureFormat.RGBA64, false);
        ColorObjectWithGradient();
    }

    void Update(){


    }

    void ColorObjectWithGradient(){
        Texture2D newTex = new Texture2D(thisObjSprite.texture.width, thisObjSprite.texture.height, TextureFormat.RGB24, false);

        // int len = thisObjSprite.texture.GetPixels().Length;
        int width = thisObjSprite.texture.width;
        int height = thisObjSprite.texture.height;

        // int len = (int)spriteRect.width;
        int total_pixels = height*width;

        

        for (int i=0; i < width; i++){
            var iColor = gradient.Evaluate((float)i/width);
            for (int j=0; j < height; j++){
                newTex.SetPixel(i, j, iColor * (j) * 2 / height);

                // newTex.SetPixel(i%height, i/height, gradient.Evaluate((float)i/total_pixels));
            // newTex.SetPixel(i%width, i/width, Color.blue);
            }
        }


        newTex.Apply();

        Vector2 pivotCopy = new Vector2(thisObjSprite.pivot.x / thisObjSprite.rect.width, thisObjSprite.pivot.y / thisObjSprite.rect.height);

        Sprite newSprite = Sprite.Create(newTex, thisObjSprite.rect, pivotCopy, thisObjSprite.pixelsPerUnit);
        // Sprite newSprite = Sprite.Create(newTex, thisObjSprite.rect, Vector2.zero);
        newSprite.name = "Gradient";

        // newSprite.bounds = thisObjSprite.bounds;

        // thisObjSprite = newSprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        // Sprite

        Debug.Log("Created New Sprite Gradient!");

        Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite.name);

    }

}