using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DraggableGridItem))]
public class DraggableGridItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();

        // 绘制每个可见属性，但将 adjacency 字段以只读标签形式显示
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            // 禁用对脚本字段的编辑
            if (prop.name == "m_Script")
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(prop, true);
                EditorGUI.EndDisabledGroup();
                continue;
            }

            if (prop.name == "adjacentLeft" || prop.name == "adjacentRight" || prop.name == "adjacentTop" || prop.name == "adjacentBottom")
            {
                // 以标签形式显示布尔值，避免用户误改
                string niceName = ObjectNames.NicifyVariableName(prop.name);
                EditorGUILayout.LabelField(niceName, prop.boolValue ? "True" : "False");
            }
            else
            {
                EditorGUILayout.PropertyField(prop, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
