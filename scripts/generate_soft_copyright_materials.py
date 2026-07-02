from __future__ import annotations

import os
from datetime import datetime
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas
from reportlab.platypus import (
    Flowable,
    KeepTogether,
    ListFlowable,
    ListItem,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[1]
OUT_DIR = ROOT / "output" / "pdf"
TMP_DIR = ROOT / "tmp" / "pdfs"

SOURCE_PDF = OUT_DIR / "拼图游戏_软件著作权源代码_前后30页.pdf"
MATERIAL_PDF = OUT_DIR / "拼图游戏_软件著作权鉴别材料.pdf"

FONT_REGULAR = "SoftCopyrightCN"
FONT_BOLD = "SoftCopyrightCN-Bold"


CORE_SOURCE_FILES = [
    "client/Assets/Scripts/picmgr.cs",
    "client/Assets/Scripts/DraggableGridItem.cs",
    "client/Assets/Scripts/Main.cs",
    "client/Assets/Scripts/PlayerData.cs",
    "client/Assets/Scripts/datamgr.cs",
    "client/Assets/Scripts/card.cs",
    "client/Assets/Scripts/BorderSpriteLoader.cs",
    "client/Assets/Scripts/recttools.cs",
    "client/Assets/Scripts/ui/frmbase.cs",
    "client/Assets/Scripts/ui/frm_setup.cs",
    "client/Assets/frm_msg.cs",
    "client/Assets/frm_tips.cs",
    "client/Assets/power.cs",
    "client/Assets/goldplay.cs",
    "client/Assets/UIParticleToUI.cs",
    "client/Assets/ParticleScreenPosConverter.cs",
    "client/Assets/Scripts/Extensions/JsonDataExtensions.cs",
    "client/Assets/Scripts/Extensions/ObjectPoolManager.cs",
    "client/Assets/Scripts/Extensions/TipsPoolManager.cs",
    "client/Assets/Scripts/Extensions/TipsPoolInitializer.cs",
    "client/Assets/Scripts/ui/frm_chapter.cs",
    "client/Assets/Scripts/ui/frm_game.cs",
    "client/Assets/frm_victory.cs",
]


MODULE_ROWS = [
    ["主控与事件总线", "Main.cs", "负责全局事件派发、暂停恢复、资源加载、CDN与兜底下载、加载状态管理。"],
    ["数据存档", "PlayerData.cs", "维护关卡、章节、体力、金币、已解锁关卡等玩家数据，并触发数据变更事件。"],
    ["章节界面", "frm_chapter.cs", "负责章节卡牌布局、章节翻页、体力显示、激励视频入口、多分辨率适配。"],
    ["游戏界面", "frm_game.cs", "负责挑战入口、体力消耗、加载关卡、游戏返回、胜利界面切换。"],
    ["拼图核心", "picmgr.cs", "负责图片切片、拼图棋盘布局、随机打乱、拼接判定、边框显示和胜利判断。"],
    ["拖拽卡牌", "DraggableGridItem.cs", "负责拖拽交互、卡牌吸附、相邻关系、翻面动画、纹理映射。"],
    ["胜利界面", "frm_victory.cs", "负责通关展示、图片预览、按钮交互和不同分辨率下的界面适配。"],
    ["配置读取", "datamgr.cs / Config", "加载关卡与章节配置，为关卡难度、图片路径和章节进度提供数据来源。"],
]


FUNCTION_ROWS = [
    ["关卡选择", "章节卡牌展示当前章节内关卡，支持翻页、解锁状态、当前进度高亮。"],
    ["体力系统", "挑战消耗体力，体力不足时拉起激励视频，看完视频奖励体力。"],
    ["图片加载", "优先从 CDN 加载图片，失败时从自有服务器 API 兜底，并缓存到本地持久目录。"],
    ["拼图生成", "按关卡配置将图片切割成网格卡牌，保持目标卡牌比例并适配设备横纵比。"],
    ["拖拽交换", "玩家拖动卡牌后系统计算最近位置、交换索引、更新卡牌相邻关系与边框。"],
    ["胜利判定", "全部卡牌回到正确位置后触发胜利流程，展示完整图片并推进关卡。"],
    ["暂停与设置", "通过统一事件控制设置界面、返回章节、游戏暂停恢复等状态。"],
    ["多分辨率适配", "章节、棋盘、胜利界面根据父容器尺寸动态计算卡牌尺寸与间距。"],
]


PLAN_ROWS = [
    ["目标用户", "休闲益智玩家，适合碎片时间游玩，核心体验是观察图片细节并完成拼图复原。"],
    ["核心循环", "选择关卡 - 消耗体力 - 加载图片 - 拖拽拼图 - 完成胜利 - 解锁下一关。"],
    ["难度设计", "通过网格规模、图片复杂度、初始打乱程度与难度标记控制挑战曲线。"],
    ["资源策略", "图片资源按命名规则托管，客户端通过关卡配置读取资源名，降低包体并便于后续扩展。"],
    ["商业化设计", "体力不足时接入抖音小游戏激励视频广告，看完广告后奖励 20 点体力。"],
    ["留存设计", "章节进度、关卡解锁、体力恢复与图片主题持续扩展形成长期游玩目标。"],
]


TEST_ROWS = [
    ["功能测试", "验证关卡进入、体力扣除、体力不足广告、奖励发放、胜利跳转、章节推进。"],
    ["兼容测试", "验证 iPhone、iPad、不同横纵比屏幕下章节卡牌、棋盘和胜利界面不拉伸不重叠。"],
    ["资源测试", "模拟 CDN 失败、SSL 异常、图片不存在等情况，确认兜底服务器与本地缓存生效。"],
    ["交互测试", "验证拖拽边界、快速点击、返回章节、设置弹窗、广告关闭后再次挑战等行为。"],
    ["性能测试", "关注图片下载、纹理创建、卡牌动画和对象复用，避免内存膨胀与帧率下降。"],
]


EVENT_ROWS = [
    ["gamebegin", "初始化章节界面，刷新章节卡牌和体力显示。"],
    ["level_play", "尝试进入指定关卡，检查并消耗体力，加载拼图资源。"],
    ["show_rewarded_power", "体力不足或点击加体力时，拉起激励视频广告。"],
    ["onpowerChange", "体力变化后刷新体力条和体力文本。"],
    ["level_next", "胜利后进入下一关，刷新章节进度和选关界面。"],
    ["level_back", "从游戏界面返回章节界面。"],
    ["show_victory", "拼图完成后显示胜利界面和完整图片。"],
    ["event_loading", "显示或隐藏加载状态，用于图片下载与关卡初始化过程。"],
]


PROCESS_ROWS = [
    ["启动流程", "Main 初始化全局对象，PlayerData 读取本地数据，datamgr 读取关卡配置，随后展示章节界面。"],
    ["选关流程", "frm_chapter 根据当前章节生成关卡卡牌，玩家点击挑战后发送 level_play 事件。"],
    ["体力流程", "frm_game 检查体力，体力充足则扣除 10 点；体力不足则触发激励视频，完整观看后奖励 20 点。"],
    ["加载流程", "picmgr 按关卡配置请求图片，Main 先尝试 CDN，失败后请求自有服务器 API，并将图片写入缓存。"],
    ["游戏流程", "picmgr 生成拼图网格，DraggableGridItem 响应拖拽交换，持续刷新卡牌位置和边框状态。"],
    ["结算流程", "picmgr 判断全部卡牌归位后发送 show_victory，胜利界面展示原图并允许进入下一关。"],
]


DATA_ROWS = [
    ["levelid", "当前关卡编号，用于恢复玩家进度和定位下一关。"],
    ["currChapter", "当前章节编号，用于章节界面展示和章节推进。"],
    ["opened", "已解锁关卡集合，判断关卡是否可挑战。"],
    ["power", "当前体力值，进入关卡时消耗，广告奖励或自然恢复时增加。"],
    ["lastpowerUpdateTime", "体力最后更新时间，用于计算离线自然恢复。"],
    ["coin", "玩家金币字段，预留用于后续道具或奖励系统。"],
]


DEPLOY_ROWS = [
    ["构建目标", "Unity/Tuanjie WebGL 构建，发布到抖音小游戏运行环境。"],
    ["图片资源", "图片资源放置于 pintu_res 仓库，同时同步到自有服务器作为兜底源。"],
    ["服务接口", "自有服务器提供 /api/pintu-res/image/:filename 接口，客户端仅允许安全图片文件名。"],
    ["广告配置", "抖音激励视频广告位 ID 由章节界面字段 rewardedVideoAdId 管理。"],
    ["上线检查", "发布前关闭调试开关，验证体力、广告、CDN 兜底、不同设备布局和关卡配置。"],
]


def register_fonts() -> None:
    regular_candidates = [
        Path("C:/Windows/Fonts/NotoSansSC-VF.ttf"),
        Path("C:/Windows/Fonts/msyh.ttc"),
        Path("C:/Windows/Fonts/simhei.ttf"),
    ]
    bold_candidates = [
        Path("C:/Windows/Fonts/msyhbd.ttc"),
        Path("C:/Windows/Fonts/simhei.ttf"),
        Path("C:/Windows/Fonts/NotoSansSC-VF.ttf"),
    ]
    regular = next((p for p in regular_candidates if p.exists()), None)
    bold = next((p for p in bold_candidates if p.exists()), None)
    if regular is None or bold is None:
        raise RuntimeError("No Chinese font found under C:/Windows/Fonts")
    pdfmetrics.registerFont(TTFont(FONT_REGULAR, str(regular)))
    pdfmetrics.registerFont(TTFont(FONT_BOLD, str(bold)))


def read_lines(path: Path) -> list[str]:
    for encoding in ("utf-8-sig", "utf-8", "gb18030"):
        try:
            return path.read_text(encoding=encoding).splitlines()
        except UnicodeDecodeError:
            continue
    return path.read_text(encoding="utf-8", errors="replace").splitlines()


def collect_source_lines() -> tuple[list[dict], list[dict]]:
    entries: list[dict] = []
    stats: list[dict] = []
    for rel in CORE_SOURCE_FILES:
        path = ROOT / rel
        if not path.exists():
            continue
        lines = read_lines(path)
        stats.append({"path": rel, "lines": len(lines)})
        entries.append({"file": rel, "line_no": 0, "text": f"// File: {rel}"})
        for idx, line in enumerate(lines, 1):
            entries.append({"file": rel, "line_no": idx, "text": line})
        entries.append({"file": rel, "line_no": 0, "text": ""})
    return entries, stats


def fit_text(text: str, font: str, size: float, max_width: float) -> str:
    if pdfmetrics.stringWidth(text, font, size) <= max_width:
        return text
    suffix = " ..."
    lo, hi = 0, len(text)
    while lo < hi:
        mid = (lo + hi + 1) // 2
        if pdfmetrics.stringWidth(text[:mid] + suffix, font, size) <= max_width:
            lo = mid
        else:
            hi = mid - 1
    return text[:lo] + suffix


def make_source_pdf(entries: list[dict]) -> None:
    per_page = 50
    needed = per_page * 30
    if len(entries) < needed * 2:
        raise RuntimeError(f"Source line count is not enough: {len(entries)}")

    selected = entries[:needed] + entries[-needed:]
    width, height = A4
    margin_x = 28 * mm
    top_y = height - 22 * mm
    line_height = 13.25
    code_size = 7.0
    header_size = 9.0
    footer_size = 8.0
    line_no_width = 19 * mm
    text_x = margin_x + line_no_width
    text_width = width - text_x - 18 * mm

    c = canvas.Canvas(str(SOURCE_PDF), pagesize=A4)
    c.setTitle("拼图游戏 软件著作权源代码 前后30页")

    for page_index in range(60):
        start = page_index * per_page
        page_lines = selected[start : start + per_page]
        part = "前30页" if page_index < 30 else "后30页"
        c.setFont(FONT_BOLD, header_size)
        c.drawString(margin_x, height - 13 * mm, "拼图游戏软件 - 源程序鉴别材料")
        c.setFont(FONT_REGULAR, header_size)
        c.drawRightString(width - margin_x, height - 13 * mm, f"{part} / 第 {page_index + 1} 页")
        c.setStrokeColor(colors.HexColor("#9AA4B2"))
        c.line(margin_x, height - 17 * mm, width - margin_x, height - 17 * mm)

        y = top_y
        c.setFont(FONT_REGULAR, code_size)
        for item in page_lines:
            if item["line_no"] == 0:
                prefix = "     "
                c.setFillColor(colors.HexColor("#334155"))
                c.setFont(FONT_BOLD, code_size)
            else:
                prefix = f"{item['line_no']:>5}"
                c.setFillColor(colors.HexColor("#64748B"))
                c.setFont(FONT_REGULAR, code_size)
            c.drawRightString(margin_x + line_no_width - 3 * mm, y, prefix)
            c.setFillColor(colors.black)
            c.setFont(FONT_REGULAR, code_size)
            c.drawString(text_x, y, fit_text(item["text"].replace("\t", "    "), FONT_REGULAR, code_size, text_width))
            y -= line_height

        c.setFillColor(colors.HexColor("#475569"))
        c.setFont(FONT_REGULAR, footer_size)
        c.drawCentredString(width / 2, 11 * mm, f"第 {page_index + 1} 页 / 共 60 页")
        c.showPage()

    c.save()


class ArchitectureFlow(Flowable):
    def __init__(self, width: float = 470, height: float = 112):
        super().__init__()
        self.width = width
        self.height = height

    def draw(self) -> None:
        c = self.canv
        labels = ["章节界面", "挑战入口", "拼图棋盘", "拖拽交互", "胜利结算"]
        colors_fill = ["#E0F2FE", "#DCFCE7", "#FEF3C7", "#FCE7F3", "#EDE9FE"]
        box_w = 78
        gap = 18
        x = 0
        y = 40
        c.setFont(FONT_BOLD, 9)
        for i, label in enumerate(labels):
            c.setFillColor(colors.HexColor(colors_fill[i]))
            c.setStrokeColor(colors.HexColor("#64748B"))
            c.roundRect(x, y, box_w, 34, 5, fill=1, stroke=1)
            c.setFillColor(colors.HexColor("#0F172A"))
            c.drawCentredString(x + box_w / 2, y + 12, label)
            if i < len(labels) - 1:
                c.setStrokeColor(colors.HexColor("#64748B"))
                c.line(x + box_w + 2, y + 17, x + box_w + gap - 4, y + 17)
                c.line(x + box_w + gap - 4, y + 17, x + box_w + gap - 10, y + 21)
                c.line(x + box_w + gap - 4, y + 17, x + box_w + gap - 10, y + 13)
            x += box_w + gap
        c.setFont(FONT_REGULAR, 8)
        c.setFillColor(colors.HexColor("#475569"))
        c.drawString(0, 16, "事件总线 Main.DispEvent / Main.RegistEvent 负责界面、数据、资源、广告之间的低耦合协作。")


def style_sheet():
    styles = getSampleStyleSheet()
    styles.add(
        ParagraphStyle(
            name="CNTitle",
            parent=styles["Title"],
            fontName=FONT_BOLD,
            fontSize=22,
            leading=30,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#0F172A"),
            spaceAfter=18,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNSubTitle",
            parent=styles["Normal"],
            fontName=FONT_REGULAR,
            fontSize=11,
            leading=18,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#475569"),
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNH1",
            parent=styles["Heading1"],
            fontName=FONT_BOLD,
            fontSize=15,
            leading=22,
            textColor=colors.HexColor("#0F172A"),
            spaceBefore=8,
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNH2",
            parent=styles["Heading2"],
            fontName=FONT_BOLD,
            fontSize=12,
            leading=18,
            textColor=colors.HexColor("#1E293B"),
            spaceBefore=6,
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNBody",
            parent=styles["BodyText"],
            fontName=FONT_REGULAR,
            fontSize=10,
            leading=17,
            firstLineIndent=18,
            textColor=colors.HexColor("#111827"),
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNBodyNoIndent",
            parent=styles["BodyText"],
            fontName=FONT_REGULAR,
            fontSize=10,
            leading=17,
            textColor=colors.HexColor("#111827"),
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNCell",
            parent=styles["BodyText"],
            fontName=FONT_REGULAR,
            fontSize=8.5,
            leading=13,
            textColor=colors.HexColor("#111827"),
        )
    )
    styles.add(
        ParagraphStyle(
            name="CNCellBold",
            parent=styles["BodyText"],
            fontName=FONT_BOLD,
            fontSize=8.8,
            leading=13,
            textColor=colors.HexColor("#0F172A"),
        )
    )
    return styles


def make_table(rows, col_widths, styles, header=True):
    data = []
    for row_idx, row in enumerate(rows):
        row_style = styles["CNCellBold"] if header and row_idx == 0 else styles["CNCell"]
        data.append([Paragraph(str(cell), row_style) for cell in row])
    table = Table(data, colWidths=col_widths, repeatRows=1 if header else 0)
    table_style = [
        ("GRID", (0, 0), (-1, -1), 0.4, colors.HexColor("#CBD5E1")),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
    ]
    if header:
        table_style.extend(
            [
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#E2E8F0")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.HexColor("#0F172A")),
            ]
        )
    table.setStyle(TableStyle(table_style))
    return table


def para(text: str, styles, name: str = "CNBody"):
    return Paragraph(text, styles[name])


def bullets(items: list[str], styles):
    return ListFlowable(
        [ListItem(Paragraph(item, styles["CNBodyNoIndent"]), leftIndent=8) for item in items],
        bulletType="bullet",
        leftIndent=18,
        bulletFontName=FONT_REGULAR,
        bulletFontSize=8,
    )


def on_doc_page(c: canvas.Canvas, doc) -> None:
    page_num = doc.page
    width, height = A4
    c.saveState()
    c.setFont(FONT_REGULAR, 8)
    c.setFillColor(colors.HexColor("#64748B"))
    c.drawString(18 * mm, height - 12 * mm, "拼图游戏软件 - 软件著作权鉴别材料")
    c.drawRightString(width - 18 * mm, height - 12 * mm, f"第 {page_num} 页")
    c.setStrokeColor(colors.HexColor("#CBD5E1"))
    c.line(18 * mm, height - 15 * mm, width - 18 * mm, height - 15 * mm)
    c.restoreState()


def make_material_pdf(stats: list[dict]) -> None:
    styles = style_sheet()
    doc = SimpleDocTemplate(
        str(MATERIAL_PDF),
        pagesize=A4,
        rightMargin=18 * mm,
        leftMargin=18 * mm,
        topMargin=22 * mm,
        bottomMargin=18 * mm,
        title="拼图游戏 软件著作权鉴别材料",
        author="拼图游戏项目组",
    )

    today = datetime.now().strftime("%Y年%m月%d日")
    story = []

    story.extend(
        [
            Spacer(1, 55 * mm),
            Paragraph("拼图游戏软件", styles["CNTitle"]),
            Paragraph("软件著作权鉴别材料", styles["CNTitle"]),
            Spacer(1, 12 * mm),
            Paragraph("版本号：V1.0", styles["CNSubTitle"]),
            Paragraph("材料类型：软件说明书 / 设计说明 / 策划说明 / 技术实现说明", styles["CNSubTitle"]),
            Paragraph(f"生成日期：{today}", styles["CNSubTitle"]),
            PageBreak(),
        ]
    )

    story.append(Paragraph("目录", styles["CNH1"]))
    toc_items = [
        "1. 软件概述",
        "2. 运行环境",
        "3. 功能设计",
        "4. 系统架构与核心模块",
        "5. 玩法策划与关卡设计",
        "6. 数据与资源管理",
        "7. 界面与适配设计",
        "8. 广告体力与容错设计",
        "9. 测试方案",
        "10. 业务流程与事件清单",
        "11. 部署维护与上线检查",
        "12. 核心源代码文件清单",
    ]
    story.append(bullets(toc_items, styles))
    story.append(PageBreak())

    story.append(Paragraph("1. 软件概述", styles["CNH1"]))
    story.append(
        para(
            "拼图游戏软件是一款面向移动端与抖音小游戏环境的休闲益智类游戏。软件以图片复原为核心玩法，玩家在章节界面选择关卡，进入游戏后通过拖拽卡牌完成拼图复原。系统结合章节进度、体力消耗、图片资源远程加载、激励视频奖励和胜利结算形成完整的游戏闭环。",
            styles,
        )
    )
    story.append(
        para(
            "本软件采用 Unity/Tuanjie 客户端实现，核心逻辑由 C# 脚本完成。项目代码中包含主控事件系统、关卡数据管理、章节界面、拼图生成、拖拽交互、胜利结算、资源下载和抖音小游戏广告接入等模块。",
            styles,
        )
    )
    story.append(
        make_table(
            [
                ["项目", "内容"],
                ["软件名称", "拼图游戏软件"],
                ["软件版本", "V1.0"],
                ["软件类型", "移动端休闲益智游戏 / 抖音小游戏"],
                ["主要语言", "C#"],
                ["主要平台", "Unity/Tuanjie、WebGL、抖音小游戏运行环境"],
                ["主要功能", "章节选关、拼图挑战、图片下载、体力系统、激励视频奖励、胜利结算"],
            ],
            [34 * mm, 125 * mm],
            styles,
        )
    )
    story.append(Spacer(1, 6))

    story.append(Paragraph("2. 运行环境", styles["CNH1"]))
    story.append(
        make_table(
            [
                ["类别", "说明"],
                ["开发环境", "Unity/Tuanjie 编辑器，C# 脚本开发，Windows 本地工程管理。"],
                ["客户端环境", "WebGL 构建产物运行于抖音小游戏容器，也可在 Unity 编辑器中进行本地调试。"],
                ["网络环境", "通过 HTTPS 下载远程图片资源；CDN 失败时使用自有服务器 API 兜底。"],
                ["存储环境", "使用 Application.persistentDataPath 保存玩家数据和图片缓存。"],
                ["适配范围", "支持手机与平板等多种横纵比屏幕，章节卡牌、拼图棋盘、胜利界面均进行动态布局。"],
            ],
            [34 * mm, 125 * mm],
            styles,
        )
    )

    story.append(Paragraph("3. 功能设计", styles["CNH1"]))
    story.append(make_table([["功能", "设计说明"]] + FUNCTION_ROWS, [38 * mm, 121 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(
        para(
            "功能之间通过 Main 事件总线进行连接，界面脚本不直接持有全部业务对象，而是通过事件通知完成页面显示、游戏开始、胜利展示、体力变化和设置弹窗等交互。",
            styles,
        )
    )

    story.append(Spacer(1, 8))
    story.append(Paragraph("4. 系统架构与核心模块", styles["CNH1"]))
    story.append(ArchitectureFlow())
    story.append(Spacer(1, 6))
    story.append(make_table([["模块", "源码文件", "职责说明"]] + MODULE_ROWS, [30 * mm, 45 * mm, 84 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(Paragraph("4.1 事件驱动设计", styles["CNH2"]))
    story.append(
        para(
            "Main.cs 定义 RegistEvent、DispEvent、SendEvent 等事件方法，章节界面、游戏界面、胜利界面和玩家数据模块通过事件进行低耦合协作。例如章节页点击挑战后发送 level_play，游戏界面检查体力并加载关卡；体力不足时发送 show_rewarded_power，由章节界面拉起激励视频。",
            styles,
        )
    )
    story.append(Paragraph("4.2 拼图生成设计", styles["CNH2"]))
    story.append(
        para(
            "picmgr.cs 根据关卡配置读取图片资源，动态计算棋盘尺寸和卡牌尺寸，将图片纹理映射到每一张卡牌。系统按当前设备可用区域计算拼图棋盘大小，保证不同屏幕下图片不变形、卡牌比例稳定、边框关系正确。",
            styles,
        )
    )
    story.append(Paragraph("4.3 拖拽交互设计", styles["CNH2"]))
    story.append(
        para(
            "DraggableGridItem.cs 实现 IBeginDragHandler、IDragHandler、IEndDragHandler 接口。玩家拖动卡牌时，系统记录初始位置、更新 UI 坐标、判断目标区域并完成交换。拖拽结束后刷新 PositionIndex、相邻关系和边框显示，最终交由拼图管理器进行胜利判定。",
            styles,
        )
    )

    story.append(Spacer(1, 8))
    story.append(Paragraph("5. 玩法策划与关卡设计", styles["CNH1"]))
    story.append(make_table([["策划项", "说明"]] + PLAN_ROWS, [38 * mm, 121 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(Paragraph("5.1 玩家流程", styles["CNH2"]))
    story.append(
        bullets(
            [
                "玩家进入章节页，系统展示当前章节的关卡卡牌和体力状态。",
                "玩家点击挑战，体力充足则进入游戏；体力不足则提示观看激励视频。",
                "游戏界面加载关卡配置和图片资源，将图片切分为拼图卡牌。",
                "玩家拖动卡牌完成复原，系统实时维护卡牌位置与边框关系。",
                "当全部卡牌归位后触发胜利界面，展示完成图片并推进下一关。",
            ],
            styles,
        )
    )
    story.append(Paragraph("5.2 关卡扩展", styles["CNH2"]))
    story.append(
        para(
            "关卡数据由配置表管理，包含关卡编号、章节归属、图片资源、难度标记和下一关信息。后续新增关卡时，可通过扩展配置和图片资源库完成，不需要重写拼图核心逻辑。",
            styles,
        )
    )

    story.append(Paragraph("6. 数据与资源管理", styles["CNH1"]))
    story.append(
        para(
            "玩家数据由 PlayerData.cs 与 GameData 维护，主要包括当前关卡、当前章节、已开放关卡、体力、金币和体力恢复时间。数据变更时通过 onpowerChange、onLevelChange、onChapterChange 等事件刷新界面。",
            styles,
        )
    )
    story.append(
        para(
            "图片资源采用远程加载方案。客户端先从 CDN 下载图片，失败后使用自有服务器 https://www.haoyouqu.net/api/pintu-res/image/ 兜底，同时将成功下载的数据写入持久化缓存目录，降低重复请求和网络波动影响。",
            styles,
        )
    )
    story.append(
        make_table(
            [
                ["数据类别", "管理方式"],
                ["关卡配置", "由 datamgr 和 Config 数据类读取，驱动章节与关卡逻辑。"],
                ["玩家进度", "保存在本地 JSON 数据中，启动时读取，变更时保存。"],
                ["体力数据", "记录当前体力和最后恢复时间，支持自然恢复与广告奖励。"],
                ["图片资源", "CDN 下载、自有服务器兜底、本地缓存复用。"],
            ],
            [38 * mm, 121 * mm],
            styles,
        )
    )

    story.append(Spacer(1, 8))
    story.append(Paragraph("7. 界面与适配设计", styles["CNH1"]))
    story.append(
        para(
            "软件主要界面包括章节页、游戏页、胜利页和设置页。章节页按父容器宽高动态计算卡牌宽度、横向间距和纵向间距；游戏页按图片比例和父容器尺寸计算棋盘区域；胜利页使用响应式容器展示完成图片和操作按钮。",
            styles,
        )
    )
    story.append(
        bullets(
            [
                "章节卡牌：根据屏幕宽高、卡牌比例、最小间距和最大宽度计算布局，兼容手机与平板。",
                "拼图棋盘：按图片切片比例控制棋盘尺寸，避免横屏或窄屏设备下拉伸。",
                "胜利界面：图片区域和按钮区域分离，按可用空间缩放，避免 UI 重叠。",
                "按钮反馈：挑战、设置、返回、下一关等按钮通过事件系统连接具体功能。",
            ],
            styles,
        )
    )

    story.append(Paragraph("8. 广告体力与容错设计", styles["CNH1"]))
    story.append(
        para(
            "体力不足时，游戏不直接开始关卡，而是触发 show_rewarded_power 事件。抖音小游戏环境下通过 TTSDK 创建激励视频广告，广告完整观看后奖励 20 点体力。若用户中途关闭视频，则提示看完视频可获得体力。",
            styles,
        )
    )
    story.append(
        para(
            "广告逻辑包含状态清理、旧广告实例销毁、异常捕获、关闭回调、错误回调和超时保护。这样可以避免用户关闭广告后再次点击挑战时被旧状态卡住，也能在广告加载失败时恢复界面可操作状态。",
            styles,
        )
    )
    story.append(
        para(
            "资源下载方面，客户端对 CDN SSL 连接失败、下载异常、图片解码失败等情况进行容错处理。下载失败时按一次 CDN 尝试后立即进入自有服务器兜底流程，以提高 Unity WebGL 和小游戏容器中的稳定性。",
            styles,
        )
    )

    story.append(Paragraph("9. 测试方案", styles["CNH1"]))
    story.append(make_table([["测试类别", "测试说明"]] + TEST_ROWS, [38 * mm, 121 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(
        para(
            "当前工程通过 C# 编译验证核心脚本语法正确，实际发布前还需在 Unity 编辑器、抖音开发者工具和真机环境中进行完整流程测试，重点覆盖广告、网络图片、不同屏幕比例和连续关卡切换。",
            styles,
        )
    )

    story.append(Spacer(1, 8))
    story.append(Paragraph("10. 业务流程与事件清单", styles["CNH1"]))
    story.append(Paragraph("10.1 主要业务流程", styles["CNH2"]))
    story.append(make_table([["流程", "说明"]] + PROCESS_ROWS, [34 * mm, 125 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(Paragraph("10.2 事件清单", styles["CNH2"]))
    story.append(make_table([["事件名称", "用途说明"]] + EVENT_ROWS, [40 * mm, 119 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(
        para(
            "事件清单体现了本软件界面层、数据层和玩法层的协作方式。事件驱动模式让章节、游戏、胜利和资源加载模块保持相对独立，降低后续新增关卡、调整 UI 和接入平台能力时的修改成本。",
            styles,
        )
    )

    story.append(Spacer(1, 8))
    story.append(Paragraph("11. 部署维护与上线检查", styles["CNH1"]))
    story.append(Paragraph("11.1 玩家数据字段", styles["CNH2"]))
    story.append(make_table([["字段", "说明"]] + DATA_ROWS, [42 * mm, 117 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(Paragraph("11.2 部署与维护", styles["CNH2"]))
    story.append(make_table([["项目", "说明"]] + DEPLOY_ROWS, [34 * mm, 125 * mm], styles))
    story.append(Spacer(1, 6))
    story.append(
        bullets(
            [
                "新增图片资源时，保持资源命名规范，避免空格和特殊字符，确保 CDN 与自有服务器文件一致。",
                "新增关卡时，优先通过配置表扩展，不修改拼图生成和拖拽核心逻辑。",
                "上线前确认 DebugForcePowerZero 等调试开关处于关闭状态。",
                "发布后关注图片加载失败率、广告关闭回调、不同设备布局和用户完成率等指标。",
            ],
            styles,
        )
    )

    story.append(PageBreak())
    story.append(Paragraph("12. 核心源代码文件清单", styles["CNH1"]))
    file_rows = [["序号", "源码文件", "行数"]]
    for idx, item in enumerate(stats, 1):
        file_rows.append([str(idx), item["path"], str(item["lines"])])
    story.append(make_table(file_rows, [14 * mm, 119 * mm, 26 * mm], styles))
    story.append(Spacer(1, 6))
    total_lines = sum(item["lines"] for item in stats)
    story.append(
        para(
            f"本鉴别材料所列核心自有源码共 {len(stats)} 个文件，合计约 {total_lines} 行。源代码提交 PDF 已按前 30 页和后 30 页生成，未纳入 DOTween、LitJson 等第三方库源码。",
            styles,
        )
    )

    doc.build(story, onFirstPage=on_doc_page, onLaterPages=on_doc_page)


def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    TMP_DIR.mkdir(parents=True, exist_ok=True)
    register_fonts()
    entries, stats = collect_source_lines()
    make_source_pdf(entries)
    make_material_pdf(stats)
    print(SOURCE_PDF)
    print(MATERIAL_PDF)


if __name__ == "__main__":
    main()
