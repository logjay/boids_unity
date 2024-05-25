using UnityEngine;
using System.Collections.Generic;


public class CameraBounds : MonoBehaviour
{

    // private Dictionary<string, Vector3> CameraCorners = new Dictionary<string, Vector3>();

    public RectBounds rectBounds = new RectBounds();

    void Start()
    {
        GetBoundaries();
    }
    void GetBoundaries(){
        Vector3 UR = Camera.main.ViewportToWorldPoint(new Vector3(1,1,Camera.main.nearClipPlane));
        Vector3 BR = Camera.main.ViewportToWorldPoint(new Vector3(1,0,Camera.main.nearClipPlane));
        Vector3 UL = Camera.main.ViewportToWorldPoint(new Vector3(0,1,Camera.main.nearClipPlane));
        Vector3 BL = Camera.main.ViewportToWorldPoint(new Vector3(0,0,Camera.main.nearClipPlane));

        Debug.DrawLine(UR, BL, Color.cyan, 9999);
        Debug.DrawLine(UL, BR, Color.cyan, 9999);

        rectBounds.upper = UL.y;
        rectBounds.lower = BR.y;
        rectBounds.left = UL.x;
        rectBounds.right = BR.x;

    }


    public float upper{
        get{
            return rectBounds.upper;
        }

        set{
            print("Cannot set this limit");
        }
    }
    public float left{
        get{
            return rectBounds.left;
        }

        set{
            print("Cannot set this limit");
        }
    }
    public float right{
        get{
            return rectBounds.right;
        }

        set{
            print("Cannot set this limit");
        }
    }
    public float lower{
        get{
            return rectBounds.lower;
        }

        set{
            print("Cannot set this limit");
        }
    }
}