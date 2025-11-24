using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class picmgr : MonoBehaviour
{
    public Texture2D pic;
    public int width=3;
    public int height=3;

    internal static picmgr instance;
    private void Awake()
    {
        instance = this;
    }

    internal void LoadLevel(DrLevel leevel)
    {
        width = leevel.LevelFigureX;
        height = leevel.LevelFigureY;
        pic = Resources.Load(leevel.LevelFigure) as Texture2D;
        CreateGridImages();
        StartCoroutine(rl());
    }
    IEnumerator rl()
    {
        yield return null;
        ShuffleGridPositions();
    }
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
        // 边框宽度（用于边以及角的边长）
        float borderWidth = 20f;

        // 为防止角与边重合，将角大小限制为单元尺寸的一半以内
        float bw = Mathf.Min(borderWidth, cellWidth / 2f, cellHeight / 2f);

        // 创建四个角（左上、右上、右下、左下）——使用正方形区域
        CreateBorder(cellObject, "TopLeftBorder", new Vector2(0, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "TopRightBorder", new Vector2(cellWidth - bw, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomRightBorder", new Vector2(cellWidth - bw, 0), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomLeftBorder", new Vector2(0, 0), new Vector2(bw, bw));

        // 创建四个边（上、下、左、右），缩进到角的内部以避免重合
        // 上/下：从 x = bw 开始，宽度为 cellWidth - 2*bw
        CreateBorder(cellObject, "TopBorder", new Vector2(bw, cellHeight - bw), new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));
        CreateBorder(cellObject, "BottomBorder", new Vector2(bw, 0), new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));

        // 左/右：从 y = bw 开始，高度为 cellHeight - 2*bw
        CreateBorder(cellObject, "LeftBorder", new Vector2(0, bw), new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
        CreateBorder(cellObject, "RightBorder", new Vector2(cellWidth - bw, bw), new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
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
        borderImage.color = Color.white; // 边框颜色设为红色以便观察

        // 为边框添加一个加载器组件，默认从 Resources 目录根据约定名称加载贴图
        BorderSpriteLoader loader = borderObject.AddComponent<BorderSpriteLoader>();
        loader.resourceName = MapBorderNameToResource(name);
        loader.LoadSprite();
    }

    // 将边框对象名映射到 Resources 中的贴图名（约定）
    private string MapBorderNameToResource(string borderName)
    {
        switch (borderName)
        {
            case "TopLeftBorder": return "lt";
            case "TopBorder": return "t";
            case "TopRightBorder": return "rt";
            case "RightBorder": return "r";
            case "BottomRightBorder": return "rb";
            case "BottomBorder": return "b";
            case "BottomLeftBorder": return "lb";
            case "LeftBorder": return "l";
            default: return borderName; // fallback: 使用相同名字
        }
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
            
            if(x==1 && y == 1)
            {
                int xxx = 0;
            }

            // 检查四个方向的逻辑邻居
            // 使用实际位置（anchoredPosition）检测是否有邻居存在在期望的位置
            float checkThreshold = 1.5f; // 允许的距离阈值（像素）

            // 使用当前项的实际anchoredPosition作为基准，加上单元格偏移去寻找相邻项（处理移动后的情况更稳健）
            Vector2 expectedRightPos = itemRect.anchoredPosition + new Vector2(cellWidth, 0);
            Vector2 expectedLeftPos = itemRect.anchoredPosition + new Vector2(-cellWidth, 0);
            Vector2 expectedTopPos = itemRect.anchoredPosition + new Vector2(0, cellHeight);
            Vector2 expectedBottomPos = itemRect.anchoredPosition + new Vector2(0, -cellHeight);

            // 查找占位的项（如果有）并判断该项是否是逻辑上正确的邻居（名称匹配）
            DraggableGridItem rightNeighborItem = FindGridItemAtAnchoredPosition(expectedRightPos, gridItems, checkThreshold);
            DraggableGridItem leftNeighborItem = FindGridItemAtAnchoredPosition(expectedLeftPos, gridItems, checkThreshold);
            DraggableGridItem topNeighborItem = FindGridItemAtAnchoredPosition(expectedTopPos, gridItems, checkThreshold);
            DraggableGridItem bottomNeighborItem = FindGridItemAtAnchoredPosition(expectedBottomPos, gridItems, checkThreshold);

            // 对角线邻居（用于判定角贴图的反转等特殊规则）
            Vector2 expectedTopRightPos = itemRect.anchoredPosition + new Vector2(cellWidth, cellHeight);
            Vector2 expectedTopLeftPos = itemRect.anchoredPosition + new Vector2(-cellWidth, cellHeight);
            Vector2 expectedBottomRightPos = itemRect.anchoredPosition + new Vector2(cellWidth, -cellHeight);
            Vector2 expectedBottomLeftPos = itemRect.anchoredPosition + new Vector2(-cellWidth, -cellHeight);

            DraggableGridItem topRightNeighborItem = FindGridItemAtAnchoredPosition(expectedTopRightPos, gridItems, checkThreshold);
            DraggableGridItem topLeftNeighborItem = FindGridItemAtAnchoredPosition(expectedTopLeftPos, gridItems, checkThreshold);
            DraggableGridItem bottomRightNeighborItem = FindGridItemAtAnchoredPosition(expectedBottomRightPos, gridItems, checkThreshold);
            DraggableGridItem bottomLeftNeighborItem = FindGridItemAtAnchoredPosition(expectedBottomLeftPos, gridItems, checkThreshold);

            bool isRightCorrect = (rightNeighborItem != null && rightNeighborItem.gameObject.name == ("GridCell_" + (x + 1) + "_" + y));
            bool isLeftCorrect = (leftNeighborItem != null && leftNeighborItem.gameObject.name == ("GridCell_" + (x - 1) + "_" + y));
            bool isTopCorrect = (topNeighborItem != null && topNeighborItem.gameObject.name == ("GridCell_" + x + "_" + (y + 1)));
            bool isBottomCorrect = (bottomNeighborItem != null && bottomNeighborItem.gameObject.name == ("GridCell_" + x + "_" + (y - 1)));

            // 对角线正确性判断
            bool isTopRightCorrect = (topRightNeighborItem != null && topRightNeighborItem.gameObject.name == ("GridCell_" + (x + 1) + "_" + (y + 1)));
            bool isTopLeftCorrect = (topLeftNeighborItem != null && topLeftNeighborItem.gameObject.name == ("GridCell_" + (x - 1) + "_" + (y + 1)));
            bool isBottomRightCorrect = (bottomRightNeighborItem != null && bottomRightNeighborItem.gameObject.name == ("GridCell_" + (x + 1) + "_" + (y - 1)));
            bool isBottomLeftCorrect = (bottomLeftNeighborItem != null && bottomLeftNeighborItem.gameObject.name == ("GridCell_" + (x - 1) + "_" + (y - 1)));

            // 当逻辑正确的邻居存在时，视为相邻合法（边不显示）；否则显示边
            bool rightShown = !isRightCorrect;
            bool leftShown = !isLeftCorrect;
            bool topShown = !isTopCorrect;
            bool bottomShown = !isBottomCorrect;

            // 将相邻正确性写回到组件以便在 Inspector 中观察（true 表示逻辑上有正确邻居）
            item.adjacentRight = isRightCorrect;
            item.adjacentLeft = isLeftCorrect;
            item.adjacentTop = isTopCorrect;
            item.adjacentBottom = isBottomCorrect;
            // （已用逻辑正确性赋值，不再使用旧的 hasXNeighbor 变量）

            // 获取边框对象（包括角）
            GameObject rightBorder = FindChildByName(item.gameObject, "RightBorder");
            GameObject leftBorder = FindChildByName(item.gameObject, "LeftBorder");
            GameObject topBorder = FindChildByName(item.gameObject, "TopBorder");
            GameObject bottomBorder = FindChildByName(item.gameObject, "BottomBorder");

            GameObject topLeftBorder = FindChildByName(item.gameObject, "TopLeftBorder");
            GameObject topRightBorder = FindChildByName(item.gameObject, "TopRightBorder");
            GameObject bottomRightBorder = FindChildByName(item.gameObject, "BottomRightBorder");
            GameObject bottomLeftBorder = FindChildByName(item.gameObject, "BottomLeftBorder");

            // 根据邻居关系和位置正确性设置边框显示状态
            if (rightBorder != null)
                rightBorder.SetActive(rightShown);

            if (leftBorder != null)
                leftBorder.SetActive(leftShown);

            if (topBorder != null)
                topBorder.SetActive(topShown);

            if (bottomBorder != null)
                bottomBorder.SetActive(bottomShown);

            // 角的显示规则：当与角相邻的任一边需要显示时，显示角
            if (topLeftBorder != null)
                topLeftBorder.SetActive(topShown || leftShown || !isTopLeftCorrect);

            if (topRightBorder != null)
                topRightBorder.SetActive(topShown || rightShown || !isTopRightCorrect);

            if (bottomRightBorder != null)
                bottomRightBorder.SetActive(bottomShown || rightShown || !isBottomRightCorrect);

            if (bottomLeftBorder != null)
                bottomLeftBorder.SetActive(bottomShown || leftShown || !isBottomLeftCorrect);

            // 刷新角/边的贴图：把具体选择规则委托给 BorderSpriteLoader
            if (topLeftBorder != null)
            {
                BorderSpriteLoader bl = topLeftBorder.GetComponent<BorderSpriteLoader>();
                if (bl != null) bl.RefreshBasedOnFlags("lt", isTopCorrect, isLeftCorrect, isRightCorrect, isBottomCorrect, isTopLeftCorrect);
            }

            if (topRightBorder != null)
            {
                BorderSpriteLoader bl = topRightBorder.GetComponent<BorderSpriteLoader>();
                if (bl != null) bl.RefreshBasedOnFlags("rt", isTopCorrect, isLeftCorrect, isRightCorrect, isBottomCorrect, isTopRightCorrect);
            }

            if (bottomRightBorder != null)
            {
                BorderSpriteLoader bl = bottomRightBorder.GetComponent<BorderSpriteLoader>();
                if (bl != null) bl.RefreshBasedOnFlags("rb",isTopCorrect, isLeftCorrect, isRightCorrect, isBottomCorrect, isBottomRightCorrect);
            }

            if (bottomLeftBorder != null)
            {
                BorderSpriteLoader bl = bottomLeftBorder.GetComponent<BorderSpriteLoader>();
                if (bl != null) bl.RefreshBasedOnFlags("lb",isTopCorrect, isLeftCorrect, isRightCorrect, isBottomCorrect, isBottomLeftCorrect);
            }

            // 额外规则：如果当前格子的左右都是正确邻居，则把左侧节点的右下角和右侧节点的左下角设为底部贴图("b")
            if (isLeftCorrect && isRightCorrect)
            {
                if (leftNeighborItem != null)
                {
                    GameObject neighBR = FindChildByName(leftNeighborItem.gameObject, "BottomRightBorder");
                    if (neighBR != null)
                    {
                        BorderSpriteLoader bl = neighBR.GetComponent<BorderSpriteLoader>();
                        if (bl != null)
                        {
                            // 如果左侧邻居是网格角（任一角），跳过覆盖
                            string[] lnParts = leftNeighborItem.gameObject.name.Replace("GridCell_", "").Split('_');
                            int lnx = int.Parse(lnParts[0]);
                            int lny = int.Parse(lnParts[1]);
                            bool lnIsCorner = (lnx == 0 && lny == (height - 1)) || (lnx == (width - 1) && lny == (height - 1)) || (lnx == (width - 1) && lny == 0) || (lnx == 0 && lny == 0);
                            if (!lnIsCorner) bl.SetResourceAndLoad("b");
                        }
                    }
                }
                if (rightNeighborItem != null)
                {
                    GameObject neighBL = FindChildByName(rightNeighborItem.gameObject, "BottomLeftBorder");
                    if (neighBL != null)
                    {
                        BorderSpriteLoader bl = neighBL.GetComponent<BorderSpriteLoader>();
                        if (bl != null)
                        {
                            // 如果右侧邻居是网格角（任一角），跳过覆盖
                            string[] rnParts = rightNeighborItem.gameObject.name.Replace("GridCell_", "").Split('_');
                            int rnx = int.Parse(rnParts[0]);
                            int rny = int.Parse(rnParts[1]);
                            bool rnIsCorner = (rnx == 0 && rny == (height - 1)) || (rnx == (width - 1) && rny == (height - 1)) || (rnx == (width - 1) && rny == 0) || (rnx == 0 && rny == 0);
                            if (!rnIsCorner) bl.SetResourceAndLoad("b");
                        }
                    }
                }
            } 
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

    // 在指定的anchoredPosition附近查找网格项（根据实际位置检测）
    private DraggableGridItem FindGridItemAtAnchoredPosition(Vector2 anchoredPosition, List<DraggableGridItem> gridItems, float threshold)
    {
        foreach (DraggableGridItem item in gridItems)
        {
            RectTransform rt = item.GetComponent<RectTransform>();
            if (rt == null) continue;
            if (Vector2.Distance(rt.anchoredPosition, anchoredPosition) <= threshold)
            {
                return item;
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