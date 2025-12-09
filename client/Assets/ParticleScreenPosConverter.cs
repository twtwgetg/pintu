using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIParticle粒子屏幕坐标转换工具
/// 解决UIParticle中粒子位置不是真实世界坐标的问题
/// </summary>
public class ParticleScreenPosConverter : MonoBehaviour
{
    [Header("组件引用")]
    public ParticleSystem uiParticle;     // UIParticle组件
    public Canvas targetCanvas;           // 目标Canvas
    public Camera uiCamera;               // UI相机（仅在ScreenSpace-Camera模式下需要）

    [Header("调试")]
    public Transform debugTarget;         // 调试用的目标对象，用于显示第一个粒子的位置

    private ParticleSystem.Particle[] particlesBuffer;

    void Start()
    {
        // 初始化粒子缓冲区
        if (uiParticle != null)
        {
            particlesBuffer = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        }
    }

    void Update()
    {
        if (uiParticle == null || targetCanvas == null) return;

        // 获取活跃粒子数量
        int activeCount = uiParticle.GetParticles(particlesBuffer);
        if (activeCount <= 0) return;

        // 获取第一个粒子的屏幕坐标
        Vector2 screenPos = GetParticleScreenPosition(0);
        
        // 可选：在场景中显示第一个粒子的位置
        if (debugTarget != null)
        {
            debugTarget.position = ScreenToWorld(screenPos);
        }
        
        // 打印第一个粒子的屏幕坐标
        Debug.Log($"第一个粒子的屏幕坐标: {screenPos}");
    }

    /// <summary>
    /// 获取指定索引粒子的屏幕坐标
    /// </summary>
    /// <param name="particleIndex">粒子索引</param>
    /// <returns>屏幕坐标</returns>
    public Vector2 GetParticleScreenPosition(int particleIndex)
    {
        // 1. 校验参数
        if (uiParticle == null || targetCanvas == null) return Vector2.zero;

        // 2. 获取粒子数据
        if (particlesBuffer == null || particlesBuffer.Length < uiParticle.main.maxParticles)
        {
            particlesBuffer = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        }

        int activeCount = uiParticle.GetParticles(particlesBuffer);
        if (particleIndex < 0 || particleIndex >= activeCount) return Vector2.zero;

        // 3. 获取粒子的世界坐标（注意：这是UIParticle的特殊坐标）
        Vector3 particleWorldPos = particlesBuffer[particleIndex].position;

        // 4. 根据Canvas渲染模式转换为屏幕坐标
        return WorldToScreenPoint(particleWorldPos);
    }

    /// <summary>
    /// 将世界坐标转换为屏幕坐标（适配不同Canvas模式）
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <returns>屏幕坐标</returns>
    private Vector2 WorldToScreenPoint(Vector3 worldPos)
    {
        Vector2 screenPos = Vector2.zero;

        switch (targetCanvas.renderMode)
        {
            // 最常用：ScreenSpace-Overlay模式（2D UI）
            case RenderMode.ScreenSpaceOverlay:
                screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
                // 注意：Unity的屏幕坐标Y轴原点在左下角，而Canvas的Y轴原点在左上角
                // 但在大多数情况下不需要手动转换，RectTransformUtility已经处理了
                break;

            // ScreenSpace-Camera模式（带相机的UI）
            case RenderMode.ScreenSpaceCamera:
                if (uiCamera == null) uiCamera = targetCanvas.worldCamera;
                screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);
                break;

            // WorldSpace模式（3D UI）
            case RenderMode.WorldSpace:
                // WorldSpace模式下的UI通常不需要转换为屏幕坐标
                screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
                break;
        }

        return screenPos;
    }

    /// <summary>
    /// 将屏幕坐标转换为世界坐标（用于调试显示）
    /// </summary>
    /// <param name="screenPos">屏幕坐标</param>
    /// <returns>世界坐标</returns>
    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        // 创建一个距离相机10单位的屏幕点
        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, 10f);
        
        switch (targetCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                return Camera.main.ScreenToWorldPoint(screenPoint);

            case RenderMode.ScreenSpaceCamera:
                if (uiCamera == null) uiCamera = targetCanvas.worldCamera;
                return uiCamera.ScreenToWorldPoint(screenPoint);

            case RenderMode.WorldSpace:
                return Camera.main.ScreenToWorldPoint(screenPoint);

            default:
                return Camera.main.ScreenToWorldPoint(screenPoint);
        }
    }

    /// <summary>
    /// 获取粒子在Canvas中的局部坐标
    /// </summary>
    /// <param name="particleIndex">粒子索引</param>
    /// <returns>Canvas局部坐标</returns>
    public Vector2 GetParticleCanvasLocalPosition(int particleIndex)
    {
        // 1. 校验参数
        if (uiParticle == null || targetCanvas == null) return Vector2.zero;

        // 2. 获取粒子数据
        if (particlesBuffer == null || particlesBuffer.Length < uiParticle.main.maxParticles)
        {
            particlesBuffer = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        }

        int activeCount = uiParticle.GetParticles(particlesBuffer);
        if (particleIndex < 0 || particleIndex >= activeCount) return Vector2.zero;

        // 3. 获取粒子的世界坐标
        Vector3 particleWorldPos = particlesBuffer[particleIndex].position;

        // 4. 转换为Canvas局部坐标
        return WorldPosToCanvasLocal(particleWorldPos);
    }

    /// <summary>
    /// 世界坐标转Canvas局部坐标
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <returns>Canvas局部坐标</returns>
    private Vector2 WorldPosToCanvasLocal(Vector3 worldPos)
    {
        Vector2 canvasLocalPos = Vector2.zero;
        RectTransform canvasRect = targetCanvas.transform as RectTransform;

        switch (targetCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                // 世界坐标→屏幕坐标→Canvas局部坐标
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPos,
                    null, // Overlay模式无需相机
                    out canvasLocalPos
                );
                break;

            case RenderMode.ScreenSpaceCamera:
                if (uiCamera == null) uiCamera = targetCanvas.worldCamera;
                Vector2 screenPosCam = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosCam,
                    uiCamera,
                    out canvasLocalPos
                );
                break;

            case RenderMode.WorldSpace:
                // 直接将世界坐标转换为Canvas局部坐标
                Vector3 localPos = targetCanvas.transform.InverseTransformPoint(worldPos);
                canvasLocalPos = new Vector2(localPos.x, localPos.y);
                break;
        }

        return canvasLocalPos;
    }
}