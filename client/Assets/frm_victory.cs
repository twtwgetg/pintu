//using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class frm_victory : frmbase
{
    private const float DesignWidth = 720f;
    private const float DesignHeight = 1280f;
    private const float DefaultPictureAspect = 16f / 9f;

    public RawImage img;
    public Button close;

    private RectTransform rootRect;
    private RectTransform middleGroup;
    private RectTransform titleGroup;
    private RectTransform pictureGroup;
    private RectTransform frameGroup;
    private RectTransform rewardGroup;
    private RectTransform bottomGroup;
    private RectTransform coinGroup;
    private RectTransform closeRect;
    private Vector2 lastRootSize;
    private float layoutScale = 1f;
    private float pictureAspect = DefaultPictureAspect;
    private bool isApplyingLayout;

    private void Awake()
    {
        CacheLayoutRefs();

        Main.RegistEvent("show_victory", (x)=>
        { 
            show();
            ApplyResponsiveLayout();

            GalleryLevel leevel =  x as GalleryLevel;
            string url = GalleryManager.GetFigureUrl(leevel.figure);
            Main.inst.StartCoroutine(Main.LoadTextureFromCDN(url, (texture) =>
            {
                img.texture = texture;
                UpdatePictureAspect(texture);
                ApplyResponsiveLayout();
            }));

            return null;
        });
        close.onClick.AddListener(()=>
        {
            Main.SendEvent("level_next");
            hide();
        });
    }

    protected override void OnShow()
    {
        CacheLayoutRefs();
        ApplyResponsiveLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled && rootRect != null && rootRect.gameObject.activeInHierarchy)
        {
            ApplyResponsiveLayout();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rootRect == null || !rootRect.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 rootSize = GetRootSize();
        if ((rootSize - lastRootSize).sqrMagnitude > 0.5f)
        {
            ApplyResponsiveLayout();
        }
    }

    private void CacheLayoutRefs()
    {
        rootRect = gb as RectTransform;
        if (rootRect == null)
        {
            return;
        }

        middleGroup = rootRect.Find("MiddleGroup") as RectTransform;
        titleGroup = middleGroup != null ? middleGroup.Find("TitleGroup") as RectTransform : null;
        pictureGroup = middleGroup != null ? middleGroup.Find("PictureGroup") as RectTransform : null;
        frameGroup = pictureGroup != null ? pictureGroup.Find("FrameGroup") as RectTransform : null;
        rewardGroup = rootRect.Find("RewardGroup") as RectTransform;
        bottomGroup = rootRect.Find("BottomGroup") as RectTransform;
        coinGroup = rootRect.Find("CoinGroup") as RectTransform;
        closeRect = close != null ? close.GetComponent<RectTransform>() : null;
    }

    private void ApplyResponsiveLayout()
    {
        if (isApplyingLayout)
        {
            return;
        }

        isApplyingLayout = true;
        try
        {
        if (rootRect == null)
        {
            CacheLayoutRefs();
        }

        if (rootRect == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        Vector2 rootSize = GetRootSize();
        lastRootSize = rootSize;

        float sidePadding = Mathf.Clamp(rootSize.x * 0.05f, 24f, 48f);
        float verticalPadding = Mathf.Clamp(rootSize.y * 0.035f, 24f, 56f);
        float contentWidth = Mathf.Max(1f, rootSize.x - sidePadding * 2f);
        float contentHeight = Mathf.Max(1f, rootSize.y - verticalPadding * 2f);
        layoutScale = Mathf.Min(contentWidth / DesignWidth, contentHeight / DesignHeight);
        layoutScale = Mathf.Clamp(layoutScale, 0.45f, 1.15f);

        float designContentWidth = DesignWidth * layoutScale;
        float topY = rootSize.y * 0.5f - verticalPadding;
        float bottomY = -rootSize.y * 0.5f + verticalPadding;
        UpdatePictureAspect(img != null ? img.texture : null);

        StretchToParent(middleGroup);

        float titleHeight = 147f * layoutScale;
        float titleY = topY - titleHeight * 0.5f - 18f * layoutScale;
        SetCentered(titleGroup, new Vector2(0f, titleY), new Vector2(designContentWidth, titleHeight));

        float bottomHeight = 140f * layoutScale;
        float bottomCenterY = bottomY + bottomHeight * 0.5f;
        SetCentered(bottomGroup, new Vector2(0f, bottomCenterY), new Vector2(designContentWidth, bottomHeight));

        float buttonWidth = 320f * layoutScale;
        float buttonHeight = 108f * layoutScale;
        SetCentered(closeRect, new Vector2(0f, 12f * layoutScale), new Vector2(buttonWidth, buttonHeight));

        float rewardHeight = 60f * layoutScale;
        float rewardCenterY = bottomCenterY + bottomHeight * 0.5f + rewardHeight * 0.5f + 18f * layoutScale;
        SetCentered(rewardGroup, new Vector2(0f, rewardCenterY), new Vector2(designContentWidth * 0.84f, rewardHeight));

        float pictureTop = titleY - titleHeight * 0.5f - 24f * layoutScale;
        float pictureBottom = rewardCenterY + rewardHeight * 0.5f + 24f * layoutScale;
        float pictureAvailableWidth = Mathf.Max(1f, designContentWidth - 76f * layoutScale);
        float pictureAvailableHeight = Mathf.Max(1f, pictureTop - pictureBottom);
        Vector2 pictureSize = FitSize(pictureAvailableWidth, pictureAvailableHeight, pictureAspect);
        float pictureCenterY = (pictureTop + pictureBottom) * 0.5f;
        SetCentered(pictureGroup, new Vector2(0f, pictureCenterY), pictureSize);
        StretchToParent(frameGroup);

        if (coinGroup != null)
        {
            float coinWidth = 200f * layoutScale;
            float coinHeight = 64f * layoutScale;
            float coinX = rootSize.x * 0.5f - sidePadding - coinWidth * 0.5f;
            float coinY = topY - coinHeight * 0.5f;
            SetCentered(coinGroup, new Vector2(coinX, coinY), new Vector2(coinWidth, coinHeight));
        }

        ApplyPictureAspect(img != null ? img.texture : null);
        }
        finally
        {
            isApplyingLayout = false;
        }
    }

    private void ApplyPictureAspect(Texture texture)
    {
        UpdatePictureAspect(texture);

        if (img == null)
        {
            return;
        }

        RectTransform imageRect = img.rectTransform;
        RectTransform parentRect = imageRect.parent as RectTransform;
        Vector2 parentSize = parentRect != null ? parentRect.rect.size : GetRootSize();

        float insetX = 24f * layoutScale;
        float insetY = 18f * layoutScale;
        float availableWidth = Mathf.Max(1f, parentSize.x - insetX * 2f);
        float availableHeight = Mathf.Max(1f, parentSize.y - insetY * 2f);
        Vector2 imageSize = FitSize(availableWidth, availableHeight, pictureAspect);

        SetCentered(imageRect, Vector2.zero, imageSize);
    }

    private void UpdatePictureAspect(Texture texture)
    {
        if (texture != null && texture.height > 0)
        {
            pictureAspect = Mathf.Max(0.01f, (float)texture.width / texture.height);
        }
    }

    private Vector2 GetRootSize()
    {
        Vector2 size = rootRect != null ? rootRect.rect.size : Vector2.zero;
        if (size.x <= 0f || size.y <= 0f)
        {
            size = new Vector2(Screen.width, Screen.height);
        }

        return size;
    }

    private static Vector2 FitSize(float availableWidth, float availableHeight, float aspect)
    {
        aspect = Mathf.Max(0.01f, aspect);
        if (availableWidth / availableHeight > aspect)
        {
            return new Vector2(availableHeight * aspect, availableHeight);
        }

        return new Vector2(availableWidth, availableWidth / aspect);
    }

    private static void StretchToParent(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void SetCentered(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }
}
