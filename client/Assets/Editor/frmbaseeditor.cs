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
        
        // 检测是否存在gb节点
        Transform gbTransform = frm.transform.Find("Root");
        if (gbTransform == null)
        {
            // 如果没有gb节点，显示初始化按钮
            if (GUILayout.Button("初始化 - 添加gb节点"))
            {
                InitializeGBNode();
            }
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
    
    /// <summary>
    /// 初始化gb节点
    /// </summary>
    private void InitializeGBNode()
    {
        // 创建gb节点
        GameObject gbObj = new GameObject("gb");
        gbObj.transform.SetParent(frm.transform, false);
        
        // 添加RectTransform组件
        RectTransform rectTransform = gbObj.AddComponent<RectTransform>();
        
        // 设置全屏参数
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        
        Debug.Log("已创建gb节点并设置为全屏");
    }
    
    frmbase frm
    {
        get
        {
            return target as frmbase;
        }
    }
}
