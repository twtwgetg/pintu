using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 在运行时从 Resources 加载用于边框的贴图（优先 Sprite，然后尝试 Texture2D）
[RequireComponent(typeof(Image))]
public class BorderSpriteLoader : MonoBehaviour
{
    [Tooltip("资源名（在 Resources 下），例如 'lt','t','rt','r','rb','b','lb','l' 等")]
    public string resourceName;

    [Tooltip("当找不到资源时是否保留颜色占位（Image.color）")]
    public bool keepColorIfMissing = true;

    void Awake()
    {
       
    }

    internal void LoadSprite()
    {
        if (string.IsNullOrEmpty(resourceName)) return;

        Image img = GetComponent<Image>();
        if (img == null) return;
        // img.material = new Material(Shader.Find("Hidden/imagec"));
        // 静态缓存，避免重复加载
        if (spriteCache == null)
            spriteCache = new Dictionary<string, Sprite>();

        Sprite s = null;
        if (spriteCache.TryGetValue(resourceName, out s)) {
            img.sprite = s;
            img.color = Color.white;
        } else {
            s = Resources.Load<Sprite>(resourceName);
            if (s != null) {
                img.sprite = s;
                img.color = Color.white;
                spriteCache[resourceName] = s;
            } else {
                img.sprite = null;
                img.color = keepColorIfMissing ? Color.gray : Color.clear;
            }
        }
        
        // 加载对应的mask贴图（添加m_前缀）
        // string maskResourceName = "m_" + resourceName;
        // if (img.material != null) {
        //     Texture2D maskTexture = Resources.Load<Texture2D>(maskResourceName);
        //     if (maskTexture != null) {
        //         img.material.SetTexture("_Mask", maskTexture);
        //     }
        // }
    }
    // 静态缓存，避免重复加载
    private static Dictionary<string, Sprite> spriteCache;

    // 直接设置资源名并加载
    public void SetResourceAndLoad(string key)
    {
        resourceName = key;
        LoadSprite();
    }

    // 解析父对象名称里保存的格子坐标，并尝试读取父级的 picmgr 网格尺寸
    private void GetCellAndGrid(out int cellX, out int cellY, out int gridW, out int gridH)
    {
        cellX = -1; cellY = -1; gridW = -1; gridH = -1;
        if (transform.parent != null)
        {
            string pname = transform.parent.gameObject.name.Replace("GridCell_", "");
            string[] parts = pname.Split('_');
            if (parts.Length == 2)
            {
                int.TryParse(parts[0], out cellX);
                int.TryParse(parts[1], out cellY);
            }
        }

        picmgr mgr = GetComponentInParent<picmgr>();
        if (mgr != null)
        {
            gridW = mgr.width;
            gridH = mgr.height;
        }
    }

     internal void RefreshBasedOnFlags(string prop, bool isTopCorrect, bool isLeftCorrect, bool isRightCorrect, bool isBottomCorrect,bool isConnerCorrect)
    {
        // 只处理四个角的逻辑，假定 resourceName 已经是角的类型（lt, rt, rb, lb）
        string key = resourceName;
        switch (prop)
        {
            case "lt": // 左上角
                if (isLeftCorrect && isTopCorrect && !isConnerCorrect)
                    key = "lt_revert";
                else if (!isLeftCorrect && !isTopCorrect)
                    key = "lt";
                else if (isLeftCorrect && !isTopCorrect)
                    key = "t";
                else if (!isLeftCorrect && isTopCorrect)
                    key = "l";
                else
                {

                }
                break;
            case "rt": // 右上角
                if (isRightCorrect && isTopCorrect && !isConnerCorrect)
                    key = "rt_revert";
                else if (!isRightCorrect && !isTopCorrect)
                    key = "rt";
                else if (isRightCorrect && !isTopCorrect)
                    key = "t";
                else if (!isRightCorrect && isTopCorrect)
                    key = "r";
                else
                {

                }
                break;
            case "rb": // 右下角
                // 修改：添加对右下方对角线邻居的判断
                if (isRightCorrect && isBottomCorrect && !isConnerCorrect)
                    key = "rb_revert";
                else if (!isRightCorrect && !isBottomCorrect)
                    key = "rb";
                else if (isRightCorrect && !isBottomCorrect)
                    key = "b";
                else if (!isRightCorrect && isBottomCorrect)
                    key = "r";
                else
                {

                }   
                break;
            case "lb": // 左下角
                if (isLeftCorrect && isBottomCorrect && !isConnerCorrect)
                    key = "lb_revert";
                else if (!isLeftCorrect && !isBottomCorrect)
                    key = "lb";
                else if (isLeftCorrect && !isBottomCorrect)
                    key = "b";
                else if (!isLeftCorrect && isBottomCorrect)
                    key = "l";
                else
                {
                }
                break;
            default:
                Debug.LogError("Invalid property: " + prop);
                break;
        }
        SetResourceAndLoad(key);
    }
}
