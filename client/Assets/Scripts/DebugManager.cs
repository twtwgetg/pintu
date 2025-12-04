using UnityEngine;

public class DebugManager : MonoBehaviour
{
    // 调试模式状态，可在运行时和编辑器中访问
    public static bool IsDebugMode = false;
    
    // 编辑器启动时初始化调试模式状态
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeDebugMode()
    {
#if UNITY_EDITOR
        // 从EditorPrefs加载调试模式状态
        IsDebugMode = UnityEditor.EditorPrefs.GetBool("DebugModeEnabled", false);
#endif
    }
}