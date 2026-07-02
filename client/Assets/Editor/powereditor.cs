using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(power))]
public class powereditor : Editor
{
    private SerializedProperty valueProperty;

    private void OnEnable()
    {
        valueProperty = serializedObject.FindProperty("value");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(valueProperty, new GUIContent("Value", "0 shows all images, 1 hides all images."));
        if (EditorGUI.EndChangeCheck())
        {
            valueProperty.floatValue = Mathf.Clamp01(valueProperty.floatValue);
            serializedObject.ApplyModifiedProperties();
            RefreshTargets();
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }

        power powerTarget = (power)target;
        int imageCount = CountImages(powerTarget.transform);
        int showCount = GetShowCount(valueProperty.floatValue, imageCount);
        int activeCount = CountActiveImages(powerTarget.transform);

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("Image State", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Direct Child Images", imageCount.ToString());
        EditorGUILayout.LabelField("Active Images", activeCount + "/" + imageCount);

        Rect previewRect = GUILayoutUtility.GetRect(1f, 24f);
        DrawPreview(previewRect, imageCount, showCount);

        if (imageCount != 5)
        {
            EditorGUILayout.HelpBox("Direct child Image count is not 5. Please check the power prefab structure.", MessageType.Warning);
        }

        EditorGUILayout.Space(6f);
        DrawQuickButtons(imageCount);

        if (GUILayout.Button("Refresh View"))
        {
            RefreshTargets();
        }
    }

    private void DrawQuickButtons(int imageCount)
    {
        EditorGUILayout.LabelField("Quick Set", EditorStyles.boldLabel);

        int count = Mathf.Max(imageCount, 5);
        for (int row = 0; row < 2; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int column = 0; column < 3; column++)
            {
                int visibleCount = count - (row * 3 + column);
                if (visibleCount < 0)
                {
                    GUILayout.FlexibleSpace();
                    continue;
                }

                if (GUILayout.Button(visibleCount + "/" + count))
                {
                    SetValue(count == 0 ? 1f : 1f - (float)visibleCount / count);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void SetValue(float value)
    {
        Undo.RecordObjects(targets, "Set Power Value");
        foreach (Object item in targets)
        {
            power powerItem = (power)item;
            powerItem.Value = value;
            EditorUtility.SetDirty(powerItem);
        }

        serializedObject.Update();
    }

    private void RefreshTargets()
    {
        foreach (Object item in targets)
        {
            power powerItem = (power)item;
            powerItem.Refresh();
            EditorUtility.SetDirty(powerItem);
        }
    }

    private static int CountImages(Transform root)
    {
        int count = 0;
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).GetComponent<Image>() != null)
            {
                count++;
            }
        }

        return count;
    }

    private static int CountActiveImages(Transform root)
    {
        int count = 0;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.GetComponent<Image>() != null && child.gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    private static int GetShowCount(float value, int imageCount)
    {
        return Mathf.Clamp(Mathf.CeilToInt((1f - value) * imageCount), 0, imageCount);
    }

    private static void DrawPreview(Rect rect, int imageCount, int showCount)
    {
        if (imageCount <= 0)
        {
            EditorGUI.LabelField(rect, "No direct child Image found.");
            return;
        }

        float gap = 3f;
        float width = (rect.width - gap * (imageCount - 1)) / imageCount;
        for (int i = 0; i < imageCount; i++)
        {
            Rect itemRect = new Rect(rect.x + (width + gap) * i, rect.y + 3f, width, rect.height - 6f);
            EditorGUI.DrawRect(itemRect, i < showCount ? new Color(0.25f, 0.75f, 0.35f) : new Color(0.28f, 0.28f, 0.28f));
        }
    }
}
