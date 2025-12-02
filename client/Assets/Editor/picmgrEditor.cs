using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(picmgr))]
public class picmgrEditor : Editor
{
    // 难度参数相关字段
    private int level = 1;
    private int maxKeepCount = -1;
    private bool isHard = false;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        picmgr picManager = (picmgr)target;

        EditorGUILayout.Space();
        
        // 显示难度参数设置
        EditorGUILayout.LabelField("难度设置", EditorStyles.boldLabel);
        level = EditorGUILayout.IntField("关卡编号", level);
        maxKeepCount = EditorGUILayout.IntField("最多原位数量 (-1为默认)", maxKeepCount);
        isHard = EditorGUILayout.Toggle("是否为困难模式", isHard);
        
        EditorGUILayout.Space();

        if (GUILayout.Button("生成网格图像"))
        {
            picManager.tCreateGridImages(level, maxKeepCount, isHard);
        }
        
        if (GUILayout.Button("打乱网格位置"))
        {
            picManager.ShuffleGridPositions(level, maxKeepCount, isHard);
        }
        
        if (GUILayout.Button("更新边框显示"))
        {
            picManager.UpdateBorderVisibility();
        }

        EditorGUILayout.HelpBox("点击上方按钮来生成网格图像。这将在当前对象下创建width*height个子对象，每个子对象包含一个RawImage组件，显示原始图片的不同部分。\n点击\"打乱网格位置\"按钮可以根据设置的参数重新排列这些网格的位置。\n生成的每个子节点都带有拖拽功能，可以拖拽节点进行位置交换。\n点击\"更新边框显示\"按钮可以手动更新边框的显示状态。", MessageType.Info);
    }
}