using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class frm_setup : frmbase
{
    public Button btnInitialize;
    public Button btnClose;
    
    private void Awake()
    {
        // 注册按钮点击事件
        btnInitialize.onClick.AddListener(OnInitializeClick);
        btnClose.onClick.AddListener(OnCloseClick);
        
        // 监听显示事件
        Main.RegistEvent("show_setup", (x) =>
        {
            show();
            return 1;
        });
    }
    
    /// <summary>
    /// 关闭设置界面
    /// </summary>
    private void OnCloseClick()
    {
        hide();
    }
    
    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void OnInitializeClick()
    {
        try
        {
            // 删除playerdata.json文件
            string filePath = Application.persistentDataPath + "/playerData.json";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Player data file deleted successfully.");
            }
            
            // 重新加载数据
            PlayerData playerData = FindObjectOfType<PlayerData>();
            if (playerData != null)
            {
                // 调用playerData的loadData方法重新加载数据
                playerData.SendMessage("loadData");
                Debug.Log("Player data reloaded successfully.");
            }
            
            // 触发事件通知其他组件数据已更新
            Main.DispEvent("onLevelChange");
            
            Debug.Log("Game initialized successfully.");
            Application.Quit();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize game data: " + e.Message);
        }
    }
}
