using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class picmgr : MonoBehaviour
{
    public Texture2D pic;
    public int width=3;
    public int height=3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 添加的方法：根据当前recttransform尺寸拆分成width*height个格子，每个格子填充一个RawImage
    public void CreateGridImages()
    {
        // 清除现有的子对象（使用倒序删除避免遍历时修改集合的问题）
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            // 在编辑器中使用DestroyImmediate，在运行时使用Destroy
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
            #else
            Destroy(child.gameObject);
            #endif
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 rectSize = rectTransform.rect.size;
        
        // 计算每个格子的尺寸
        float cellWidth = rectSize.x / width;
        float cellHeight = rectSize.y / height;

        // 创建width*height个格子
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 创建一个空的游戏对象
                GameObject cellObject = new GameObject("GridCell_" + x + "_" + y);
                cellObject.transform.SetParent(transform, false);

                // 添加RectTransform组件
                RectTransform cellRect = cellObject.AddComponent<RectTransform>();
                
                // 设置位置和尺寸
                cellRect.anchorMin = new Vector2(0, 0);
                cellRect.anchorMax = new Vector2(0, 0);
                cellRect.pivot = new Vector2(0, 0);
                cellRect.anchoredPosition = new Vector2(x * cellWidth, y * cellHeight);
                cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);

                // 添加RawImage组件
                RawImage rawImage = cellObject.AddComponent<RawImage>();
                rawImage.texture = pic;

                // 计算并设置UV坐标
                float uvX = (float)x / width;
                float uvY = (float)y / height;
                float uvWidth = 1.0f / width;
                float uvHeight = 1.0f / height;
                
                rawImage.uvRect = new Rect(uvX, uvY, uvWidth, uvHeight);
                
                // 添加拖拽组件
                cellObject.AddComponent<DraggableGridItem>();
                
                // 添加事件触发器组件（用于拖拽）
                UnityEngine.EventSystems.EventTrigger eventTrigger = cellObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                
                // 创建边框
                CreateBorders(cellObject, cellWidth, cellHeight);
            }
        }
        
        // 更新边框显示状态
        UpdateBorderVisibility();
    }
    
    // 创建边框
    private void CreateBorders(GameObject cellObject, float cellWidth, float cellHeight)
    {
        // 边框宽度
        float borderWidth = 20f;
        
        // 创建上边框（在子节点内部顶部）
        CreateBorder(cellObject, "TopBorder", new Vector2(0, cellHeight - borderWidth), new Vector2(cellWidth, borderWidth));
        
        // 创建下边框（在子节点内部底部）
        CreateBorder(cellObject, "BottomBorder", new Vector2(0, 0), new Vector2(cellWidth, borderWidth));
        
        // 创建左边框（在子节点内部左侧）
        CreateBorder(cellObject, "LeftBorder", new Vector2(0, 0), new Vector2(borderWidth, cellHeight));
        
        // 创建右边框（在子节点内部右侧）
        CreateBorder(cellObject, "RightBorder", new Vector2(cellWidth - borderWidth, 0), new Vector2(borderWidth, cellHeight));
    }
    
    // 创建单个边框
    private void CreateBorder(GameObject parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject borderObject = new GameObject(name);
        borderObject.transform.SetParent(parent.transform, false);
        
        RectTransform borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = new Vector2(0, 0);
        borderRect.anchorMax = new Vector2(0, 0);
        borderRect.pivot = new Vector2(0, 0);
        borderRect.anchoredPosition = anchoredPosition;
        borderRect.sizeDelta = sizeDelta;
        
        Image borderImage = borderObject.AddComponent<Image>();
        borderImage.color = Color.red; // 边框颜色设为红色以便观察
    }
    
    // 更新边框显示状态
    public void UpdateBorderVisibility()
    {
        // 获取所有子节点
        List<DraggableGridItem> gridItems = new List<DraggableGridItem>();
        foreach (Transform child in transform)
        {
            DraggableGridItem item = child.GetComponent<DraggableGridItem>();
            if (item != null)
            {
                gridItems.Add(item);
            }
        }
        
        // 计算每个格子的尺寸
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 rectSize = rectTransform.rect.size;
        float cellWidth = rectSize.x / width;
        float cellHeight = rectSize.y / height;
        
        // 检查每对相邻节点的关系
        foreach (DraggableGridItem item in gridItems)
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            if (itemRect == null) continue;
            
            // 解析节点坐标（逻辑位置）
            string[] parts = item.gameObject.name.Replace("GridCell_", "").Split('_');
            if (parts.Length != 2) continue;
            
            int x, y;
            if (!int.TryParse(parts[0], out x) || !int.TryParse(parts[1], out y)) continue;
            
            // 检查四个方向的逻辑邻居
            DraggableGridItem rightNeighbor = FindGridItemByName("GridCell_" + (x + 1) + "_" + y);
            DraggableGridItem leftNeighbor = FindGridItemByName("GridCell_" + (x - 1) + "_" + y);
            DraggableGridItem topNeighbor = FindGridItemByName("GridCell_" + x + "_" + (y + 1));
            DraggableGridItem bottomNeighbor = FindGridItemByName("GridCell_" + x + "_" + (y - 1));
            
            // 检查邻居是否在正确的位置
            bool isRightCorrect = false;
            bool isLeftCorrect = false;
            bool isTopCorrect = false;
            bool isBottomCorrect = false;
            
            if (rightNeighbor != null)
            {
                RectTransform neighborRect = rightNeighbor.GetComponent<RectTransform>();
                if (neighborRect != null)
                {
                    // 右邻居应该在(item.x + 1, item.y)位置
                    Vector2 expectedPosition = new Vector2((x + 1) * cellWidth, y * cellHeight);
                    isRightCorrect = Vector2.Distance(neighborRect.anchoredPosition, expectedPosition) < 1f;
                }
            }
            
            if (leftNeighbor != null)
            {
                RectTransform neighborRect = leftNeighbor.GetComponent<RectTransform>();
                if (neighborRect != null)
                {
                    // 左邻居应该在(item.x - 1, item.y)位置
                    Vector2 expectedPosition = new Vector2((x - 1) * cellWidth, y * cellHeight);
                    isLeftCorrect = Vector2.Distance(neighborRect.anchoredPosition, expectedPosition) < 1f;
                }
            }
            
            if (topNeighbor != null)
            {
                RectTransform neighborRect = topNeighbor.GetComponent<RectTransform>();
                if (neighborRect != null)
                {
                    // 上邻居应该在(item.x, item.y + 1)位置
                    Vector2 expectedPosition = new Vector2(x * cellWidth, (y + 1) * cellHeight);
                    isTopCorrect = Vector2.Distance(neighborRect.anchoredPosition, expectedPosition) < 1f;
                }
            }
            
            if (bottomNeighbor != null)
            {
                RectTransform neighborRect = bottomNeighbor.GetComponent<RectTransform>();
                if (neighborRect != null)
                {
                    // 下邻居应该在(item.x, item.y - 1)位置
                    Vector2 expectedPosition = new Vector2(x * cellWidth, (y - 1) * cellHeight);
                    isBottomCorrect = Vector2.Distance(neighborRect.anchoredPosition, expectedPosition) < 1f;
                }
            }
            
            // 获取边框对象
            GameObject rightBorder = FindChildByName(item.gameObject, "RightBorder");
            GameObject leftBorder = FindChildByName(item.gameObject, "LeftBorder");
            GameObject topBorder = FindChildByName(item.gameObject, "TopBorder");
            GameObject bottomBorder = FindChildByName(item.gameObject, "BottomBorder");
            
            // 根据邻居关系和位置正确性设置边框显示状态
            if (rightBorder != null)
                rightBorder.SetActive(rightNeighbor == null || !isRightCorrect); // 没有右邻居或右邻居位置不正确时显示右边框
                
            if (leftBorder != null)
                leftBorder.SetActive(leftNeighbor == null || !isLeftCorrect); // 没有左邻居或左邻居位置不正确时显示左边框
                
            if (topBorder != null)
                topBorder.SetActive(topNeighbor == null || !isTopCorrect); // 没有上邻居或上邻居位置不正确时显示上边框
                
            if (bottomBorder != null)
                bottomBorder.SetActive(bottomNeighbor == null || !isBottomCorrect); // 没有下邻居或下邻居位置不正确时显示下边框
        }
    }
    
    // 根据名称查找网格项
    private DraggableGridItem FindGridItemByName(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child.GetComponent<DraggableGridItem>();
            }
        }
        return null;
    }
    
    // 根据名称查找子对象
    private GameObject FindChildByName(GameObject parent, string name)
    {
        Transform child = parent.transform.Find(name);
        return child != null ? child.gameObject : null;
    }
    
    // 新增方法：打乱网格位置
    public void ShuffleGridPositions()
    {
        // 收集所有子对象
        List<RectTransform> children = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                children.Add(rect);
            }
        }
        
        // 如果子对象数量不足2个，无法打乱
        if (children.Count < 2)
        {
            return;
        }
        
        // 获取网格单元的尺寸（从第一个子对象获取）
        Vector2 cellSize = children[0].sizeDelta;
        
        // 创建位置列表
        List<Vector2> positions = new List<Vector2>();
        for (int i = 0; i < children.Count; i++)
        {
            positions.Add(children[i].anchoredPosition);
        }
        
        // 使用Fisher-Yates算法打乱位置
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // 交换位置
            Vector2 temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }
        
        // 应用打乱后的位置
        for (int i = 0; i < children.Count; i++)
        {
            children[i].anchoredPosition = positions[i];
        }
        
        // 更新边框显示状态
        UpdateBorderVisibility();
    }
}