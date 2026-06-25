using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class card : MonoBehaviour
{
    public Texture2D back; 
    internal int levelid;
    internal float uvX;
    internal float uvY;
    internal float uvWidth;
    internal float uvHeight;
    internal Texture texture;
    public Sprite blocktex,frame;
    public Image src;
    public TextMeshProUGUI level;
    RawImage rawImage
    {
        get
        {
            return GetComponent<RawImage>();
        }
    }
    public bool isTurning = false;

    internal void Load()
    {
        if(levelid< PlayerData.gd.levelid )
        {
            if (PlayerData.gd.isOpened(levelid))
            {
                rawImage.uvRect = new Rect(uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                rawImage.texture = texture;
                //level.gameObject.SetActive(false);
                show();
            }
            else
            {
                DOVirtual.DelayedCall(1.0f, () =>
                {
                    rawImage.uvRect = new Rect(0, 0, 1, 1);
                    rawImage.texture = back;
                    //level.gameObject.SetActive(false);
                    isTurning = true;
                    transform.DOScaleX(0, .5f).onComplete = () =>
                    {
                        transform.localScale = new Vector3(0, 1, 1);

                        rawImage.uvRect = new Rect(uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                        rawImage.texture = texture;
                        show();
                        transform.DOScaleX(1, 0.5f).onComplete = () =>
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                            isTurning = false;
                        };
                    };
                    var x = transform.localPosition.x;
                    float wid = transform.GetComponent<RectTransform>().rect.width; 
                    PlayerData.gd.Open(levelid);
                });
                hide();
            }
        }
        else
        {
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            rawImage.texture = back;
            //level.gameObject.SetActive(true);
            hide();
        }
    }

    public void show()
    {
        this.src.sprite = frame;
    }

    public void hide()
    {
        this.src.sprite = blocktex;
    }
}
