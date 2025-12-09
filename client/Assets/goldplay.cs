using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class goldplay : MonoBehaviour
{
    public float delay = 1f;
    public void Play()
    {
        tagmt.endRange = 0;
        GetComponent<UIParticle>().Stop();
        GetComponent<UIParticle>().Play();
        DOVirtual.DelayedCall(delay, () =>
        {

            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect,
                targetIcon.position,
                null, // Overlay模式无需相机
                out targetWorldPos
            );
            tagmt.transform.position = targetWorldPos;
            var local = ptk.transform.InverseTransformPoint(targetWorldPos);
            tagmt.transform.SetParent(ptk.transform);
            tagmt.transform.localPosition /= (10 / 0.6666666667f);
            tagmt.endRange = 180;
        });
    }

    public void Trans()
    {

    }

    private ParticleSystem ptk
    {
        get
        {
            return GetComponentInChildren<ParticleSystem>();
        }
    }
    private int maxParticles; // 缓存最大粒子数
    
    private void Awake()
    {
        if (ptk != null)
        {
            maxParticles = ptk.main.maxParticles;
            particles = new ParticleSystem.Particle[maxParticles];
        }
    }
     
    // 添加死亡距离阈值
    public float deathDistance = 0.1f;
    
    // 添加力的衰减因子
    public float forceAttenuation = 1.0f;
    
    // 吸引力强度
    public float attractForce = 1;
    
    private Vector3 targetWorldPos; // 目标图标的世界坐标
    public Transform targetIcon; // 目标UI图标（Image/Button）
    public Canvas canvas; // 主Canvas（Overlay模式）
    public Image follow;
    
    public ParticleSystemForceField tagmt; 

    public float speed = 1f;
    private ParticleSystem.Particle[] particles;
    public float flySpeed = 500;
    public float drag = 5f;
}