using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class picmgr : MonoBehaviour
{
    public Texture2D pic;
    public int width=3;
    public int height=3;

    internal static picmgr instance;
    private void Awake()
    {
        instance = this;
        ResizeChapterContent();
    }
    public void ResizeChapterContent()
    {
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        //// 设置边界边距（最低50像素）
        float margin = 100f;

        //// 计算可用空间
        float availableWidth = screenWidth - 2 * margin;
        float availableHeight = screenHeight - 2 * margin;

        //// 9:16宽高比的目标宽高
        float targetWidth, targetHeight;

        //// 根据可用空间计算最佳尺寸
        if (availableWidth / availableHeight > 9f / 16f)
        {
            // 以高度为基准
            targetHeight = availableHeight;
            targetWidth = targetHeight * 9f / 16f;
        }
        else
        {
            // 以宽度为基准
            targetWidth = availableWidth;
            targetHeight = targetWidth * 16f / 9f;
        }

        float wside = (screenWidth - targetWidth) / 2;
        float hside = (screenHeight - targetHeight) / 2;
        trans.offsetMin = new Vector2(wside, hside);  // Left和Bottom（offsetMin = (Left, Bottom)）
        trans.offsetMax = new Vector2(-wside, -hside); // Right和Top（offsetMax = (-Right, -Top)）

        //// 设置chaptercontent的尺寸
        //chaptercontent.sizeDelta = new Vector2(targetWidth, targetHeight);

        //// 设置chaptercontent居中
        //chaptercontent.anchoredPosition = Vector2.zero;
    }
    RectTransform trans
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }
    public IEnumerator  LoadLevel(DrLevel leevel)
    {
       
        width = leevel.LevelFigureX;
        height = leevel.LevelFigureY;
        pic = Resources.Load(leevel.LevelFigure) as Texture2D;
        // 传递关卡ID、最大原位数量和难度参数
        yield return Main.inst.StartCoroutine( CreateGridImages(leevel.Id, leevel.OutOfPlaceNumber, leevel.DifficultyTier == 2)); 
    } 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void tCreateGridImages()
    {
        // 编辑器测试方法，使用默认参数
        StartCoroutine(CreateGridImages(1, -1, false));
    }
    // 添加的方法：根据当前recttransform尺寸拆分成width*height个格子，每个格子填充一个RawImage
    // level: 关卡编号，用于确保每个关卡每次刷新出的位置是相同的
    // maxKeepCount: 最多留在原位的卡牌数量，-1表示使用默认值
    // isHard: 是否为困难模式
    public IEnumerator CreateGridImages(int level = 1, int maxKeepCount = -1, bool isHard = false)
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
        float time_fp = 1f;
        float delay = time_fp / (width * height);
        // 创建width*height个格子
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                yield return new WaitForSeconds(delay);
                // 创建一个空的游戏对象
                GameObject cellObject = GameObject.Instantiate(Resources.Load("GridCell") as GameObject);
                cellObject.name = $"GridCell_{x}_{y}";
                cellObject.transform.SetParent(transform, false);

                // 添加RectTransform组件
                RectTransform cellRect = cellObject.GetComponent<RectTransform>();
                
                // 设置位置和尺寸
                cellRect.anchorMin = new Vector2(0, 0);
                cellRect.anchorMax = new Vector2(0, 0);
                cellRect.pivot = new Vector2(0, 0);
                cellRect.anchoredPosition = new Vector2(0f, 0);//
                                                              //new Vector2(x * cellWidth, y * cellHeight);


                cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);


                // 计算并设置UV坐标
                float uvX = (float)x / width;
                float uvY = (float)y / height;
                float uvWidth = 1.0f / width;
                float uvHeight = 1.0f / height;

                
                // 添加拖拽组件
                var dg = cellObject.GetComponent<DraggableGridItem>();
                dg.canvas = GetComponentInParent<Canvas>();
                dg.pic = pic;
                dg.uvX = uvX;
                dg.uvY = uvY;
                dg.uvWidth = uvWidth;
                dg.uvHeight = uvHeight;

                
                // 创建边框
                CreateBorders(cellObject, cellWidth, cellHeight);
                var pos = new Vector2(x * cellWidth, y * cellHeight);
                cellRect.DOAnchorPos(pos, 0.2f).onComplete=()=>
                {
                    cellRect.anchoredPosition = pos;
                };

            }
        }
        yield return new WaitForSeconds(0.3f);
        ShuffleGridPositions(level, maxKeepCount, isHard); 
        for (int i = 0; i < transform.childCount; i++)
        {
            var x = transform.GetChild(i);
            var d = x.GetComponent<DraggableGridItem>();
            d.Turn();
        }
     
    }
    
    // 创建边框
    private void CreateBorders(GameObject cellObject, float cellWidth, float cellHeight)
    {
        for(int i = cellObject.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy( cellObject.transform.GetChild(i).gameObject);
        }
        
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
    /*
     * 检查胜利
     */
    internal void CheckSucess()
    {
        bool suc = true;
        foreach (Transform child in transform)
        {
            var c = child.GetComponent<DraggableGridItem>();
            if (c == null) continue;
            
            // 解析当前卡牌名称，格式为 "GridCell_x_y"
            string[] parts = child.name.Replace("GridCell_", "").Split('_');
            if (parts.Length != 2) continue;
            
            int x, y;
            if (!int.TryParse(parts[0], out x) || !int.TryParse(parts[1], out y)) continue;
            
            // 计算预期的邻居名称
            string expectedRightName = "GridCell_" + (x + 1) + "_" + y;
            string expectedLeftName = "GridCell_" + (x - 1) + "_" + y;
            string expectedTopName = "GridCell_" + x + "_" + (y + 1);
            string expectedBottomName = "GridCell_" + x + "_" + (y - 1);
            
            // 查找实际的邻居
            DraggableGridItem rightNeighbor = null;
            DraggableGridItem leftNeighbor = null;
            DraggableGridItem topNeighbor = null;
            DraggableGridItem bottomNeighbor = null;
            
            foreach (Transform otherChild in transform)
            {
                if (otherChild == child) continue;
                
                var otherItem = otherChild.GetComponent<DraggableGridItem>();
                if (otherItem == null) continue;
                
                // 检查相邻位置
                RectTransform otherRect = otherChild.GetComponent<RectTransform>();
                RectTransform currentRect = child.GetComponent<RectTransform>();
                if (otherRect == null || currentRect == null) continue;
                
                // 计算位置差异
                Vector2 diff = otherRect.anchoredPosition - currentRect.anchoredPosition;
                float threshold = 1.0f; // 位置差异阈值
                
                if (Mathf.Abs(diff.x - currentRect.sizeDelta.x) < threshold && Mathf.Abs(diff.y) < threshold)
                {
                    rightNeighbor = otherItem;
                }
                else if (Mathf.Abs(diff.x + currentRect.sizeDelta.x) < threshold && Mathf.Abs(diff.y) < threshold)
                {
                    leftNeighbor = otherItem;
                }
                else if (Mathf.Abs(diff.y - currentRect.sizeDelta.y) < threshold && Mathf.Abs(diff.x) < threshold)
                {
                    topNeighbor = otherItem;
                }
                else if (Mathf.Abs(diff.y + currentRect.sizeDelta.y) < threshold && Mathf.Abs(diff.x) < threshold)
                {
                    bottomNeighbor = otherItem;
                }
            }
            
            // 检查邻居名称是否正确
            bool isRightCorrect = (rightNeighbor == null) || (rightNeighbor.gameObject.name == expectedRightName);
            bool isLeftCorrect = (leftNeighbor == null) || (leftNeighbor.gameObject.name == expectedLeftName);
            bool isTopCorrect = (topNeighbor == null) || (topNeighbor.gameObject.name == expectedTopName);
            bool isBottomCorrect = (bottomNeighbor == null) || (bottomNeighbor.gameObject.name == expectedBottomName);
            
            // 如果任何一个方向不正确，设置suc为false
            if (!isRightCorrect || !isLeftCorrect || !isTopCorrect || !isBottomCorrect)
            {
                suc = false;
                break;
            }
        }

        if (suc)
        {
            var t = transform.GetComponent<RectTransform>();
            if (t != null)
            {
                // 向上移动100像素
                Vector2 currentPos = t.anchoredPosition;
                t.DOAnchorPos(new Vector2(currentPos.x, currentPos.y + 100f), 0.5f).onComplete=() =>
                {
                    // 显示胜利界面
                    for(int i = 0;i< transform.childCount; i++)
                    {
                        var x= transform.GetChild(i);
                        x.GetComponent<DraggableGridItem>().enabled=false;
                    }
                    Main.DispEvent("show_next");
                };
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
    // level: 关卡编号，用于确保每个关卡每次刷新出的位置是相同的
    // maxKeepCount: 最多留在原位的卡牌数量，-1表示使用默认值
    // isHard: 是否为困难模式
    public void ShuffleGridPositions(int level = 1, int maxKeepCount = -1, bool isHard = false)
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
        List<Vector2> originalPositions = new List<Vector2>();
        for (int i = 0; i < children.Count; i++)
        {
            originalPositions.Add(children[i].anchoredPosition);
        }
        
        // 创建可用位置列表（初始为所有位置）
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < children.Count; i++)
        {
            availableIndices.Add(i);
        }
        
        // 计算网格的宽高
        int gridWidth = width;
        int gridHeight = height;
        
        // 计算默认maxKeepCount：总卡牌数的30%
        if (maxKeepCount == -1)
        {
            maxKeepCount = Mathf.Max(0, Mathf.RoundToInt(children.Count * 0.3f));
        }
        // 确保maxKeepCount在有效范围内
        maxKeepCount = Mathf.Clamp(maxKeepCount, 0, children.Count - 1);
        
        // 随机选择要保留在原位的卡牌数量（0到maxKeepCount之间）
        // 使用关卡编号和难度作为随机种子，确保每次结果一致
        int seed = level * 1000 + (isHard ? 1 : 0);
        System.Random rng = new System.Random(seed);
        int cardsToKeep = rng.Next(0, maxKeepCount + 1);
        
        // 随机选择要保留在原位的卡牌索引
        List<int> keepIndices = new List<int>();
        List<int> allIndices = new List<int>();
        for (int i = 0; i < children.Count; i++)
        {
            allIndices.Add(i);
        }
        
        // 打乱allIndices列表，使用固定种子确保结果一致
        System.Random shuffleRng = new System.Random(seed + 1);
        for (int i = allIndices.Count - 1; i > 0; i--)
        {
            int j = shuffleRng.Next(0, i + 1);
            int temp = allIndices[i];
            allIndices[i] = allIndices[j];
            allIndices[j] = temp;
        }
        
        // 选择前cardsToKeep个索引作为要保留的卡牌
        for (int i = 0; i < cardsToKeep; i++)
        {
            keepIndices.Add(allIndices[i]);
            // 从可用位置列表中移除这些索引，它们将保持原位
            availableIndices.Remove(allIndices[i]);
        }
        
        // 为剩余的卡牌分配新位置
        List<int> targetIndices = new List<int>(availableIndices);
        
        // 打乱targetIndices列表，使用固定种子确保结果一致
        System.Random targetRng = new System.Random(seed + 2);
        for (int i = targetIndices.Count - 1; i > 0; i--)
        {
            int j = targetRng.Next(0, i + 1);
            int temp = targetIndices[i];
            targetIndices[i] = targetIndices[j];
            targetIndices[j] = temp;
        }
        
        // 应用位置变化
        for (int i = 0; i < children.Count; i++)
        {
            if (keepIndices.Contains(i))
            {
                // 保持原位
                continue;
            }
            
            // 找到当前索引在availableIndices中的位置
            int indexInAvailable = availableIndices.IndexOf(i);
            if (indexInAvailable >= 0 && indexInAvailable < targetIndices.Count)
            {
                int targetIndex = targetIndices[indexInAvailable];
                children[i].anchoredPosition = originalPositions[targetIndex];
            }
        }
        
        // 更新边框显示状态
        UpdateBorderVisibility();
    }
}