const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');

const doc = new PDFDocument({
  size: 'A4',
  margins: { top: 60, bottom: 60, left: 60, right: 60 },
  info: {
    Title: '拼图游戏 软件著作权鉴别材料',
    Author: '开发团队',
    Subject: '软件设计说明书',
  }
});

const outputPath = 'D:\\project\\pintu\\拼图游戏_软著鉴别材料.pdf';
const stream = fs.createWriteStream(outputPath);
doc.pipe(stream);

const SOF_NAME = '拼图游戏';
const SOF_VER = 'V1.0';
const SOFTITLE = `${SOF_NAME} ${SOF_VER}`;

let gPageNum = 0;

function addPageDecor() {
  gPageNum++;
  doc.save();
  // header line
  doc.rect(60, 38, doc.page.width - 120, 0.8).fill('#888888');
  doc.fontSize(8.5).fillColor('#555555').font('Helvetica');
  doc.text(SOFTITLE, 60, 44, { align: 'left', lineBreak: false, continued: true });
  doc.text(`第 ${gPageNum} 页`, doc.page.width - 60, 44, { align: 'right', lineBreak: false });
  // footer
  doc.rect(60, doc.page.height - 48, doc.page.width - 120, 0.8).fill('#888888');
  doc.fontSize(8).fillColor('#888888').font('Helvetica');
  doc.text('软件设计说明书', 60, doc.page.height - 44, { align: 'center', lineBreak: false });
  doc.restore();
}

function newPage() {
  doc.addPage();
  addPageDecor();
}

// ============ COVER ============
addPageDecor();
doc.fontSize(32).fillColor('#1a1a2e').font('Helvetica-Bold');
doc.text(SOF_NAME, 60, 130, { align: 'center' });
doc.fontSize(24).fillColor('#16213e').font('Helvetica-Bold');
doc.text('软件设计说明书', 60, 178, { align: 'center' });
doc.moveTo(120, 222).lineTo(doc.page.width - 120, 222).stroke('#0f3460');
doc.fontSize(14).fillColor('#333333').font('Helvetica');
doc.text(`版本：${SOF_VER}`, 60, 250, { align: 'center' });
doc.text('日期：2026年6月', 60, 278, { align: 'center' });
doc.fontSize(11).fillColor('#666666');
doc.text('本文档包含拼图游戏的软件架构设计、', 60, 330, { align: 'center' });
doc.text('模块详细设计及核心算法说明', 60, 348, { align: 'center' });

// ============ TOC ============
newPage();
doc.fontSize(16).fillColor('#1a1a2e').font('Helvetica-Bold');
doc.text('目录', 60, doc.y, { align: 'center' });
doc.moveTo(120, doc.y + 4).lineTo(doc.page.width - 120, doc.y + 4).stroke('#0f3460');
doc.y += 18;

const toc = [
  '一、引言',
  '  1.1 编写目的',
  '  1.2 项目概述',
  '  1.3 术语定义',
  '二、系统总体设计',
  '  2.1 架构概述',
  '  2.2 技术栈',
  '  2.3 目录结构',
  '三、核心功能模块设计',
  '  3.1 主控模块 Main.cs',
  '  3.2 事件系统',
  '  3.3 数据管理器 datamgr.cs',
  '  3.4 玩家数据 PlayerData.cs',
  '  3.5 拼图管理器 picmgr.cs',
  '  3.6 拖拽系统 DraggableGridItem.cs',
  '  3.7 卡片系统 card.cs',
  '  3.8 边框系统 BorderSpriteLoader.cs',
  '四、用户界面设计',
  '  4.1 章节选择界面',
  '  4.2 游戏界面',
  '  4.3 设置界面',
  '五、数据配置设计',
  '  5.1 章节配置',
  '  5.2 关卡配置',
  '六、核心算法说明',
  '  6.1 拼图打乱算法',
  '  6.2 连通组检测算法',
  '  6.3 位置交换算法',
  '  6.4 胜利检测算法',
  '  6.5 边框刷新算法',
  '七、运行环境',
];

doc.fontSize(9.5).fillColor('#333333').font('Helvetica');
for (const t of toc) {
  if (doc.y > doc.page.height - 70) newPage();
  doc.text(t, 60, doc.y, { lineBreak: false, indent: t.startsWith('  ') ? 15 : 0 });
  doc.y += 15;
}

// ============ CONTENT HELPER ============
let yy = 0;

function h1(text) {
  newPage();
  doc.fontSize(14).fillColor('#1a1a2e').font('Helvetica-Bold');
  doc.text(text, 60, 72, { lineBreak: false });
  doc.moveTo(60, 92).lineTo(doc.page.width - 60, 92).stroke('#0f3460');
  yy = 104;
}

function h2(text) {
  if (yy > doc.page.height - 80) newPage();
  doc.fontSize(11.5).fillColor('#0f3460').font('Helvetica-Bold');
  doc.text(text, 60, yy + 4, { lineBreak: false });
  yy += 22;
}

function para(text) {
  if (yy > doc.page.height - 80) newPage();
  doc.fontSize(9.5).fillColor('#333333').font('Helvetica');
  doc.text(text, 60, yy, { width: doc.page.width - 120, lineBreak: true, align: 'justify' });
  yy += doc.heightOfString(text, { width: doc.page.width - 120 }) + 6;
}

function code(text) {
  const lines = text.split('\n');
  if (yy > doc.page.height - 80) newPage();
  doc.save();
  doc.roundedRect(64, yy + 2, doc.page.width - 128, 3, 1.5).fill('#f0f0f5');
  doc.restore();
  yy += 5;
  for (const cl of lines) {
    if (yy > doc.page.height - 68) newPage();
    doc.font('Courier').fontSize(6.5).fillColor('#2d3436');
    const display = cl.length > 108 ? cl.substring(0, 105) + '...' : cl;
    doc.text(display, 68, yy, { lineBreak: false });
    yy += 9;
  }
  yy += 3;
}

// ============ SECTION 1 ============
h1('一、引言');
h2('1.1 编写目的');
para('本文档详细描述了拼图游戏软件的总体架构设计、模块划分、核心算法及实现方案，为软件著作权申请提供技术文档支撑。本文档适用于软件开发人员、测试人员及相关评审人员。');

h2('1.2 项目概述');
para('本软件是一款基于 Unity引擎开发的拼图游戏，运行于 iOS 和 Android 移动平台。玩家通过拖拽拼图碎片完成图片复原，游戏提供多种难度等级和丰富的关卡设计。核心玩法包括：图片网格切割、碎片随机打乱、拖拽交互、连通组整体移动、自动边框刷新、胜利检测等。');

h2('1.3 术语定义');
para('拼图碎片：构成完整图片的最小单元，每个碎片对应原图的一个矩形区域。\n连通组：在拼图网格中位置相邻且逻辑关系正确的碎片集合，可整体拖拽移动。\n体力：玩家进行游戏所需消耗的资源，随时间自动恢复。\n关卡：拼图的基本单位，包含待拼合的图片及切割参数。\n章节：关卡的集合，同一章节内的关卡共享一张预览图片。\nLuban：配置表工具，将 Excel 配置导出为 JSON 供游戏读取。');

// ============ SECTION 2 ============
h1('二、系统总体设计');
h2('2.1 架构概述');
para('本游戏采用 Unity 引擎组件化架构，分为三大层级：表现层（UI 模块）、逻辑层（核心玩法）、数据层（配置与持久化）。各模块通过自定义事件系统实现松耦合通信，降低模块间依赖。');

para('表现层：负责所有用户界面交互，包括章节选择、游戏主界面、设置界面等，各界面继承自统一的 frmbase 基类。逻辑层：实现拼图核心玩法，包括图片切割、碎片打乱、拖拽交互、连通组检测、位置交换、边框刷新、胜利判定等。数据层：基于 Luban 框架加载 JSON 配置，通过 LitJson 实现玩家数据的本地持久化存储。');

h2('2.2 技术栈');
para('游戏引擎：Unity 2022.3 LTS\n脚本语言：C# (.NET Standard 2.1)\n动画引擎：DOTween\nJSON 序列化：LitJson\n配置表：Luban\nUI 系统：UGUI + TextMeshPro\n目标平台：iOS / Android\n版本管理：Git');

h2('2.3 目录结构');
code(`Assets/
├── Scenes/SampleScene.unity     # 主场景
├── Scripts/
│   ├── Main.cs                  # 入口 + 事件系统
│   ├── card.cs                  # 关卡卡片
│   ├── picmgr.cs                # 拼图管理器
│   ├── datamgr.cs               # 数据管理器
│   ├── PlayerData.cs            # 玩家数据
│   ├── DraggableGridItem.cs     # 拖拽系统
│   ├── BorderSpriteLoader.cs    # 边框加载
│   ├── DebugManager.cs          # 调试管理
│   └── ui/
│       ├── frmbase.cs           # UI 基类
│       ├── frm_chapter.cs       # 章节界面
│       ├── frm_game.cs          # 游戏界面
│       └── frm_setup.cs         # 设置界面
├── Resources/                   # 资源目录
│   ├── Card/                    # 边框贴图
│   └── *.png                    # 关卡图片
└── Plugins/                     # 第三方库`);

// ============ SECTION 3 ============
h1('三、核心功能模块设计');

h2('3.1 主控模块 Main.cs');
para('Main 是游戏启动入口，采用单例模式全局访问。负责：初始化 DOTween 引擎；提供事件注册/派发/注销机制（RegistEvent / DispEvent / UnRegistEvent）；管理游戏暂停/恢复状态；提供关卡重新开始功能（消耗 10 体力）。');
code(`public class Main : MonoBehaviour {
  public static Main inst;
  delegate object registfun(object p);
  static Dictionary<string, List<registfun>> evs = new();

  public static void RegistEvent(string ev, registfun f) {
    if (!evs.ContainsKey(ev)) evs[ev] = new();
    evs[ev].Add(f);
  }
  public static object DispEvent(string ev, object p = null) {
    if (!evs.ContainsKey(ev)) return null;
    foreach (var fn in evs[ev]) { var r = fn(p); if (r != null) return r; }
    return null;
  }
  public static void UnRegistEvent(string ev, registfun f) {
    if (evs.ContainsKey(ev)) evs[ev].Remove(f); }

  void Awake() { inst = this; DOTween.Init();
    RegistEvent("game_restart", p => { RestartLevel(); return null; }); }
  void Start() {
    float bl = (float)Screen.width / Screen.height;
    Screen.SetResolution((int)(1920*bl), 1920, false);
    DispEvent("gamebegin");
  }
  public static void PauseGame() { Time.timeScale = 0; }
  public static void ResumeGame() { Time.timeScale = 1; }
  public static void RestartLevel() {
    if (!PlayerData.gd.hasEnoughpower(10)) return;
    PlayerData.gd.消耗power(10);
  }
}`);

h2('3.2 事件系统');
para('事件系统采用观察者模式，维护以事件名为键、处理函数列表为值的字典。事件被派发时依次调用所有注册函数，支持返回值中断传播。系统中定义的主要事件：gamebegin（游戏开始）、level_play（进入关卡）、level_next（下一关）、level_back（返回）、show_setup（设置）、onLevelChange（关卡变化）、onpowerChange（体力变化）、onChapterChange（章节变化）等。事件驱动各 UI 模块的显示切换和数据刷新。');

h2('3.3 数据管理器 datamgr.cs');
para('datamgr 采用单例模式，基于 Luban 配置表框架加载 JSON 格式配置数据。支持两种加载方式：编辑器中拖拽 TextAsset 直接加载，或从文件系统读取 Assets/data/*.json 文件。提供 GetChapter(id) 和 GetLevel(id) 接口供其他模块获取配置数据。');
code(`public class datamgr : MonoBehaviour {
  public TextAsset tbchapter, tblevel;
  static datamgr _instance;
  public static datamgr Instance {
    get { if (_instance == null) {
        var x = Instantiate(Resources.Load("DataManager")) as GameObject;
        _instance = x.GetComponent<datamgr>(); } return _instance; } }
  public Tables Tables { get; private set; }
  public void LoadConfigTables() {
    Func<string, JSONNode> loader = file => {
      TextAsset ta = file=="tbchapter"?tbchapter:file=="tblevel"?tblevel:null;
      if (ta != null) return JSON.Parse(ta.text);
      string fp = Path.Combine(Application.dataPath, "data", file+".json");
      return File.Exists(fp) ? JSON.Parse(File.ReadAllText(fp)) : null; };
    Tables = new Tables(loader);
  }
  public DrChapter GetChapter(int id) => Tables?.TbChapter?.GetOrDefault(id);
  public DrLevel GetLevel(int id) => Tables?.TbLevel?.GetOrDefault(id);
}`);

h2('3.4 玩家数据 PlayerData.cs');
para('管理玩家游戏进度和资源数据，包括当前关卡、已解锁关卡集合、章节、体力值等。数据以 JSON 格式持久化到 Application.persistentDataPath/playerData.json。体力系统实现自动恢复：每15分钟恢复1点，上限100点，离线期间累积。');
code(`public class GameData {
  JsonData data; opend op;
  public GameData(JsonData d) { data = d; op = new opend(openeddata); }
  internal int levelid {
    get => int.Parse(data.Has("levelid")?data["levelid"].ToString():"100001");
    set { data["levelid"]=value.ToString(); Main.DispEvent("onLevelChange"); } }
  internal int currChapter {
    get => int.Parse(data.Has("currChapter")?data["currChapter"].ToString():"1");
    set { data["currChapter"]=value.ToString(); Main.DispEvent("onChapterChange"); } }
  internal int power {
    get {
      int cur = int.Parse(data.Has("power")?data["power"].ToString():"100");
      long now = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
      long elapsed = now - lastpowerUpdateTime;
      int recv = (int)(elapsed / 900);
      if (recv > 0) { cur = Math.Min(100, cur+recv);
        data["power"] = cur.ToString(); lastpowerUpdateTime = now; }
      return cur; }
    set { data["power"] = Math.Min(100,value).ToString();
      lastpowerUpdateTime = DateTime.Now.Ticks/TimeSpan.TicksPerSecond;
      Main.DispEvent("onpowerChange"); } }
  public bool hasEnoughpower(int c=10) => power >= c;
  public bool 消耗power(int c=10) { if (power>=c) { power-=c; return true; } return false; }
  public bool isOpened(int id) => op.Opened(id);
  public void Open(int id) { op.Open(id); Main.DispEvent("onLevelChange"); }
  JsonData openeddata {
    get { if(!data.Has("opened")||!data["opened"].IsArray)
      { var x=new JsonData(); x.SetJsonType(JsonType.Array); data["opened"]=x; }
      return data["opened"]; } }
}
public class PlayerData : MonoBehaviour {
  public static GameData gd;
  void Awake() { loadData();
    Main.RegistEvent("onLevelChange", x => {
      if(chapterOpend()) gd.currChapter++; saveData(); return 1; });
    Main.RegistEvent("onpowerChange", x => { saveData(); return null; }); }
  void loadData() {
    var pa = Application.persistentDataPath+"/playerData.json";
    if(File.Exists(pa)) try {
      gd = new GameData(JsonMapper.ToObject<JsonData>(File.ReadAllText(pa)));
    } catch { gd = new GameData(new JsonData()); }
    else gd = new GameData(new JsonData()); }
  public void saveData() {
    File.WriteAllText(Application.persistentDataPath+"/playerData.json",
      JsonMapper.ToJson(gd.getData())); }
}`);

h2('3.5 拼图管理器 picmgr.cs');
para('picmgr 是拼图核心管理类，负责：图片按配置切割为网格碎片；碎片 GameObject 创建和 UV 坐标设置；确定性随机打乱算法；九宫格边框动态刷新；胜利检测。是整个游戏最核心的模块。');

para('图片切割：根据 DrLevel 配置的 LevelFigureX（列数）和 LevelFigureY（行数），将原图均匀切割为 width × height 个碎片。每个碎片使用 RawImage 组件，通过 UV 矩形区域控制原图显示区域。碎片使用 GridCell 预制体，锚点设置在左下角（pivot=0,0），使用 anchoredPosition 定位。');

code(`public class picmgr : MonoBehaviour {
  public Texture2D pic; public int width=3, height=3;
  internal static picmgr instance;
  internal float carWid => trans.rect.width/width;
  internal float carHei => trans.rect.height/height;
  RectTransform trans => GetComponent<RectTransform>();

  public IEnumerator LoadLevel(DrLevel lv) {
    width=lv.LevelFigureX; height=lv.LevelFigureY;
    pic = Resources.Load(lv.LevelFigure) as Texture2D;
    yield return StartCoroutine(CreateGridImages(lv.Id,lv.OutOfPlaceNumber,lv.DifficultyTier==2));
  }

  public IEnumerator CreateGridImages(int lv=1, int maxKeep=-1, bool hard=false) {
    clearOld();
    Vector2 sz = trans.rect.size;
    float cw = sz.x/width, ch = sz.y/height;
    float delay = 1f/(width*height);
    for(int x=0;x<width;x++) for(int y=0;y<height;y++) {
      yield return new WaitForSeconds(delay); create(x,y,cw,ch,hard,false); }
    yield return new WaitForSeconds(0.3f);
    ShuffleGridPositions(lv,maxKeep,hard);
    for(int i=0;i<transform.childCount;i++)
      transform.GetChild(i).GetComponent<DraggableGridItem>().Turn();
  }

  void create(int x, int y, float cw, float ch, bool hard, bool bed) {
    var go = Instantiate(Resources.Load("GridCell")) as GameObject;
    go.name = $"GridCell_{x}_{y}"; go.transform.SetParent(transform,false);
    var rt = go.GetComponent<RectTransform>();
    var dg = go.GetComponent<DraggableGridItem>();
    dg.pic = pic; dg.canvas = GetComponentInParent<Canvas>();
    dg.uvX = (float)x/width; dg.uvY = (float)y/height;
    dg.uvWidth = 1f/width; dg.uvHeight = 1f/height;
    dg.PositionIndex = x*height + y;
    rt.anchorMin=rt.anchorMax=rt.pivot=Vector2.zero;
    rt.sizeDelta = new Vector2(cw,ch);
    if(bed) { rt.anchoredPosition=new Vector2(x*cw,y*ch); dg.Turn(true); }
    else rt.anchoredPosition=Vector2.zero;
    CreateBorders(go,cw,ch);
    rt.DOAnchorPos(new Vector2(x*cw,y*ch),0.2f);
  }`);

para('打乱算法（ShuffleGridPositions）：使用确定性随机种子（seed = levelId×1000 + (isHard?1:0)），确保同一关卡每次打乱一致。普通模式保留约 30% 碎片原位；困难模式通过曼哈顿距离贪心优化使碎片尽量远离原位。');

code(`public void ShuffleGridPositions(int lv=1, int maxKeep=-1, bool hard=false) {
  var children = new List<RectTransform>();
  var origPos = new List<Vector2>();
  var origGrid = new List<Vector2Int>();
  foreach(Transform c in transform) {
    var r = c.GetComponent<RectTransform>(); if(r==null) continue;
    children.Add(r); origPos.Add(r.anchoredPosition);
    var p = c.name.Replace("GridCell_","").Split('_');
    if(p.Length==2 && int.TryParse(p[0],out int x) && int.TryParse(p[1],out int y))
      origGrid.Add(new Vector2Int(x,y));
    else origGrid.Add(new Vector2Int(-1,-1)); }
  if(children.Count<2) return;
  var avail = new List<int>(); for(int i=0;i<children.Count;i++) avail.Add(i);
  int seed = lv*1000 + (hard?1:0);
  var rng = new System.Random(seed);
  if(maxKeep==-1) maxKeep=Mathf.Max(0,Mathf.RoundToInt(children.Count*0.3f));
  maxKeep = Mathf.Clamp(maxKeep,0,children.Count-1);
  int keep = rng.Next(0,maxKeep+1);
  var allIdx = new List<int>(); for(int i=0;i<children.Count;i++) allIdx.Add(i);
  var sr = new System.Random(seed+1);
  for(int i=allIdx.Count-1;i>0;i--) { int j=sr.Next(0,i+1);
    (allIdx[i],allIdx[j]) = (allIdx[j],allIdx[i]); }
  var keepIdx = allIdx.GetRange(0,keep);
  foreach(int k in keepIdx) avail.Remove(k);
  var target = new List<int>(avail);
  if(hard && origGrid.Count==children.Count) {
    var dr = new System.Random(seed+3);
    for(int a=0;a<3;a++) for(int i=0;i<target.Count-1;i++) {
      int ci=avail[i], ct=target[i];
      int cd = Mathf.Abs(origGrid[ci].x-origGrid[ct].x)+Mathf.Abs(origGrid[ci].y-origGrid[ct].y);
      for(int j=i+1;j<target.Count;j++) {
        int oi=avail[j], ot=target[j];
        int nd1=Mathf.Abs(origGrid[ci].x-origGrid[ot].x)+Mathf.Abs(origGrid[ci].y-origGrid[ot].y);
        int nd2=Mathf.Abs(origGrid[oi].x-origGrid[ct].x)+Mathf.Abs(origGrid[oi].y-origGrid[ct].y);
        int od=Mathf.Abs(origGrid[oi].x-origGrid[ot].x)+Mathf.Abs(origGrid[oi].y-origGrid[ot].y);
        if(nd1>cd && nd2>od) { (target[i],target[j])=(target[j],target[i]); break; } } } }
  else { var tr = new System.Random(seed+2);
    for(int i=target.Count-1;i>0;i--) { int j=tr.Next(0,i+1);
      (target[i],target[j])=(target[j],target[i]); } }
  for(int i=0;i<children.Count;i++) {
    if(keepIdx.Contains(i)) continue; int idx=avail.IndexOf(i);
    if(idx>=0 && idx<target.Count) children[i].anchoredPosition=origPos[target[idx]]; }
  RefreshAllPositionIndices(); UpdateBorderVisibility();
}`);

para('边框刷新算法是游戏视觉反馈的核心，分为两组独立处理：非拖拽组（参数无参重载）和拖拽组（参数为 List 的重载）。每组只在其组内搜索邻居，确保拖拽中的碎片边框能正确反映组内连接关系。');
code(`public void UpdateBorderVisibility() {
  var all = new List<DraggableGridItem>();
  foreach(Transform c in transform) {
    var item = c.GetComponent<DraggableGridItem>();
    if(item!=null && !item.IsBeingDragged) all.Add(item); }
  if(all.Count>0) UpdateBorderVisibility(all); }

public void UpdateBorderVisibility(List<DraggableGridItem> group) {
  if(group==null||group.Count==0) return;
  Vector2 sz = trans.rect.size;
  float cw=sz.x/width, ch=sz.y/height, th=1.5f;
  foreach(var item in group) {
    var rt = item.GetComponent<RectTransform>(); if(rt==null) continue;
    var p = item.name.Replace("GridCell_","").Split('_');
    if(p.Length!=2) continue; int x=int.Parse(p[0]), y=int.Parse(p[1]);
    var rN=Find(rt.anchoredPosition+new Vector2(cw,0),group,th);
    var lN=Find(rt.anchoredPosition+new Vector2(-cw,0),group,th);
    var tN=Find(rt.anchoredPosition+new Vector2(0,ch),group,th);
    var bN=Find(rt.anchoredPosition+new Vector2(0,-ch),group,th);
    var trN=Find(rt.anchoredPosition+new Vector2(cw,ch),group,th);
    var tlN=Find(rt.anchoredPosition+new Vector2(-cw,ch),group,th);
    var brN=Find(rt.anchoredPosition+new Vector2(cw,-ch),group,th);
    var blN=Find(rt.anchoredPosition+new Vector2(-cw,-ch),group,th);
    bool rc=rN!=null && rN.name==$"GridCell_{x+1}_{y}";
    bool lc=lN!=null && lN.name==$"GridCell_{x-1}_{y}";
    bool tc=tN!=null && tN.name==$"GridCell_{x}_{y+1}";
    bool bc=bN!=null && bN.name==$"GridCell_{x}_{y-1}";
    bool trc=trN!=null && trN.name==$"GridCell_{x+1}_{y+1}";
    bool tlc=tlN!=null && tlN.name==$"GridCell_{x-1}_{y+1}";
    bool brc=brN!=null && brN.name==$"GridCell_{x+1}_{y-1}";
    bool blc=blN!=null && blN.name==$"GridCell_{x-1}_{y-1}";
    item.adjacentRight=rc; item.adjacentLeft=lc;
    item.adjacentTop=tc; item.adjacentBottom=bc;
    SetBorder(item,"RightBorder",!rc); SetBorder(item,"LeftBorder",!lc);
    SetBorder(item,"TopBorder",!tc); SetBorder(item,"BottomBorder",!bc);
    SetBorder(item,"TopLeftBorder",!tc||!lc||!tlc);
    SetBorder(item,"TopRightBorder",!tc||!rc||!trc);
    SetBorder(item,"BottomRightBorder",!bc||!rc||!brc);
    SetBorder(item,"BottomLeftBorder",!bc||!lc||!blc);
  }
}
DraggableGridItem Find(Vector2 p, List<DraggableGridItem> items, float t) {
  foreach(var i in items) { var r=i.GetComponent<RectTransform>();
    if(r!=null && Vector2.Distance(r.anchoredPosition,p)<=t) return i; } return null; }
void SetBorder(GameObject p, string n, bool v) {
  var c=p.transform.Find(n); if(c!=null) c.gameObject.SetActive(v); }`);

para('胜利检测（CheckSucess）：遍历所有碎片，对每个碎片检查四个方向是否存在且名称匹配的邻居。所有碎片的所有方向都正确时判定胜利，触发界面动画并显示下一关按钮。');

h2('3.6 拖拽系统 DraggableGridItem.cs');
para('DraggableGridItem 实现拼图碎片的完整拖拽交互，实现了 IBeginDragHandler、IDragHandler、IEndDragHandler。核心特性包括：连通组整体拖拽（DFS 算法收集正确连接的相邻碎片）、边界限制防止碎片拖出网格、位置交换算法及 DOTween 动画。');

code(`public class DraggableGridItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler {
  public Canvas canvas;
  static int origIdx; public static int targetIdx;
  public int PositionIndex { get; set; }
  public bool IsBeingDragged => isDrag; bool isDrag;
  public static bool isAnyItemDrag = false;
  public bool adjacentLeft,adjacentRight,adjacentTop,adjacentBottom;
  List<DraggableGridItem> dragGroup;
  Dictionary<DraggableGridItem,Vector2> gp;
  Dictionary<DraggableGridItem,int> gs;
  internal Texture2D pic; internal float uvX,uvY,uvWidth,uvHeight;
  public RectTransform rectTr => GetComponent<RectTransform>();
  Transform par => transform.parent;

  static Vector2 origPos => new((origIdx/wid)*cw,(origIdx%hei)*ch);
  static Vector2 tgtPos => new((targetIdx/wid)*cw,(targetIdx%hei)*ch);
  Vector2 curPos => new((PositionIndex/wid)*cw,(PositionIndex%hei)*ch);
  public static float cw => picmgr.instance.carWid;
  public static float ch => picmgr.instance.carHei;
  public static int wid => picmgr.instance.width;
  public static int hei => picmgr.instance.height;

  public void OnBeginDrag(PointerEventData e) {
    if(isAnyItemDrag) return; isAnyItemDrag=true;
    origIdx=PositionIndex; CollectGroup(); isDrag=true;
    if(dragGroup!=null) foreach(var it in dragGroup) it.transform.SetAsLastSibling(); }

  public void OnDrag(PointerEventData e) {
    var pr = transform.parent as RectTransform; if(pr==null) return;
    var rd = (rectTr.anchoredPosition+e.delta/canvas.scaleFactor)-origPos;
    var all = dragGroup ?? new List<DraggableGridItem>(){this};
    float mnX=float.MinValue,mxX=float.MaxValue,mnY=float.MinValue,mxY=float.MaxValue;
    foreach(var item in all) {
      var o = item==this ? origPos : (gp!=null&&gp.TryGetValue(item,out var ov)?ov:item.rectTr.anchoredPosition);
      float iw=item.rectTr.rect.width, ih=item.rectTr.rect.height;
      mnX=Mathf.Max(mnX,-o.x); mxX=Mathf.Min(mxX,pr.rect.width-iw-o.x);
      mnY=Mathf.Max(mnY,-o.y); mxY=Mathf.Min(mxY,pr.rect.height-ih-o.y); }
    var d = new Vector2(Mathf.Clamp(rd.x,mnX,mxX),Mathf.Clamp(rd.y,mnY,mxY));
    rectTr.anchoredPosition = origPos+d;
    if(dragGroup!=null&&gp!=null) foreach(var it in dragGroup) if(it!=this&&gp.TryGetValue(it,out var ov)) it.rectTr.anchoredPosition=ov+d;
    if(picmgr.instance!=null) { picmgr.instance.UpdateBorderVisibility();
      if(dragGroup!=null) picmgr.instance.UpdateBorderVisibility(dragGroup); }
    var df = rectTr.anchoredPosition - curPos;
    if(Mathf.Abs(df.x)>cw/2||Mathf.Abs(df.y)>ch/2) {
      int x=Mathf.RoundToInt(rectTr.anchoredPosition.x/cw), y=Mathf.RoundToInt(rectTr.anchoredPosition.y/ch);
      targetIdx = Mathf.Clamp(x,0,wid)*hei + Mathf.Clamp(y,0,hei); }
    else targetIdx = PositionIndex; }

  public void OnEndDrag(PointerEventData e) {
    if(origIdx!=targetIdx) { bool ok = CheckValid(); if(ok) Swap(tgtPos);
      else ResetAll(); } else ResetAll();
    dragGroup=null; gp=null; gs=null; isDrag=false; }

  void CollectGroup() {
    dragGroup=new(); gp=new(); gs=new();
    var map = new Dictionary<(int,int),DraggableGridItem>();
    foreach(Transform c in par) { var it=c.GetComponent<DraggableGridItem>();
      if(it!=null&&TryParse(c.name,out int cx,out int cy)) map[(cx,cy)]=it; }
    if(!TryParse(name,out int sx,out int sy)) { dragGroup.Add(this); return; }
    void dfs(int x,int y) {
      if(!map.TryGetValue((x,y),out var cur)||dragGroup.Contains(cur)) return;
      dragGroup.Add(cur); gp[cur]=cur.rectTr.anchoredPosition; gs[cur]=cur.transform.GetSiblingIndex();
      if(cur.adjacentLeft) dfs(x-1,y); if(cur.adjacentRight) dfs(x+1,y);
      if(cur.adjacentTop) dfs(x,y+1); if(cur.adjacentBottom) dfs(x,y-1); }
    dfs(sx,sy); }
  bool TryParse(string n, out int x, out int y) { x=y=-1;
    if(!n.StartsWith("GridCell_")) return false;
    var p=n.Replace("GridCell_","").Split('_'); return p.Length==2&&int.TryParse(p[0],out x)&&int.TryParse(p[1],out y); }
  bool CheckValid() { /* check bounds */ return true; }
  void Swap(Vector2 tp) { /* position swap with animation */ }
  void ResetAll() { /* reset to original positions */ }
  public void Turn(bool at=false) { /* flip animation */ }
}`);

h2('3.7 卡片系统 card.cs');
para('card 用于章节选择界面的关卡卡片展示。每张卡片对应一个关卡，显示章节预览图的对应区域。已通关卡片自动执行翻转动画显示内容，未解锁卡片显示背面和关卡编号。');
code(`public class card : MonoBehaviour {
  public Texture2D back; internal int levelid;
  internal float uvX,uvY,uvWidth,uvHeight; internal Texture texture;
  public TextMeshProUGUI level; public bool isTurning=false;
  RawImage ri => GetComponent<RawImage>();
  internal void Load() {
    if(levelid<PlayerData.gd.levelid) {
      if(PlayerData.gd.isOpened(levelid)) {
        ri.uvRect=new Rect(uvX,1-uvY-uvHeight,uvWidth,uvHeight); ri.texture=texture; level.gameObject.SetActive(false); }
      else { DOVirtual.DelayedCall(1f,()=>{ ri.uvRect=new Rect(0,0,1,1); ri.texture=back; isTurning=true;
        transform.DOScaleX(0,.5f).OnComplete(()=>{ ri.uvRect=new Rect(uvX,1-uvY-uvHeight,uvWidth,uvHeight); ri.texture=texture;
          transform.DOScaleX(1,.5f).OnComplete(()=>{ isTurning=false; }); }); PlayerData.gd.Open(levelid); }); } }
    else { ri.uvRect=new Rect(0,0,1,1); ri.texture=back; level.gameObject.SetActive(true); } } }`);

h2('3.8 边框系统 BorderSpriteLoader.cs');
para('BorderSpriteLoader 负责为九宫格边框子对象加载贴图，支持运行时动态切换。贴图从 Resources/Card 目录加载，具有静态缓存避免重复加载。RefreshBasedOnFlags 方法根据四个方向邻居连接状态切换角贴图为角、边或反转角贴图。');

// ============ SECTION 4 ============
h1('四、用户界面设计');
h2('4.1 章节选择界面');
para('章节选择界面以网格形式展示当前章节的所有关卡卡片。每张卡片使用 card 组件显示章节预览图的对应区域。界面顶部显示当前关卡编号和体力值，点击「开始」按钮触发 level_play 事件进入游戏。界面通过事件驱动刷新：gamebegin 首次加载，onLevelChange/onChapterChange 刷新显示。');

h2('4.2 游戏界面');
para('游戏界面是拼图玩法的主舞台。加载关卡后调用 picmgr.LoadLevel 进行图片切割和打乱，碎片以动画方式逐个显示。界面显示关卡编号和难度标签（困难模式有特殊动画提示），提供返回章节、设置、下一关（通关后显示）等按钮。');

h2('4.3 设置界面');
para('设置界面提供游戏初始化功能（删除存档后重启应用）和体力购买（增加 100 体力，测试用途）。所有 UI 窗体继承自 frmbase 基类，实现统一的 show/hide 生命周期管理。');

// ============ SECTION 5 ============
h1('五、数据配置设计');
h2('5.1 章节配置');
para('DrChapter 定义章节数据：Id 章节 ID、ChapterTitle 标题、ChapterFigure 预览图资源名、ChapterFigureX/ChapterFigureY 行列关卡数、NextChapter 下一章、LevelId 关卡 ID 列表。');

h2('5.2 关卡配置');
para('DrLevel 定义关卡参数：Id 关卡 ID、NextLevel 下一关、LevelFigure 图片资源、LevelFigureX/LevelFigureY 切割行列、DifficultyTier 难度（1普通/2困难）、OutOfPlaceNumber 最多原位保留数、MDistanceRange 曼哈顿距离范围。配置通过 Luban 工具从 Excel 导出为 JSON。');

// ============ SECTION 6 ============
h1('六、核心算法说明');
h2('6.1 拼图打乱算法');
para('使用确定性随机种子（levelId×1000 + difficultyFlag），保证同一关卡打乱结果一致。Fisher-Yates 洗牌随机排列碎片位置，普通模式保留约 30% 原位，困难模式通过贪心迭代最大化碎片偏离原位的曼哈顿距离。');

h2('6.2 连通组检测算法');
para('基于 DFS 深度优先搜索：从当前碎片出发，沿 adjacentLeft/Right/Top/Bottom 四个标志位为 true 的方向递归扩展，收集所有相连的碎片形成拖拽组。坐标映射通过碎片名称 GridCell_x_y 解析。');

h2('6.3 位置交换算法');
para('计算拖拽组目标位置 → 识别被覆盖碎片和空出位置 → 建立映射 → DOTween Sequence 并行动画 → 完成回调刷新索引、边框和胜利检测。');

h2('6.4 胜利检测算法');
para('遍历碎片，基于实际 anchoredPosition 检测四个方向是否存在名称匹配的邻居。全部匹配则触发胜利序列。');

h2('6.5 边框刷新算法');
para('分为两组独立刷新：非拖拽组和拖拽组。每组只在组内搜索邻居（位置阈值 1.5px），正确连接隐藏边框，否则显示。角贴图根据连接状态自动切换。');

// ============ SECTION 7 ============
h1('七、运行环境');
para('硬件要求：ARM64/x86_64 1.5GHz+ 处理器、2GB+ 内存、100MB 存储、1280×720+ 分辨率。\n软件要求：iOS 13.0+ / Android 8.0+、Unity 2022.3 LTS、.NET Standard 2.1。\n第三方依赖：DOTween（动画引擎）、LitJson（JSON 序列化）、Luban（配置表框架）。');

// ============ FINISH ============
doc.end();

stream.on('finish', () => {
  const b = fs.readFileSync(outputPath);
  const m = b.toString().match(/\/Type\s*\/Page[^s]/g);
  console.log('PDF generated successfully!');
  console.log('Pages:', m ? m.length : '?');
  console.log('File:', outputPath);
});
stream.on('error', err => console.error('Error:', err));
