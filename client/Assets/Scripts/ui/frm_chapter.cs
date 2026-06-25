using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.PackageManager;

#if UNITY_WEBGL && MINIGAME_SUBPLATFORM_DOUYIN
using TTSDK;
#endif

public class frm_chapter : frmbase
{
    private const float CardAspect = 102f / 148f;
    private const float SpacingXRatio = 0.18f;
    private const float SpacingYRatio = 0.12f;
    private const float MinSpacingX = 18f;
    private const float MinSpacingY = 16f;
    private const float MaxCardWidth = 118f;

    public RectTransform chaptercontent;
    public Button btn;
    public Button btnsetup;
    public Button btn_addpower;
    public TextMeshProUGUI staminaText, levelname;
    public power mpower;
    public string rewardedVideoAdId = "3snlsbdejtn8h73ih6";
     
    private TextMeshProUGUI btnText;
    private bool isShowingRewardedVideo;

    private void Awake()
    { 
        FindBtnText();

        Main.RegistEvent("gamebegin", (x) =>
        {
            Main.inst.StartCoroutine(brushChapterContent());
            show();
            UpdateStaminaDisplay();
            return 1;
        });

        Main.RegistEvent("onpowerChange", (x) =>
        {
            UpdateStaminaDisplay();
            return null;
        });
        Main.RegistEvent("level_next", (x) =>
        {
            PlayerData.gd.levelid = datamgr.Instance.GetLevel(PlayerData.gd.levelid).NextLevel;
            Main.inst.StartCoroutine(brushChapterContent());
            show();
            return 1;
        });
        Main.RegistEvent("onChapterChange", (x) =>
        {
            Main.inst.StartCoroutine(brushChapterContent());
            show();
            return 1;
        });
        Main.RegistEvent("level_back", (x) =>
        {
            show();
            return null;
        });
        btn.onClick.AddListener(() =>
        {
            if (!isTurning())
            {
                int ret = (int)Main.DispEvent("level_play", PlayerData.gd.levelid);
                if (ret == 1)
                {
                    hide();
                }
            }
        });
        btn_addpower.onClick.AddListener(() =>
        {
            ShowRewardedVideoForPower();
        });
        btnsetup.onClick.AddListener(() =>
        {
            Main.DispEvent("show_setup", null);
        });
    }

    private void ShowRewardedVideoForPower()
    {
        if (isShowingRewardedVideo) return;
        isShowingRewardedVideo = true;

#if UNITY_WEBGL && MINIGAME_SUBPLATFORM_DOUYIN
        if (Application.isEditor)
        {
            AddRewardedPower();
            isShowingRewardedVideo = false;
            return;
        }

        if (string.IsNullOrEmpty(rewardedVideoAdId))
        {
            isShowingRewardedVideo = false;
            Main.DispEvent("event_msg", "激励视频广告位未配置");
            return;
        }
        Debug.LogWarning($"begin create rewardvideo");
        TT.CreateRewardedVideoAd(
            rewardedVideoAdId,
            (isEnded, rewardCount) =>
            {
                isShowingRewardedVideo = false;
                if (isEnded)
                {
                    AddRewardedPower();
                }
                else
                {
                    Main.DispEvent("event_msg", "看完视频可获得体力");
                }
            },
            (errorCode, errorMsg) =>
            {
                isShowingRewardedVideo = false;
                Debug.LogWarning($"Rewarded video failed: {errorCode} {errorMsg}");
                Main.DispEvent("event_msg", "视频暂不可用，请稍后再试");
            },
            false,
            null,
            0,
            false
        );
#else
        AddRewardedPower();
        isShowingRewardedVideo = false;
#endif
    }

    private void AddRewardedPower()
    {
        PlayerData.gd.AddPower(20);
        Main.DispEvent("event_msg", "体力+20");
    }

    private void FindBtnText()
    {
        if (btnText != null || btn == null) return;
        btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
    }

    private bool isTurning()
    {
        for (int i = 0; i < chaptercontent.childCount; i++)
        {
            if (chaptercontent.GetChild(i).GetComponent<card>().isTurning)
                return true;
        }
        return false;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateStaminaDisplay();
    }

    private IEnumerator brushChapterContent()
    {
        btn.gameObject.SetActive(false);
        levelname.gameObject.SetActive(false);
        Main.DispEvent("event_loading", true);

        var chapter = datamgr.Instance.GetChapter(PlayerData.gd.currChapter);

        Texture pic = null;
        yield return Main.inst.StartCoroutine(Main.LoadTextureFromCDN(chapter.ChapterFigure, (texture) =>
        {
            pic = texture;
        }));

        Main.DispEvent("event_loading", false);
        int chapterNum = PlayerData.gd.currChapter;
        int levelNum = PlayerData.gd.levelid % 100000;
        levelname.text = $"{chapterNum}-{levelNum}";
        FindBtnText();
        if (btnText != null) btnText.text = $"挑战 {chapterNum}-{levelNum}";

        for (int i = chaptercontent.childCount - 1; i >= 0; i--)
        {
            Transform child = chaptercontent.GetChild(i);
#if UNITY_EDITOR
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        GridLayoutGroup grid = chaptercontent.GetComponent<GridLayoutGroup>();
        ApplyChapterGridLayout(grid, chapter.ChapterFigureX, chapter.ChapterFigureY);

        for (int j = 0; j < chapter.ChapterFigureY; j++)
        {
            for (int i = 0; i < chapter.ChapterFigureX; i++)
            {
                int idx = j * chapter.ChapterFigureX + i;
                int lid = chapter.LevelId[idx];

                GameObject cellObject = Instantiate(Resources.Load("levelpic")) as GameObject;
                cellObject.transform.SetParent(chaptercontent, false);
                card pcard = cellObject.GetComponent<card>();
                pcard.levelid = lid;

                RectTransform cellRect = cellObject.GetComponent<RectTransform>();
                // GridLayoutGroup 自动管理位置和尺寸，只需设置 anchor 和 pivot
                cellRect.anchorMin = new Vector2(0, 0);
                cellRect.anchorMax = new Vector2(1, 1);
                cellRect.pivot = new Vector2(0.5f, 0.5f);
                cellRect.anchoredPosition = Vector2.zero;
                cellRect.sizeDelta = Vector2.zero;

                pcard.uvX = (float)i / chapter.ChapterFigureX;
                pcard.uvY = (float)j / chapter.ChapterFigureY;
                pcard.uvWidth = 1.0f / chapter.ChapterFigureX;
                pcard.uvHeight = 1.0f / chapter.ChapterFigureY;
                pcard.texture = pic;
                pcard.Load();
                int showLevel = lid % 100000;
                pcard.level.text = showLevel.ToString();
            }
        }

        btn.gameObject.SetActive(true);
        levelname.gameObject.SetActive(true);
    }

    private void ApplyChapterGridLayout(GridLayoutGroup grid, int cols, int rows)
    {
        if (grid == null || cols <= 0 || rows <= 0) return;

        Canvas.ForceUpdateCanvases();

        Vector2 rectSize = chaptercontent.rect.size;
        float availableWidth = Mathf.Max(1f, rectSize.x - grid.padding.left - grid.padding.right);
        float availableHeight = Mathf.Max(1f, rectSize.y - grid.padding.top - grid.padding.bottom);

        float widthUnits = cols + SpacingXRatio * Mathf.Max(0, cols - 1);
        float heightUnits = rows + SpacingYRatio * Mathf.Max(0, rows - 1);

        float cardWidthByWidth = availableWidth / widthUnits;
        float cardHeightByHeight = availableHeight / heightUnits;
        float cardWidthByHeight = cardHeightByHeight * CardAspect;
        float cardWidthWithMinSpacing = (availableWidth - MinSpacingX * Mathf.Max(0, cols - 1)) / cols;
        float cardHeightWithMinSpacing = (availableHeight - MinSpacingY * Mathf.Max(0, rows - 1)) / rows;
        float cardWidthByMinSpacingHeight = cardHeightWithMinSpacing * CardAspect;

        float cardWidth = Mathf.Min(cardWidthByWidth, cardWidthByHeight, cardWidthWithMinSpacing, cardWidthByMinSpacingHeight, MaxCardWidth);
        cardWidth = Mathf.Max(1f, cardWidth);
        float cardHeight = cardWidth / CardAspect;

        grid.cellSize = new Vector2(cardWidth, cardHeight);
        grid.spacing = new Vector2(
            Mathf.Max(MinSpacingX, cardWidth * SpacingXRatio),
            Mathf.Max(MinSpacingY, cardHeight * SpacingYRatio)
        );
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;
    }

    private void UpdateStaminaDisplay()
    {
        float v = (float)PlayerData.gd.power / 100f;
        mpower.Value = v;
    }
}
