using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(goldplay))]
public class goldplayeditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        goldplay goldplay = (goldplay)target;
        if (GUILayout.Button("Goldplay"))
        {
            goldplay.Play();
        }
    }
}
