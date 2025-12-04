using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using cfg;
using System;

/// <summary>
/// 数据管理器 - 用于加载和管理Luban配置表
/// </summary>
public class datamgr : MonoBehaviour
{
    [Header("数据文件")]
    public TextAsset tbchapter;
    public TextAsset tblevel; 
    private static datamgr _instance;
    public static datamgr Instance
    {
        get
        { 
            if (_instance == null)
            {
                var x  =Instantiate( Resources.Load("DataManager")) as GameObject ;
                _instance = x.GetComponent<datamgr>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Luban配置表集合
    /// </summary>
    public Tables Tables { get; private set; }

    /// <summary>
    /// 配置表是否已加载
    /// </summary>
    public bool IsLoaded { get; private set; } = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfigTables();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 加载所有配置表
    /// </summary>
    public void LoadConfigTables()
    {
        Debug.Log("开始加载配置表...");
        
        try
        {
            // 定义JSON数据加载器
            System.Func<string, JSONNode> loader = (string file) =>
            {
                // 根据文件名选择对应的数据源
                TextAsset dataAsset = null;
                switch (file)
                {
                    case "tbchapter":
                        dataAsset = tbchapter;
                        break;
                    case "tblevel":
                        dataAsset = tblevel;
                        break;
                    default:
                        Debug.LogError($"没有找到对应的数据源: {file}");
                        break;
                }
                
                // 首先尝试从TextAsset加载（编辑器中拖拽的数据文件）
                if (dataAsset != null && !string.IsNullOrEmpty(dataAsset.text))
                {
                    return JSON.Parse(dataAsset.text);
                }
                
                // 如果没有拖拽数据文件，则从文件系统加载
                string filePath = Path.Combine(Application.dataPath, "data", file + ".json");
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    return JSON.Parse(content);
                }
                else
                {
                    Debug.LogError($"配置文件不存在: {filePath}");
                    return null;
                }
            };
            
            // 初始化配置表
            Tables = new Tables(loader);
            IsLoaded = true;
            
            Debug.Log("配置表加载完成!");
            LogTableInfo();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"配置表加载失败: {e.Message}");
            IsLoaded = false;
        }
    }

    /// <summary>
    /// 重新加载配置表
    /// </summary>
    public void ReloadConfigTables()
    {
        IsLoaded = false;
        Tables = null;
        LoadConfigTables();
    }

    /// <summary>
    /// 输出配置表信息
    /// </summary>
    private void LogTableInfo()
    {
        if (Tables == null) return;

        Debug.Log("=== 配置表信息 ===");
        Debug.Log($"章节表: {Tables.TbChapter?.DataList?.Count ?? 0}");
        Debug.Log($"关卡表: {Tables.TbLevel?.DataList?.Count ?? 0}"); 
        Debug.Log("================");
    }

    #region 便利方法 - 获取配置表
     
    public TbChapter GetTbChapter()
    {
        return Tables?.TbChapter;
    }

    public TbLevel GetTbLevel()
    {
        return Tables?.TbLevel;
    }
    #endregion

    #region 便利方法 - 根据ID获取数据

    /// <summary>
    /// 根据ID获取关卡数据
    /// </summary>
    public DrChapter GetChapter(int id)
    {
        return Tables?.TbChapter?.GetOrDefault(id);
    }

    internal List<DrChapter> GetChapters()
    {
        return Tables?.TbChapter?.DataList;
    }

    /// <summary>
    /// 根据ID获取波次数据
    /// </summary>
    public DrLevel GetLevel(int id)
    {
        return Tables?.TbLevel?.GetOrDefault(id);
    }
     

    #endregion
 
}