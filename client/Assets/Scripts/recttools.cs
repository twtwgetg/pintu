using UnityEngine;

public static class RectTransformExtension
{
    /// <summary>
    /// 判断子RectTransform是否超出父RectTransform的范围
    /// </summary>
    /// <param name="child">子节点RectTransform</param>
    /// <param name="parent">父节点RectTransform（必须是child的直接或间接父节点）</param>
    /// <param name="checkPartial">true=判断是否部分超出；false=判断是否完全超出</param>
    /// <returns>是否超出范围</returns>
    public static bool IsOutOfParentBounds(this RectTransform child, RectTransform parent, bool checkPartial = true)
    {
        // 1. 校验父节点关系（避免无效计算）
        if (!child.IsChildOf(parent))
        {
            Debug.LogWarning("子节点不是父节点的层级子节点，无法判断边界！");
            return true;
        }

        // 2. 获取父节点的本地矩形（pivot不影响，直接用rect属性）
        Rect parentRect = parent.rect;

        // 3. 获取子节点在父节点本地坐标系下的矩形
        Rect childLocalRect = GetRectInParentSpace(child, parent);

        // 4. 判断是否超出：根据checkPartial选择逻辑（修正Overlaps参数问题）
        if (checkPartial)
        {
            // 部分超出：子节点矩形与父节点矩形不完全包含（即有部分在父节点外）
            // 逻辑：父节点矩形无法完全包含子节点矩形 → 超出
            return !parentRect.Contains(childLocalRect.min) ||
                   !parentRect.Contains(childLocalRect.max) ||
                   !parentRect.Contains(new Vector2(childLocalRect.xMin, childLocalRect.yMax)) ||
                   !parentRect.Contains(new Vector2(childLocalRect.xMax, childLocalRect.yMin));
        }
        else
        {
            // 完全超出：子节点矩形与父节点矩形无任何重叠
            return !parentRect.Overlaps(childLocalRect);
        }
    }
    /// <summary>
    /// 判断世界坐标点是否在 RectTransform 范围内（支持旋转、缩放、任意锚点）
    /// </summary>
    /// <param name="rectTransform">目标 RectTransform（UI节点）</param>
    /// <param name="worldPoint">世界坐标系下的点（如3D物体位置、UI世界坐标）</param>
    /// <returns>点是否在范围内</returns>
    public static bool IsWorldPointInBounds(this RectTransform rectTransform, Vector3 worldPoint)
    {
        // 将世界坐标转换为 RectTransform 的本地坐标（忽略Z轴，UI是2D平面）
        Vector2 localPoint = rectTransform.InverseTransformPoint(worldPoint);
        // 判断本地坐标是否在 RectTransform 的本地矩形内
        return rectTransform.rect.Contains(localPoint);
    }

    /// <summary>
    /// 判断屏幕坐标点是否在 RectTransform 范围内（适用于点击、触摸检测）
    /// </summary>
    /// <param name="rectTransform">目标 RectTransform（UI节点）</param>
    /// <param name="screenPoint">屏幕坐标系下的点（如 Input.mousePosition）</param>
    /// <param name="camera">Canvas 对应的相机（Screen Space - Overlay 模式传 null）</param>
    /// <returns>点是否在范围内</returns>
    public static bool IsScreenPointInBounds(this RectTransform rectTransform, Vector2 screenPoint, Camera camera = null)
    {
        // 将屏幕坐标转换为 RectTransform 的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPoint, camera, out Vector2 localPoint))
        {
            // 转换成功，判断是否在本地矩形内
            return rectTransform.rect.Contains(localPoint);
        }
        // 转换失败（点不在相机可视范围内），返回false
        return false;
    }
    /// <summary>
    /// 获取子节点在父节点本地坐标系下的Rect
    /// </summary>
    private static Rect GetRectInParentSpace(RectTransform child, RectTransform parent)
    {
        // 存储子节点的4个世界坐标角点（顺序：左下、右下、右上、左上）
        Vector3[] childWorldCorners = new Vector3[4];
        child.GetWorldCorners(childWorldCorners);

        // 存储转换后的父节点本地坐标角点
        Vector2[] childLocalCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            // 将世界坐标转换为父节点的本地坐标（忽略Z轴，UI是2D平面）
            childLocalCorners[i] = parent.InverseTransformPoint(childWorldCorners[i]);
        }

        // 计算子节点本地矩形的边界（min/max坐标）
        float xMin = Mathf.Min(childLocalCorners[0].x, childLocalCorners[1].x, childLocalCorners[2].x, childLocalCorners[3].x);
        float xMax = Mathf.Max(childLocalCorners[0].x, childLocalCorners[1].x, childLocalCorners[2].x, childLocalCorners[3].x);
        float yMin = Mathf.Min(childLocalCorners[0].y, childLocalCorners[1].y, childLocalCorners[2].y, childLocalCorners[3].y);
        float yMax = Mathf.Max(childLocalCorners[0].y, childLocalCorners[1].y, childLocalCorners[2].y, childLocalCorners[3].y);

        // 返回子节点在父节点本地的Rect
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    /// <summary>
    /// 可选：将子节点限制在父节点范围内（超出时自动回弹）
    /// </summary>
    public static void ConstrainToParent(this RectTransform child, RectTransform parent)
    {
        Rect parentRect = parent.rect;
        Rect childLocalRect = GetRectInParentSpace(child, parent);

        // 计算需要偏移的距离（确保子节点完全在父节点内）
        float offsetX = 0;
        float offsetY = 0;

        // 左边界超出
        if (childLocalRect.xMin < parentRect.xMin)
            offsetX = parentRect.xMin - childLocalRect.xMin;
        // 右边界超出
        else if (childLocalRect.xMax > parentRect.xMax)
            offsetX = parentRect.xMax - childLocalRect.xMax;

        // 下边界超出
        if (childLocalRect.yMin < parentRect.yMin)
            offsetY = parentRect.yMin - childLocalRect.yMin;
        // 上边界超出
        else if (childLocalRect.yMax > parentRect.yMax)
            offsetY = parentRect.yMax - childLocalRect.yMax;

        // 应用偏移（在父节点本地坐标系下调整子节点位置）
        if (offsetX != 0 || offsetY != 0)
        {
            Vector2 localPos = child.localPosition;
            child.localPosition = new Vector2(localPos.x + offsetX, localPos.y + offsetY);
        }
    }
}