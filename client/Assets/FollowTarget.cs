using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float maxdis = 1000f;
    public float time = 1f;
    public float delaytime = 2f;
    public ParticleSystemForceField fd
    {
        get
        {
            return GetComponent<ParticleSystemForceField>();
        }
    }
    
    public void Play()
    {
        if (fd != null)
        {
            fd.endRange = 0; // 初始值设为0
            DOTween.To(() => fd.endRange, x => fd.endRange = x, maxdis, time).onComplete=()=>
            {
                DOVirtual.DelayedCall(delaytime, () =>
                 {
                     fd.endRange = 0;
                 });
            };
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(target)
            transform.position = target.position;
    }
}
