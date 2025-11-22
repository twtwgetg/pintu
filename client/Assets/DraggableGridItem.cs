using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // 引入DOTween

public class DraggableGridItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private DraggableGridItem targetItem; // 拖拽时靠近的目标项
    public int originalSiblingIndex { get; private set; } // 原始的层级索引，公开供其他脚本访问
    // 在 Inspector 中显示的邻居合法状态（true 表示在对应方向存在邻居/相邻）
    public bool adjacentLeft = false;
    public bool adjacentRight = false;
    public bool adjacentTop = false;
    public bool adjacentBottom = false;

    // 可由外部调用以一次性更新所有邻居状态
    public void SetAdjacency(bool left, bool right, bool top, bool bottom)
    {
        adjacentLeft = left;
        adjacentRight = right;
        adjacentTop = top;
        adjacentBottom = bottom;
    }
    // 拖拽时一起移动的组
    private List<DraggableGridItem> dragGroup = null;
    private Dictionary<DraggableGridItem, Vector2> groupOriginalPositions = null;
    private Dictionary<DraggableGridItem, int> groupOriginalSiblingIndices = null;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // 查找Canvas组件
        canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录原始位置和层级
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // 收集和本格连接的邻居，一起移动
        CollectConnectedGroup();

        // 将组内所有项置于最上层，记录原始层级和位置
        if (dragGroup != null)
        {
            foreach (var it in dragGroup)
            {
                if (groupOriginalSiblingIndices != null && !groupOriginalSiblingIndices.ContainsKey(it))
                    groupOriginalSiblingIndices[it] = it.transform.GetSiblingIndex();
                if (groupOriginalPositions != null && !groupOriginalPositions.ContainsKey(it))
                    groupOriginalPositions[it] = it.rectTransform.anchoredPosition;
                it.transform.SetAsLastSibling();
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 拖拽过程中更新位置
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // 计算位移并应用到组内其他项
        if (dragGroup != null && groupOriginalPositions != null)
        {
            Vector2 delta = rectTransform.anchoredPosition - originalPosition;
            foreach (var it in dragGroup)
            {
                if (it == this) continue;
                if (groupOriginalPositions.TryGetValue(it, out Vector2 orig))
                {
                    it.rectTransform.anchoredPosition = orig + delta;
                }
            }
        }

        // 检查是否靠近其他节点（忽略组内项）
        CheckNearbyItems();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否有目标项
        if (targetItem != null)
        {
            // 如果有目标项，交换位置（使用原始位置而不是当前位置）
            SwapPositions(targetItem);
        }
        else
        {
            // 如果没有目标项，回到原始位置（使用动画）
            // 对组内每个元素做回退动画
            if (dragGroup != null && groupOriginalPositions != null)
            {
                Sequence seq = DOTween.Sequence();
                foreach (var it in dragGroup)
                {
                    if (groupOriginalPositions.TryGetValue(it, out Vector2 orig))
                    {
                        seq.Join(it.rectTransform.DOAnchorPos(orig, 0.25f));
                    }
                }
                seq.OnComplete(() => {
                    // 恢复原始层级
                    if (groupOriginalSiblingIndices != null)
                    {
                        foreach (var kv in groupOriginalSiblingIndices)
                        {
                            kv.Key.transform.SetSiblingIndex(kv.Value);
                        }
                    }
                    // 更新边框显示状态
                    picmgr picManager = GetComponentInParent<picmgr>();
                    if (picManager != null)
                    {
                        picManager.UpdateBorderVisibility();
                    }
                });
            }
            else
            {
                Tween tween = rectTransform.DOAnchorPos(originalPosition, 0.3f);
                tween.OnComplete(() => {
                    transform.SetSiblingIndex(originalSiblingIndex);
                    picmgr picManager = GetComponentInParent<picmgr>();
                    if (picManager != null)
                    {
                        picManager.UpdateBorderVisibility();
                    }
                });
            }
        }
        
        // 清空目标项引用并清理组数据
        targetItem = null;
        dragGroup = null;
        groupOriginalPositions = null;
        groupOriginalSiblingIndices = null;
    }
    
    // 检查附近是否有其他节点
    private void CheckNearbyItems()
    {
        targetItem = null;
        float minDistance = float.MaxValue;
        
        // 获取父对象下的所有子节点
        Transform parent = transform.parent;
        if (parent == null) return;
        
        foreach (Transform child in parent)
        {
            // 跳过自己
            if (child == transform) continue;
            
            DraggableGridItem item = child.GetComponent<DraggableGridItem>();
            if (item != null)
            {
                // 忽略拖拽组内项
                if (dragGroup != null && dragGroup.Contains(item)) continue;
                // 计算距离
                float distance = Vector2.Distance(rectTransform.anchoredPosition, item.rectTransform.anchoredPosition);
                
                // 如果距离小于某个阈值并且是最近的
                if (distance < 100f && distance < minDistance)
                {
                    minDistance = distance;
                    targetItem = item;
                }
            }
        }
    }
    
    // 交换两个节点的位置
    private void SwapPositions(DraggableGridItem otherItem)
    {
        // 使用原始位置进行交换，而不是当前位置（当前只交换主项）
        Tween tween1 = otherItem.rectTransform.DOAnchorPos(originalPosition, 0.3f);
        Tween tween2 = rectTransform.DOAnchorPos(otherItem.rectTransform.anchoredPosition, 0.3f);

        // 创建一个序列来确保两个动画都完成后才恢复层级和更新边框
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tween1);
        sequence.Join(tween2);
        sequence.OnComplete(() => {
            // 交换层级索引
            int myIndex = transform.GetSiblingIndex();
            int otherIndex = otherItem.transform.GetSiblingIndex();
            transform.SetSiblingIndex(otherIndex);
            otherItem.transform.SetSiblingIndex(myIndex);

            // 恢复原始层级
            transform.SetSiblingIndex(originalSiblingIndex);
            otherItem.transform.SetSiblingIndex(otherItem.originalSiblingIndex);

            // 更新边框显示状态
            picmgr picManager = GetComponentInParent<picmgr>();
            if (picManager != null)
            {
                picManager.UpdateBorderVisibility();
            }
        });
    }

    // 收集与当前项连接的邻居并加入拖拽组
    private void CollectConnectedGroup()
    {
        // 初始化组与记录字典
        dragGroup = new List<DraggableGridItem>();
        groupOriginalPositions = new Dictionary<DraggableGridItem, Vector2>();
        groupOriginalSiblingIndices = new Dictionary<DraggableGridItem, int>();

        // 使用递归（深度优先）从当前节点扩展，但基于网格坐标而不是位置
        // 构建坐标到项的映射：解析子对象名 "GridCell_x_y"
        Dictionary<(int x, int y), DraggableGridItem> map = new Dictionary<(int, int), DraggableGridItem>();
        foreach (Transform child in parent)
        {
            var item = child.GetComponent<DraggableGridItem>();
            if (item == null) continue;
            int cx, cy;
            if (TryParseCellName(child.gameObject.name, out cx, out cy))
            {
                map[(cx, cy)] = item;
            }
        }

        bool TryParseCellName(string name, out int ox, out int oy)
        {
            ox = oy = -1;
            if (!name.StartsWith("GridCell_")) return false;
            string s = name.Replace("GridCell_", "");
            string[] parts = s.Split('_');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out ox)) return false;
            if (!int.TryParse(parts[1], out oy)) return false;
            return true;
        }

        // 获取本格坐标
        int startX = -1, startY = -1;
        if (!TryParseCellName(this.gameObject.name, out startX, out startY))
        {
            // 回退：如果无法解析坐标，保守地只加入自己
            dragGroup.Add(this);
            groupOriginalPositions[this] = rectTransform.anchoredPosition;
            groupOriginalSiblingIndices[this] = transform.GetSiblingIndex();
            return;
        }

        void dfsCoord(int x, int y)
        {
            if (!map.TryGetValue((x, y), out DraggableGridItem cur)) return;
            if (dragGroup.Contains(cur)) return;
            dragGroup.Add(cur);
            groupOriginalPositions[cur] = cur.rectTransform.anchoredPosition;
            groupOriginalSiblingIndices[cur] = cur.transform.GetSiblingIndex();

            // 四方向扩展：只根据 cur 的 adjacent 标志决定是否扩展
            if (cur.adjacentLeft) dfsCoord(x - 1, y);
            if (cur.adjacentRight) dfsCoord(x + 1, y);
            if (cur.adjacentTop) dfsCoord(x, y + 1);
            if (cur.adjacentBottom) dfsCoord(x, y - 1);
        }

        dfsCoord(startX, startY);
    }
    Transform parent
    {
        get
        {
            return transform.parent;
        }
    }
    // 在 parent 的子项中查找在方向 (dx,dy) 上最近的邻居（dx/dy 为 -1,0,1 表示方向），使用 spacing 作为格距参考
    private DraggableGridItem FindNeighborInDirection(Transform parent, Vector2 fromPos, int dx, int dy, float spacing, Transform exclude)
    {
        DraggableGridItem best = null;
        float bestDist = float.MaxValue;

        foreach (Transform child in parent)
        {
            if (child == exclude) continue;
            // To ensure we find neighbors relative to the provided position, allow checking all children
            var item = child.GetComponent<DraggableGridItem>();
            if (item == null) continue;
            Vector2 p = item.rectTransform.anchoredPosition;
            Vector2 delta = p - fromPos;

            // 要求主方向的分量有相应的符号，并且主方向值大致接近 spacing
            float main = (dx != 0) ? delta.x : delta.y;
            float cross = (dx != 0) ? delta.y : delta.x;

            if (dx != 0 && Mathf.Sign(main) != dx) continue;
            if (dy != 0 && Mathf.Sign(main) != dy) continue;

            // 主方向距离应接近 spacing（容差 0.4~1.6 倍），横向偏差应较小
            if (Mathf.Abs(main) < spacing * 0.4f || Mathf.Abs(main) > spacing * 1.6f) continue;
            if (Mathf.Abs(cross) > spacing * 0.6f) continue;

            float dist = Mathf.Abs(main);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = item;
            }
        }

        return best;
    }
}