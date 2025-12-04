using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class frm_chapter : frmbase
{
    public RectTransform chaptercontent;
    public Button btn;
    public Button btnsetup;
    public TextMeshProUGUI staminaText; // power显示文本组件
    public Image fillimg;
    private void Awake()
    {
        Main.RegistEvent("gamebegin", (x) =>
        {
            brushChapterContent();
            show();
            UpdateStaminaDisplay();
            return 1;
        });
        
        // 注册power变化事件
        Main.RegistEvent("onpowerChange", (x) =>
        {
            UpdateStaminaDisplay();
            return null;
        });
        Main.RegistEvent("level_next", (x) =>
        {
            PlayerData.gd.levelid = datamgr.Instance.GetLevel(PlayerData.gd.levelid).NextLevel;
            brushChapterContent();
            show();
            return 1;
        });
        Main.RegistEvent("onChapterChange", (x) =>
        { 
            brushChapterContent();
            show();
            return 1;
        });
        btn.onClick.AddListener(() =>
        {
            if (!isTurning())
            {
                int ret =(int)Main.DispEvent("level_play", PlayerData.gd.levelid);
                if (ret==1)
                {
                    hide();
                }
            }
        });
        btnsetup.onClick.AddListener(() =>
        {
            Main.DispEvent("show_setup",null);
        });
    }

    private bool isTurning()
    {
        bool ret = false;
        for(int i=0;i< chaptercontent.childCount; i++)
        {
            var c = chaptercontent.GetChild(i);
            if (c.GetComponent<card>().isTurning)
            {
                ret = true;
                break;
            }
        }
        return ret;
    }

    private void Update()
    {
        // 检测屏幕尺寸变化，自动调整chaptercontent大小
       // ResizeChapterContent();
    }
    
    private void ResizeChapterContent()
    {
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        //// 设置边界边距（最低50像素）
        float margin = 130f;
        
        //// 计算可用空间
        float availableWidth = screenWidth - 2 * margin;
        float availableHeight = screenHeight - 2 * margin;

        //// 9:16宽高比的目标宽高
        float targetWidth, targetHeight;

        //// 根据可用空间计算最佳尺寸
        if (availableWidth / availableHeight > 9f / 16f)
        {
            // 以高度为基准
            targetHeight = availableHeight;
            targetWidth = targetHeight * 9f / 16f;
        }
        else
        {
            // 以宽度为基准
            targetWidth = availableWidth;
            targetHeight = targetWidth * 16f / 9f;
        }

        float wside = (screenWidth-targetWidth)/ 2;
        float hside = (screenHeight - targetHeight) / 2;
        chaptercontent.offsetMin = new Vector2(wside, hside);  // Left和Bottom（offsetMin = (Left, Bottom)）
        chaptercontent.offsetMax = new Vector2(-wside, -hside); // Right和Top（offsetMax = (-Right, -Top)）

        //// 设置chaptercontent的尺寸
        //chaptercontent.sizeDelta = new Vector2(targetWidth, targetHeight);

        //// 设置chaptercontent居中
        //chaptercontent.anchoredPosition = Vector2.zero;
    }
   
    protected override void OnShow()
    {
        base.OnShow();
        //ResizeChapterContent();
    }
    
    private void brushChapterContent()
    {
        var chapter = datamgr.Instance.GetChapter(PlayerData.gd.currChapter);
        var pic = Resources.Load(chapter.ChapterFigure) as Texture;
        
        // 调整chaptercontent大小
        //ResizeChapterContent();
        
        // 清除现有的子对象（使用倒序删除避免遍历时修改集合的问题）
        for (int i = chaptercontent.childCount - 1; i >= 0; i--)
        {
            Transform child = chaptercontent.GetChild(i);
            // 在编辑器中使用DestroyImmediate，在运行时使用Destroy
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
            #else
            Destroy(child.gameObject);
            #endif
        }
        
        // 获取父容器的尺寸
        Vector2 rectSize = chaptercontent.rect.size;
        
        // 计算每个格子的尺寸
        float cellWidth = (rectSize.x / chapter.ChapterFigureX);
        float cellHeight = (rectSize.y / chapter.ChapterFigureY);
        

        // 外层循环为行（从上到下），内层循环为列（从左到右）
        for(int j=0; j< chapter.ChapterFigureY; j++)
        {
            for(int i=0; i< chapter.ChapterFigureX; i++)
            {
                // 索引计算修改为：行号 * 每行元素数 + 列号，确保从左到右、从上到下的顺序
                int idx = j * chapter.ChapterFigureX + i;
                int levelid = chapter.LevelId[idx];
                
                // 创建一个空的游戏对象作为格子
                GameObject cellObject = GameObject.Instantiate(Resources.Load("levelpic")) as GameObject;// new GameObject("ChapterCell_" + i + "_" + j);
                cellObject.transform.SetParent(chaptercontent, false);
                card pcard = cellObject.GetComponent<card>();
                pcard.levelid = levelid;
                // 添加RectTransform组件
                RectTransform cellRect = cellObject.GetComponent<RectTransform>();
                
                // 设置位置和尺寸
                cellRect.anchorMin = new Vector2(0, 0);
                cellRect.anchorMax = new Vector2(0, 0);
                cellRect.pivot = new Vector2(0.5f, 0);
                cellRect.anchoredPosition = new Vector2((i+0.5f) * cellWidth,(chapter.ChapterFigureY- 1 - j) * cellHeight);
                cellRect.sizeDelta = new Vector2(cellWidth-4 , cellHeight-4);
                  // 添加RawImage组件
                RawImage rawImage = cellObject.GetComponent<RawImage>();

                pcard.uvX = (float)i / chapter.ChapterFigureX;
                pcard.uvY = (float)(j) / chapter.ChapterFigureY;
                pcard.uvWidth = 1.0f / chapter.ChapterFigureX;
                pcard.uvHeight = 1.0f / chapter.ChapterFigureY;
                pcard.texture = pic;
                pcard.Load();
                pcard.level.text = levelid.ToString(); 
            }
        }
        
    }
    
    // 创建边框
    private void CreateBorders(GameObject cellObject, float cellWidth, float cellHeight)
    {
        // 边框宽度
        float borderWidth = 20f;
        
        // 为防止角与边重合，将角大小限制为单元尺寸的一半以内
        float bw = Mathf.Min(borderWidth, cellWidth / 2f, cellHeight / 2f);
        
        // 创建四个角
        CreateBorder(cellObject, "TopLeftBorder", new Vector2(0, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "TopRightBorder", new Vector2(cellWidth - bw, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomRightBorder", new Vector2(cellWidth - bw, 0), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomLeftBorder", new Vector2(0, 0), new Vector2(bw, bw));
        
        // 创建四个边，缩进到角的内部以避免重合
        CreateBorder(cellObject, "TopBorder", new Vector2(bw, cellHeight - bw), new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));
        CreateBorder(cellObject, "BottomBorder", new Vector2(bw, 0), new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));
        CreateBorder(cellObject, "LeftBorder", new Vector2(0, bw), new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
        CreateBorder(cellObject, "RightBorder", new Vector2(cellWidth - bw, bw), new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
    }
    
    // 创建单个边框
    private void CreateBorder(GameObject parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject borderObject = new GameObject(name);
        borderObject.transform.SetParent(parent.transform, false);
        
        RectTransform borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = new Vector2(0, 0);
        borderRect.anchorMax = new Vector2(0, 0);
        borderRect.pivot = new Vector2(0, 0);
        borderRect.anchoredPosition = anchoredPosition;
        borderRect.sizeDelta = sizeDelta;
        
        Image borderImage = borderObject.AddComponent<Image>();
        borderImage.color = Color.white; // 默认为白色，后续会通过BorderSpriteLoader加载实际图片
        
        // 为边框添加加载器组件
        BorderSpriteLoader loader = borderObject.AddComponent<BorderSpriteLoader>();
        loader.resourceName = MapBorderNameToResource(name);
        loader.LoadSprite();
    }
    
    // 将边框对象名映射到Resources中的贴图名
    private string MapBorderNameToResource(string borderName)
    {
        switch (borderName)
        {
            case "TopLeftBorder": return "lt";
            case "TopBorder": return "t";
            case "TopRightBorder": return "rt";
            case "RightBorder": return "r";
            case "BottomRightBorder": return "rb";
            case "BottomBorder": return "b";
            case "BottomLeftBorder": return "lb";
            case "LeftBorder": return "l";
            default: return borderName;
        }
    }
    
    // 更新边框显示状态
    private void UpdateBorderVisibility()
    {
        // 获取所有子节点（格子）
        List<RectTransform> gridItems = new List<RectTransform>();
        foreach (Transform child in chaptercontent)
        {
            if (child.name.StartsWith("ChapterCell_"))
            {
                gridItems.Add(child.GetComponent<RectTransform>());
            }
        }
        
        if (gridItems.Count == 0) return;
        
        // 计算每个格子的尺寸（假设所有格子大小相同）
        RectTransform firstItem = gridItems[0];
        Vector2 cellSize = firstItem.sizeDelta;
        
        // 对于简单的章节显示，可以简单地显示所有边框
        // 在实际应用中，可以根据相邻关系优化显示
        foreach (RectTransform item in gridItems)
        {
            // 获取边框对象
            GameObject topLeftBorder = FindChildByName(item.gameObject, "TopLeftBorder");
            GameObject topRightBorder = FindChildByName(item.gameObject, "TopRightBorder");
            GameObject bottomRightBorder = FindChildByName(item.gameObject, "BottomRightBorder");
            GameObject bottomLeftBorder = FindChildByName(item.gameObject, "BottomLeftBorder");
            GameObject topBorder = FindChildByName(item.gameObject, "TopBorder");
            GameObject bottomBorder = FindChildByName(item.gameObject, "BottomBorder");
            GameObject leftBorder = FindChildByName(item.gameObject, "LeftBorder");
            GameObject rightBorder = FindChildByName(item.gameObject, "RightBorder");
            
            // 设置边框显示状态
            if (topLeftBorder != null) topLeftBorder.SetActive(true);
            if (topRightBorder != null) topRightBorder.SetActive(true);
            if (bottomRightBorder != null) bottomRightBorder.SetActive(true);
            if (bottomLeftBorder != null) bottomLeftBorder.SetActive(true);
            if (topBorder != null) topBorder.SetActive(true);
            if (bottomBorder != null) bottomBorder.SetActive(true);
            if (leftBorder != null) leftBorder.SetActive(true);
            if (rightBorder != null) rightBorder.SetActive(true);
        }
    }
    
    // 根据名称查找子对象
    private GameObject FindChildByName(GameObject parent, string name)
    {
        Transform child = parent.transform.Find(name);
        return child != null ? child.gameObject : null;
    }
    
    // 更新power显示
    private void UpdateStaminaDisplay()
    {
        if (staminaText != null)
        {
            staminaText.text = $"{PlayerData.gd.power}/100";
            float v = (float)PlayerData.gd.power / 100f;
            fillimg.fillAmount = v;
        }
    }
}
