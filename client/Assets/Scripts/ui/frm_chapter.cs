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
    public TextMeshProUGUI staminaText,levelname; // power显示文本组件
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
        Main.RegistEvent("level_back", (x) =>
        {
            show();
            return null;
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

        levelname.text= $"Level { PlayerData.gd.levelid%10000}";
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
     
    // 更新power显示
 private void UpdateStaminaDisplay()
{
    if (staminaText != null)
    {
        string powerText;
        if (PlayerData.gd.power < 1000)
        {
            powerText = PlayerData.gd.power.ToString();
        }
        else
        {
            float kValue = PlayerData.gd.power / 1000f;
            powerText = $"{kValue:F1}K";
        }
        
        staminaText.text = $"{powerText}";
        float v = (float)PlayerData.gd.power / 100f;
        //fillimg.fillAmount = v;
    }
}
}
