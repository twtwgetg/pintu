using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class frm_tips : frmbase
{
    public GameObject text;
    private void Awake()
    {
        Main.RegistEvent("event_tip", (x) =>
         {
             var xgb = GameObject.Instantiate(this.gameObject,gb);
             xgb.gameObject.SetActive(true);
             xgb.GetComponent<TextMeshProUGUI>().text = x as string;
             xgb.transform.DOScale(Vector3.one,0.5f).OnComplete(()=>
             {
                 xgb.transform.DOScale(Vector3.zero,0.5f).OnComplete(()=>
                 {
                     GameObject.Destroy(xgb);
                 });
                 xgb.GetComponent<RectTransform>().DOMoveY(20,0.5f).SetEase(Ease.OutBack);
             });
             return 1;
         });
    } 
}
