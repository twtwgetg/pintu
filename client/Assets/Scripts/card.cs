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
    public TextMeshProUGUI level;
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
            if (PlayerData.gd.isOpened(levelid))
            {
                rawImage.uvRect = new Rect(uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                rawImage.texture = texture;
                level.gameObject.SetActive(false);
            }
            else
            {
                DOVirtual.DelayedCall(1.0f, () =>
                {
                    rawImage.uvRect = new Rect(0, 0, 1, 1);
                    rawImage.texture = back;
                    level.gameObject.SetActive(false);
                    transform.DOScaleX(0, 1.0f).onComplete = () =>
                    {
                        transform.localScale = new Vector3(0, 1, 1);

                        rawImage.uvRect = new Rect(uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                        rawImage.texture = texture;

                        transform.DOScaleX(1, 1f).onComplete = () =>
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                        };
                    };
                    var x = transform.localPosition.x;
                    float wid = transform.GetComponent<RectTransform>().rect.width;
                    transform.DOLocalMoveX(x + wid / 2, 1.0f).onComplete = () =>
                    {
                        var p = transform.localPosition;
                        p.x = x + wid / 2;
                        transform.localPosition = p;

                        transform.DOLocalMoveX(x, 1.0f).onComplete = () =>
                        {
                            var p = transform.localPosition;
                            p.x = x;
                            transform.localPosition = p;
                        };
                    };
                    PlayerData.gd.Open(levelid);
                }); 
            }
        }
        else
        {
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            rawImage.texture = back;
            level.gameObject.SetActive(true);
        }
    }
}
