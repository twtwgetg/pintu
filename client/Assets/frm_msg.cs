using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class frm_msg : frmbase
{
    public GameObject msg;
    
    public GameObject  panel_loading;
    public TextMeshProUGUI txt;
    private IEnumerator LoadingTextAnim()
    { 
        WaitForSeconds delay = new WaitForSeconds(0.18f);
        while (true)
        {
            txt.ForceMeshUpdate();
            var textInfo = txt.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int matIdx = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIdx = textInfo.characterInfo[i].vertexIndex;
                Vector3[] verts = textInfo.meshInfo[matIdx].vertices;

                Vector3 orig0 = verts[vertIdx];
                Vector3 orig1 = verts[vertIdx + 1];
                Vector3 orig2 = verts[vertIdx + 2];
                Vector3 orig3 = verts[vertIdx + 3];
                Vector3 center = (orig0 + orig2) * 0.5f;

                float elapsed = 0;
                while (elapsed < 0.35f)
                {
                    elapsed += Time.deltaTime;
                    float p = Mathf.PingPong(elapsed, 0.35f / 2f) / (0.35f / 2f);
                    float s = 1f + p * 0.45f;
                    verts[vertIdx] = center + (orig0 - center) * s;
                    verts[vertIdx + 1] = center + (orig1 - center) * s;
                    verts[vertIdx + 2] = center + (orig2 - center) * s;
                    verts[vertIdx + 3] = center + (orig3 - center) * s;
                    txt.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    yield return null;
                }

                verts[vertIdx] = orig0;
                verts[vertIdx + 1] = orig1;
                verts[vertIdx + 2] = orig2;
                verts[vertIdx + 3] = orig3;
                txt.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                yield return delay;
            }
        }
    }
    private Coroutine loadingAnim;
    private void Awake()
    {
        Main.RegistEvent("event_loading", (a) => {
            bool set = (bool)a;
            panel_loading.gameObject.SetActive(set);
            if (set)
            {
                if (loadingAnim == null)
                    loadingAnim = StartCoroutine(LoadingTextAnim());
            }
            else
            {
                if (loadingAnim != null)
                {
                    StopCoroutine(loadingAnim);
                    loadingAnim = null;
                    txt.ForceMeshUpdate();
                }
            }
            return 1;
        });
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
