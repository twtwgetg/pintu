using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class frm_game : frmbase
{
    public picmgr mgr;
    public TextMeshProUGUI level;
    public Button next;
    private void Awake()
    {
        Main.RegistEvent("level_play", (x) =>
        {

            next.gameObject.SetActive(false);
            var rectTrans =mgr.GetComponent<RectTransform>();
            // 2. 设置边距（Left/Right/Top/Bottom）：与图片中数值一致
            rectTrans.offsetMin = new Vector2(82.82125f, 124.4235f);  // Left和Bottom（offsetMin = (Left, Bottom)）
            rectTrans.offsetMax = new Vector2(-82.82125f, -208.2435f); // Right和Top（offsetMax = (-Right, -Top)）
            mgr.ResizeChapterContent();
            var leevel = datamgr.Instance.GetLevel((int)x);
            level.text = $"Level { leevel.Id}";
            show();
            StartCoroutine(load(leevel));
            return 1;
        });
        Main.RegistEvent("show_next", (x) =>
        {
            next.gameObject.SetActive(true);
            return 1;
        });
        next.onClick.AddListener(() =>
        {
            Main.SendEvent("level_next");
            hide();
        });
    }
    IEnumerator load(cfg.DrLevel leevel)
    {

        yield return StartCoroutine(mgr.LoadLevel(leevel)); 
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
