using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private bool isDragging = false; // 标记是否正在拖拽
    
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
        
        // 设置拖拽状态为true
        isDragging = true;

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
    
    // 检查拖拽组是否在父对象边界内
    private bool IsGroupWithinParentBounds()
    {
        // 获取父对象的RectTransform
        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect == null) return false;
        
        // 获取父对象的世界空间边界
        Rect parentWorldRect = parentRect.rect;
        
        // 检查组内所有项是否都在父对象边界内
        if (dragGroup != null)
        {
            foreach (var item in dragGroup)
            {
                RectTransform itemRect = item.rectTransform;
                
                // 计算项的边界
                float itemLeft = itemRect.anchoredPosition.x - itemRect.rect.width / 2;
                float itemRight = itemRect.anchoredPosition.x + itemRect.rect.width / 2;
                float itemTop = itemRect.anchoredPosition.y + itemRect.rect.height / 2;
                float itemBottom = itemRect.anchoredPosition.y - itemRect.rect.height / 2;
                
                // 检查是否超出父对象边界
                if (itemLeft < parentWorldRect.xMin || 
                    itemRight > parentWorldRect.xMax || 
                    itemBottom < parentWorldRect.yMin || 
                    itemTop > parentWorldRect.yMax)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否有目标项，并且拖拽组在父对象边界内
        if (targetItem != null)
        {
            // 如果有目标项且位置有效，交换位置
            SwapPositions(targetItem);
        }
        else
        {
            // 拖拽失败，整个组退回原位置
            ResetAllDraggedItemsToOriginalPosition();
        }
        
        // 清空目标项引用并清理组数据
        targetItem = null;
        dragGroup = null;
        groupOriginalPositions = null;
        groupOriginalSiblingIndices = null;
        
        isDragging = false;
    }
    
    // 将所有拖拽的项重置到原始位置
    private void ResetAllDraggedItemsToOriginalPosition()
    {
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
            // 单个元素回退
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
        // 准备要交换的两组：源组（dragGroup 或 单个 this）和目标组（包含目标位置相关的所有卡牌）
        List<DraggableGridItem> srcGroup = (dragGroup != null && dragGroup.Count > 0) ? new List<DraggableGridItem>(dragGroup) : new List<DraggableGridItem>() { this };

        // 确保 source 原始位置/索引可用
        Dictionary<DraggableGridItem, Vector2> srcPositions = new Dictionary<DraggableGridItem, Vector2>();
        Dictionary<DraggableGridItem, int> srcSibs = new Dictionary<DraggableGridItem, int>();
        foreach (var it in srcGroup)
        {
            if (groupOriginalPositions != null && groupOriginalPositions.TryGetValue(it, out Vector2 p)) srcPositions[it] = p;
            else srcPositions[it] = it.rectTransform.anchoredPosition;
            srcSibs[it] = it.transform.GetSiblingIndex();
        }

        // 收集目标地点的所有卡牌（不仅仅是连接的组）
        List<DraggableGridItem> tgtGroup = new List<DraggableGridItem>();
        Dictionary<DraggableGridItem, Vector2> tgtPositions = new Dictionary<DraggableGridItem, Vector2>();
        Dictionary<DraggableGridItem, int> tgtSibs = new Dictionary<DraggableGridItem, int>();
        
        // 首先获取目标项的连接组
        Dictionary<DraggableGridItem, Vector2Int> dummyCoords;
        List<DraggableGridItem> connectedTgtGroup = CollectGroupForItem(otherItem, out dummyCoords, out tgtPositions, out tgtSibs);
        
        if (connectedTgtGroup != null && connectedTgtGroup.Count > 0)
        {
            tgtGroup.AddRange(connectedTgtGroup);
        }
        else
        {
            // 如果没有连接组，至少添加目标项本身
            tgtGroup.Add(otherItem);
            tgtPositions[otherItem] = otherItem.rectTransform.anchoredPosition;
            tgtSibs[otherItem] = otherItem.transform.GetSiblingIndex();
        }

        // 按坐标排序以获得稳定的配对
        List<KeyValuePair<DraggableGridItem, Vector2Int>> srcPairs = new List<KeyValuePair<DraggableGridItem, Vector2Int>>();
        foreach (var it in srcGroup)
        {
            if (TryParseCellName(it.gameObject.name, out int sx, out int sy)) 
                srcPairs.Add(new KeyValuePair<DraggableGridItem, Vector2Int>(it, new Vector2Int(sx, sy)));
            else 
                srcPairs.Add(new KeyValuePair<DraggableGridItem, Vector2Int>(it, new Vector2Int(int.MinValue, int.MinValue)));
        }
        srcPairs.Sort((a, b) => { 
            int c = a.Value.x.CompareTo(b.Value.x); 
            return c != 0 ? c : a.Value.y.CompareTo(b.Value.y); 
        });

        // 按坐标排序目标组
        List<KeyValuePair<DraggableGridItem, Vector2Int>> tgtPairs = new List<KeyValuePair<DraggableGridItem, Vector2Int>>();
        foreach (var it in tgtGroup)
        {
            if (TryParseCellName(it.gameObject.name, out int tx, out int ty)) 
                tgtPairs.Add(new KeyValuePair<DraggableGridItem, Vector2Int>(it, new Vector2Int(tx, ty)));
            else 
                tgtPairs.Add(new KeyValuePair<DraggableGridItem, Vector2Int>(it, new Vector2Int(int.MinValue, int.MinValue)));
        }
        tgtPairs.Sort((a, b) => { 
            int c = a.Value.x.CompareTo(b.Value.x); 
            return c != 0 ? c : a.Value.y.CompareTo(b.Value.y); 
        });

        // 准备位置数组：使用源组的原始位置
        List<Vector2> srcPosList = new List<Vector2>();
        foreach (var kv in srcPairs) 
            srcPosList.Add(srcPositions[kv.Key]);
        
        // 准备目标位置数组：使用目标组的当前位置
        List<Vector2> tgtPosList = new List<Vector2>();
        foreach (var kv in tgtPairs)
        {
            if (tgtPositions.TryGetValue(kv.Key, out Vector2 p)) 
                tgtPosList.Add(p);
            else 
                tgtPosList.Add(kv.Key.rectTransform.anchoredPosition);
        }

        // 动画：实现拖拽组整体替换目标地点所有卡牌
        // 源组移动到目标位置，目标组移动到源组的原始位置
        Sequence seq = DOTween.Sequence();
        int sCount = srcPairs.Count;
        int tCount = tgtPairs.Count;
        
        // 源组移动到目标组的位置
        for (int i = 0; i < sCount; i++)
        {
            var srcItem = srcPairs[i].Key;
            // 确保有足够的目标位置，否则使用第一个目标位置
            Vector2 dest = tCount > 0 ? tgtPosList[i % tCount] : srcItem.rectTransform.anchoredPosition;
            seq.Join(srcItem.rectTransform.DOAnchorPos(dest, 0.25f));
        }
        
        // 目标组移动到源组的原始位置
        for (int i = 0; i < tCount; i++)
        {
            var tgtItem = tgtPairs[i].Key;
            // 确保有足够的源位置，否则使用第一个源位置
            Vector2 dest = sCount > 0 ? srcPosList[i % sCount] : tgtItem.rectTransform.anchoredPosition;
            seq.Join(tgtItem.rectTransform.DOAnchorPos(dest, 0.25f));
        }

        seq.OnComplete(() => {
            // 交换 sibling 索引，确保层级关系正确
            // 首先记录所有需要交换的索引
            Dictionary<DraggableGridItem, int> finalSiblings = new Dictionary<DraggableGridItem, int>();
            
            // 为源组和目标组中的每个项分配新的层级索引
            for (int i = 0; i < Mathf.Max(sCount, tCount); i++)
            {
                if (i < sCount && i < tCount)
                {
                    var sItem = srcPairs[i].Key;
                    var tItem = tgtPairs[i].Key;
                    int sIdx = srcSibs.ContainsKey(sItem) ? srcSibs[sItem] : sItem.transform.GetSiblingIndex();
                    int tIdx = tgtSibs.ContainsKey(tItem) ? tgtSibs[tItem] : tItem.transform.GetSiblingIndex();
                    
                    finalSiblings[sItem] = tIdx;
                    finalSiblings[tItem] = sIdx;
                }
                else if (i < sCount)
                {
                    var sItem = srcPairs[i].Key;
                    // 如果源组项多于目标组，使用其原始索引
                    finalSiblings[sItem] = srcSibs.ContainsKey(sItem) ? srcSibs[sItem] : sItem.transform.GetSiblingIndex();
                }
                else if (i < tCount)
                {
                    var tItem = tgtPairs[i].Key;
                    // 如果目标组项多于源组，使用其原始索引
                    finalSiblings[tItem] = tgtSibs.ContainsKey(tItem) ? tgtSibs[tItem] : tItem.transform.GetSiblingIndex();
                }
            }
            
            // 应用新的层级索引
            foreach (var kv in finalSiblings)
            {
                kv.Key.transform.SetSiblingIndex(kv.Value);
            }

            // 更新边框显示状态
            picmgr picManager = GetComponentInParent<picmgr>();
            if (picManager != null)
            {
                picManager.UpdateBorderVisibility();
            }
        });
    }

    // 收集与当前项连接的邻居并加入拖拽组
    // 解析单元格名称获取坐标
    private bool TryParseCellName(string name, out int ox, out int oy)
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

    // 收集与指定项连接的邻居组
    private List<DraggableGridItem> CollectGroupForItem(DraggableGridItem item, out Dictionary<DraggableGridItem, Vector2Int> coords, out Dictionary<DraggableGridItem, Vector2> positions, out Dictionary<DraggableGridItem, int> siblings)
    {
        // 创建临时变量，避免在本地函数中直接访问out参数
        List<DraggableGridItem> group = new List<DraggableGridItem>();
        Dictionary<DraggableGridItem, Vector2Int> tempCoords = new Dictionary<DraggableGridItem, Vector2Int>();
        Dictionary<DraggableGridItem, Vector2> tempPositions = new Dictionary<DraggableGridItem, Vector2>();
        Dictionary<DraggableGridItem, int> tempSiblings = new Dictionary<DraggableGridItem, int>();
        
        // 构建坐标到项的映射
        Dictionary<(int x, int y), DraggableGridItem> map = new Dictionary<(int, int), DraggableGridItem>();
        foreach (Transform child in parent)
        {
            var gridItem = child.GetComponent<DraggableGridItem>();
            if (gridItem == null) continue;
            int cx, cy;
            if (TryParseCellName(child.gameObject.name, out cx, out cy))
            {
                map[(cx, cy)] = gridItem;
            }
        }
        
        // 获取起始坐标
        int startX = -1, startY = -1;
        if (!TryParseCellName(item.gameObject.name, out startX, out startY))
        {
            // 回退：如果无法解析坐标，只加入指定项
            group.Add(item);
            tempCoords[item] = new Vector2Int(-1, -1);
            tempPositions[item] = item.rectTransform.anchoredPosition;
            tempSiblings[item] = item.transform.GetSiblingIndex();
            
            // 赋值给out参数
            coords = tempCoords;
            positions = tempPositions;
            siblings = tempSiblings;
            return group;
        }
        
        // 递归收集连接的项
        void dfsCoord(int x, int y)
        {
            if (!map.TryGetValue((x, y), out DraggableGridItem cur)) return;
            if (group.Contains(cur)) return;
            
            group.Add(cur);
            tempCoords[cur] = new Vector2Int(x, y);
            tempPositions[cur] = cur.rectTransform.anchoredPosition;
            tempSiblings[cur] = cur.transform.GetSiblingIndex();
            
            // 四方向扩展
            if (cur.adjacentLeft) dfsCoord(x - 1, y);
            if (cur.adjacentRight) dfsCoord(x + 1, y);
            if (cur.adjacentTop) dfsCoord(x, y + 1);
            if (cur.adjacentBottom) dfsCoord(x, y - 1);
        }
        
        dfsCoord(startX, startY);
        
        // 在返回前赋值给out参数
        coords = tempCoords;
        positions = tempPositions;
        siblings = tempSiblings;
        
        return group;
    }

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