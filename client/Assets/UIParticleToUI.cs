using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIParticle 粒子坐标转 UI 坐标工具
/// </summary>
public class UIParticleToUI : MonoBehaviour
{
    [Header("必填组件")]
    public ParticleSystem uiParticle; // 你的UIParticle粒子系统
    public Canvas targetCanvas;       // 目标Canvas（UI所在的Canvas）
    public Camera uiCamera;           // 仅Canvas=ScreenSpace-Camera时需要赋值（Canvas的Render Camera）

    // 测试：获取第0个粒子的UI坐标并打印
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 uiCoord = GetParticleUICoord(0);
            Debug.Log($"第0个粒子的UI坐标（Canvas局部）：{uiCoord}");
        }
    }

    /// <summary>
    /// 获取指定索引粒子的UI坐标（Canvas局部，像素单位）
    /// </summary>
    /// <param name="particleIndex">粒子索引（从0开始）</param>
    /// <returns>UI坐标（可直接赋值给RectTransform.anchoredPosition，需注意单位转换）</returns>
    public Vector2 GetParticleUICoord(int particleIndex)
    {
        // 1. 校验组件
        if (uiParticle == null || targetCanvas == null) return Vector2.zero;

        // 2. 获取粒子数组（复用数组，减少GC）
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        int activeCount = uiParticle.GetParticles(particles);
        if (particleIndex < 0 || particleIndex >= activeCount) return Vector2.zero;

        // 3. 粒子坐标 → 世界坐标（适配粒子的Simulation Space）
        Vector3 particleWorldPos =  particles[particleIndex].position;

        // 4. 世界坐标 → UI坐标（Canvas局部，适配Canvas渲染模式）
        return WorldPosToUICoord(particleWorldPos);
    }

    #region 内部辅助方法
    /// <summary>
    /// 粒子坐标转世界坐标（处理粒子不同的空间模式）
    /// </summary>
 
    /// <summary>
    /// 世界坐标转UI坐标（Canvas局部，像素单位）
    /// </summary>
    private Vector2 WorldPosToUICoord(Vector3 worldPos)
    {
        Vector2 uiCoord = Vector2.zero;
        RectTransform canvasRect = targetCanvas.transform as RectTransform;

        switch (targetCanvas.renderMode)
        {
            // 最常用：ScreenSpace-Overlay（纯2D UI）
            case RenderMode.ScreenSpaceOverlay:
                // 世界坐标 → 屏幕坐标（修正Y轴：Screen原点在左下，Canvas在左上）
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
                screenPos.y = Screen.height - screenPos.y;
                // 屏幕坐标 → Canvas局部坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPos, null, out uiCoord
                );
                break;

            // ScreenSpace-Camera（带UI相机的模式）
            case RenderMode.ScreenSpaceCamera:
                if (uiCamera == null) uiCamera = targetCanvas.worldCamera;
                // 世界坐标 → 屏幕坐标（依赖UI相机）
                Vector2 screenPosCam = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);
                // 屏幕坐标 → Canvas局部坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPosCam, uiCamera, out uiCoord
                );
                break;

            // WorldSpace（Canvas是3D物体）
            case RenderMode.WorldSpace:
                // 直接将世界坐标转Canvas局部坐标
                Vector3 canvasLocalPos = targetCanvas.transform.InverseTransformPoint(worldPos);
                uiCoord = new Vector2(canvasLocalPos.x, canvasLocalPos.y);
                break;
        }

        return uiCoord;
    }
    #endregion
    public Image followImage;
    public Transform followtag;
    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[uiParticle.main.maxParticles];
        int activeCount = uiParticle.GetParticles(particles);
        if (activeCount <= 0) return;
        //Vector2 pixelCoord = GetParticleUICoord(0);
        //Vector2 uiUnitCoord = PixelToUIUnit(pixelCoord);
        //followImage.rectTransform.anchoredPosition = uiUnitCoord;
        followtag.position = particles[0].position;
    }
    // 扩展：UI坐标单位转换（像素 → UI单位，适配anchoredPosition）
    public Vector2 PixelToUIUnit(Vector2 pixelCoord)
    {
        float refPPU = targetCanvas.GetComponent<CanvasScaler>().referencePixelsPerUnit;
        return pixelCoord / refPPU; // 默认100像素 = 1UI单位
    }
}