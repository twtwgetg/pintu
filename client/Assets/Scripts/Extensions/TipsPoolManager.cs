using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Tips对象池管理器
/// </summary>
public class TipsPoolManager : MonoBehaviour
{
    [Header("普通提示预制体")]
    public GameObject normalTipsPrefab;
    
    [Header("城墙提示预制体")]
    public GameObject wallTipsPrefab;
    
    [Header("对象池设置")]
    public int initialPoolSize = 10;
    public int maxPoolSize = 50;
    
    private GameObjectPoolManager normalTipsPool;
    private GameObjectPoolManager bigTipsPool;
    private Transform poolParent;
    
    private static TipsPoolManager _instance;
    public static TipsPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TipsPoolManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("TipsPoolManager");
                    _instance = obj.AddComponent<TipsPoolManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // 初始化对象池将在InitializePools方法中进行
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化对象池
    /// </summary>
    public void InitializePools()
    {
        // 如果已经初始化过，则先清理
        if (poolParent != null)
        {
            Destroy(poolParent.gameObject);
        }
        
        // 创建池的父物体
        poolParent = new GameObject("TipsPool").transform;
        poolParent.SetParent(transform);
        
        // 初始化普通提示对象池
        if (normalTipsPrefab != null)
        {
            normalTipsPool = new GameObjectPoolManager(normalTipsPrefab, initialPoolSize, poolParent, true, maxPoolSize);
        }
        
        // 初始化城墙提示对象池
        if (wallTipsPrefab != null)
        {
            bigTipsPool = new GameObjectPoolManager(wallTipsPrefab, initialPoolSize, poolParent, true, maxPoolSize);
        }
    }
    
    /// <summary>
    /// 获取普通提示对象
    /// </summary>
    /// <returns>普通提示对象</returns>
    public GameObject GetNormalTips()
    {
        if (normalTipsPool != null)
        {
            return normalTipsPool.GetObject();
        }
        return null;
    }

    /// <summary>
    /// 获取大提示，红色的
    /// </summary>
    /// <returns>红色的对象</returns>
    public GameObject GetBigTips()
    {
        if (bigTipsPool != null)
        {
            return bigTipsPool.GetObject();
        }
        return null;
    }
    
    /// <summary>
    /// 回收普通提示对象
    /// </summary>
    /// <param name="tipsObject">要回收的普通提示对象</param>
    public void ReturnNormalTips(GameObject tipsObject)
    {
        if (normalTipsPool != null && tipsObject != null)
        {
            // 重置TextMeshPro组件的内容和属性
            var textMeshPro = tipsObject.GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = "";
                textMeshPro.fontSize = 36; // 重置为默认小字体大小
            }
            tipsObject.transform.localScale = Vector3.one;
            normalTipsPool.ReturnObject(tipsObject);
        }
    }
    
    /// <summary>
    /// 回收城墙提示对象
    /// </summary>
    /// <param name="tipsObject">要回收的城墙提示对象</param>
    public void ReturnBigTips(GameObject tipsObject)
    {
        if (bigTipsPool != null && tipsObject != null)
        {
            // 重置TextMeshPro组件的内容和属性
            var textMeshPro = tipsObject.GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = "";
                textMeshPro.fontSize = 36; // 重置为默认小字体大小
            }
            tipsObject.transform.localScale = Vector3.one;
            bigTipsPool.ReturnObject(tipsObject);
        }
    }
    
    /// <summary>
    /// 预加载指定数量的对象
    /// </summary>
    /// <param name="normalCount">普通提示预加载数量</param>
    /// <param name="wallCount">城墙提示预加载数量</param>
    public void PreloadTips(int normalCount, int wallCount)
    {
        if (normalTipsPool != null)
        {
            normalTipsPool.Preload(normalCount);
        }
        
        if (bigTipsPool != null)
        {
            bigTipsPool.Preload(wallCount);
        }
    }
}