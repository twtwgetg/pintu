using UnityEditor;
using UnityEngine;

public class DebugModeTools : EditorWindow
{
    // 使用DebugManager中的静态变量来存储调试模式状态
    // 这样运行时脚本也可以访问
    
    [MenuItem("Tools/调试模式", false, 0)]
    private static void ToggleDebugMode()
    {
        bool newState = !DebugManager.IsDebugMode;
        DebugManager.IsDebugMode = newState;
        
        // 保存设置到EditorPrefs
        EditorPrefs.SetBool("DebugModeEnabled", newState);
        
        // 显示调试信息
        Debug.Log("调试模式已" + (DebugManager.IsDebugMode ? "开启" : "关闭"));
        
        // 重新绘制菜单，以便更新勾选状态
        //EditorApplication.RepaintMenus();
    }
    
    // 在菜单中显示勾选状态
    [MenuItem("Tools/调试模式", true)]
    private static bool ToggleDebugModeValidate()
    {
        Menu.SetChecked("Tools/调试模式", DebugManager.IsDebugMode);
        return true;
    }
}