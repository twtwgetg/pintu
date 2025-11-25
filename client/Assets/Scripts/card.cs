using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class card : MonoBehaviour
{
    public Texture2D back; 
    internal int levelid;
    internal float uvX;
    internal float uvY;
    internal float uvWidth;
    internal float uvHeight;
    internal Texture texture;

    RawImage rawImage
    {
        get
        {
            return GetComponent<RawImage>();
        }
    }
 

    internal void Load()
    {
        if(levelid< PlayerData.gd.levelid )
        {
            rawImage.uvRect = new Rect(uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
            rawImage.texture = texture;
        }
        else
        {
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            rawImage.texture = back;
        }
    }
}
