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
    public Button back;
    public Button setup;
    private void Awake()
    {
        Main.RegistEvent("level_play", (x) =>
        {
            // 检查并消耗power
            if (!PlayerData.gd.hasEnoughpower(10))
            {
                Debug.Log("power不足，无法开始游戏");
                // 可以在这里添加power不足的提示
                Main.DispEvent("event_msg", "power不足，无法开始游戏");
                return 0;
            }
            
            // 消耗10点power
            PlayerData.gd.消耗power(10);
            
            next.gameObject.SetActive(false);
            var rectTrans =mgr.GetComponent<RectTransform>();
            // 2. 设置边距(Left/Right/Top/Bottom)和图片大小一致
            rectTrans.offsetMin = new Vector2(82.82125f, 124.4235f);  // Left和Bottom，offsetMin = (Left, Bottom)
            rectTrans.offsetMax = new Vector2(-82.82125f, -208.2435f); // Right和Top，offsetMax = (-Right, -Top)
            mgr.ResizeChapterContent();
            var leevel = datamgr.Instance.GetLevel((int)x);
            level.text = $"Level { leevel.Id}";
            if (leevel.DifficultyTier == 2)
            {
                var df = gb.Find("diff");
                df.gameObject.SetActive(true);
                DG.Tweening.DOVirtual.Float(0, 1, .5f, (xvx) =>
                {
                    df.GetComponent<CanvasGroup>().alpha = xvx;

                }).onComplete=()=> { 
                    
                    DG.Tweening.DOVirtual.DelayedCall(1, () =>
                    {
                        DG.Tweening.DOVirtual.Float(1, 0, .5f, (xvx) =>
                        {
                            df.GetComponent<CanvasGroup>().alpha = xvx;
                        }).onComplete = () =>
                        {
                            df.gameObject.SetActive(false);
                        };
                    });
                };
            }
            show();
            StartCoroutine(load(leevel));
            return 1;
        });
        Main.RegistEvent("level_next", (x) =>
        {
            hide();
            return null;
        });
        next.onClick.AddListener(() =>
        {
            Main.SendEvent("level_next");
            hide();
        });
        back.onClick.AddListener(() =>
        {
            Main.SendEvent("level_back");
            hide();
        });
        setup.onClick.AddListener(() =>
        {
            Main.DispEvent("show_setup");
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
