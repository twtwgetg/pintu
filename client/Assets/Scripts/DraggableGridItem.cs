using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; // 引入DOTween
using System.Collections;

public class DraggableGridItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }
    public Canvas canvas;
    //private Vector2 originalPosition;
    private static int originalPositionIndex; // 记录原始位置索引
    public static int targetPositionIndex;
    public static Vector2 targetPosition
    {
        get
        {
            return new Vector2((targetPositionIndex /wid) * carWid, (targetPositionIndex % hei) * carHei);
        }
    }
    static Vector2 originalPosition
    {
        get
        {
            return new Vector2((originalPositionIndex / wid) * carWid, (originalPositionIndex % hei) * carHei);
        }
    }
    Vector2 vecPosition
    {
        get
        {
            return new Vector2((PositionIndex/wid)*carWid,(PositionIndex%hei)*carHei);
        }
    }
    public static float carWid
    {
        get
        {
            return picmgr.instance.carWid;
        }
    }
    public static float carHei
    {
        get
        {
            return picmgr.instance.carHei;
        }
    }
    public static int wid
    {
        get
        {
            return picmgr.instance.width;
        }
    }
    public static int hei
    {
        get
        {
            return picmgr.instance.height;
        }
    }
    public int originalSiblingIndex { get; private set; } // 原始的层级索引，公开供其他脚本访问
    public static bool isAnyItemDragging = false; // 静态变量，标记是否有任何卡牌正在被拖拽
    public int PositionIndex { get; set; } // 当前卡牌的位置索引
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
    internal Texture2D pic;
    internal float uvWidth;
    internal float uvHeight;
    internal float uvY;
    internal float uvX;
     
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果已经有卡牌在拖拽，则直接返回，禁止同时拖拽多张卡牌
        if (isAnyItemDragging)
        {
            return;
        }
        
        // 设置静态拖拽状态为true
        isAnyItemDragging = true;
        
        // 记录原始位置和层级 
        originalPositionIndex = PositionIndex;
          
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

        var diff = rectTransform.anchoredPosition - vecPosition;
        if (Mathf.Abs(diff.x)> carWid/2 || 
            Mathf.Abs(diff.y)> carHei/2)
        {
            int x = Mathf.RoundToInt(rectTransform.anchoredPosition.x / carWid);
            int y = Mathf.RoundToInt(rectTransform.anchoredPosition.y / carHei);
//            targetPosition = new Vector2(x * carWid, y * carHei);
            targetPositionIndex = x*hei + y;
        }
    }
 
    // 检查拖拽组是否在父对象边界内
    private bool IsGroupWithinParentBounds(Dictionary<DraggableGridItem, Vector2> potentialPositions = null)
    {
        // 获取父对象的RectTransform
        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect == null) return false;
        
        // 获取父对象的世界空间边界
        Rect parentWorldRect = parentRect.rect;
        
        // 确定要检查的项目集合
        IEnumerable<DraggableGridItem> itemsToCheck;
        if (potentialPositions != null)
        {
            // 如果提供了潜在位置，使用该字典中的所有项
            itemsToCheck = potentialPositions.Keys;
        }
        else if (dragGroup != null)
        {
            // 否则使用当前拖拽组中的项
            itemsToCheck = dragGroup;
        }
        else
        {
            // 如果没有拖拽组，只有当前项
            itemsToCheck = new List<DraggableGridItem> { this };
        }
        
        // 检查组内所有项是否都在父对象边界内
        foreach (var item in itemsToCheck)
        {
            RectTransform itemRect = item.rectTransform;
            
            // 获取项的位置
            Vector2 itemPosition;
            if (potentialPositions != null && potentialPositions.TryGetValue(item, out Vector2 pos))
            {
                // 使用潜在位置
                itemPosition = pos;
            }
            else
            {
                // 使用当前位置
                itemPosition = itemRect.anchoredPosition;
            }
            
            // 计算项的边界
            float itemLeft = itemPosition.x - itemRect.rect.width / 2;
            float itemRight = itemPosition.x + itemRect.rect.width / 2;
            float itemTop = itemPosition.y + itemRect.rect.height / 2;
            float itemBottom = itemPosition.y - itemRect.rect.height / 2;
            
            // 检查是否超出父对象边界
            if (itemLeft < parentWorldRect.xMin || 
                itemRight > parentWorldRect.xMax || 
                itemBottom < parentWorldRect.yMin || 
                itemTop > parentWorldRect.yMax)
            {
                return false;
            }
        }
        return true;
    }

    public void Turn(bool atonce=false)
    {
        if (atonce)
        { 
            settex();  
        }
        else
        {

            transform.DOScaleX(0, 0.25f).onComplete = () =>
            {
                transform.localScale = new Vector3(0, 1, 1);
                settex();
                transform.DOScaleX(1, 0.25f).onComplete = () =>
                {
                    transform.localScale = new Vector3(1, 1, 1);
                };
            };


            var x = transform.localPosition.x;
            float wid = transform.GetComponent<RectTransform>().rect.width;
            transform.DOLocalMoveX(x + wid / 2, 0.25f).onComplete = () =>
            {
                var p = transform.localPosition;
                p.x = x + wid / 2;
                transform.localPosition = p;

                transform.DOLocalMoveX(x, 0.25f).onComplete = () =>
                {
                    var p = transform.localPosition;
                    p.x = x;
                    transform.localPosition = p;
                };
            };

        }
       
    }

    internal void settex()
    {
        rawImage.color = Color.white;
        rawImage.uvRect = new Rect(uvX, uvY, uvWidth, uvHeight);
        rawImage.texture = pic;
    }
                
    RawImage rawImage
    {
        get
        {
            return GetComponent<RawImage>();
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
         
        bool shouldSwap = false;
 
        if(originalPositionIndex != targetPositionIndex)
        {
            // 条件2: 所有拖拽组成员位置是否合法
            bool isPositionValid = CheckPotentialPositionValidity();
            if (isPositionValid)
            {
                shouldSwap = true;
            }
        }
        if (shouldSwap)
        {
            // 执行交换位置
            SwapPositions(targetPosition);
        }
        else
        {
            // 拖拽失败，整个组退回原位置
            ResetAllDraggedItemsToOriginalPosition();
        }
         
        dragGroup = null;
        groupOriginalPositions = null;
        groupOriginalSiblingIndices = null;
        
        isDragging = false;
        
        // 不要立即重置静态拖拽状态，等待动画完成后再重置
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
                    // 重置位置索引
                    int index = GetIndexFromAnchoredPosition(orig);
                    it.PositionIndex = index;
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
                    picManager.RefreshAllPositionIndices();
                    picManager.UpdateBorderVisibility();
                }
                // 动画完成，重置静态拖拽状态，允许新的操作
                isAnyItemDragging = false;
            });
        }
        else
        {
            // 单个元素回退
            Tween tween = rectTransform.DOAnchorPos(originalPosition, 0.3f);
            tween.OnComplete(() => {
                rectTransform.anchoredPosition = originalPosition;
                transform.SetSiblingIndex(originalSiblingIndex);
                // 重置位置索引
                PositionIndex = originalPositionIndex;
                picmgr picManager = GetComponentInParent<picmgr>();
                if (picManager != null)
                {
                    picManager.RefreshAllPositionIndices();
                    picManager.UpdateBorderVisibility();
                }
                // 动画完成，重置静态拖拽状态，允许新的操作
                isAnyItemDragging = false;
            });
        }
    }
    
    void Reset()
    {

    }
 
    
    // 检查拖拽组挪到潜在目标位置后是否合法
    private bool CheckPotentialPositionValidity(Vector2 potentialTargetPos = default)
    {
        // 准备拖拽组（源组）
        List<DraggableGridItem> dragGroupList = (dragGroup != null && dragGroup.Count > 0) ? new List<DraggableGridItem>(dragGroup) : new List<DraggableGridItem>() { this };
        
        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect == null) return false;

        // 获取父对象的边界
        Rect parentWorldRect = new Rect(0, 0, parentRect.rect.width, parentRect.rect.height);
        
        // 计算拖拽组移动后的目标位置集合，并检查每个位置是否合法
        foreach (var item in dragGroupList)
        {
            Vector2 newPosition = item.rectTransform.anchoredPosition+new Vector2(item.rectTransform.sizeDelta.x/2,item.rectTransform.sizeDelta.y/2);
            if(!parentWorldRect.Contains(newPosition))
            {
                return false;
            }
        }
        
        return true;
    }
 
    // 交换位置
    private void SwapPositions(Vector2 targetPos)
    {
        // 更新当前卡牌的位置索引
        int newIndex = GetIndexFromAnchoredPosition(targetPos);
        PositionIndex = newIndex;
        // 准备拖拽组（源组）
        List<DraggableGridItem> dragGroupList = (dragGroup != null && dragGroup.Count > 0) ? new List<DraggableGridItem>(dragGroup) : new List<DraggableGridItem>() { this };
        
        // 确保 source 原始位置/索引可用
        Dictionary<DraggableGridItem, Vector2> originalPositions = new Dictionary<DraggableGridItem, Vector2>();
        Dictionary<DraggableGridItem, int> originalSiblings = new Dictionary<DraggableGridItem, int>();
        
        // 记录拖拽组所有项的原始位置和层级
        foreach (var item in dragGroupList)
        {
            if (groupOriginalPositions != null && groupOriginalPositions.TryGetValue(item, out Vector2 p))
                originalPositions[item] = p;
            else
                originalPositions[item] = item.rectTransform.anchoredPosition;
            originalSiblings[item] = item.transform.GetSiblingIndex();
        }
        
        // 获取拖拽源项（当前拖拽的卡牌）的原始位置
        Vector2 dragSourcePosition = originalPositions[this];
        
        // 计算拖拽组移动后的目标位置集合
        Dictionary<DraggableGridItem, Vector2> targetPositions = new Dictionary<DraggableGridItem, Vector2>();
        foreach (var item in dragGroupList)
        {
            // 计算项与拖拽源项的相对位置差
            Vector2 relativePos = originalPositions[item] - dragSourcePosition;
            
            // 基于目标位置，计算该项应该移动到的目标位置
            Vector2 itemTargetPos = targetPos + relativePos;
            targetPositions[item] = itemTargetPos;
        }
        
        // 识别拖拽组移动后空出的位置（排除拖拽组成员互相填补的情况）
        List<Vector2> emptyPositions = new List<Vector2>();
        foreach (var kvp in originalPositions)
        {
            Vector2 originalPos = kvp.Value;
            bool isFilledByDragGroup = false;
            
            // 检查是否被拖拽组的其他成员填补
            foreach (var targetKvp in targetPositions)
            {
                if (targetKvp.Key != kvp.Key && Vector2.Distance(targetKvp.Value, originalPos) < 1f) // 使用小阈值判断位置是否相同
                {
                    isFilledByDragGroup = true;
                    break;
                }
            }
            
            // 如果没有被拖拽组的其他成员填补，则认为是空出的位置
            if (!isFilledByDragGroup)
            {
                emptyPositions.Add(originalPos);
            }
        }
        
        // 识别拖拽组覆盖的卡牌（排除已在拖拽组中的卡牌）
        List<DraggableGridItem> coveredItems = new List<DraggableGridItem>();
        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                DraggableGridItem item = child.GetComponent<DraggableGridItem>();
                if (item == null || dragGroupList.Contains(item))
                    continue;
                
                // 检查该项是否在拖拽组目标位置的覆盖范围内
                foreach (var vtargetPos in targetPositions.Values)
                {
                    if (Vector2.Distance(item.rectTransform.anchoredPosition, vtargetPos) < 1f)
                    {
                        coveredItems.Add(item);
                        break;
                    }
                }
            }
        }
        
        // 按照从左到右、从上到下的规则排序空出的位置和覆盖的卡牌
        SortPositionsByGridOrder(emptyPositions);
        SortItemsByGridOrder(coveredItems);
        
        // 创建位置映射：覆盖的卡牌映射到空出的位置
        Dictionary<DraggableGridItem, Vector2> replacementMapping = new Dictionary<DraggableGridItem, Vector2>();
        for (int i = 0; i < Mathf.Min(coveredItems.Count, emptyPositions.Count); i++)
        {
            replacementMapping[coveredItems[i]] = emptyPositions[i];
        }
        
        // 准备动画序列
        Sequence seq = DOTween.Sequence();
        
        // 移动拖拽组中的所有项到它们的目标位置
        foreach (var item in dragGroupList)
        {
            if (targetPositions.TryGetValue(item, out Vector2 vtargetPos))
            {
                seq.Join(item.rectTransform.DOAnchorPos(vtargetPos, 0.25f));
                // 更新位置索引
                int vnewIndex = GetIndexFromAnchoredPosition(vtargetPos);
                item.PositionIndex = vnewIndex;
            }
        }
        
        // 移动被覆盖的卡牌到空出的位置
        foreach (var kvp in replacementMapping)
        {
            DraggableGridItem coveredItem = kvp.Key;
            Vector2 emptyPos = kvp.Value;
            
            seq.Join(coveredItem.rectTransform.DOAnchorPos(emptyPos, 0.25f));
            // 更新位置索引
            int vnewIndex = GetIndexFromAnchoredPosition(emptyPos);
            coveredItem.PositionIndex = vnewIndex;
        }
        
        // 保存所有需要交换层级的项
        Dictionary<DraggableGridItem, int> allOriginalSiblings = new Dictionary<DraggableGridItem, int>();
        foreach (var item in dragGroupList)
        {
            allOriginalSiblings[item] = originalSiblings[item];
        }
        foreach (var item in coveredItems)
        {
            allOriginalSiblings[item] = item.transform.GetSiblingIndex();
        }
        
        seq.OnComplete(() => {
            // 交换层级关系：保持原有的层级相对顺序
            // 先记录所有项的原始层级
            List<DraggableGridItem> allItems = new List<DraggableGridItem>(dragGroupList);
            allItems.AddRange(coveredItems);
            
            // 创建一个临时列表存储所有项及其原始层级，用于排序
            List<(DraggableGridItem item, int originalIndex)> itemWithOriginalIndex = new List<(DraggableGridItem, int)>();
            foreach (var item in allItems)
            {
                itemWithOriginalIndex.Add((item, allOriginalSiblings[item]));
            }
            
            // 按照原始层级排序
            itemWithOriginalIndex.Sort((a, b) => a.originalIndex.CompareTo(b.originalIndex));
            
            // 重新设置层级，保持相对顺序
            foreach (var (item, _) in itemWithOriginalIndex)
            {
                item.transform.SetAsLastSibling();
            }
            
            // 刷新所有卡牌的位置索引
            picmgr picManager = GetComponentInParent<picmgr>();
            if (picManager != null)
            {
                // 刷新所有卡牌的位置索引
                picManager.RefreshAllPositionIndices();
                // 更新边框显示状态
                picManager.UpdateBorderVisibility();
                picManager.CheckSucess();
                // 动画完成，重置静态拖拽状态，允许新的操作
                isAnyItemDragging = false;
            }

        });
    }
    
    // 按照从左到右、从上到下的规则对位置进行排序
    private void SortPositionsByGridOrder(List<Vector2> positions)
    {
        positions.Sort((a, b) => {
            // 首先按照y坐标排序（从上到下）
            int yComparison = b.y.CompareTo(a.y);
            if (yComparison != 0)
                return yComparison;
            // 然后按照x坐标排序（从左到右）
            return a.x.CompareTo(b.x);
        });
    }
    
    // 按照从左到右、从上到下的规则对卡牌项进行排序
    private void SortItemsByGridOrder(List<DraggableGridItem> items)
    {
        items.Sort((a, b) => {
            // 首先按照y坐标排序（从上到下）
            int yComparison = b.rectTransform.anchoredPosition.y.CompareTo(a.rectTransform.anchoredPosition.y);
            if (yComparison != 0)
                return yComparison;
            // 然后按照x坐标排序（从左到右）
            return a.rectTransform.anchoredPosition.x.CompareTo(b.rectTransform.anchoredPosition.x);
        });
    }
    
    // 根据位置索引获取anchoredPosition
    private Vector2 GetAnchoredPositionFromIndex(int index)
    {
        // 获取关卡的行数和列数
        picmgr picManager = GetComponentInParent<picmgr>();
        if (picManager == null) return Vector2.zero;
        
        int cols = picManager.width;
        int rows = picManager.height;
        
        // 计算x和y坐标
        int y = index / cols;
        int x = index % cols;
        
        // 计算anchoredPosition
        float xPos = x * carWid;
        float yPos = y * carHei;
        
        return new Vector2(xPos, yPos);
    }
    
    // 根据anchoredPosition获取位置索引
    private int GetIndexFromAnchoredPosition(Vector2 position)
    {
        // 获取关卡的行数和列数
        picmgr picManager = GetComponentInParent<picmgr>();
        if (picManager == null) return 0;
        
        int cols = picManager.height;
        
        // 计算网格坐标
        int x = Mathf.RoundToInt(position.x / carWid);
        int y = Mathf.RoundToInt(position.y / carHei);
        
        // 计算位置索引：y * cols + x
        return x * cols + y;
    }
    
    // 按照从左到右、从上到下的规则对卡牌项进行排序
 
    // 根据位置找到对应的卡牌项
    private DraggableGridItem FindItemAtPosition(Vector2 targetPos)
    {
        Transform parent = transform.parent;
        if (parent == null) return null;
        
        float minDistance = float.MaxValue;
        DraggableGridItem closestItem = null;
        
        float gridSpacing = 100f; // 假设网格间距为100，可以根据实际情况调整
        float tolerance = gridSpacing * 0.5f; // 容差设为网格间距的一半
        
        foreach (Transform child in parent)
        {
            // 跳过拖拽组内的项
            if (dragGroup != null && dragGroup.Contains(child.GetComponent<DraggableGridItem>()))
                continue;
            
            DraggableGridItem item = child.GetComponent<DraggableGridItem>();
            if (item == null) continue;
            
            float distance = Vector2.Distance(item.rectTransform.anchoredPosition, targetPos);
            if (distance < tolerance && distance < minDistance)
            {
                minDistance = distance;
                closestItem = item;
            }
        }
        
        return closestItem;
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