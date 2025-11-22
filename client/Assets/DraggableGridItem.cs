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
        
        // 将当前拖拽的节点设置为最上层
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 拖拽过程中更新位置
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        
        // 检查是否靠近其他节点
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
            Tween tween = rectTransform.DOAnchorPos(originalPosition, 0.3f);
            // 动画结束后恢复原始层级并更新边框
            tween.OnComplete(() => {
                transform.SetSiblingIndex(originalSiblingIndex);
                
                // 更新边框显示状态
                picmgr picManager = GetComponentInParent<picmgr>();
                if (picManager != null)
                {
                    picManager.UpdateBorderVisibility();
                }
            });
        }
        
        // 清空目标项引用
        targetItem = null;
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
        // 使用原始位置进行交换，而不是当前位置
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
}