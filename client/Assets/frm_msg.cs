using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class frm_msg : frmbase
{
    public GameObject msg;
    private void Awake()
    {
        Main.RegistEvent("event_msg", (x) =>
         { 
             var xx = x as string;
             var t = GameObject.Instantiate(msg,gb);
             t.SetActive(true);
             t.GetComponent<TextMeshProUGUI>().text = xx;
             
             // 初始缩放为0
             t.transform.localScale = Vector3.zero;
             // 从0到1的缩放动画
             t.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
             {
                 // 停留0.5秒后执行后续动画
                 DOVirtual.DelayedCall(0.5f, () =>
                 {
                     // 向上移动并淡出的动画
                     var r = t.GetComponent<RectTransform>();
                     var pos = r.anchoredPosition + new Vector2(0, 50); 
                     t.GetComponent<RectTransform>().DOAnchorPos(pos, 0.5f);
                     t.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(() =>
                     {
                         // 动画结束后销毁对象
                         Destroy(t);
                     });
                 });
             });

             return 1;
         });
    }
}
