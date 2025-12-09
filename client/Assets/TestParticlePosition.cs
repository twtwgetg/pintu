using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试UIParticle中粒子位置获取的脚本
/// </summary>
public class TestParticlePosition : MonoBehaviour
{
    public ParticleSystem uiParticle;
    public Canvas targetCanvas;
    public Transform debugMarker;  // 用于可视化粒子位置的标记
    
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        if (uiParticle != null)
        {
            particles = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        }
    }

    void Update()
    {
        if (uiParticle == null || targetCanvas == null) return;

        int count = uiParticle.GetParticles(particles);
        if (count > 0)
        {
            // 获取第一个粒子的位置
            Vector3 particleWorldPos = particles[0].position;
            
            // 显示原始世界坐标
            Debug.Log($"粒子世界坐标: {particleWorldPos}");
            
            // 转换为屏幕坐标
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, particleWorldPos);
            Debug.Log($"粒子屏幕坐标: {screenPos}");
            
            // 如果有标记对象，将其放置在粒子位置
            if (debugMarker != null)
            {
                debugMarker.position = particleWorldPos;
            }
        }
    }
}