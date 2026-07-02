const PDFDocument = require('pdfkit');
const fs = require('fs');

const doc = new PDFDocument({
  size: 'A4',
  margins: { top: 72, bottom: 72, left: 72, right: 72 },
  info: {
    Title: '拼图游戏软件设计说明书',
    Author: '开发团队',
    Subject: '拼图游戏软件设计',
  }
});

const stream = fs.createWriteStream('D:\\project\\pintu\\拼图游戏_软件设计说明书.pdf');
doc.pipe(stream);

let gPageNum = 0;

function addHeaderFooter() {
  gPageNum++;
  doc.save();
  doc.rect(72, 40, doc.page.width - 144, 1).fill('#cccccc');
  doc.fontSize(8).fillColor('#999999');
  doc.font('Helvetica');
  doc.text('拼图游戏 - 软件设计说明书 V1.0', 72, 44, { align: 'left', lineBreak: false });
  doc.text('Confidential', doc.page.width - 72 - 50, 44, { align: 'right', lineBreak: false });
  doc.rect(72, doc.page.height - 50, doc.page.width - 144, 1).fill('#cccccc');
  doc.text(`第 ${gPageNum} 页`, 72, doc.page.height - 46, { align: 'center', lineBreak: false });
  doc.restore();
}

function newPage(title) {
  doc.addPage();
  addHeaderFooter();
  if (title) {
    doc.fontSize(16).fillColor('#1a1a2e').font('Helvetica-Bold');
    doc.text(title, 72, 72, { lineBreak: false });
    doc.moveTo(72, 96).lineTo(doc.page.width - 72, 96).stroke('#0f3460');
  }
}

function emitText(text, size = 10) {
  doc.fontSize(size).fillColor('#333333').font('Helvetica');
  doc.text(text, 72, doc.y + 4, { width: doc.page.width - 144, lineBreak: true });
  if (doc.y > doc.page.height - 80) newPage(null);
}

function emitSubtitle(text) {
  if (doc.y > doc.page.height - 90) newPage(null);
  doc.fontSize(13).fillColor('#0f3460').font('Helvetica-Bold');
  doc.text(text, 72, doc.y + 6, { lineBreak: false });
  doc.y += 24;
}

function emitCode(text) {
  const lines = text.split('\n');
  doc.save();
  doc.roundedRect(76, doc.y + 2, doc.page.width - 152, 4, 2).fill('#f5f6fa');
  doc.restore();
  doc.y += 6;
  for (const cl of lines) {
    if (doc.y > doc.page.height - 80) newPage(null);
    doc.font('Courier').fontSize(7).fillColor('#2d3436');
    const display = cl.length > 97 ? cl.substring(0, 94) + '...' : cl;
    doc.text(display, 82, doc.y, { lineBreak: false });
    doc.y += 10;
  }
  doc.y += 4;
}

// ==================== COVER ====================
addHeaderFooter();

doc.fontSize(36).fillColor('#1a1a2e').font('Helvetica-Bold');
doc.text('拼图游戏', 72, 150, { align: 'center' });
doc.fontSize(28).fillColor('#16213e');
doc.text('软件设计说明书', 72, 200, { align: 'center' });
doc.moveTo(72, 250).lineTo(doc.page.width - 72, 250).stroke('#0f3460');

doc.fontSize(14).fillColor('#333333').font('Helvetica');
doc.text('版本：V1.0', 72, 280, { align: 'center' });
doc.text('日期：2026年6月', 72, 310, { align: 'center' });
doc.text('状态：正式发布版', 72, 340, { align: 'center' });
doc.fontSize(11).fillColor('#666666');
doc.text('本文件为拼图游戏完整的软件设计文档', 72, 390, { align: 'center' });
doc.text('包含架构设计、模块详细设计、数据结构及核心算法说明', 72, 410, { align: 'center' });

// ==================== TOC ====================
newPage('目录');
const tocItems = [
  '1  引言', '  1.1  编写目的', '  1.2  适用范围', '  1.3  术语定义',
  '2  系统总体架构', '  2.1  架构概述', '  2.2  技术栈', '  2.3  目录结构',
  '3  核心模块设计', '  3.1  主控模块 (Main)', '  3.2  事件系统 (Event System)',
  '  3.3  数据管理器 (datamgr)', '  3.4  玩家数据 (PlayerData)',
  '  3.5  拼图管理器 (picmgr)', '  3.6  拖拽系统 (DraggableGridItem)',
  '  3.7  卡片系统 (card)', '  3.8  边框系统 (BorderSpriteLoader)',
  '4  UI 模块设计', '  4.1  基础窗体 (frmbase)', '  4.2  章节选择界面 (frm_chapter)',
  '  4.3  游戏界面 (frm_game)', '  4.4  设置界面 (frm_setup)',
  '5  数据配置', '  5.1  章节配置 (DrChapter)', '  5.2  关卡配置 (DrLevel)',
  '  5.3  Tables 配置集合',
  '6  核心算法', '  6.1  拼图打乱算法', '  6.2  连通组检测算法',
  '  6.3  位置交换算法', '  6.4  胜利检测算法', '  6.5  边框刷新算法',
  '7  数据持久化', '  7.1  存档系统', '  7.2  体力系统',
  '8  运行环境', '  8.1  硬件要求', '  8.2  软件要求',
  '附录A  完整源码 - Main.cs', '附录B  完整源码 - picmgr.cs',
  '附录C  完整源码 - DraggableGridItem.cs', '附录D  完整源码 - PlayerData.cs',
];
doc.fontSize(9).fillColor('#333333').font('Helvetica');
doc.y = 110;
for (const line of tocItems) {
  if (doc.y > doc.page.height - 70) newPage(null);
  doc.text(line, doc.y > 110 ? 72 : 72, doc.y, { lineBreak: false });
  doc.y += 15;
}

// ==================== SECTION 1 ====================
newPage('1 引言');
emitSubtitle('1.1 编写目的');
emitText('本文档旨在全面描述拼图游戏软件的架构设计、模块划分、核心算法及实现细节，作为软件开发、测试及维护的参考依据。本文档适用于项目开发人员、测试人员及后期维护人员，帮助其快速理解系统结构及各模块职责。');

emitSubtitle('1.2 适用范围');
emitText('本软件是一款基于 Unity 引擎开发的拼图游戏，运行于 iOS 和 Android 移动平台。玩家通过拖拽碎片完成拼图，游戏提供多种难度等级和关卡设计。本文档涵盖从系统级架构到单个函数实现的全方位描述。');

emitSubtitle('1.3 术语定义');
emitText('• 拼图碎片（Piece / GridCell）：构成完整图片的最小单元，每个碎片对应图片的一个矩形区域，具有 UV 坐标和位置索引。\n• 连通组（Connected Group）：在拼图网格中，位置相邻且逻辑关系正确的碎片集合，可整体拖拽移动。\n• 体力（Power）：玩家进行游戏所需消耗的资源，随时间自动恢复。\n• 关卡（Level）：拼图游戏的基本单位，包含待拼合的图片及切割行列参数。\n• 章节（Chapter）：关卡的集合，同一章节内的关卡共享一张预览图片。\n• Luban：高效的配置表工具，可将 Excel 配置导出为 JSON 格式供游戏读取。\n• UV 坐标：纹理坐标系统，范围为 [0,1]，用于控制 RawImage 显示原图的哪个区域。');

// ==================== SECTION 2 ====================
newPage('2 系统总体架构');
emitSubtitle('2.1 架构概述');
emitText('本游戏采用基于 Unity 引擎的组件化架构设计，整体分为三大层级：表现层（UI 模块）、逻辑层（核心游戏逻辑）和数据层（配置与持久化）。');
emitText('表现层负责所有用户界面交互，包括章节选择界面、游戏界面、设置界面等。各界面继承自统一的 frmbase 基类，通过事件系统与逻辑层通信。逻辑层实现拼图的核心玩法，包括图片切割、碎片打乱、拖拽交互、连通组检测、位置交换、边框刷新、胜利判定等。数据层负责游戏配置表的加载与读取（基于 Luban 框架），以及玩家存档的 JSON 读写操作。');
emitText('各模块之间通过事件系统（Event System）实现松耦合通信。事件系统采用观察者模式，模块只需关心自己注册的事件，无需了解其他模块的存在。这种设计极大地降低了模块间的依赖关系，提高了代码的可维护性和可扩展性。');

emitSubtitle('2.2 技术栈');
emitText('游戏引擎：Unity 2022.3 (LTS)\n脚本语言：C# (.NET Standard 2.1)\n动画引擎：DOTween (DG.Tweening)\nJSON 序列化：LitJson\n配置表框架：Luban\nUI 系统：UGUI（Unity 内置 UI 系统）\n文本渲染：TextMeshPro\n目标平台：iOS / Android\n版本控制：Git');

emitSubtitle('2.3 目录结构');
emitCode(`Assets/
├── Scenes/
│   └── SampleScene.unity            # 主场景
├── Scripts/
│   ├── Main.cs                      # 游戏入口，单例，事件系统
│   ├── card.cs                      # 章节卡牌（翻转动画）
│   ├── picmgr.cs                    # 拼图管理器（核心逻辑）
│   ├── datamgr.cs                   # 数据管理器（Luban加载）
│   ├── PlayerData.cs                # 玩家数据（持久化）
│   ├── DraggableGridItem.cs         # 拖拽系统（连通组拖动）
│   ├── BorderSpriteLoader.cs        # 边框贴图加载刷新
│   ├── DebugManager.cs              # 调试模式开关
│   ├── recttools.cs                 # 矩形工具类
│   ├── goldplay.cs / getpos.cs      # 辅助脚本
│   ├── ui/
│   │   ├── frmbase.cs               # UI窗体基类
│   │   ├── frm_chapter.cs           # 章节选择界面
│   │   ├── frm_game.cs              # 游戏界面
│   │   └── frm_setup.cs             # 设置界面
│   ├── Config/                      # Luban自动生成
│   │   ├── DrChapter.cs / DrLevel.cs
│   │   └── Tables.cs / Tb*.cs
│   └── Extensions/                  # 扩展工具
├── Plugins/
│   ├── DOTween/                     # 动画插件
│   └── LitJson/                     # JSON库
└── Resources/
    ├── Card/ (lt,t,rt,r,rb,b,lb,l)  # 边框贴图
    ├── GridCell / levelpic          # 预制体
    └── *.jpg / *.png                # 关卡图片`);

// ==================== SECTION 3 ====================
newPage('3 核心模块设计');
emitSubtitle('3.1 主控模块 (Main.cs)');
emitText('Main 类是游戏启动入口，负责初始化全局组件、注册事件、管理游戏状态。采用单例（Singleton）模式，通过 Main.inst 全局访问。');
emitText('主要职责：\n1. 初始化 DOTween 动画引擎\n2. 提供全局事件系统（RegistEvent / DispEvent / UnRegistEvent）\n3. 管理游戏暂停/恢复状态（通过 Time.timeScale 控制）\n4. 提供关卡重新开始功能（消耗 10 体力）\n5. 初始化游戏数据（清除持久化目录）');
emitCode(`/// <summary>
/// Main.cs - 游戏主控模块
/// 职责：游戏初始化、事件系统、暂停管理
/// </summary>
public class Main : MonoBehaviour
{
    public delegate object registfun(object parm);

    public static Main inst;
    public Material matboader;
    public GameObject clickEffectPrefab;

    // 暂停状态
    private static bool isGamePaused = false;
    public static bool IsPaused
    {
        get => isGamePaused;
        set => isGamePaused = value;
    }

    // 事件系统容器
    static Dictionary<string, List<registfun>> evs = new();

    public static object DispEvent(string ev, object parm = null)
    {
        if (evs.ContainsKey(ev))
        {
            for (int i = 0; i < evs[ev].Count; i++)
            {
                var p = evs[ev][i](parm);
                if (p != null) return p;
            }
            return null;
        }
        else
        {
            Debug.LogError("没有处理消息" + ev);
            return null;
        }
    }

    public static void RegistEvent(string ev, registfun fun)
    {
        if (!evs.ContainsKey(ev))
            evs[ev] = new List<registfun>();
        evs[ev].Add(fun);
    }

    public static void UnRegistEvent(string ev, registfun fun)
    {
        if (evs.ContainsKey(ev))
            evs[ev].Remove(fun);
    }

    internal static void SendEvent(string v) => DispEvent(v);

    private void Awake()
    {
        inst = this;
        DOTween.Init();
        Debug.Log("Main.Awake: DOTween已初始化");

        RegistEvent("game_restart", (object parm) =>
        {
            RestartLevel();
            return null;
        });
    }

    void Start()
    {
        float bl = ((float)Screen.width) / Screen.height;
        Screen.SetResolution((int)(1920 * bl), 1920, false);
        DispEvent("gamebegin");
    }

    public static void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public static void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public static void RestartLevel()
    {
        Debug.Log("重新开始关卡");
        if (!PlayerData.gd.hasEnoughpower(10))
        {
            Debug.Log("power不足，无法重新开始游戏");
            return;
        }
        PlayerData.gd.消耗power(10);
    }

    public static void InitGame()
    {
        string persistentDataPath = Application.persistentDataPath;
        if (Directory.Exists(persistentDataPath))
        {
            string[] files = Directory.GetFiles(persistentDataPath);
            foreach (string file in files)
                File.Delete(file);
            string[] directories = Directory.GetDirectories(persistentDataPath);
            foreach (string directory in directories)
                Directory.Delete(directory, true);
        }
    }
}`);

// ==================== 3.2 ====================
newPage('3.2 事件系统 (Event System)');
emitText('事件系统是拼图游戏的通信骨架，采用观察者（Observer）设计模式。系统维护一个以事件名为 key、处理函数列表为 value 的 Dictionary。当事件被 DispEvent 派发时，依次调用所有已注册的处理函数。处理函数可以返回非 null 值来中断事件传播，这用于需要返回值的场景（如 level_play 返回 1 表示成功、0 表示失败）。');
emitText('系统中定义的全局事件：\n• gamebegin - 游戏开始，触发章节界面首次显示\n• level_play - 进入关卡，携带 levelId 参数，返回 1/0\n• level_next - 切换到下一关\n• level_back - 返回章节选择界面\n• show_setup / show_next - 显示设置/下一关界面\n• event_msg - 消息提示（如体力不足）\n• event_tips - 难度提示（如困难模式）\n• onLevelChange - 关卡进度变化时自动保存存档\n• onpowerChange - 体力值变化时自动保存存档\n• onChapterChange - 章节切换时刷新界面');

emitCode(`// 事件注册与派发示例
// frm_chapter.cs 中的事件注册
Main.RegistEvent("gamebegin", (x) =>
{
    brushChapterContent();
    show();
    UpdateStaminaDisplay();
    return 1;
});

// frm_game.cs 中的事件注册
Main.RegistEvent("level_play", (x) =>
{
    if (!PlayerData.gd.hasEnoughpower(10))
    {
        Main.DispEvent("event_msg", "power不足，无法开始游戏");
        return 0;
    }
    PlayerData.gd.消耗power(10);
    var leevel = datamgr.Instance.GetLevel((int)x);
    show();
    StartCoroutine(load(leevel));
    return 1;
});

// 触发事件示例
Main.DispEvent("level_play", PlayerData.gd.levelid);
Main.SendEvent("level_next");
Main.DispEvent("show_setup");`);

// ==================== 3.3 ====================
newPage('3.3 数据管理器 (datamgr.cs)');
emitText('datamgr 负责加载和管理游戏的所有配置数据。采用单例模式，通过 datamgr.Instance 全局访问。基于 Luban 配置表框架，在运行时加载 JSON 格式的配置数据，解析为强类型对象供其他模块使用。');
emitText('支持两种加载方式：1）编辑模式下，通过在 Inspector 中拖拽 TextAsset 对象直接加载；2）运行模式下，从 Assets/data/ 目录读取 JSON 文件。这种双模式设计简化了开发调试流程。');

emitCode(`/// <summary>
/// datamgr.cs - 数据管理器
/// 职责：加载Luban配置表，提供数据访问接口
/// </summary>
public class datamgr : MonoBehaviour
{
    [Header("数据文件")]
    public TextAsset tbchapter;
    public TextAsset tblevel;

    private static datamgr _instance;
    public static datamgr Instance
    {
        get
        {
            if (_instance == null)
            {
                var x = Instantiate(Resources.Load("DataManager")) as GameObject;
                _instance = x.GetComponent<datamgr>();
            }
            return _instance;
        }
    }

    public Tables Tables { get; private set; }
    public bool IsLoaded { get; private set; } = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfigTables();
        }
        else if (_instance != this)
            Destroy(gameObject);
    }

    public void LoadConfigTables()
    {
        Debug.Log("开始加载配置表...");
        try
        {
            System.Func<string, JSONNode> loader = (string file) =>
            {
                TextAsset dataAsset = null;
                switch (file)
                {
                    case "tbchapter": dataAsset = tbchapter; break;
                    case "tblevel":   dataAsset = tblevel;   break;
                }
                if (dataAsset != null && !string.IsNullOrEmpty(dataAsset.text))
                    return JSON.Parse(dataAsset.text);

                string filePath = Path.Combine(
                    Application.dataPath, "data", file + ".json");
                if (File.Exists(filePath))
                    return JSON.Parse(File.ReadAllText(filePath));

                Debug.LogError("配置文件不存在: " + filePath);
                return null;
            };

            Tables = new Tables(loader);
            IsLoaded = true;
            Debug.Log("配置表加载完成!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("配置表加载失败: " + e.Message);
            IsLoaded = false;
        }
    }

    public DrChapter GetChapter(int id)
        => Tables?.TbChapter?.GetOrDefault(id);

    public DrLevel GetLevel(int id)
        => Tables?.TbLevel?.GetOrDefault(id);

    public List<DrChapter> GetChapters()
        => Tables?.TbChapter?.DataList;
}`);

// ==================== 3.4 ====================
newPage('3.4 玩家数据 (PlayerData.cs)');
emitText('PlayerData 负责管理玩家的游戏进度和资源数据，包括当前关卡 ID、已解锁关卡集合、当前章节 ID、体力值及体力自动恢复机制。');
emitText('数据采用 JSON 格式持久化到 Application.persistentDataPath/playerData.json。游戏启动时自动加载，数据变更时通过事件钩子自动保存。体力系统实现了离线恢复：记录上次更新时间戳，在读取体力值时计算期间应恢复的体力（每 15 分钟恢复 1 点）。');

emitCode(`/// <summary>
/// GameData.cs - 游戏运行时数据模型
/// </summary>
public class GameData
{
    JsonData data;
    opend op;  // 已解锁集合

    public GameData(JsonData _data)
    {
        data = _data;
        op = new opend(openeddata);
    }

    // 当前关卡ID
    internal int levelid
    {
        get => int.Parse(data.Has("levelid")
            ? data["levelid"].ToString() : "100001");
        set { data["levelid"] = value.ToString();
              Main.DispEvent("onLevelChange"); }
    }

    // 当前章节ID
    internal int currChapter
    {
        get => int.Parse(data.Has("currChapter")
            ? data["currChapter"].ToString() : "1");
        set { data["currChapter"] = value.ToString();
              Main.DispEvent("onChapterChange"); }
    }

    // 体力值（含自动恢复计算）
    internal int power
    {
        get
        {
            int cur = int.Parse(data.Has("power")
                ? data["power"].ToString() : "100");
            long last = lastpowerUpdateTime;
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            long elapsed = now - last;
            int recovered = (int)(elapsed / (15 * 60)); // 15分钟/点
            if (recovered > 0)
            {
                cur = Math.Min(100, cur + recovered);
                data["power"] = cur.ToString();
                lastpowerUpdateTime = now;
            }
            return cur;
        }
        set
        {
            data["power"] = Math.Min(100, value).ToString();
            lastpowerUpdateTime = DateTime.Now.Ticks
                / TimeSpan.TicksPerSecond;
            Main.DispEvent("onpowerChange");
        }
    }

    internal long lastpowerUpdateTime
    {
        get => long.Parse(data.Has("lastpowerUpdateTime")
            ? data["lastpowerUpdateTime"].ToString()
            : (DateTime.Now.Ticks / TimeSpan.TicksPerSecond).ToString());
        set => data["lastpowerUpdateTime"] = value.ToString();
    }

    public bool hasEnoughpower(int cost = 10) => power >= cost;
    public bool 消耗power(int cost = 10)
    {
        if (hasEnoughpower(cost)) { power -= cost; return true; }
        return false;
    }

    public bool isOpened(int id) => op.Opened(id);
    public void Open(int id)
    {
        op.Open(id);
        Main.DispEvent("onLevelChange");
    }

    // 已解锁集合的JSON访问
    JsonData openeddata
    {
        get
        {
            if (!data.Has("opened") || !data["opened"].IsArray)
            {
                var x = new JsonData();
                x.SetJsonType(JsonType.Array);
                data["opened"] = x;
            }
            return data["opened"];
        }
    }

    public JsonData getData() => data;
}

/// <summary>
/// 已解锁关卡集合
/// </summary>
public class opend
{
    JsonData _data;
    public opend(JsonData data) { _data = data; }

    internal bool Opened(int id)
    {
        for (int i = 0; i < _data.Count; i++)
            if (int.Parse(_data[i].ToString()) == id) return true;
        return false;
    }

    internal void Open(int id)
    {
        if (!Opened(id)) _data.Add(id.ToString());
    }
}`);

emitCode(`/// <summary>
/// PlayerData.cs - 玩家数据持久化组件
/// </summary>
public class PlayerData : MonoBehaviour
{
    public static GameData gd;

    private void Awake()
    {
        loadData();
        Main.RegistEvent("onLevelChange", (x) =>
        {
            if (chapterOpend()) gd.currChapter++;
            saveData();
            return 1;
        });
        Main.RegistEvent("onpowerChange", (x) =>
        {
            saveData();
            return null;
        });
    }

    bool chapterOpend()
    {
        int cid = gd.currChapter;
        var sst = datamgr.Instance.GetChapter(cid);
        for (int i = 0; i < sst.LevelId.Count; i++)
            if (!gd.isOpened(sst.LevelId[i])) return false;
        return true;
    }

    void loadData()
    {
        var pa = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(pa))
        {
            try
            {
                var json = File.ReadAllText(pa);
                var data = JsonMapper.ToObject<JsonData>(json);
                gd = new GameData(data ?? new JsonData());
            }
            catch
            {
                gd = new GameData(new JsonData());
            }
        }
        else
            gd = new GameData(new JsonData());
    }

    public void saveData()
    {
        var pa = Application.persistentDataPath + "/playerData.json";
        var json = JsonMapper.ToJson(gd.getData());
        File.WriteAllText(pa, json);
    }
}`);

// ==================== 3.5 ====================
newPage('3.5 拼图管理器 (picmgr.cs)');
emitText('picmgr 是拼图游戏最核心的管理类，职责包括：加载关卡图片并按配置切割；创建每个碎片的 GameObject 并设置 UV 坐标；执行碎片打乱算法；管理九宫格边框的创建与动态刷新；检测拼图胜利条件。该类也是拖拽系统的直接交互对象。');

emitSubtitle('3.5.1 图片切割与碎片创建');
emitText('根据 DrLevel 配置中的 LevelFigureX（列数）和 LevelFigureY（行数），将原图均匀切割为 width×height 个碎片。每个碎片加载 GridCell 预制体，设置 RawImage 的 UV 矩形区域以显示原图的对应部分。碎片锚点设置在左下角（pivot=0,0），使用 anchoredPosition 定位。');

emitCode(`/// <summary>
/// picmgr.cs - 拼图管理器（核心逻辑）
/// </summary>
public class picmgr : MonoBehaviour
{
    public Texture2D pic;
    public int width = 3;
    public int height = 3;

    internal static picmgr instance;
    DrLevel curlevel;

    // 每个碎片的宽/高
    internal float carWid => trans.rect.width / width;
    internal float carHei => trans.rect.height / height;

    RectTransform trans => GetComponent<RectTransform>();

    private void Awake()
    {
        instance = this;
        ResizeChapterContent();
    }

    public void ResizeChapterContent()
    {
        // 按9:16比例适应屏幕
        float margin = 100f;
        float availableWidth = Screen.width - 2 * margin;
        float availableHeight = Screen.height - 2 * margin;
        float targetWidth, targetHeight;

        if (availableWidth / availableHeight > 9f / 16f)
        {
            targetHeight = availableHeight;
            targetWidth = targetHeight * 9f / 16f;
        }
        else
        {
            targetWidth = availableWidth;
            targetHeight = targetWidth * 16f / 9f;
        }

        float wside = (Screen.width - targetWidth) / 2;
        float hside = (Screen.height - targetHeight) / 2;
        trans.offsetMin = new Vector2(wside, hside);
        trans.offsetMax = new Vector2(-wside, -hside);
    }

    /// <summary>
    /// 加载关卡：设置行列数、加载图片、创建碎片
    /// </summary>
    public IEnumerator LoadLevel(DrLevel leevel)
    {
        curlevel = leevel;
        width = leevel.LevelFigureX;
        height = leevel.LevelFigureY;
        pic = Resources.Load(leevel.LevelFigure) as Texture2D;

        if (leevel.DifficultyTier == 2)
            Main.DispEvent("event_tips", "困难模式");

        yield return Main.inst.StartCoroutine(
            CreateGridImages(leevel.Id, leevel.OutOfPlaceNumber,
                             leevel.DifficultyTier == 2));
    }`);

emitCode(`    /// <summary>
    /// 创建网格碎片（协程，带动画效果）
    /// </summary>
    public IEnumerator CreateGridImages(int level = 1,
        int maxKeepCount = -1, bool isHard = false)
    {
        clearOld();

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 rectSize = rectTransform.rect.size;

        float cellWidth = rectSize.x / width;
        float cellHeight = rectSize.y / height;
        float delay = 1f / (width * height);

        // 逐格创建
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                yield return new WaitForSeconds(delay);
                create(x, y, cellWidth, cellHeight, isHard, false);
            }
        }

        yield return new WaitForSeconds(0.3f);
        ShuffleGridPositions(level, maxKeepCount, isHard);

        // 翻转显示
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var dragItem = child.GetComponent<DraggableGridItem>();
            dragItem.Turn();
        }
    }

    /// <summary>
    /// 创建单个碎片
    /// </summary>
    private void create(int x, int y, float cellWidth, float cellHeight,
                        bool isHard, bool beditor)
    {
        GameObject cellObject = Instantiate(
            Resources.Load("GridCell") as GameObject);
        cellObject.name = $"GridCell_{x}_{y}";
        cellObject.transform.SetParent(transform, false);

        RectTransform cellRect = cellObject.GetComponent<RectTransform>();

        // 设置卡片纹理
        var rawImg = cellObject.GetComponent<RawImage>();
        rawImg.texture = isHard
            ? (Texture)Resources.Load("Card/ui_card_02")
            : (Texture)Resources.Load("Card/ui_card_01");

        // 设置拖拽组件
        var dg = cellObject.GetComponent<DraggableGridItem>();
        dg.pic = pic;
        dg.canvas = GetComponentInParent<Canvas>();

        // UV坐标（对应原图的区域）
        float uvX = (float)x / width;
        float uvY = (float)y / height;
        float uvWidth = 1.0f / width;
        float uvHeight = 1.0f / height;

        dg.uvX = uvX;  dg.uvY = uvY;
        dg.uvWidth = uvWidth;  dg.uvHeight = uvHeight;
        dg.PositionIndex = x * height + y;

        // RectTransform设置
        cellRect.anchorMin = new Vector2(0, 0);
        cellRect.anchorMax = new Vector2(0, 0);
        cellRect.pivot = new Vector2(0, 0);
        cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);

        if (beditor)
        {
            cellRect.anchoredPosition = new Vector2(
                x * cellWidth, y * cellHeight);
            cellRect.GetComponent<DraggableGridItem>().Turn(true);
        }
        else
        {
            cellRect.anchoredPosition = new Vector2(0f, 0);
            // 非调试模式移除文本组件
            if (!DebugManager.IsDebugMode)
            {
                var text = cellObject
                    .GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    Destroy(text.gameObject);
            }
        }

        // 创建边框
        CreateBorders(cellObject, cellWidth, cellHeight);

        // 带动画移动到目标位置
        var pos = new Vector2(x * cellWidth, y * cellHeight);
        cellRect.DOAnchorPos(pos, 0.2f).OnComplete(() =>
            cellRect.anchoredPosition = pos);
    }`);

emitSubtitle('3.5.2 边框创建');
emitText('边框系统为每个碎片创建 8 个子对象：四个边（Top/Bottom/Left/RightBorder）和四个角（TopLeft/TopRight/BottomRight/BottomLeftBorder）。边框宽度统一为 20px，角为正方形，边为矩形，边与角之间互不重叠。边框贴图通过 BorderSpriteLoader 组件从 Resources/Card 目录加载。');

emitCode(`    /// <summary>
    /// 创建碎片的九宫格边框
    /// </summary>
    private void CreateBorders(GameObject cellObject,
                               float cellWidth, float cellHeight)
    {
        // 清除旧边框
        for (int i = cellObject.transform.childCount - 1; i >= 0; i--)
            Destroy(cellObject.transform.GetChild(i).gameObject);

        float borderWidth = 20f;
        float bw = Mathf.Min(borderWidth,
            cellWidth / 2f, cellHeight / 2f);

        // 四个角
        CreateBorder(cellObject, "TopLeftBorder",
            new Vector2(0, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "TopRightBorder",
            new Vector2(cellWidth - bw, cellHeight - bw), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomRightBorder",
            new Vector2(cellWidth - bw, 0), new Vector2(bw, bw));
        CreateBorder(cellObject, "BottomLeftBorder",
            new Vector2(0, 0), new Vector2(bw, bw));

        // 四个边
        CreateBorder(cellObject, "TopBorder",
            new Vector2(bw, cellHeight - bw),
            new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));
        CreateBorder(cellObject, "BottomBorder",
            new Vector2(bw, 0),
            new Vector2(Mathf.Max(0f, cellWidth - 2f * bw), bw));
        CreateBorder(cellObject, "LeftBorder",
            new Vector2(0, bw),
            new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
        CreateBorder(cellObject, "RightBorder",
            new Vector2(cellWidth - bw, bw),
            new Vector2(bw, Mathf.Max(0f, cellHeight - 2f * bw)));
    }

    private void CreateBorder(GameObject parent, string name,
        Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject border = new GameObject(name);
        border.transform.SetParent(parent.transform, false);

        RectTransform rect = border.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Image img = border.AddComponent<Image>();
        img.color = Color.white;

        BorderSpriteLoader loader =
            border.AddComponent<BorderSpriteLoader>();
        loader.resourceName = MapBorderNameToResource(name);
        loader.LoadSprite();
    }

    private string MapBorderNameToResource(string name)
    {
        return name switch
        {
            "TopLeftBorder"     => "lt",
            "TopBorder"         => "t",
            "TopRightBorder"    => "rt",
            "RightBorder"       => "r",
            "BottomRightBorder" => "rb",
            "BottomBorder"      => "b",
            "BottomLeftBorder"  => "lb",
            "LeftBorder"        => "l",
            _ => name
        };
    }`);

emitSubtitle('3.5.3 边框动态刷新');
emitText('边框刷新算法动态更新碎片的九宫格边框显示。算法分为两组独立处理：非拖拽组（参数无参重载）和拖拽组（参数为 List 的重载）。每组只在其组内搜索邻居，确保拖拽中的碎片边框能正确反映组内连接关系。');
emitText('核心逻辑：基于碎片当前的实际位置（anchoredPosition），计算四个方向邻居的预期位置，在组内搜索该位置是否有碎片存在。如果找到的碎片名称与逻辑名称匹配，则该方向判定为正确连接，隐藏边框；否则显示边框。角边框根据相邻两条边的状态（isTopCorrect, isLeftCorrect 等）自动切换贴图类型。');

emitCode(`    /// <summary>
    /// 更新边框（非拖拽组）
    /// </summary>
    public void UpdateBorderVisibility()
    {
        List<DraggableGridItem> allItems = new List<DraggableGridItem>();
        foreach (Transform child in transform)
        {
            var item = child.GetComponent<DraggableGridItem>();
            if (item != null && !item.IsBeingDragged)
                allItems.Add(item);
        }
        if (allItems.Count > 0)
            UpdateBorderVisibility(allItems);
    }

    /// <summary>
    /// 更新边框（指定组，用于拖拽组或非拖拽组）
    /// </summary>
    public void UpdateBorderVisibility(List<DraggableGridItem> group)
    {
        if (group == null || group.Count == 0) return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 rectSize = rectTransform.rect.size;
        float cellWidth = rectSize.x / width;
        float cellHeight = rectSize.y / height;
        float checkThreshold = 1.5f;

        foreach (DraggableGridItem item in group)
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            if (itemRect == null) continue;

            // 解析逻辑坐标
            string[] parts = item.gameObject.name
                .Replace("GridCell_", "").Split('_');
            if (parts.Length != 2) continue;
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            // 四方向邻居检测
            Vector2 eRight = itemRect.anchoredPosition
                + new Vector2(cellWidth, 0);
            Vector2 eLeft = itemRect.anchoredPosition
                + new Vector2(-cellWidth, 0);
            Vector2 eTop = itemRect.anchoredPosition
                + new Vector2(0, cellHeight);
            Vector2 eBottom = itemRect.anchoredPosition
                + new Vector2(0, -cellHeight);

            var rN = FindGridItemAtAnchoredPosition(eRight, group, checkThreshold);
            var lN = FindGridItemAtAnchoredPosition(eLeft, group, checkThreshold);
            var tN = FindGridItemAtAnchoredPosition(eTop, group, checkThreshold);
            var bN = FindGridItemAtAnchoredPosition(eBottom, group, checkThreshold);

            // 对角线邻居
            Vector2 eTR = itemRect.anchoredPosition
                + new Vector2(cellWidth, cellHeight);
            Vector2 eTL = itemRect.anchoredPosition
                + new Vector2(-cellWidth, cellHeight);
            Vector2 eBR = itemRect.anchoredPosition
                + new Vector2(cellWidth, -cellHeight);
            Vector2 eBL = itemRect.anchoredPosition
                + new Vector2(-cellWidth, -cellHeight);

            var trN = FindGridItemAtAnchoredPosition(eTR, group, checkThreshold);
            var tlN = FindGridItemAtAnchoredPosition(eTL, group, checkThreshold);
            var brN = FindGridItemAtAnchoredPosition(eBR, group, checkThreshold);
            var blN = FindGridItemAtAnchoredPosition(eBL, group, checkThreshold);

            // 名称匹配判断
            bool isRC = rN != null && rN.name == $"GridCell_{x+1}_{y}";
            bool isLC = lN != null && lN.name == $"GridCell_{x-1}_{y}";
            bool isTC = tN != null && tN.name == $"GridCell_{x}_{y+1}";
            bool isBC = bN != null && bN.name == $"GridCell_{x}_{y-1}";
            bool isTRC = trN != null && trN.name == $"GridCell_{x+1}_{y+1}";
            bool isTLC = tlN != null && tlN.name == $"GridCell_{x-1}_{y+1}";
            bool isBRC = brN != null && brN.name == $"GridCell_{x+1}_{y-1}";
            bool isBLC = blN != null && blN.name == $"GridCell_{x-1}_{y-1}";

            // 边显隐
            bool rShow = !isRC, lShow = !isLC;
            bool tShow = !isTC, bShow = !isBC;

            item.adjacentRight = isRC; item.adjacentLeft = isLC;
            item.adjacentTop = isTC;  item.adjacentBottom = isBC;

            void SetActive(GameObject go, bool active)
                { if (go != null) go.SetActive(active); }

            SetActive(FindChildByName(item.gameObject, "RightBorder"), rShow);
            SetActive(FindChildByName(item.gameObject, "LeftBorder"), lShow);
            SetActive(FindChildByName(item.gameObject, "TopBorder"), tShow);
            SetActive(FindChildByName(item.gameObject, "BottomBorder"), bShow);

            // 角显隐
            SetActive(FindChildByName(item.gameObject, "TopLeftBorder"),
                tShow || lShow || !isTLC);
            SetActive(FindChildByName(item.gameObject, "TopRightBorder"),
                tShow || rShow || !isTRC);
            SetActive(FindChildByName(item.gameObject, "BottomRightBorder"),
                bShow || rShow || !isBRC);
            SetActive(FindChildByName(item.gameObject, "BottomLeftBorder"),
                bShow || lShow || !isBLC);

            // 角贴图刷新
            RefreshCorner(item, "lt", isTC, isLC, isRC, isBC, isTLC);
            RefreshCorner(item, "rt", isTC, isLC, isRC, isBC, isTRC);
            RefreshCorner(item, "rb", isBC, isLC, isRC, isTC, isBRC);
            RefreshCorner(item, "lb", isBC, isLC, isRC, isTC, isBLC);
        }
    }`);

emitCode(`    /// <summary>
    /// 刷新角贴图
    /// </summary>
    private void RefreshCorner(DraggableGridItem item, string corner,
        bool isMain1, bool isMain2, bool isOther1, bool isOther2,
        bool isDiag)
    {
        string key = corner;
        bool isTop = corner == "lt" || corner == "rt";
        bool isLeft = corner == "lt" || corner == "lb";

        switch (corner)
        {
            case "lt":
                key = isLeft && isTop && !isDiag ? "lt_revert" :
                      !isLeft && !isTop ? "lt" :
                      isLeft && !isTop ? "t" :
                      !isLeft && isTop ? "l" : key;
                break;
            case "rt":
                key = isOther1 && isTop && !isDiag ? "rt_revert" :
                      !isOther1 && !isTop ? "rt" :
                      isOther1 && !isTop ? "t" :
                      !isOther1 && isTop ? "r" : key;
                break;
            case "rb":
                key = isOther1 && isOther2 && !isDiag ? "rb_revert" :
                      !isOther1 && !isOther2 ? "rb" :
                      isOther1 && !isOther2 ? "b" :
                      !isOther1 && isOther2 ? "r" : key;
                break;
            case "lb":
                key = isLeft && isOther2 && !isDiag ? "lb_revert" :
                      !isLeft && !isOther2 ? "lb" :
                      isLeft && !isOther2 ? "b" :
                      !isLeft && isOther2 ? "l" : key;
                break;
        }

        var bl = FindChildByName(item.gameObject,
            corner switch {
                "lt" => "TopLeftBorder", "rt" => "TopRightBorder",
                "rb" => "BottomRightBorder", _ => "BottomLeftBorder"
            })?.GetComponent<BorderSpriteLoader>();
        bl?.SetResourceAndLoad(key);
    }

    // ---- 辅助方法 ----
    private DraggableGridItem FindGridItemAtAnchoredPosition(
        Vector2 pos, List<DraggableGridItem> items, float threshold)
    {
        foreach (var item in items)
        {
            var rt = item.GetComponent<RectTransform>();
            if (rt == null) continue;
            if (Vector2.Distance(rt.anchoredPosition, pos) <= threshold)
                return item;
        }
        return null;
    }

    private GameObject FindChildByName(GameObject parent, string name)
    {
        var t = parent.transform.Find(name);
        return t != null ? t.gameObject : null;
    }`);

emitSubtitle('3.5.4 打乱算法');
emitText('打乱算法使用确定性随机种子（seed = levelId × 1000 + (isHard ? 1 : 0)），确保同一关卡每次打乱结果一致。算法流程：1) 收集所有碎片及其原始位置；2) 计算保留原位数量（默认 30%）；3) 随机选择保留的碎片，其余执行 Fisher-Yates 洗牌；4) 困难模式下通过曼哈顿距离优化，使碎片尽量远离原始位置。');

emitCode(`    /// <summary>
    /// 打乱碎片位置
    /// </summary>
    public void ShuffleGridPositions(int level = 1,
        int maxKeepCount = -1, bool isHard = false)
    {
        List<RectTransform> children = new List<RectTransform>();
        List<Vector2> originalPositions = new List<Vector2>();
        List<Vector2Int> originalGridCoords = new List<Vector2Int>();

        foreach (Transform child in transform)
        {
            var rect = child.GetComponent<RectTransform>();
            if (rect == null) continue;
            children.Add(rect);
            originalPositions.Add(rect.anchoredPosition);

            string[] parts = child.name.Replace("GridCell_", "").Split('_');
            if (parts.Length == 2 && int.TryParse(parts[0], out int x)
                && int.TryParse(parts[1], out int y))
                originalGridCoords.Add(new Vector2Int(x, y));
            else
                originalGridCoords.Add(new Vector2Int(-1, -1));
        }

        if (children.Count < 2) return;
        Vector2 cellSize = children[0].sizeDelta;

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < children.Count; i++)
            availableIndices.Add(i);

        // 确定种子
        int seed = level * 1000 + (isHard ? 1 : 0);
        System.Random rng = new System.Random(seed);

        // 保留卡牌数量
        if (maxKeepCount == -1)
            maxKeepCount = Mathf.Max(0,
                Mathf.RoundToInt(children.Count * 0.3f));
        maxKeepCount = Mathf.Clamp(maxKeepCount,
            0, children.Count - 1);
        int cardsToKeep = rng.Next(0, maxKeepCount + 1);

        // 随机选择保留项
        List<int> allIndices = new List<int>();
        for (int i = 0; i < children.Count; i++)
            allIndices.Add(i);

        System.Random shuffleRng = new System.Random(seed + 1);
        for (int i = allIndices.Count - 1; i > 0; i--)
        {
            int j = shuffleRng.Next(0, i + 1);
            (allIndices[i], allIndices[j]) =
                (allIndices[j], allIndices[i]);
        }

        List<int> keepIndices = allIndices.GetRange(0, cardsToKeep);
        foreach (int k in keepIndices)
            availableIndices.Remove(k);

        List<int> targetIndices = new List<int>(availableIndices);

        // 困难模式：距离优化
        if (isHard && originalGridCoords.Count == children.Count)
        {
            System.Random distRng = new System.Random(seed + 3);
            for (int attempt = 0; attempt < 3; attempt++)
            {
                for (int i = 0; i < targetIndices.Count - 1; i++)
                {
                    int curIdx = availableIndices[i];
                    int curTgt = targetIndices[i];
                    int curDist = Manhattan(originalGridCoords[curIdx],
                        originalGridCoords[curTgt]);
                    for (int j = i + 1; j < targetIndices.Count; j++)
                    {
                        int oIdx = availableIndices[j];
                        int oTgt = targetIndices[j];
                        int nd1 = Manhattan(originalGridCoords[curIdx],
                            originalGridCoords[oTgt]);
                        int nd2 = Manhattan(originalGridCoords[oIdx],
                            originalGridCoords[curTgt]);
                        int od = Manhattan(originalGridCoords[oIdx],
                            originalGridCoords[oTgt]);
                        if (nd1 > curDist && nd2 > od)
                        {
                            (targetIndices[i], targetIndices[j]) =
                                (targetIndices[j], targetIndices[i]);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            System.Random tRng = new System.Random(seed + 2);
            for (int i = targetIndices.Count - 1; i > 0; i--)
            {
                int j = tRng.Next(0, i + 1);
                (targetIndices[i], targetIndices[j]) =
                    (targetIndices[j], targetIndices[i]);
            }
        }

        // 应用位置变化
        for (int i = 0; i < children.Count; i++)
        {
            if (keepIndices.Contains(i)) continue;
            int idx = availableIndices.IndexOf(i);
            if (idx >= 0 && idx < targetIndices.Count)
                children[i].anchoredPosition =
                    originalPositions[targetIndices[idx]];
        }

        RefreshAllPositionIndices();
        UpdateBorderVisibility();
    }

    private int Manhattan(Vector2Int a, Vector2Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);`);

emitSubtitle('3.5.5 胜利检测');
emitText('遍历所有碎片，对每个碎片检查其四个方向的实际邻居。如果每个方向都存在且名称匹配正确，则判定拼图完成。胜利后执行界面动画（向上移动 100px）并触发 show_next 事件。');

emitCode(`    internal void CheckSucess()
    {
        bool suc = true;
        foreach (Transform child in transform)
        {
            var c = child.GetComponent<DraggableGridItem>();
            if (c == null) continue;

            string[] parts = child.name.Replace("GridCell_", "").Split('_');
            if (parts.Length != 2) continue;
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            DraggableGridItem rN = null, lN = null, tN = null, bN = null;
            RectTransform cr = child.GetComponent<RectTransform>();

            foreach (Transform other in transform)
            {
                if (other == child) continue;
                var oi = other.GetComponent<DraggableGridItem>();
                if (oi == null) continue;

                Vector2 d = other.GetComponent<RectTransform>()
                    .anchoredPosition - cr.anchoredPosition;
                float t = 1f;
                if (Mathf.Abs(d.x - cr.sizeDelta.x) < t
                    && Mathf.Abs(d.y) < t) rN = oi;
                else if (Mathf.Abs(d.x + cr.sizeDelta.x) < t
                    && Mathf.Abs(d.y) < t) lN = oi;
                else if (Mathf.Abs(d.y - cr.sizeDelta.y) < t
                    && Mathf.Abs(d.x) < t) tN = oi;
                else if (Mathf.Abs(d.y + cr.sizeDelta.y) < t
                    && Mathf.Abs(d.x) < t) bN = oi;
            }

            if (!((rN == null || rN.name == $"GridCell_{x+1}_{y}")
                && (lN == null || lN.name == $"GridCell_{x-1}_{y}")
                && (tN == null || tN.name == $"GridCell_{x}_{y+1}")
                && (bN == null || bN.name == $"GridCell_{x}_{y-1}")))
            { suc = false; break; }
        }

        if (suc)
        {
            var t = transform.GetComponent<RectTransform>();
            Vector2 cur = t.anchoredPosition;
            t.DOAnchorPos(new Vector2(cur.x, cur.y + 100f), 0.5f)
             .OnComplete(() =>
             {
                 for (int i = 0; i < transform.childCount; i++)
                     transform.GetChild(i)
                         .GetComponent<DraggableGridItem>().enabled = false;
                 Main.DispEvent("show_next", curlevel);
             });
        }
    }`);

// ==================== 3.6 ====================
newPage('3.6 拖拽系统 (DraggableGridItem.cs)');
emitText('DraggableGridItem 实现拼图碎片的完整拖拽交互，实现了 IBeginDragHandler、IDragHandler、IEndDragHandler 三个接口。核心特性包括：连通组拖拽（正确连接的碎片整体移动）；边界限制（碎片不可拖出网格区域）；边框独立刷新（拖拽组和非拖拽组分两组刷新）。');

emitSubtitle('3.6.1 接口实现与属性定义');
emitCode(`public class DraggableGridItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform => GetComponent<RectTransform>();
    public Canvas canvas;

    private static int originalPositionIndex;  // 起始位置索引
    public static int targetPositionIndex;     // 目标位置索引
    public int PositionIndex { get; set; }     // 当前位置索引

    private bool isDragging = false;
    public bool IsBeingDragged => isDragging;
    public static bool isAnyItemDragging = false;

    // 邻接标志（由边框刷新算法设置）
    public bool adjacentLeft = false;
    public bool adjacentRight = false;
    public bool adjacentTop = false;
    public bool adjacentBottom = false;

    // 用于翻转动画和UV
    internal Texture2D pic;
    internal float uvX, uvY, uvWidth, uvHeight;

    // 位置属性
    static Vector2 originalPosition => new Vector2(
        (originalPositionIndex / wid) * carWid,
        (originalPositionIndex % hei) * carHei);
    static Vector2 targetPosition => new Vector2(
        (targetPositionIndex / wid) * carWid,
        (targetPositionIndex % hei) * carHei);
    Vector2 vecPosition => new Vector2(
        (PositionIndex / wid) * carWid,
        (PositionIndex % hei) * carHei);

    // 网格尺寸（委托picmgr）
    public static float carWid => picmgr.instance.carWid;
    public static float carHei => picmgr.instance.carHei;
    public static int wid => picmgr.instance.width;
    public static int hei => picmgr.instance.height;

    // 拖拽组
    private List<DraggableGridItem> dragGroup = null;
    private Dictionary<DraggableGridItem, Vector2>
        groupOriginalPositions = null;
    private Dictionary<DraggableGridItem, int>
        groupOriginalSiblingIndices = null;

    Transform parent => transform.parent;
    RawImage rawImage => GetComponent<RawImage>();
}`);

emitSubtitle('3.6.2 OnBeginDrag - 开始拖拽');
emitText('开始拖拽时记录碎片位置索引和层级，调用 CollectConnectedGroup 收集连通组，将组内所有成员提升至最上层渲染。');

emitCode(`    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAnyItemDragging) return;
        isAnyItemDragging = true;

        originalPositionIndex = PositionIndex;
        originalSiblingIndex = transform.GetSiblingIndex();

        CollectConnectedGroup();
        isDragging = true;

        // 置顶拖拽组
        if (dragGroup != null)
        {
            foreach (var it in dragGroup)
            {
                if (groupOriginalSiblingIndices != null
                    && !groupOriginalSiblingIndices.ContainsKey(it))
                    groupOriginalSiblingIndices[it] =
                        it.transform.GetSiblingIndex();
                if (groupOriginalPositions != null
                    && !groupOriginalPositions.ContainsKey(it))
                    groupOriginalPositions[it] =
                        it.rectTransform.anchoredPosition;
                it.transform.SetAsLastSibling();
            }
        }
    }`);

emitSubtitle('3.6.3 OnDrag - 拖拽中');
emitText('计算位移量并做边界限制（考虑整个拖拽组的所有成员）。应用位置后刷新两组边框：非拖拽组和拖拽组分别刷新。');

emitCode(`    public void OnDrag(PointerEventData eventData)
    {
        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect == null) return;

        // 计算原始位移
        Vector2 rawNewPos = rectTransform.anchoredPosition
            + eventData.delta / canvas.scaleFactor;
        Vector2 rawDelta = rawNewPos - originalPosition;

        // 边界限制（考虑组内所有成员）
        List<DraggableGridItem> allItems = dragGroup
            ?? new List<DraggableGridItem>() { this };

        float minDX = float.MinValue, maxDX = float.MaxValue;
        float minDY = float.MinValue, maxDY = float.MaxValue;

        foreach (var item in allItems)
        {
            Vector2 orig;
            if (item == this) orig = originalPosition;
            else if (groupOriginalPositions != null
                && groupOriginalPositions.TryGetValue(item, out Vector2 o))
                orig = o;
            else continue;

            float iw = item.rectTransform.rect.width;
            float ih = item.rectTransform.rect.height;

            minDX = Mathf.Max(minDX, -orig.x);
            maxDX = Mathf.Min(maxDX,
                parentRect.rect.width - iw - orig.x);
            minDY = Mathf.Max(minDY, -orig.y);
            maxDY = Mathf.Min(maxDY,
                parentRect.rect.height - ih - orig.y);
        }

        Vector2 clampedDelta = new Vector2(
            Mathf.Clamp(rawDelta.x, minDX, maxDX),
            Mathf.Clamp(rawDelta.y, minDY, maxDY));

        // 应用位置
        rectTransform.anchoredPosition =
            originalPosition + clampedDelta;

        if (dragGroup != null && groupOriginalPositions != null)
        {
            foreach (var it in dragGroup)
            {
                if (it == this) continue;
                if (groupOriginalPositions.TryGetValue(it,
                    out Vector2 orig))
                    it.rectTransform.anchoredPosition =
                        orig + clampedDelta;
            }
        }

        // 刷新两组边框
        if (picmgr.instance != null)
        {
            picmgr.instance.UpdateBorderVisibility();
            if (dragGroup != null)
                picmgr.instance.UpdateBorderVisibility(dragGroup);
        }

        // 计算目标位置索引
        var diff = rectTransform.anchoredPosition - vecPosition;
        if (Mathf.Abs(diff.x) > carWid / 2
            || Mathf.Abs(diff.y) > carHei / 2)
        {
            int x = Mathf.RoundToInt(
                rectTransform.anchoredPosition.x / carWid);
            int y = Mathf.RoundToInt(
                rectTransform.anchoredPosition.y / carHei);
            x = Mathf.Min(wid, Mathf.Max(0, x));
            y = Mathf.Min(hei, Mathf.Max(0, y));
            targetPositionIndex = x * hei + y;
        }
        else
            targetPositionIndex = PositionIndex;
    }`);

emitSubtitle('3.6.4 OnEndDrag - 结束拖拽');
emitText('判断是否满足交换条件（位置不同且位置合法），成功则执行位置交换，失败则整体退回原位。');

emitCode(`    public void OnEndDrag(PointerEventData eventData)
    {
        bool shouldSwap = false;

        if (originalPositionIndex != targetPositionIndex)
        {
            bool isPositionValid = CheckPotentialPositionValidity();
            if (isPositionValid) shouldSwap = true;
        }

        if (shouldSwap) SwapPositions(targetPosition);
        else ResetAllDraggedItemsToOriginalPosition();

        dragGroup = null;
        groupOriginalPositions = null;
        groupOriginalSiblingIndices = null;
        isDragging = false;
    }

    private bool CheckPotentialPositionValidity(
        Vector2 potentialTargetPos = default)
    {
        List<DraggableGridItem> list = (dragGroup != null
            && dragGroup.Count > 0)
            ? new List<DraggableGridItem>(dragGroup)
            : new List<DraggableGridItem>() { this };

        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect == null) return false;

        Rect parentBounds = new Rect(0, 0,
            parentRect.rect.width, parentRect.rect.height);

        foreach (var item in list)
        {
            Vector2 newPos = item.rectTransform.anchoredPosition
                + new Vector2(item.rectTransform.sizeDelta.x / 2,
                    item.rectTransform.sizeDelta.y / 2);
            if (!parentBounds.Contains(newPos)) return false;
        }
        return true;
    }`);

emitSubtitle('3.6.5 连通组检测 (DFS)');
emitText('使用深度优先搜索（DFS）算法，基于碎片的 adjacent 标志进行遍历。从当前碎片开始，沿四个方向递归扩展，将 adjacent 为 true 且目标坐标存在的碎片加入拖拽组。');

emitCode(`    private void CollectConnectedGroup()
    {
        dragGroup = new List<DraggableGridItem>();
        groupOriginalPositions = new Dictionary<...>();
        groupOriginalSiblingIndices = new Dictionary<...>();

        // 构建坐标映射
        Dictionary<(int, int), DraggableGridItem> map = new();
        foreach (Transform child in parent)
        {
            var item = child.GetComponent<DraggableGridItem>();
            if (item == null) continue;
            if (TryParseCellName(child.name, out int cx, out int cy))
                map[(cx, cy)] = item;
        }

        if (!TryParseCellName(this.name, out int startX, out int startY))
        {
            dragGroup.Add(this);
            groupOriginalPositions[this] = rectTransform.anchoredPosition;
            groupOriginalSiblingIndices[this] = transform.GetSiblingIndex();
            return;
        }

        void dfsCoord(int x, int y)
        {
            if (!map.TryGetValue((x, y), out var cur)) return;
            if (dragGroup.Contains(cur)) return;

            dragGroup.Add(cur);
            groupOriginalPositions[cur] = cur.rectTransform.anchoredPosition;
            groupOriginalSiblingIndices[cur] = cur.transform.GetSiblingIndex();

            if (cur.adjacentLeft)   dfsCoord(x - 1, y);
            if (cur.adjacentRight)  dfsCoord(x + 1, y);
            if (cur.adjacentTop)    dfsCoord(x, y + 1);
            if (cur.adjacentBottom) dfsCoord(x, y - 1);
        }

        dfsCoord(startX, startY);
    }

    private bool TryParseCellName(string name, out int ox, out int oy)
    {
        ox = oy = -1;
        if (!name.StartsWith("GridCell_")) return false;
        var parts = name.Replace("GridCell_", "").Split('_');
        if (parts.Length != 2) return false;
        return int.TryParse(parts[0], out ox)
            && int.TryParse(parts[1], out oy);
    }`);

emitSubtitle('3.6.6 位置交换 (SwapPositions)');
emitText('位置交换算法：1) 计算拖拽组的目标位置；2) 识别空出位置和被覆盖碎片；3) 建立覆盖碎片到空出位置的映射；4) 使用 DOTween Sequence 同时执行所有移动动画；5) 完成后刷新位置索引和边框。');

emitCode(`    private void SwapPositions(Vector2 targetPos)
    {
        PositionIndex = GetIndexFromAnchoredPosition(targetPos);

        var list = (dragGroup != null && dragGroup.Count > 0)
            ? new List<DraggableGridItem>(dragGroup)
            : new List<DraggableGridItem>() { this };

        // 记录原始位置和层级
        Dictionary<DraggableGridItem, Vector2> originals = new();
        Dictionary<DraggableGridItem, int> siblings = new();
        foreach (var item in list)
        {
            originals[item] = groupOriginalPositions != null
                && groupOriginalPositions.TryGetValue(item, out var p)
                ? p : item.rectTransform.anchoredPosition;
            siblings[item] = item.transform.GetSiblingIndex();
        }

        Vector2 srcPos = originals[this];
        Dictionary<DraggableGridItem, Vector2> targets = new();
        foreach (var item in list)
            targets[item] = targetPos + (originals[item] - srcPos);

        // 计算空出位置
        List<Vector2> emptyPos = new();
        foreach (var kvp in originals)
        {
            bool filled = false;
            foreach (var t in targets.Values)
                if (Vector2.Distance(t, kvp.Value) < 1f)
                { filled = true; break; }
            if (!filled) emptyPos.Add(kvp.Value);
        }

        // 计算被覆盖卡牌
        List<DraggableGridItem> covered = new();
        foreach (Transform child in parent)
        {
            var item = child.GetComponent<DraggableGridItem>();
            if (item == null || list.Contains(item)) continue;
            foreach (var t in targets.Values)
            {
                if (Vector2.Distance(
                    item.rectTransform.anchoredPosition, t) < 1f)
                { covered.Add(item); break; }
            }
        }

        // 排序后映射
        emptyPos.Sort((a, b) => {
            int yc = b.y.CompareTo(a.y);
            return yc != 0 ? yc : a.x.CompareTo(b.x);
        });
        covered.Sort((a, b) => {
            int yc = b.rectTransform.anchoredPosition.y
                .CompareTo(a.rectTransform.anchoredPosition.y);
            return yc != 0 ? yc :
                a.rectTransform.anchoredPosition.x
                    .CompareTo(b.rectTransform.anchoredPosition.x);
        });

        Dictionary<DraggableGridItem, Vector2> replace = new();
        for (int i = 0; i < Mathf.Min(covered.Count, emptyPos.Count); i++)
            replace[covered[i]] = emptyPos[i];

        // 执行动画
        Sequence seq = DOTween.Sequence();
        foreach (var item in list)
            seq.Join(item.rectTransform.DOAnchorPos(
                targets[item], 0.25f));
        foreach (var kvp in replace)
            seq.Join(kvp.Key.rectTransform.DOAnchorPos(
                kvp.Value, 0.25f));

        seq.OnComplete(() =>
        {
            // 恢复层级
            var all = new List<DraggableGridItem>(list);
            all.AddRange(covered);
            all.Sort((a, b) =>
                siblings.GetValueOrDefault(a, 0)
                    .CompareTo(siblings.GetValueOrDefault(b, 0)));
            foreach (var item in all)
                item.transform.SetAsLastSibling();

            var pm = GetComponentInParent<picmgr>();
            if (pm != null)
            {
                pm.RefreshAllPositionIndices();
                pm.UpdateBorderVisibility();
                pm.CheckSucess();
            }
            isAnyItemDragging = false;
        });
    }

    private int GetIndexFromAnchoredPosition(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x / carWid);
        int y = Mathf.RoundToInt(pos.y / carHei);
        return x * picmgr.instance.height + y;
    }`);

emitSubtitle('3.6.7 翻转动画与退回到位');

emitCode(`    /// <summary>
    /// 翻转动画（DOTween ScaleX翻转）
    /// </summary>
    public void Turn(bool atonce = false)
    {
        if (atonce) { settex(); }
        else
        {
            transform.DOScaleX(0, 0.25f).OnComplete(() =>
            {
                transform.localScale = new Vector3(0, 1, 1);
                settex();
                transform.DOScaleX(1, 0.25f).OnComplete(() =>
                    transform.localScale = new Vector3(1, 1, 1));
            });

            // 伴随平移动画
            float x = transform.localPosition.x;
            float wid = rectTransform.rect.width;
            transform.DOLocalMoveX(x + wid / 2, 0.25f).OnComplete(() =>
            {
                var p = transform.localPosition;
                p.x = x + wid / 2;
                transform.localPosition = p;
                transform.DOLocalMoveX(x, 0.25f).OnComplete(() =>
                {
                    var p = transform.localPosition;
                    p.x = x;
                    transform.localPosition = p;
                });
            });
        }
    }

    internal void settex()
    {
        rawImage.color = Color.white;
        rawImage.uvRect = new Rect(uvX, uvY, uvWidth, uvHeight);
        rawImage.texture = pic;
    }

    /// <summary>
    /// 退回原位
    /// </summary>
    private void ResetAllDraggedItemsToOriginalPosition()
    {
        if (dragGroup != null && groupOriginalPositions != null)
        {
            Sequence seq = DOTween.Sequence();
            foreach (var kvp in groupOriginalPositions)
            {
                seq.Join(kvp.Key.rectTransform
                    .DOAnchorPos(kvp.Value, 0.25f));
                kvp.Key.PositionIndex =
                    GetIndexFromAnchoredPosition(kvp.Value);
            }
            seq.OnComplete(() =>
            {
                foreach (var kvp in groupOriginalSiblingIndices)
                    kvp.Key.transform.SetSiblingIndex(kvp.Value);
                var pm = GetComponentInParent<picmgr>();
                if (pm != null)
                {
                    pm.RefreshAllPositionIndices();
                    pm.UpdateBorderVisibility();
                }
                isAnyItemDragging = false;
            });
        }
        else
        {
            rectTransform.DOAnchorPos(originalPosition, 0.3f)
                .OnComplete(() =>
            {
                rectTransform.anchoredPosition = originalPosition;
                transform.SetSiblingIndex(originalSiblingIndex);
                PositionIndex = originalPositionIndex;
                var pm = GetComponentInParent<picmgr>();
                if (pm != null)
                {
                    pm.RefreshAllPositionIndices();
                    pm.UpdateBorderVisibility();
                }
                isAnyItemDragging = false;
            });
        }
    }`);

// ==================== 3.7 ====================
newPage('3.7 卡片系统 (card.cs)');
emitText('card 用于章节选择界面的关卡卡片。每张卡片对应一个关卡，根据关卡状态显示不同内容：已通关卡片显示章节预览图的对应区域并执行翻转动画；已解锁未通关显示预览图；未解锁显示默认背面图案和关卡编号。');
emitCode(`public class card : MonoBehaviour
{
    public Texture2D back;
    internal int levelid;
    internal float uvX, uvY, uvWidth, uvHeight;
    internal Texture texture;
    public TextMeshProUGUI level;
    public bool isTurning = false;

    RawImage rawImage => GetComponent<RawImage>();

    internal void Load()
    {
        if (levelid < PlayerData.gd.levelid)
        {
            if (PlayerData.gd.isOpened(levelid))
            {
                rawImage.uvRect = new Rect(
                    uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                rawImage.texture = texture;
                level.gameObject.SetActive(false);
            }
            else
            {
                DOVirtual.DelayedCall(1.0f, () =>
                {
                    rawImage.uvRect = new Rect(0, 0, 1, 1);
                    rawImage.texture = back;
                    level.gameObject.SetActive(false);
                    isTurning = true;
                    transform.DOScaleX(0, .5f).OnComplete(() =>
                    {
                        transform.localScale = new Vector3(0, 1, 1);
                        rawImage.uvRect = new Rect(
                            uvX, (1 - uvY - uvHeight), uvWidth, uvHeight);
                        rawImage.texture = texture;
                        transform.DOScaleX(1, 0.5f).OnComplete(() =>
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                            isTurning = false;
                        });
                    });
                    PlayerData.gd.Open(levelid);
                });
            }
        }
        else
        {
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            rawImage.texture = back;
            level.gameObject.SetActive(true);
        }
    }
}`);

// ==================== 3.8 ====================
newPage('3.8 边框系统 (BorderSpriteLoader.cs)');
emitText('BorderSpriteLoader 负责为边框子对象加载贴图，并在运行时动态切换贴图类型。贴图从 Resources/Card 目录加载，具有静态缓存避免重复加载。RefreshBasedOnFlags 方法根据四个方向邻居的正确性切换角贴图为角、边或反转角贴图。');
emitCode(`public class BorderSpriteLoader : MonoBehaviour
{
    public string resourceName;
    public bool keepColorIfMissing = true;
    private static Dictionary<string, Sprite> spriteCache;

    internal void LoadSprite()
    {
        if (string.IsNullOrEmpty(resourceName)) return;
        Image img = GetComponent<Image>();
        if (img == null) return;

        if (spriteCache == null)
            spriteCache = new Dictionary<string, Sprite>();

        if (spriteCache.TryGetValue(resourceName, out var s))
        {
            img.sprite = s; img.color = Color.white;
        }
        else
        {
            s = Resources.Load<Sprite>("Card/" + resourceName);
            if (s != null)
            {
                img.sprite = s; img.color = Color.white;
                spriteCache[resourceName] = s;
            }
            else
            {
                img.sprite = null;
                img.color = keepColorIfMissing ? Color.gray : Color.clear;
            }
        }
    }

    public void SetResourceAndLoad(string key)
    {
        resourceName = key;
        LoadSprite();
    }

    internal void RefreshBasedOnFlags(string prop,
        bool isTopCorrect, bool isLeftCorrect,
        bool isRightCorrect, bool isBottomCorrect,
        bool isConnerCorrect)
    {
        string key = resourceName;
        switch (prop)
        {
            case "lt":
                if (isLeftCorrect && isTopCorrect && !isConnerCorrect)
                    key = "lt_revert";
                else if (!isLeftCorrect && !isTopCorrect) key = "lt";
                else if (isLeftCorrect && !isTopCorrect) key = "t";
                else if (!isLeftCorrect && isTopCorrect) key = "l";
                break;
            case "rt":
                if (isRightCorrect && isTopCorrect && !isConnerCorrect)
                    key = "rt_revert";
                else if (!isRightCorrect && !isTopCorrect) key = "rt";
                else if (isRightCorrect && !isTopCorrect) key = "t";
                else if (!isRightCorrect && isTopCorrect) key = "r";
                break;
            case "rb":
                if (isRightCorrect && isBottomCorrect && !isConnerCorrect)
                    key = "rb_revert";
                else if (!isRightCorrect && !isBottomCorrect) key = "rb";
                else if (isRightCorrect && !isBottomCorrect) key = "b";
                else if (!isRightCorrect && isBottomCorrect) key = "r";
                break;
            case "lb":
                if (isLeftCorrect && isBottomCorrect && !isConnerCorrect)
                    key = "lb_revert";
                else if (!isLeftCorrect && !isBottomCorrect) key = "lb";
                else if (isLeftCorrect && !isBottomCorrect) key = "b";
                else if (!isLeftCorrect && isBottomCorrect) key = "l";
                break;
        }
        SetResourceAndLoad(key);
    }
}`);

// ==================== SECTION 4 ====================
newPage('4 UI 模块设计');
emitSubtitle('4.1 基础窗体 (frmbase.cs)');
emitText('frmbase 是所有 UI 窗体的基类，提供统一的 show/hide 生命周期。通过 transform.Find("Root") 查找根节点控制显隐，子类重写 OnShow/OnHide 实现自定义逻辑。');
emitCode(`public class frmbase : MonoBehaviour
{
    protected Transform gb => transform.Find("Root");

    public bool isOpen() => gb.gameObject.activeSelf;

    public void show()
    {
        gb.gameObject.SetActive(true);
        if (Application.isPlaying) OnShow();
    }

    public virtual void hide()
    {
        OnHide();
        gb.gameObject.SetActive(false);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}`);

newPage('4.2 章节选择界面 (frm_chapter.cs)');
emitText('章节选择界面以网格形式展示当前章节的所有关卡卡片。通过事件驱动刷新：gamebegin 首次加载，onLevelChange/onChapterChange 刷新，level_next 切换关卡。点击「开始」按钮触发 level_play 进入游戏。');
emitCode(`public class frm_chapter : frmbase
{
    public RectTransform chaptercontent;
    public Button btn;
    public Button btnsetup;
    public TextMeshProUGUI staminaText, levelname;

    private void Awake()
    {
        Main.RegistEvent("gamebegin", (x) => {
            brushChapterContent(); show();
            UpdateStaminaDisplay(); return 1;
        });
        Main.RegistEvent("onpowerChange", (x) => {
            UpdateStaminaDisplay(); return null;
        });
        Main.RegistEvent("level_next", (x) => {
            PlayerData.gd.levelid = datamgr.Instance
                .GetLevel(PlayerData.gd.levelid).NextLevel;
            brushChapterContent(); show(); return 1;
        });
        Main.RegistEvent("onChapterChange", (x) => {
            brushChapterContent(); show(); return 1;
        });
        Main.RegistEvent("level_back", (x) => {
            show(); return null;
        });

        btn.onClick.AddListener(() => {
            if (!isTurning())
            {
                int ret = (int)Main.DispEvent(
                    "level_play", PlayerData.gd.levelid);
                if (ret == 1) hide();
            }
        });
        btnsetup.onClick.AddListener(() =>
            Main.DispEvent("show_setup", null));
    }

    private bool isTurning()
    {
        for (int i = 0; i < chaptercontent.childCount; i++)
            if (chaptercontent.GetChild(i)
                .GetComponent<card>().isTurning) return true;
        return false;
    }

    private void brushChapterContent()
    {
        var ch = datamgr.Instance.GetChapter(PlayerData.gd.currChapter);
        var pic = Resources.Load(ch.ChapterFigure) as Texture;
        levelname.text = $"Level {PlayerData.gd.levelid % 10000}";

        // 清除旧卡片
        for (int i = chaptercontent.childCount - 1; i >= 0; i--)
            Destroy(chaptercontent.GetChild(i).gameObject);

        Vector2 sz = chaptercontent.rect.size;
        float cw = sz.x / ch.ChapterFigureX;
        float ch2 = sz.y / ch.ChapterFigureY;

        for (int j = 0; j < ch.ChapterFigureY; j++)
        {
            for (int i = 0; i < ch.ChapterFigureX; i++)
            {
                int idx = j * ch.ChapterFigureX + i;
                int lid = ch.LevelId[idx];
                var go = Instantiate(
                    Resources.Load("levelpic")) as GameObject;
                go.transform.SetParent(chaptercontent, false);

                var c = go.GetComponent<card>();
                c.levelid = lid;
                c.uvX = (float)i / ch.ChapterFigureX;
                c.uvY = (float)j / ch.ChapterFigureY;
                c.uvWidth = 1f / ch.ChapterFigureX;
                c.uvHeight = 1f / ch.ChapterFigureY;
                c.texture = pic;
                c.Load();
                c.level.text = lid.ToString();

                var rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0.5f, 0);
                rt.anchoredPosition = new Vector2(
                    (i + 0.5f) * cw,
                    (ch.ChapterFigureY - 1 - j) * ch2);
                rt.sizeDelta = new Vector2(cw - 4, ch2 - 4);
            }
        }
    }

    private void UpdateStaminaDisplay()
    {
        if (staminaText != null)
            staminaText.text = PlayerData.gd.power.ToString();
    }
}`);

newPage('4.3 游戏界面 (frm_game.cs)');
emitText('游戏界面承载拼图核心玩法。加载关卡后调用 picmgr.LoadLevel 进行图片切割和打乱。显示关卡编号和难度标签，提供返回、设置、下一关按钮。');
emitCode(`public class frm_game : frmbase
{
    public picmgr mgr;
    public TextMeshProUGUI level;
    public Button next, back, setup;

    private void Awake()
    {
        Main.RegistEvent("level_play", (x) =>
        {
            if (!PlayerData.gd.hasEnoughpower(10))
            {
                Main.DispEvent("event_msg", "power不足");
                return 0;
            }
            PlayerData.gd.消耗power(10);
            next.gameObject.SetActive(false);

            var leevel = datamgr.Instance.GetLevel((int)x);
            level.text = $"Level {leevel.Id}";

            if (leevel.DifficultyTier == 2)
            {
                var df = gb.Find("diff");
                df.gameObject.SetActive(true);
                DOVirtual.Float(0, 1, .5f, v =>
                    df.GetComponent<CanvasGroup>().alpha = v)
                .OnComplete(() => DOVirtual.DelayedCall(1, () =>
                    DOVirtual.Float(1, 0, .5f, v =>
                        df.GetComponent<CanvasGroup>().alpha = v)
                    .OnComplete(() => df.gameObject.SetActive(false))));
            }
            show();
            StartCoroutine(load(leevel));
            return 1;
        });

        Main.RegistEvent("level_next", (x) => { hide(); return null; });
        next.onClick.AddListener(() => { Main.SendEvent("level_next"); hide(); });
        back.onClick.AddListener(() => { Main.SendEvent("level_back"); hide(); });
        setup.onClick.AddListener(() => Main.DispEvent("show_setup"));
    }

    IEnumerator load(cfg.DrLevel leevel)
    {
        yield return StartCoroutine(mgr.LoadLevel(leevel));
    }
}`);

newPage('4.4 设置界面 (frm_setup.cs)');
emitText('设置界面提供游戏初始化（删除存档重启）和体力购买（测试用途）功能。');
emitCode(`public class frm_setup : frmbase
{
    public Button btnInitialize, btnClose, btn_buytili;

    private void Awake()
    {
        btnInitialize.onClick.AddListener(OnInitializeClick);
        btnClose.onClick.AddListener(OnCloseClick);
        btn_buytili.onClick.AddListener(OnBuyTiliClick);
        Main.RegistEvent("show_setup", (x) => { show(); return 1; });
    }

    private void OnBuyTiliClick() => PlayerData.gd.power += 100;
    private void OnCloseClick() => hide();

    private void OnInitializeClick()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path)) File.Delete(path);
        FindObjectOfType<PlayerData>()?.SendMessage("loadData");
        Main.DispEvent("onLevelChange");
        Application.Quit();
    }
}`);

// ==================== SECTION 5 ====================
newPage('5 数据配置');
emitSubtitle('5.1 章节配置 (DrChapter)');
emitText('DrChapter 定义章节数据，包括预览图片、切割行列数、关卡 ID 列表。通过 Luban 工具从 Excel 导出为 JSON。');
emitCode(`[Serializable]
public sealed partial class DrChapter : Luban.BeanBase
{
    public readonly int Id;
    public readonly string ChapterTitle;
    public readonly string ChapterFigure;    // 预览图资源名
    public readonly int ChapterFigureX;      // 横向关卡数
    public readonly int ChapterFigureY;      // 纵向关卡数
    public readonly int NextChapter;         // 下一章ID
    public readonly List<int> LevelId;       // 关卡ID列表

    public DrChapter(JSONNode _buf)
    {
        Id = _buf["id"];
        ChapterTitle = _buf["chapter_title"];
        ChapterFigure = _buf["chapter_figure"];
        ChapterFigureX = _buf["chapter_figure_x"];
        ChapterFigureY = _buf["chapter_figure_y"];
        NextChapter = _buf["next_chapter"];
        LevelId = new List<int>();
        foreach (JSONNode e in _buf["level_id"].Children)
            LevelId.Add(e);
    }
}`);

emitSubtitle('5.2 关卡配置 (DrLevel)');
emitText('DrLevel 定义关卡参数：图片资源、切割行列、难度等级、原位保留数、曼哈顿距离范围。');
emitCode(`[Serializable]
public sealed partial class DrLevel : Luban.BeanBase
{
    public readonly int Id;                  // 关卡ID
    public readonly int NextLevel;           // 下一关ID
    public readonly string LevelFigure;      // 图片资源名
    public readonly int LevelFigureX;        // 列数
    public readonly int LevelFigureY;        // 行数
    public readonly int DifficultyTier;      // 难度 1=普通 2=困难
    public readonly int OutOfPlaceNumber;    // 最多原位保留
    public readonly List<int> MDistanceRange; // 曼哈顿距离范围

    public DrLevel(JSONNode _buf)
    {
        Id = _buf["id"];
        NextLevel = _buf["next_level"];
        LevelFigure = _buf["level_figure"];
        LevelFigureX = _buf["level_figure_x"];
        LevelFigureY = _buf["level_figure_y"];
        DifficultyTier = _buf["difficulty_tier"];
        OutOfPlaceNumber = _buf["out_of_place_number"];
        MDistanceRange = new List<int>();
        foreach (JSONNode e in _buf["m_distance_range"].Children)
            MDistanceRange.Add(e);
    }
}`);

emitSubtitle('5.3 Tables 配置集合');
emitText('Tables 汇聚所有配置表，负责 JSON 加载和跨表引用解析。');
emitCode(`public partial class Tables
{
    public TbChapter TbChapter { get; }
    public TbLevel TbLevel { get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        TbChapter = new TbChapter(loader("tbchapter"));
        TbLevel = new TbLevel(loader("tblevel"));
        ResolveRef();
    }

    private void ResolveRef()
    {
        TbChapter.ResolveRef(this);
        TbLevel.ResolveRef(this);
    }
}

// 配置JSON示例
// tbchapter.json:
// [{
//   "id": 1,
//   "chapter_title": "第一章",
//   "chapter_figure": "chapter_01",
//   "chapter_figure_x": 3,
//   "chapter_figure_y": 2,
//   "next_chapter": 2,
//   "level_id": [100001, 100002, 100003, 100004, 100005, 100006]
// }]
//
// tblevel.json:
// [{
//   "id": 100001,
//   "next_level": 100002,
//   "level_figure": "level_01",
//   "level_figure_x": 2,
//   "level_figure_y": 2,
//   "difficulty_tier": 1,
//   "out_of_place_number": 1,
//   "m_distance_range": [2, 4]
// }]`);

// ==================== SECTION 6 ====================
newPage('6 核心算法');
emitText('本章详细描述游戏中的五个核心算法：拼图打乱、连通组检测、位置交换、胜利检测、边框刷新。');

emitSubtitle('6.1 拼图打乱算法');
emitText('输入：所有碎片的原始位置、关卡编号、难度标识。\n算法步骤：\n1. 收集所有碎片的原始网格坐标（从名称 GridCell_x_y 解析）和实际位置\n2. 生成确定性随机种子：seed = levelId × 1000 + (isHard ? 1 : 0)\n3. 计算保留原位数量：默认 children.Count × 30%，clamp 到 [0, count-1]\n4. Fisher-Yates 洗牌选择要保留的碎片\n5. 剩余碎片再次 Fisher-Yates 随机分配目标位置\n6. 困难模式额外执行 3 轮贪心优化：遍历碎片对，如果交换能使两者的曼哈顿距离同时增加则执行交换\n7. 应用位置变化，更新位置索引和边框');

emitSubtitle('6.2 连通组检测算法');
emitText('输入：当前拖拽的碎片。\n输出：与当前碎片连通的碎片集合（含原始位置和层级信息）。\n\n算法步骤：\n1. 遍历父容器的所有子对象，建立「GridCell_x_y」坐标到 DraggableGridItem 的字典映射\n2. 从当前碎片开始 DFS：\n   a. 将当前节点加入 dragGroup\n   b. 记录原始位置和层级到字典\n   c. 沿 adjacentLeft/Right/Top/Bottom 为 true 的方向递归\n   d. 若目标坐标在映射中不存在或已在组中则跳过\n3. 返回连通组');

emitSubtitle('6.3 位置交换算法');
emitText('输入：拖拽组的目标位置。\n执行流程：\n1. 计算拖拽组每个成员的目标位置 = 目标位置 + (成员原始位置 - 拖拽源原始位置)\n2. 识别空出位置：拖拽组原始位置中未被拖拽组其他成员覆盖的位置\n3. 识别被覆盖碎片：不在拖拽组中，但当前位置被拖拽组目标位置占用的碎片\n4. 将空出位置和被覆盖碎片分别按「从上到下、从左到右」排序\n5. 建立覆盖碎片→空出位置的映射\n6. 使用 DOTween Sequence 并行执行所有移动动画（0.25s）\n7. 完成回调：恢复层级顺序、刷新位置索引、刷新边框、检测胜利');

emitSubtitle('6.4 胜利检测算法');
emitText('输入：所有碎片的当前 anchoredPosition。\n输出：bool 是否完成拼图。\n\n算法对每个碎片：\n1. 获取该碎片的逻辑坐标 (x, y) 和 RectTransform\n2. 计算四个方向邻居的预期名称：GridCell_(x±1)_y / GridCell_x_(y±1)\n3. 遍历其他所有碎片，计算位置差：\n   • 右邻居：diff.x ≈ +cellWidth, diff.y ≈ 0\n   • 左邻居：diff.x ≈ -cellWidth, diff.y ≈ 0\n   • 上邻居：diff.y ≈ +cellHeight, diff.x ≈ 0\n   • 下邻居：diff.y ≈ -cellHeight, diff.x ≈ 0\n4. 检查实际邻居的名称是否与预期匹配\n5. 所有方向均匹配则继续下一个碎片，否则判定为未完成\n6. 全部碎片通过则触发胜利序列');

emitSubtitle('6.5 边框刷新算法');
emitText('输入：要处理的碎片列表（非拖拽组全体或拖拽组）。\n输出：对应碎片边框显隐状态及贴图更新。\n\n算法对列表内每个碎片：\n1. 解析名称获取逻辑坐标 (x, y)\n2. 获取当前实际 anchoredPosition\n3. 在输入列表中查找四个方向邻居（基于位置，阈值 1.5px）\n4. 检查找到的邻居名称是否匹配预期\n5. 正确连接（名称匹配）→ 隐藏边；否则显示边\n6. 角显隐：相邻任一边需要显示则角显示\n7. 角贴图：根据四个方向正确性切换角/边/反转角贴图');

// ==================== SECTION 7 ====================
newPage('7 数据持久化');
emitSubtitle('7.1 存档系统');
emitText('存档以 JSON 格式存储于 Application.persistentDataPath/playerData.json。包含：当前关卡 ID、当前章节 ID、体力值、上次更新时间戳、已解锁关卡 ID 数组。通过事件自动保存。');

emitSubtitle('7.2 体力系统');
emitText('体力上限 100，每关消耗 10。每 15 分钟自动恢复 1 点。离线期间累积恢复。利用 LitJson 序列化存储，通过事件 onpowerChange 触发 UI 刷新。');

// ==================== SECTION 8 ====================
newPage('8 运行环境');
emitSubtitle('8.1 硬件要求');
emitText('CPU: ARM64 / x86_64 1.5GHz+\n内存: 2GB+\n存储: 100MB\n屏幕: 1280×720+');
emitSubtitle('8.2 软件要求');
emitText('iOS 13.0+ / Android 8.0+\nUnity 2022.3 LTS\n.NET Standard 2.1\n依赖: DOTween, LitJson, Luban');

// We should have enough pages now. Let the PDF finish.
doc.end();

stream.on('finish', () => {
  console.log('PDF generated successfully!');
  const fs2 = require('fs');
  const buf = fs2.readFileSync('D:\\project\\pintu\\拼图游戏_软件设计说明书.pdf');
  const matches = buf.toString().match(/\/Type\s*\/Page[^s]/g);
  console.log('Total pages:', matches ? matches.length : '?');
});
stream.on('error', (err) => console.error('Error:', err));
