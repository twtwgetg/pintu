using System.Collections;
using TMPro;
using UnityEngine;

public class frm_login : frmbase
{
    public TextMeshProUGUI txt;
    private Coroutine anim;
    private Coroutine delayCoroutine;

    private void Awake()
    { 

        Main.RegistEvent("login_begin", (x) =>
        {
            show();
            StartAnim();
            if (delayCoroutine != null) StopCoroutine(delayCoroutine);
            delayCoroutine = StartCoroutine(OpenChapterAfterDelay());
            return 1;
        });
    }

    private void StartAnim()
    {
        if (txt == null) return;
        StopAnim();
        anim = StartCoroutine(AnimLoop());
    }

    private void StopAnim()
    {
        if (anim != null) { StopCoroutine(anim); anim = null; }
        if (txt != null) txt.ForceMeshUpdate();
    }

    private IEnumerator AnimLoop()
    {
        var wait = new WaitForSeconds(0.18f);
        while (true)
        {
            txt.ForceMeshUpdate();
            var info = txt.textInfo;
            for (int i = 0; i < info.characterCount; i++)
            {
                if (!info.characterInfo[i].isVisible) continue;
                int mi = info.characterInfo[i].materialReferenceIndex;
                int vi = info.characterInfo[i].vertexIndex;
                var verts = info.meshInfo[mi].vertices;
                var o0 = verts[vi]; var o1 = verts[vi + 1];
                var o2 = verts[vi + 2]; var o3 = verts[vi + 3];
                var c = (o0 + o2) * 0.5f;
                float e = 0;
                while (e < 0.35f)
                {
                    e += Time.deltaTime;
                    float s = 1f + Mathf.PingPong(e, 0.175f) / 0.175f * 0.45f;
                    verts[vi] = c + (o0 - c) * s; verts[vi + 1] = c + (o1 - c) * s;
                    verts[vi + 2] = c + (o2 - c) * s; verts[vi + 3] = c + (o3 - c) * s;
                    txt.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    yield return null;
                }
                verts[vi] = o0; verts[vi + 1] = o1;
                verts[vi + 2] = o2; verts[vi + 3] = o3;
                txt.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                yield return wait;
            }
        }
    }

    IEnumerator OpenChapterAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StopAnim();
        object result = Main.DispEvent("gamebegin");
        if (result is int ret && ret == 1) hide();
        delayCoroutine = null;
    }
}
