using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();
        var x = target as card;
        if (GUILayout.Button("show"))
        {
            x.show();
        }
        if (GUILayout.Button("hide"))
        {
            x.hide();
        }
    }
}
