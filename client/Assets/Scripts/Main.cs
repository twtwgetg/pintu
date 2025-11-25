using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening; // 添加DOTween引用

public class Main : MonoBehaviour
{
    public delegate object registfun(object parm);
    // Start is called before the first frame update
    public static Main inst;
    public Material matboader;
    // 添加游戏暂停状态变量
    private static bool isGamePaused = false;
    
    // 添加公共属性来访问暂停状态
    public static bool IsPaused
    {
        get { return isGamePaused; }
        set
        {
            isGamePaused = value;
        }
    }

    internal static void SendEvent(string v)
    {
        DispEvent(v);
    }

    public static void InitGame()
    {
        try
        {
            // 获取持久化数据路径
            string persistentDataPath = Application.persistentDataPath;

            // 检查目录是否存在
            if (Directory.Exists(persistentDataPath))
            {
                // 删除目录下的所有文件
                string[] files = Directory.GetFiles(persistentDataPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                    Debug.Log("已删除文件: " + file);
                }

                // 删除目录下的所有子目录
                string[] directories = Directory.GetDirectories(persistentDataPath);
                foreach (string directory in directories)
                {
                    Directory.Delete(directory, true);
                    Debug.Log("已删除目录: " + directory);
                }

                Debug.Log("游戏数据初始化完成，所有持久化数据已清除。");
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("初始化完成", "游戏数据已成功初始化，所有持久化数据已清除。", "确定");
#endif
            }
            else
            {
                Debug.LogWarning("持久化数据目录不存在: " + persistentDataPath);
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("初始化失败", "持久化数据目录不存在。", "确定");
#endif
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("初始化游戏数据时发生错误: " + e.Message);
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("初始化失败", "初始化过程中发生错误，请查看控制台日志。", "确定");
#endif
        }
    }

    // 当前关卡和波次信息
    private static int currentLevel = 1;
    private static int currentWave = 0;
    
    // 添加技能待释放状态标记
    private static bool isSkillPending = false;
    
    // 点击效果预制体
    public GameObject clickEffectPrefab;
    
    private void Awake()
    {
        inst = this;
        
        // 初始化DOTween
        DOTween.Init();
        Debug.Log("Main.Awake: DOTween已初始化");
        
        // 注册游戏重新开始事件
        RegistEvent("game_restart", (object parm) =>
        {
            RestartLevel();
            return null;
        });
    }
    
    void Start()
    {
        float bl = ((float)Screen.width) / Screen.height;
        Screen.SetResolution((int)(1920*bl),1920,false);
        DispEvent("gamebegin");
    }
    
    static Dictionary<string, List<registfun>> evs = new();
    public static object DispEvent(string ev, object parm = null)
    {

        if (evs.ContainsKey(ev))
        {
            for (int i = 0; i < evs[ev].Count; i++)
            {
                var p = evs[ev][i](parm);
                if (p != null)
                {
                    return p;
                }
            }
           
            return null;
        }
        else
        {

            Debug.LogError("没有处理消息" + ev);
            return null;
        }
    }
    public static void RegistEvent(string ev, registfun fun)
    {
        if (evs.ContainsKey(ev))
        {
            //Debug.LogError("已经注册消息" + ev);
        }
        else
        {
            evs[ev] = new List<registfun>();
        }
        evs[ev].Add(fun);
    }
    public static void UnRegistEvent(string ev, registfun fun)
    {
        if (evs.ContainsKey(ev))
        {
            evs[ev].Remove(fun);
        }
    }
    
    // 添加暂停游戏的方法
    public static void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("游戏已暂停");
    }
    
    // 添加恢复游戏的方法
    public static void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("游戏已恢复");
    }
    
    // 添加获取游戏暂停状态的方法
    public static bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    // 添加重新开始关卡的方法
    public static void RestartLevel()
    {
        Debug.Log("重新开始关卡");
      
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }
}