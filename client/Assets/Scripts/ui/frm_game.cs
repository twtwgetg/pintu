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
            if (PlayerData.DebugForcePowerZero)
            {
                PlayerData.gd.power = 0;
            }

            // 检查并消耗power
            if (!PlayerData.gd.hasEnoughpower(10))
            {
                Debug.Log("power不足，无法开始游戏");
                Main.DispEvent("show_rewarded_power");
                return 0;
            }
            
            PlayerData.gd.消耗power(10);

            next.gameObject.SetActive(false);
            level.gameObject.SetActive(false);
            back.gameObject.SetActive(false);
            setup.gameObject.SetActive(false);

            mgr.ResizeChapterContent();
            var leevel = GalleryManager.GetLevel((int)x);
            level.text = $"当前关卡 {leevel.id / 100000}-{leevel.id % 100000}";

            Main.DispEvent("event_loading", true);
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
        Main.RegistEvent("show_victory", (a) =>
        {
            hide();
            return null;
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
    IEnumerator load(GalleryLevel leevel)
    {
        yield return StartCoroutine(mgr.LoadLevel(leevel));

        level.gameObject.SetActive(true);
        back.gameObject.SetActive(true);
        setup.gameObject.SetActive(true);

        if (leevel.difficulty == 2)
        {
            var df = gb.Find("diff");
            df.gameObject.SetActive(true);
            DG.Tweening.DOVirtual.Float(0, 1, .5f, (xvx) =>
            {
                df.GetComponent<CanvasGroup>().alpha = xvx;
            }).onComplete = () =>
            {
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
