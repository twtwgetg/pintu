using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(frmbase),true)]
public class frmbaseeditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("显示"))
        {
            frm.show();
        }
        if (GUILayout.Button("隐藏"))
        {
            frm.hide();
        }
        //if (GUILayout.Button("addString"))
        //{
        //    var sx = frm.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        //    for(int i=0;i<sx.Length;i++)
        //    {
        //        var x = sx[i];
        //        var tx = x.GetComponent<stringinfo>();
        //        if (tx == null)
        //        {
        //            tx = x.gameObject.AddComponent<stringinfo>();
        //        }
        //        tx.getKey();
        //    }
        //}
        //if (GUILayout.Button("英文"))
        //{
        //    LangData.SetLanguage(LangData.Language.English);
        //    frm.loadString();
        //}
        //if (GUILayout.Button("中文"))
        //{
        //    LangData.SetLanguage(LangData.Language.Chinese);
        //    frm.loadString();
        //}
    }
    frmbase frm
    {
        get
        {
            return target as frmbase;
        }
    }
}
