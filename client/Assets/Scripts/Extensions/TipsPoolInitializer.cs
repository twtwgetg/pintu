using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Tips对象池初始化器
/// 用于在运行时创建tips预制件并初始化对象池
/// </summary>
public class TipsPoolInitializer : MonoBehaviour
{
    [Header("Tips UI设置")]
    public Font font;
    public Color normalTipsColor = Color.white;
    public Color wallTipsColor = Color.red;
    public int fontSize = 36;
    
    [Header("对象池设置")]
    public int initialNormalTipsCount = 10;
    public int initialWallTipsCount = 5;
    
    private TipsPoolManager tipsPoolManager;
    
    void Awake()
    {
        InitializeTipsPool();
    }
    
    /// <summary>
    /// 初始化Tips对象池
    /// </summary>
    private void InitializeTipsPool()
    {
        // 获取或创建TipsPoolManager实例
        tipsPoolManager = TipsPoolManager.Instance;
        
        // 创建普通提示预制件
        if (tipsPoolManager.normalTipsPrefab == null)
        {
            GameObject normalTipsPrefab = CreateTipsPrefab("NormalTipsPrefab", normalTipsColor);
            tipsPoolManager.normalTipsPrefab = normalTipsPrefab;
        }
        
        // 创建城墙提示预制件
        if (tipsPoolManager.wallTipsPrefab == null)
        {
            GameObject wallTipsPrefab = CreateTipsPrefab("WallTipsPrefab", wallTipsColor);
            tipsPoolManager.wallTipsPrefab = wallTipsPrefab;
        }
        
        // 初始化对象池
        tipsPoolManager.InitializePools();
        
        // 预加载对象
        tipsPoolManager.PreloadTips(initialNormalTipsCount, initialWallTipsCount);
    }
    
    /// <summary>
    /// 创建Tips预制件
    /// </summary>
    /// <param name="name">预制件名称</param>
    /// <param name="color">文本颜色</param>
    /// <returns>创建的预制件对象</returns>
    private GameObject CreateTipsPrefab(string name, Color color)
    {
        // 创建空的游戏对象
        GameObject tipsPrefab = new GameObject(name);
        
        // 添加RectTransform组件
        RectTransform rectTransform = tipsPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);
        
        // 添加CanvasRenderer组件
        tipsPrefab.AddComponent<CanvasRenderer>();
        
        // 添加TextMeshProUGUI组件
        TextMeshProUGUI textMeshPro = tipsPrefab.AddComponent<TextMeshProUGUI>();
        textMeshPro.text = "";
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = color;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        
        // 如果指定了字体，则设置字体
        if (font != null)
        {
            // 注意：TMP字体设置需要使用TMP_FontAsset，而不是普通Font
            // 这里仅作示意，实际使用时需要TMP字体资源
        }
        
        return tipsPrefab;
    }
}