const PDFDocument = require('pdfkit');
const fs = require('fs');

const FONT = 'C:\\Windows\\Fonts\\simhei.ttf';
const TITLE = '拼图游戏';
const VER = 'V1.0';

const doc = new PDFDocument({
  size: 'A4', margins: { top: 56, bottom: 56, left: 56, right: 56 },
  info: { Title: `${TITLE} 软件著作权鉴别材料`, Author: '开发团队' }
});
doc.registerFont('F', FONT);

const outPath = 'D:\\project\\pintu\\拼图游戏_软著鉴别材料.pdf';
const stream = fs.createWriteStream(outPath);
doc.pipe(stream);

let pg = 0;
function hd() {
  pg++;
  doc.save();
  doc.rect(56, 36, doc.page.width - 112, 0.7).fill('#888');
  doc.font('F').fontSize(8).fillColor('#555');
  doc.text(`${TITLE} ${VER}`, 56, 41, { align: 'left', continued: true });
  doc.text(`第 ${pg} 页`, doc.page.width - 56, 41, { align: 'right' });
  doc.rect(56, doc.page.height - 46, doc.page.width - 112, 0.7).fill('#888');
  doc.font('F').fontSize(8).fillColor('#888');
  doc.text('软件设计说明书', 56, doc.page.height - 42, { align: 'center' });
  doc.restore();
}
function np() { doc.addPage(); hd(); }
function h1(t) { np(); doc.font('F').fontSize(14).fillColor('#1a1a2e'); doc.text(t, 56, 70); doc.moveTo(56, 90).lineTo(doc.page.width - 56, 90).stroke('#0f3460'); doc.y = 100; }
function h2(t) { if (doc.y > doc.page.height - 80) np(); doc.font('F').fontSize(11).fillColor('#0f3460'); doc.text(t, 56, doc.y + 3); doc.y += 19; }
function p(t) { if (doc.y > doc.page.height - 80) np(); doc.font('F').fontSize(9.5).fillColor('#333'); doc.text(t, 56, doc.y, { width: doc.page.width - 112, align: 'justify' }); doc.y += doc.heightOfString(t, { width: doc.page.width - 112 }) + 5; }
function cd(t) {
  const lines = t.split('\n');
  if (doc.y > doc.page.height - 80) np();
  doc.save(); doc.roundedRect(60, doc.y + 2, doc.page.width - 120, 3, 1.5).fill('#f0f0f5'); doc.restore();
  doc.y += 5;
  for (const l of lines) {
    if (doc.y > doc.page.height - 66) np();
    doc.font('F').fontSize(6.8).fillColor('#2d3436');
    doc.text(l.length > 110 ? l.substring(0, 107) + '...' : l, 64, doc.y);
    doc.y += 9;
  }
  doc.y += 4;
}

// ==================== COVER ====================
hd();
doc.font('F').fontSize(30).fillColor('#1a1a2e'); doc.text(TITLE, 56, 120, { align: 'center' });
doc.font('F').fontSize(22).fillColor('#16213e'); doc.text('软件设计说明书', 56, 168, { align: 'center' });
doc.moveTo(110, 210).lineTo(doc.page.width - 110, 210).stroke('#0f3460');
doc.font('F').fontSize(13).fillColor('#333'); doc.text(`版本：${VER}`, 56, 238, { align: 'center' }); doc.text('日期：2026年6月', 56, 265, { align: 'center' });
doc.font('F').fontSize(11).fillColor('#666'); doc.text('本文档为拼图游戏软件著作权申请用鉴别材料', 56, 320, { align: 'center' }); doc.text('包含架构设计、模块详细设计及核心算法说明', 56, 340, { align: 'center' });

// ==================== TOC ====================
np(); doc.font('F').fontSize(14).fillColor('#1a1a2e'); doc.text('目录', 56, doc.y, { align: 'center' }); doc.moveTo(120, doc.y + 2).lineTo(doc.page.width - 120, doc.y + 2).stroke('#0f3460'); doc.y += 14;
const toc = ['一、引言','  1.1 编写目的','  1.2 项目概述','  1.3 术语定义','二、系统总体设计','  2.1 架构概述','  2.2 技术栈','  2.3 目录结构','三、核心模块设计','  3.1 主控模块 Main.cs','  3.2 事件系统','  3.3 数据管理器 datamgr.cs','  3.4 玩家数据 PlayerData.cs','  3.5 拼图管理器 picmgr.cs','  3.6 拖拽系统 DraggableGridItem.cs','  3.7 卡片系统 card.cs','  3.8 边框系统 BorderSpriteLoader.cs','四、UI 模块设计','  4.1 基础窗体 frmbase.cs','  4.2 章节选择 frm_chapter.cs','  4.3 游戏界面 frm_game.cs','  4.4 设置界面 frm_setup.cs','五、数据配置','  5.1 章节配置 DrChapter','  5.2 关卡配置 DrLevel','  5.3 Tables 配置集合','六、核心算法','  6.1 拼图打乱算法','  6.2 连通组检测算法','  6.3 位置交换算法','  6.4 胜利检测算法','  6.5 边框刷新算法','七、数据持久化','  7.1 存档系统','  7.2 体力系统','八、运行环境'];
doc.font('F').fontSize(9.5).fillColor('#333');
for (const t of toc) { if (doc.y > doc.page.height - 70) np(); doc.text(t, t.startsWith('  ') ? 72 : 56, doc.y); doc.y += 14; }

// ==================== S1: 引言 ====================
h1('一、引言');
h2('1.1 编写目的');
p('本文档详细描述了拼图游戏软件的总体架构设计、模块划分、核心算法及实现方案，为软件著作权申请提供技术文档支撑。');
h2('1.2 项目概述');
p('本软件是一款基于 Unity 引擎开发的拼图游戏，运行于 iOS 和 Android 移动平台。玩家通过拖拽拼图碎片完成图片复原，游戏提供多种难度等级和关卡设计。核心玩法包括：图片网格切割、碎片随机打乱、拖拽交互、连通组整体移动、自动边框刷新、胜利检测等。');
h2('1.3 术语定义');
p('拼图碎片：构成完整图片的最小单元，每个碎片对应原图的一个矩形区域，具有 UV 坐标和位置索引。\n连通组：在拼图网格中位置相邻且逻辑关系正确的碎片集合，可整体拖拽移动。\n体力：玩家进行游戏所需消耗的资源，随时间自动恢复。\n关卡：拼图的基本单位，包含待拼合的图片及切割参数。\n章节：关卡的集合，同一章节内的关卡共享一张预览图片。');

// ==================== S2: 系统总体设计 ====================
h1('二、系统总体设计');
h2('2.1 架构概述');
p('本游戏采用 Unity 引擎组件化架构，分为三大层级：表现层（UI 模块）、逻辑层（核心玩法）、数据层（配置与持久化）。各模块通过自定义事件系统实现松耦合通信。表现层负责所有用户界面交互，包括章节选择、游戏主界面、设置界面等。逻辑层实现拼图核心玩法，包括图片切割、碎片打乱、拖拽交互、连通组检测、位置交换、边框刷新、胜利判定等。数据层基于 Luban 框架加载 JSON 配置，通过 LitJson 实现玩家数据的本地持久化存储。');
h2('2.2 技术栈');
p('游戏引擎：Unity 2022.3 LTS\n脚本语言：C# (.NET Standard 2.1)\n动画引擎：DOTween\nJSON 序列化：LitJson\n配置表：Luban\nUI 系统：UGUI + TextMeshPro\n目标平台：iOS / Android');
h2('2.3 目录结构');
cd(`Assets/
  Scenes/SampleScene.unity
  Scripts/
    Main.cs                    # 入口 + 事件系统
    card.cs                    # 关卡卡片
    picmgr.cs                  # 拼图管理器(核心)
    datamgr.cs                 # 数据管理器
    PlayerData.cs              # 玩家数据
    DraggableGridItem.cs       # 拖拽系统
    BorderSpriteLoader.cs      # 边框系统
    DebugManager.cs            # 调试管理器
    recttools.cs               # 矩形工具
    goldplay.cs / getpos.cs    # 辅助脚本
    ui/
      frmbase.cs               # UI 基类
      frm_chapter.cs           # 章节界面
      frm_game.cs              # 游戏界面
      frm_setup.cs             # 设置界面
    Config/                    # Luban 生成
      DrChapter.cs / DrLevel.cs / Tables.cs
  Resources/Card/ (lt,t,rt,r,rb,b,lb,l)
  Plugins/DOTween/ + LitJson/`);

// ==================== S3: 核心模块设计 ====================
h1('三、核心模块设计');

h2('3.1 主控模块 Main.cs');
p('Main 是游戏启动入口，单例模式全局访问。职责：初始化 DOTween 引擎；提供事件注册/派发/注销机制（RegistEvent/DispEvent/UnRegistEvent）；管理游戏暂停/恢复状态；提供关卡重新开始功能（消耗 10 体力）。事件系统是模块间通信的核心机制，各模块通过 RegistEvent 注册监听特定事件，通过 DispEvent 触发事件，实现完全解耦。');
cd(`public class Main : MonoBehaviour {
  public static Main inst;
  delegate object registfun(object p);
  static Dictionary<string, List<registfun>> evs = new();

  public static void RegistEvent(string ev, registfun f) {
    if (!evs.ContainsKey(ev)) evs[ev] = new();
    evs[ev].Add(f); }

  public static object DispEvent(string ev, object p = null) {
    if (!evs.ContainsKey(ev)) return null;
    foreach (var fn in evs[ev]) { var r = fn(p); if (r != null) return r; }
    return null; }

  public static void UnRegistEvent(string ev, registfun f) {
    if (evs.ContainsKey(ev)) evs[ev].Remove(f); }

  public static void SendEvent(string v) => DispEvent(v);

  void Awake() {
    inst = this; DOTween.Init();
    RegistEvent("game_restart", p => { RestartLevel(); return null; }); }

  void Start() {
    float bl = (float)Screen.width / Screen.height;
    Screen.SetResolution((int)(1920*bl), 1920, false);
    DispEvent("gamebegin"); }

  // 暂停/恢复
  static bool isPaused = false;
  public static bool IsPaused { get => isPaused; set => isPaused = value; }
  public static void PauseGame() { isPaused = true; Time.timeScale = 0f; }
  public static void ResumeGame() { isPaused = false; Time.timeScale = 1f; }

  public static void RestartLevel() {
    if (!PlayerData.gd.hasEnoughpower(10)) return;
    PlayerData.gd.消耗power(10); }

  public static void InitGame() {
    string path = Application.persistentDataPath;
    if (Directory.Exists(path)) {
      foreach (string f in Directory.GetFiles(path)) File.Delete(f);
      foreach (string d in Directory.GetDirectories(path)) Directory.Delete(d, true); } }
}`);

h2('3.2 事件系统');
p('事件系统采用观察者模式，维护事件名到处理函数列表的映射。处理函数可返回非 null 值中断传播，用于需要返回值的场景（如 level_play 返回 1 表示成功）。系统中主要事件：\n• gamebegin - 游戏开始，触发章节界面\n• level_play(levelId) - 进入关卡，返回 1/0\n• level_next - 下一关\n• level_back - 返回章节\n• show_setup - 设置界面\n• show_next(curlevel) - 显示下一关\n• event_msg(msg) - 消息提示\n• onLevelChange - 关卡进度变化\n• onpowerChange - 体力变化\n• onChapterChange - 章节切换');
cd(`// 事件注册示例 - frm_chapter.cs
Main.RegistEvent("gamebegin", (x) => {
    brushChapterContent(); show(); UpdateStaminaDisplay(); return 1; });

// 事件注册示例 - frm_game.cs
Main.RegistEvent("level_play", (x) => {
    if (!PlayerData.gd.hasEnoughpower(10)) {
        Main.DispEvent("event_msg", "power不足"); return 0; }
    PlayerData.gd.消耗power(10);
    var lv = datamgr.Instance.GetLevel((int)x);
    show(); StartCoroutine(load(lv)); return 1; });

// 触发事件
Main.DispEvent("level_play", PlayerData.gd.levelid);
Main.SendEvent("level_next");`);

h2('3.3 数据管理器 datamgr.cs');
p('采用单例模式，基于 Luban 配置表框架加载 JSON 数据。支持编辑器中拖拽 TextAsset 和运行时读取文件两种方式。提供 GetChapter(id) 和 GetLevel(id) 接口。');
cd(`public class datamgr : MonoBehaviour {
  public TextAsset tbchapter, tblevel;
  static datamgr _instance;
  public static datamgr Instance {
    get { if (_instance == null) {
        var x = Instantiate(Resources.Load("DataManager")) as GameObject;
        _instance = x.GetComponent<datamgr>(); } return _instance; } }
  public Tables Tables { get; private set; }
  public bool IsLoaded { get; private set; } = false;

  void Awake() {
    if (_instance == null) { _instance = this;
      DontDestroyOnLoad(gameObject); LoadConfigTables(); }
    else if (_instance != this) Destroy(gameObject); }

  public void LoadConfigTables() {
    Func<string, JSONNode> loader = file => {
      TextAsset ta = file=="tbchapter" ? tbchapter : file=="tblevel" ? tblevel : null;
      if (ta != null) return JSON.Parse(ta.text);
      string fp = Path.Combine(Application.dataPath, "data", file+".json");
      if (File.Exists(fp)) return JSON.Parse(File.ReadAllText(fp));
      Debug.LogError("配置文件不存在: " + fp); return null; };
    Tables = new Tables(loader);
    IsLoaded = true; }

  public DrChapter GetChapter(int id) => Tables?.TbChapter?.GetOrDefault(id);
  public DrLevel GetLevel(int id) => Tables?.TbLevel?.GetOrDefault(id);
  public List<DrChapter> GetChapters() => Tables?.TbChapter?.DataList;
}`);

h2('3.4 玩家数据 PlayerData.cs');
p('管理玩家游戏进度和资源数据：当前关卡、已解锁关卡集合、当前章节、体力值及自动恢复机制。数据以 JSON 持久化到 persistentDataPath/playerData.json，通过事件自动保存。');
cd(`public class GameData {
  JsonData data;
  public GameData(JsonData d) { data = d; }

  internal int levelid {
    get => int.Parse(data.Has("levelid") ? data["levelid"].ToString() : "100001");
    set { data["levelid"] = value.ToString();
          Main.DispEvent("onLevelChange"); } }

  internal int currChapter {
    get => int.Parse(data.Has("currChapter") ? data["currChapter"].ToString() : "1");
    set { data["currChapter"] = value.ToString();
          Main.DispEvent("onChapterChange"); } }

  internal int power {
    get {
      int cur = int.Parse(data.Has("power") ? data["power"].ToString() : "100");
      long now = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
      long elapsed = now - lastUpdateTime;
      int recv = (int)(elapsed / 900); // 15min per point
      if (recv > 0) { cur = Math.Min(100, cur+recv);
        data["power"] = cur.ToString(); lastUpdateTime = now; }
      return cur; }
    set { data["power"] = Math.Min(100,value).ToString();
      lastUpdateTime = DateTime.Now.Ticks/TimeSpan.TicksPerSecond;
      Main.DispEvent("onpowerChange"); } }

  long lastUpdateTime {
    get => long.Parse(data.Has("lastUpdate") ? data["lastUpdate"].ToString()
      : (DateTime.Now.Ticks/TimeSpan.TicksPerSecond).ToString());
    set => data["lastUpdate"] = value.ToString(); }

  public bool hasEnoughpower(int c=10) => power >= c;
  public bool 消耗power(int c=10) { if(power>=c){power-=c;return true;} return false; }

  public bool isOpened(int id) {
    var arr = data["opened"];
    for(int i=0;i<arr.Count;i++) if(int.Parse(arr[i].ToString())==id) return true;
    return false; }
  public void Open(int id) { data["opened"].Add(id.ToString());
    Main.DispEvent("onLevelChange"); }
  public JsonData getData() => data;
}

public class PlayerData : MonoBehaviour {
  public static GameData gd;
  void Awake() { loadData();
    Main.RegistEvent("onLevelChange", x => { saveData(); return 1; });
    Main.RegistEvent("onpowerChange", x => { saveData(); return null; }); }

  void loadData() {
    var pa = Application.persistentDataPath+"/playerData.json";
    if(File.Exists(pa)) try {
      gd = new GameData(JsonMapper.ToObject<JsonData>(File.ReadAllText(pa))); }
    catch { gd = new GameData(new JsonData()); }
    else gd = new GameData(new JsonData()); }

  public void saveData() {
    File.WriteAllText(Application.persistentDataPath+"/playerData.json",
      JsonMapper.ToJson(gd.getData())); }
}`);

h2('3.5 拼图管理器 picmgr.cs');
p('picmgr 是拼图最核心的管理类，负责图片切割、碎片创建、打乱算法、边框刷新、胜利检测。根据 DrLevel 配置的 LevelFigureX（列）和 LevelFigureY（行），将原图均匀切割。每个碎片加载 GridCell 预制体，设置 RawImage 的 UV 矩形区域显示原图对应部分。锚点设置在左下角（pivot=0,0），使用 anchoredPosition 定位。');
cd(`public class picmgr : MonoBehaviour {
  public Texture2D pic; public int width=3, height=3;
  internal static picmgr instance;
  internal float carWid => trans.rect.width/width;
  internal float carHei => trans.rect.height/height;
  RectTransform trans => GetComponent<RectTransform>();
  DrLevel curLevel;

  void Awake() { instance = this; ResizeChapterContent(); }

  public void ResizeChapterContent() {
    float m=100f, aw=Screen.width-2*m, ah=Screen.height-2*m, tw, th;
    if(aw/ah > 9f/16f) { th=ah; tw=th*9f/16f; }
    else { tw=aw; th=tw*16f/9f; }
    trans.offsetMin=new Vector2((Screen.width-tw)/2,(Screen.height-th)/2);
    trans.offsetMax=new Vector2(-(Screen.width-tw)/2,-(Screen.height-th)/2); }

  public IEnumerator LoadLevel(DrLevel lv) {
    curLevel=lv; width=lv.LevelFigureX; height=lv.LevelFigureY;
    pic = Resources.Load(lv.LevelFigure) as Texture2D;
    if(lv.DifficultyTier==2) Main.DispEvent("event_tips","困难模式");
    yield return StartCoroutine(CreateGridImages(lv.Id,lv.OutOfPlaceNumber,lv.DifficultyTier==2)); }

  public IEnumerator CreateGridImages(int lv=1,int maxKeep=-1,bool hard=false) {
    clearOld(); Vector2 sz=trans.rect.size;
    float cw=sz.x/width, ch=sz.y/height, delay=1f/(width*height);
    for(int x=0;x<width;x++) for(int y=0;y<height;y++) {
      yield return new WaitForSeconds(delay); create(x,y,cw,ch,hard,false); }
    yield return new WaitForSeconds(0.3f);
    ShuffleGridPositions(lv,maxKeep,hard);
    for(int i=0;i<transform.childCount;i++)
      transform.GetChild(i).GetComponent<DraggableGridItem>().Turn(); }

  void create(int x,int y,float cw,float ch,bool hard,bool bed) {
    var go=Instantiate(Resources.Load("GridCell")) as GameObject;
    go.name=$"GridCell_{x}_{y}"; go.transform.SetParent(transform,false);
    var rt=go.GetComponent<RectTransform>();
    go.GetComponent<RawImage>().texture = (Texture)Resources.Load(
      hard?"Card/ui_card_02":"Card/ui_card_01");
    var dg=go.GetComponent<DraggableGridItem>();
    dg.pic=pic; dg.canvas=GetComponentInParent<Canvas>();
    dg.uvX=(float)x/width; dg.uvY=(float)y/height;
    dg.uvWidth=1f/width; dg.uvHeight=1f/height;
    dg.PositionIndex=x*height+y;
    rt.anchorMin=rt.anchorMax=rt.pivot=Vector2.zero;
    rt.sizeDelta=new Vector2(cw,ch);
    if(bed){rt.anchoredPosition=new Vector2(x*cw,y*ch);dg.Turn(true);}
    else rt.anchoredPosition=Vector2.zero;
    CreateBorders(go,cw,ch);
    if(!DebugManager.IsDebugMode) {
      var txt=go.GetComponentInChildren<TextMeshProUGUI>();
      if(txt!=null) Destroy(txt.gameObject); }
    rt.DOAnchorPos(new Vector2(x*cw,y*ch),0.2f); }`);

p('打乱算法（ShuffleGridPositions）：确定性随机种子 seed=levelId*1000+(hard?1:0)，保证同一关卡结果一致。普通模式保留约 30% 原位；困难模式通过曼哈顿距离贪心优化使碎片尽量远离原位。');
cd(`public void ShuffleGridPositions(int lv,int maxKeep,bool hard) {
  var children=new List<RectTransform>();
  var origPos=new List<Vector2>();
  var origGrid=new List<Vector2Int>();
  foreach(Transform c in transform) {
    var r=c.GetComponent<RectTransform>(); if(r==null) continue;
    children.Add(r); origPos.Add(r.anchoredPosition);
    var p=c.name.Replace("GridCell_","").Split('_');
    if(p.Length==2&&int.TryParse(p[0],out int x)&&int.TryParse(p[1],out int y))
      origGrid.Add(new Vector2Int(x,y));
    else origGrid.Add(new Vector2Int(-1,-1)); }
  if(children.Count<2) return;
  var avail=new List<int>(); for(int i=0;i<children.Count;i++) avail.Add(i);
  int seed=lv*1000+(hard?1:0); var rng=new System.Random(seed);
  if(maxKeep==-1) maxKeep=Mathf.Max(0,Mathf.RoundToInt(children.Count*0.3f));
  maxKeep=Mathf.Clamp(maxKeep,0,children.Count-1);
  int keepCt=rng.Next(0,maxKeep+1);
  var ai=new List<int>(); for(int i=0;i<children.Count;i++) ai.Add(i);
  var sr=new System.Random(seed+1);
  for(int i=ai.Count-1;i>0;i--) { int j=sr.Next(0,i+1); (ai[i],ai[j])=(ai[j],ai[i]); }
  var keepIdx=ai.GetRange(0,keepCt);
  foreach(int k in keepIdx) avail.Remove(k);
  var target=new List<int>(avail);
  if(hard&&origGrid.Count==children.Count) {
    var dr=new System.Random(seed+3);
    for(int a=0;a<3;a++) for(int i=0;i<target.Count-1;i++) {
      int ci=avail[i],ct=target[i];
      int cd=MDist(origGrid[ci],origGrid[ct]);
      for(int j=i+1;j<target.Count;j++) {
        int oi=avail[j],ot=target[j];
        if(MDist(origGrid[ci],origGrid[ot])>cd&&
           MDist(origGrid[oi],origGrid[ct])>MDist(origGrid[oi],origGrid[ot]))
        { (target[i],target[j])=(target[j],target[i]); break; } } } }
  else { var tr=new System.Random(seed+2);
    for(int i=target.Count-1;i>0;i--) { int j=tr.Next(0,i+1);
      (target[i],target[j])=(target[j],target[i]); } }
  for(int i=0;i<children.Count;i++) { if(keepIdx.Contains(i)) continue;
    int idx=avail.IndexOf(i);
    if(idx>=0&&idx<target.Count) children[i].anchoredPosition=origPos[target[idx]]; }
  RefreshAllPositionIndices(); UpdateBorderVisibility(); }
int MDist(Vector2Int a,Vector2Int b)=>Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);`);

p('边框创建（CreateBorders）：为每个碎片创建 8 个子对象（4 边 + 4 角），边框宽度 20px。贴图通过 BorderSpriteLoader 从 Resources/Card/ 加载，命名约定 lt/t/rt/r/rb/b/lb/l。');
cd(`void CreateBorders(GameObject go,float cw,float ch) {
  for(int i=go.transform.childCount-1;i>=0;i--) Destroy(go.transform.GetChild(i).gameObject);
  float bw=Mathf.Min(20f,cw/2f,ch/2f);
  B(go,"TopLeftBorder",new Vector2(0,ch-bw),new Vector2(bw,bw));
  B(go,"TopRightBorder",new Vector2(cw-bw,ch-bw),new Vector2(bw,bw));
  B(go,"BottomRightBorder",new Vector2(cw-bw,0),new Vector2(bw,bw));
  B(go,"BottomLeftBorder",new Vector2(0,0),new Vector2(bw,bw));
  B(go,"TopBorder",new Vector2(bw,ch-bw),new Vector2(Mathf.Max(0,cw-2*bw),bw));
  B(go,"BottomBorder",new Vector2(bw,0),new Vector2(Mathf.Max(0,cw-2*bw),bw));
  B(go,"LeftBorder",new Vector2(0,bw),new Vector2(bw,Mathf.Max(0,ch-2*bw)));
  B(go,"RightBorder",new Vector2(cw-bw,bw),new Vector2(bw,Mathf.Max(0,ch-2*bw))); }
void B(GameObject p,string n,Vector2 ap,Vector2 sd) {
  var o=new GameObject(n); o.transform.SetParent(p.transform,false);
  var r=o.AddComponent<RectTransform>(); r.anchorMin=r.anchorMax=r.pivot=Vector2.zero;
  r.anchoredPosition=ap; r.sizeDelta=sd;
  var img=o.AddComponent<Image>(); img.color=Color.white;
  var l=o.AddComponent<BorderSpriteLoader>();
  l.resourceName=n switch{"TopLeftBorder"=>"lt","TopBorder"=>"t","TopRightBorder"=>"rt",
    "RightBorder"=>"r","BottomRightBorder"=>"rb","BottomBorder"=>"b",
    "BottomLeftBorder"=>"lb","LeftBorder"=>"l",_=>n}; l.LoadSprite(); }`);

p('边框刷新（UpdateBorderVisibility）分两组独立处理：非拖拽组和拖拽组。每组只在组内搜索邻居（位置阈值 1.5px），正确连接隐藏边框，角贴图根据连接状态自动切换。');
cd(`public void UpdateBorderVisibility() {
  var all=new List<DraggableGridItem>();
  foreach(Transform c in transform) { var it=c.GetComponent<DraggableGridItem>();
    if(it!=null&&!it.IsBeingDragged) all.Add(it); }
  if(all.Count>0) UpdateBorderVisibility(all); }

public void UpdateBorderVisibility(List<DraggableGridItem> group) {
  if(group==null||group.Count==0) return;
  Vector2 sz=trans.rect.size; float cw=sz.x/width,ch=sz.y/height,th=1.5f;
  foreach(var item in group) {
    var rt=item.GetComponent<RectTransform>(); if(rt==null) continue;
    var p=item.name.Replace("GridCell_","").Split('_');
    if(p.Length!=2) continue;
    int x=int.Parse(p[0]), y=int.Parse(p[1]);
    var rN=F(rt.anchoredPosition+new Vector2(cw,0),group,th);
    var lN=F(rt.anchoredPosition-new Vector2(cw,0),group,th);
    var tN=F(rt.anchoredPosition+new Vector2(0,ch),group,th);
    var bN=F(rt.anchoredPosition-new Vector2(0,ch),group,th);
    var trN=F(rt.anchoredPosition+new Vector2(cw,ch),group,th);
    var tlN=F(rt.anchoredPosition+new Vector2(-cw,ch),group,th);
    var brN=F(rt.anchoredPosition+new Vector2(cw,-ch),group,th);
    var blN=F(rt.anchoredPosition+new Vector2(-cw,-ch),group,th);
    bool rc=rN!=null&&rN.name==$"GridCell_{x+1}_{y}";
    bool lc=lN!=null&&lN.name==$"GridCell_{x-1}_{y}";
    bool tc=tN!=null&&tN.name==$"GridCell_{x}_{y+1}";
    bool bc=bN!=null&&bN.name==$"GridCell_{x}_{y-1}";
    bool trc=trN!=null&&trN.name==$"GridCell_{x+1}_{y+1}";
    bool tlc=tlN!=null&&tlN.name==$"GridCell_{x-1}_{y+1}";
    bool brc=brN!=null&&brN.name==$"GridCell_{x+1}_{y-1}";
    bool blc=blN!=null&&blN.name==$"GridCell_{x-1}_{y-1}";
    item.adjacentRight=rc; item.adjacentLeft=lc; item.adjacentTop=tc; item.adjacentBottom=bc;
    SB(item,"RightBorder",!rc);SB(item,"LeftBorder",!lc);
    SB(item,"TopBorder",!tc);SB(item,"BottomBorder",!bc);
    SB(item,"TopLeftBorder",!tc||!lc||!tlc);SB(item,"TopRightBorder",!tc||!rc||!trc);
    SB(item,"BottomRightBorder",!bc||!rc||!brc);SB(item,"BottomLeftBorder",!bc||!lc||!blc);
    RC(item,"lt",tc,lc,rc,bc,tlc);RC(item,"rt",tc,lc,rc,bc,trc);
    RC(item,"rb",bc,lc,rc,tc,brc);RC(item,"lb",bc,lc,rc,tc,blc);
  } }
DraggableGridItem F(Vector2 p,List<DraggableGridItem> items,float t) {
  foreach(var i in items) { var r=i.GetComponent<RectTransform>();
    if(r!=null&&Vector2.Distance(r.anchoredPosition,p)<=t) return i; } return null; }
void SB(GameObject p,string n,bool v) { var c=p.transform.Find(n); if(c!=null) c.gameObject.SetActive(v); }
void RC(DraggableGridItem item,string c,bool m1,bool m2,bool o1,bool o2,bool d) {
  string k=c; var bl=item.transform.Find(c switch{"lt"=>"TopLeftBorder","rt"=>"TopRightBorder",
    "rb"=>"BottomRightBorder",_=>"BottomLeftBorder"})?.GetComponent<BorderSpriteLoader>();
  switch(c) {
    case"lt":k=m2&&m1&&!d?"lt_revert":!m2&&!m1?"lt":m2&&!m1?"t":!m2&&m1?"l":k;break;
    case"rt":k=o1&&m1&&!d?"rt_revert":!o1&&!m1?"rt":o1&&!m1?"t":!o1&&m1?"r":k;break;
    case"rb":k=o1&&o2&&!d?"rb_revert":!o1&&!o2?"rb":o1&&!o2?"b":!o1&&o2?"r":k;break;
    case"lb":k=m2&&o2&&!d?"lb_revert":!m2&&!o2?"lb":m2&&!o2?"b":!m2&&o2?"l":k;break; }
  if(bl!=null) bl.SetResourceAndLoad(k); }`);

p('胜利检测（CheckSucess）：遍历所有碎片，基于实际 anchoredPosition 检测四个方向是否存在名称匹配的邻居。全部匹配则触发胜利动画，向上移动 100px 后显示下一关按钮。');
cd(`internal void CheckSucess() {
  bool suc = true;
  foreach(Transform child in transform) {
    var c=child.GetComponent<DraggableGridItem>(); if(c==null) continue;
    var p=child.name.Replace("GridCell_","").Split('_');
    if(p.Length!=2) continue; int x=int.Parse(p[0]),y=int.Parse(p[1]);
    DraggableGridItem rN=null,lN=null,tN=null,bN=null;
    var cr=child.GetComponent<RectTransform>();
    foreach(Transform o in transform) { if(o==child) continue;
      var oi=o.GetComponent<DraggableGridItem>(); if(oi==null) continue;
      var d=o.GetComponent<RectTransform>().anchoredPosition-cr.anchoredPosition;
      float th=1f;
      if(Mathf.Abs(d.x-cr.sizeDelta.x)<th&&Mathf.Abs(d.y)<th) rN=oi;
      else if(Mathf.Abs(d.x+cr.sizeDelta.x)<th&&Mathf.Abs(d.y)<th) lN=oi;
      else if(Mathf.Abs(d.y-cr.sizeDelta.y)<th&&Mathf.Abs(d.x)<th) tN=oi;
      else if(Mathf.Abs(d.y+cr.sizeDelta.y)<th&&Mathf.Abs(d.x)<th) bN=oi; }
    if(!((rN==null||rN.name==$"GridCell_{x+1}_{y}")&&
         (lN==null||lN.name==$"GridCell_{x-1}_{y}")&&
         (tN==null||tN.name==$"GridCell_{x}_{y+1}")&&
         (bN==null||bN.name==$"GridCell_{x}_{y-1}"))){suc=false;break;} }
  if(suc) {
    var t=transform.GetComponent<RectTransform>();
    t.DOAnchorPos(new Vector2(t.anchoredPosition.x,t.anchoredPosition.y+100f),0.5f)
     .OnComplete(()=>{
       for(int i=0;i<transform.childCount;i++)
         transform.GetChild(i).GetComponent<DraggableGridItem>().enabled=false;
       Main.DispEvent("show_next",curLevel); }); }
}`);

h2('3.6 拖拽系统 DraggableGridItem.cs');
p('实现拼图碎片的完整拖拽交互（IBeginDragHandler/IDragHandler/IEndDragHandler）。核心特性：连通组整体拖拽（DFS 收集正确连接的碎片）；边界限制防止碎片拖出网格；位置交换算法及 DOTween 动画。');
cd(`public class DraggableGridItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler {
  public Canvas canvas;
  static int origIdx; public static int targetIdx;
  public int PositionIndex { get; set; }
  public bool IsBeingDragged => isD; bool isD;
  public static bool isAnyDragging = false;
  public bool adjL,adjR,adjT,adjB;
  List<DraggableGridItem> grp;
  Dictionary<DraggableGridItem,Vector2> gp;
  Dictionary<DraggableGridItem,int> gs;
  internal Texture2D pic; internal float uvX,uvY,uvW,uvH;
  public RectTransform rt => GetComponent<RectTransform>();
  Transform par => transform.parent;

  Vector2 OP() => new((origIdx/wid)*cw(),(origIdx%hei)*ch());
  Vector2 TP() => new((targetIdx/wid)*cw(),(targetIdx%hei)*ch());
  Vector2 CP() => new((PositionIndex/wid)*cw(),(PositionIndex%hei)*ch());
  float cw()=>picmgr.instance.carWid; float ch()=>picmgr.instance.carHei;
  int wid=>picmgr.instance.width; int hei=>picmgr.instance.height;

  public void OnBeginDrag(PointerEventData e) {
    if(isAnyDragging) return; isAnyDragging=true;
    origIdx=PositionIndex; Collect(); isD=true;
    if(grp!=null) { foreach(var it in grp) {
        if(gs!=null&&!gs.ContainsKey(it)) gs[it]=it.transform.GetSiblingIndex();
        if(gp!=null&&!gp.ContainsKey(it)) gp[it]=it.rt.anchoredPosition;
        it.transform.SetAsLastSibling(); } } }

  public void OnDrag(PointerEventData e) {
    var pr=transform.parent as RectTransform; if(pr==null) return;
    var all=grp??new List<DraggableGridItem>(){this};
    var rd=rt.anchoredPosition+e.delta/canvas.scaleFactor-OP();
    float mnX=float.MinValue,mxX=float.MaxValue,mnY=float.MinValue,mxY=float.MaxValue;
    foreach(var it in all) {
      var o=it==this?OP():(gp!=null&&gp.TryGetValue(it,out var ov)?ov:it.rt.anchoredPosition);
      float iw=it.rt.rect.width,ih=it.rt.rect.height;
      mnX=Mathf.Max(mnX,-o.x); mxX=Mathf.Min(mxX,pr.rect.width-iw-o.x);
      mnY=Mathf.Max(mnY,-o.y); mxY=Mathf.Min(mxY,pr.rect.height-ih-o.y); }
    var d=new Vector2(Mathf.Clamp(rd.x,mnX,mxX),Mathf.Clamp(rd.y,mnY,mxY));
    rt.anchoredPosition=OP()+d;
    if(grp!=null&&gp!=null) foreach(var it in grp)
      if(it!=this&&gp.TryGetValue(it,out var ov)) it.rt.anchoredPosition=ov+d;
    if(picmgr.instance!=null) { picmgr.instance.UpdateBorderVisibility();
      if(grp!=null) picmgr.instance.UpdateBorderVisibility(grp); }
    var df=rt.anchoredPosition-CP();
    if(Mathf.Abs(df.x)>cw()/2||Mathf.Abs(df.y)>ch()/2) {
      int x=Mathf.RoundToInt(rt.anchoredPosition.x/cw());
      int y=Mathf.RoundToInt(rt.anchoredPosition.y/ch());
      targetIdx=Mathf.Clamp(x,0,wid)*hei+Mathf.Clamp(y,0,hei); }
    else targetIdx=PositionIndex; }

  public void OnEndDrag(PointerEventData e) {
    bool ok=false;
    if(origIdx!=targetIdx) ok=CheckValid();
    if(ok) Swap(TP()); else Reset();
    grp=null;gp=null;gs=null;isD=false; }

  bool CheckValid() {
    var list=(grp!=null&&grp.Count>0)?new List<DraggableGridItem>(grp):new List<DraggableGridItem>(){this};
    var pr=transform.parent as RectTransform; if(pr==null) return false;
    var b=new Rect(0,0,pr.rect.width,pr.rect.height);
    foreach(var it in list) {
      var np=it.rt.anchoredPosition+new Vector2(it.rt.sizeDelta.x/2,it.rt.sizeDelta.y/2);
      if(!b.Contains(np)) return false; } return true; }

  void Collect() {
    grp=new();gp=new();gs=new();
    var map=new Dictionary<(int,int),DraggableGridItem>();
    foreach(Transform c in par) { var it=c.GetComponent<DraggableGridItem>();
      if(it!=null&&TPN(c.name,out int cx,out int cy)) map[(cx,cy)]=it; }
    if(!TPN(name,out int sx,out int sy)){grp.Add(this);return;}
    void dfs(int x,int y) {
      if(!map.TryGetValue((x,y),out var cur)||grp.Contains(cur))return;
      grp.Add(cur);gp[cur]=cur.rt.anchoredPosition;gs[cur]=cur.transform.GetSiblingIndex();
      if(cur.adjL) dfs(x-1,y); if(cur.adjR) dfs(x+1,y);
      if(cur.adjT) dfs(x,y+1); if(cur.adjB) dfs(x,y-1); }
    dfs(sx,sy); }
  bool TPN(string n,out int x,out int y){x=y=-1;
    if(!n.StartsWith("GridCell_"))return false;
    var p=n.Replace("GridCell_","").Split('_');return p.Length==2&&int.TryParse(p[0],out x)&&int.TryParse(p[1],out y);}

  void Swap(Vector2 tp) {
    PositionIndex=GetIdx(tp);
    var list=(grp!=null&&grp.Count>0)?new List<DraggableGridItem>(grp):new List<DraggableGridItem>(){this};
    var origs=new Dictionary<DraggableGridItem,Vector2>();
    var sibs=new Dictionary<DraggableGridItem,int>();
    foreach(var it in list){origs[it]=gp!=null&&gp.TryGetValue(it,out var p)?p:it.rt.anchoredPosition;sibs[it]=it.transform.GetSiblingIndex();}
    var src=origs[this]; var tgts=new Dictionary<DraggableGridItem,Vector2>();
    foreach(var it in list) tgts[it]=tp+(origs[it]-src);
    var empty=new List<Vector2>(); foreach(var kvp in origs){bool filled=false;
      foreach(var t in tgts.Values) if(Vector2.Distance(t,kvp.Value)<1f){filled=true;break;}
      if(!filled) empty.Add(kvp.Value);}
    var covered=new List<DraggableGridItem>();
    foreach(Transform c in par){var it=c.GetComponent<DraggableGridItem>();
      if(it==null||list.Contains(it))continue;
      foreach(var t in tgts.Values) if(Vector2.Distance(it.rt.anchoredPosition,t)<1f){covered.Add(it);break;}}
    empty.Sort((a,b)=>{int yc=b.y.CompareTo(a.y);return yc!=0?yc:a.x.CompareTo(b.x);});
    covered.Sort((a,b)=>{int yc=b.rt.anchoredPosition.y.CompareTo(a.rt.anchoredPosition.y);return yc!=0?yc:a.rt.anchoredPosition.x.CompareTo(b.rt.anchoredPosition.x);});
    var repl=new Dictionary<DraggableGridItem,Vector2>();
    for(int i=0;i<Mathf.Min(covered.Count,empty.Count);i++) repl[covered[i]]=empty[i];
    var seq=DOTween.Sequence();
    foreach(var it in list) seq.Join(it.rt.DOAnchorPos(tgts[it],0.25f));
    foreach(var kvp in repl) seq.Join(kvp.Key.rt.DOAnchorPos(kvp.Value,0.25f));
    seq.OnComplete(()=>{
      var all=new List<DraggableGridItem>(list); all.AddRange(covered);
      all.Sort((a,b)=>sibs.GetValueOrDefault(a,0).CompareTo(sibs.GetValueOrDefault(b,0)));
      foreach(var it in all) it.transform.SetAsLastSibling();
      var pm=GetComponentInParent<picmgr>(); if(pm!=null){pm.RefreshAllPositionIndices();pm.UpdateBorderVisibility();pm.CheckSucess();}
      isAnyDragging=false;}); }
  int GetIdx(Vector2 p)=>Mathf.RoundToInt(p.x/cw())*hei+Mathf.RoundToInt(p.y/ch());

  void Reset() {
    if(grp!=null&&gp!=null){var seq=DOTween.Sequence();
      foreach(var kvp in gp){seq.Join(kvp.Key.rt.DOAnchorPos(kvp.Value,0.25f));kvp.Key.PositionIndex=GetIdx(kvp.Value);}
      seq.OnComplete(()=>{foreach(var kvp in gs)kvp.Key.transform.SetSiblingIndex(kvp.Value);
        var pm=GetComponentInParent<picmgr>();if(pm!=null){pm.RefreshAllPositionIndices();pm.UpdateBorderVisibility();}
        isAnyDragging=false;});}
    else{rt.DOAnchorPos(OP(),0.3f).OnComplete(()=>{rt.anchoredPosition=OP();transform.SetSiblingIndex(transform.GetSiblingIndex());
        PositionIndex=origIdx;var pm=GetComponentInParent<picmgr>();if(pm!=null){pm.RefreshAllPositionIndices();pm.UpdateBorderVisibility();}
        isAnyDragging=false;});} }

  public void Turn(bool at=false) {
    if(at) SetTex();
    else{transform.DOScaleX(0,0.25f).OnComplete(()=>{transform.localScale=new Vector3(0,1,1);SetTex();transform.DOScaleX(1,0.25f);});
      float x=transform.localPosition.x,w=rt.rect.width;
      transform.DOLocalMoveX(x+w/2,0.25f).OnComplete(()=>{transform.DOLocalMoveX(x,0.25f);});} }
  void SetTex() { rawImage.color=Color.white; rawImage.uvRect=new Rect(uvX,uvY,uvW,uvH); rawImage.texture=pic; }
  RawImage rawImage=>GetComponent<RawImage>();
}`);

h2('3.7 卡片系统 card.cs');
p('card 用于章节选择界面的关卡卡片展示。根据关卡状态显示不同内容：已通关显示章节预览图对应区域并翻转动画；已解锁未通关显示预览图；未解锁显示背面和关卡编号。');
cd(`public class card : MonoBehaviour {
  public Texture2D back; internal int levelid;
  internal float uvX,uvY,uvW,uvH; internal Texture texture;
  public TextMeshProUGUI level; public bool isTurning=false;
  RawImage ri=>GetComponent<RawImage>();
  internal void Load() {
    if(levelid<PlayerData.gd.levelid) {
      if(PlayerData.gd.isOpened(levelid)) {
        ri.uvRect=new Rect(uvX,1-uvY-uvH,uvW,uvH); ri.texture=texture; level.gameObject.SetActive(false); }
      else { DOVirtual.DelayedCall(1f,()=>{ri.uvRect=new Rect(0,0,1,1);ri.texture=back;isTurning=true;
        transform.DOScaleX(0,.5f).OnComplete(()=>{ri.uvRect=new Rect(uvX,1-uvY-uvH,uvW,uvH);ri.texture=texture;
          transform.DOScaleX(1,.5f).OnComplete(()=>{isTurning=false;});});PlayerData.gd.Open(levelid);});} }
    else { ri.uvRect=new Rect(0,0,1,1); ri.texture=back; level.gameObject.SetActive(true); } } }`);

h2('3.8 边框系统 BorderSpriteLoader.cs');
p('负责为九宫格边框子对象加载贴图，支持运行时动态切换。贴图从 Resources/Card 目录加载，具有静态缓存避免重复加载。RefreshBasedOnFlags 根据方向连接状态切换角贴图（角/边/反转角三种）。');
cd(`public class BorderSpriteLoader : MonoBehaviour {
  public string resourceName; public bool keepColorIfMissing=true;
  static Dictionary<string,Sprite> cache;
  internal void LoadSprite() {
    if(string.IsNullOrEmpty(resourceName)) return;
    var img=GetComponent<Image>(); if(img==null) return;
    if(cache==null) cache=new Dictionary<string,Sprite>();
    if(cache.TryGetValue(resourceName,out var s)){img.sprite=s;img.color=Color.white;}
    else{s=Resources.Load<Sprite>("Card/"+resourceName);
      if(s!=null){img.sprite=s;img.color=Color.white;cache[resourceName]=s;}
      else{img.sprite=null;img.color=keepColorIfMissing?Color.gray:Color.clear;} } }
  public void SetResourceAndLoad(string k){resourceName=k;LoadSprite();}
  internal void RefreshBasedOnFlags(string p,bool t,bool l,bool r,bool b,bool d) {
    string k=resourceName;
    switch(p) {
      case"lt":k=l&&t&&!d?"lt_revert":!l&&!t?"lt":l&&!t?"t":!l&&t?"l":k;break;
      case"rt":k=r&&t&&!d?"rt_revert":!r&&!t?"rt":r&&!t?"t":!r&&t?"r":k;break;
      case"rb":k=r&&b&&!d?"rb_revert":!r&&!b?"rb":r&&!b?"b":!r&&b?"r":k;break;
      case"lb":k=l&&b&&!d?"lb_revert":!l&&!b?"lb":l&&!b?"b":!l&&b?"l":k;break; }
    SetResourceAndLoad(k); } }`);

// ==================== S4: UI 模块 ====================
h1('四、UI 模块设计');
h2('4.1 基础窗体 frmbase.cs');
p('frmbase 是所有 UI 窗体的基类，提供统一 show/hide 生命周期管理。通过 transform.Find("Root") 查找根节点控制显隐，子类可重写 OnShow/OnHide 实现自定义逻辑。');
cd(`public class frmbase : MonoBehaviour {
  protected Transform gb => transform.Find("Root");
  public bool isOpen() => gb.gameObject.activeSelf;
  public void show() { gb.gameObject.SetActive(true); if(Application.isPlaying) OnShow(); }
  public virtual void hide() { OnHide(); gb.gameObject.SetActive(false); }
  protected virtual void OnShow() { } protected virtual void OnHide() { } }`);

h2('4.2 章节选择界面 frm_chapter.cs');
p('以网格形式展示当前章节的所有关卡卡片。通过事件驱动刷新。点击「开始」触发 level_play 进入游戏。显示体力值。');
cd(`public class frm_chapter : frmbase {
  public RectTransform content; public Button btn,btnsetup;
  public TextMeshProUGUI staminaText,levelname;
  void Awake() {
    Main.RegistEvent("gamebegin",x=>{brush();show();UpdateStamina();return 1;});
    Main.RegistEvent("onpowerChange",x=>{UpdateStamina();return null;});
    Main.RegistEvent("level_next",x=>{PlayerData.gd.levelid=datamgr.Instance.GetLevel(PlayerData.gd.levelid).NextLevel;brush();show();return 1;});
    Main.RegistEvent("onChapterChange",x=>{brush();show();return 1;});
    Main.RegistEvent("level_back",x=>{show();return null;});
    btn.onClick.AddListener(()=>{if(!IsTurning()){int r=(int)Main.DispEvent("level_play",PlayerData.gd.levelid);if(r==1)hide();}});
    btnsetup.onClick.AddListener(()=>Main.DispEvent("show_setup")); }
  bool IsTurning(){for(int i=0;i<content.childCount;i++)if(content.GetChild(i).GetComponent<card>().isTurning)return true;return false;}
  void brush(){/*创建关卡卡片网格*/}
  void UpdateStamina(){if(staminaText!=null)staminaText.text=PlayerData.gd.power.ToString();} }`);

h2('4.3 游戏界面 frm_game.cs');
p('承载拼图玩法。加载关卡后调用 picmgr.LoadLevel 切割打乱碎片，显示关卡编号和难度标签。');
cd(`public class frm_game : frmbase {
  public picmgr mgr; public TextMeshProUGUI level;
  public Button next,back,setup;
  void Awake() {
    Main.RegistEvent("level_play",x=>{
      if(!PlayerData.gd.hasEnoughpower(10)){Main.DispEvent("event_msg","power不足");return 0;}
      PlayerData.gd.消耗power(10); next.gameObject.SetActive(false);
      var lv=datamgr.Instance.GetLevel((int)x); level.text=$"Level {lv.Id}";
      if(lv.DifficultyTier==2){/*显示困难标签动画*/}
      show(); StartCoroutine(load(lv)); return 1; });
    Main.RegistEvent("level_next",x=>{hide();return null;});
    next.onClick.AddListener(()=>{Main.SendEvent("level_next");hide();});
    back.onClick.AddListener(()=>{Main.SendEvent("level_back");hide();});
    setup.onClick.AddListener(()=>Main.DispEvent("show_setup")); }
  IEnumerator load(cfg.DrLevel lv){yield return StartCoroutine(mgr.LoadLevel(lv));} }`);

h2('4.4 设置界面 frm_setup.cs');
p('提供游戏初始化（删除存档重启）和体力购买（测试用途，增加 100 体力）功能。');
cd(`public class frm_setup : frmbase {
  public Button btnInit,btnClose,btnBuy;
  void Awake() {
    btnInit.onClick.AddListener(OnInit);
    btnClose.onClick.AddListener(()=>hide());
    btnBuy.onClick.AddListener(()=>PlayerData.gd.power+=100);
    Main.RegistEvent("show_setup",x=>{show();return 1;}); }
  void OnInit() {
    string fp=Application.persistentDataPath+"/playerData.json";
    if(File.Exists(fp)) File.Delete(fp);
    FindObjectOfType<PlayerData>()?.SendMessage("loadData");
    Main.DispEvent("onLevelChange"); Application.Quit(); } }`);

// ==================== S5: 数据配置 ====================
h1('五、数据配置');
h2('5.1 章节配置 DrChapter');
p('Id（章节 ID）、ChapterTitle（标题）、ChapterFigure（预览图资源名）、ChapterFigureX/ChapterFigureY（横向/纵向关卡数）、NextChapter（下一章节）、LevelId（关卡 ID 列表）。通过 Luban 从 Excel 导出为 JSON。');
cd(`public sealed class DrChapter : Luban.BeanBase {
  public readonly int Id, ChapterFigureX, ChapterFigureY, NextChapter;
  public readonly string ChapterTitle, ChapterFigure;
  public readonly List<int> LevelId;
  public DrChapter(JSONNode b) {
    Id=b["id"]; ChapterTitle=b["chapter_title"]; ChapterFigure=b["chapter_figure"];
    ChapterFigureX=b["chapter_figure_x"]; ChapterFigureY=b["chapter_figure_y"];
    NextChapter=b["next_chapter"]; LevelId=new List<int>();
    foreach(JSONNode e in b["level_id"].Children) LevelId.Add(e); } }
// JSON: [{"id":1,"chapter_title":"第一章","chapter_figure":"chapter_01",
//         "chapter_figure_x":3,"chapter_figure_y":2,"next_chapter":2,
//         "level_id":[100001,100002,100003,100004,100005,100006]}]`);

h2('5.2 关卡配置 DrLevel');
p('Id（关卡 ID）、NextLevel（下一关）、LevelFigure（图片资源名）、LevelFigureX/LevelFigureY（切割行列）、DifficultyTier（难度 1普通/2困难）、OutOfPlaceNumber（最多原位保留数）、MDistanceRange（曼哈顿距离范围 [min,max]）。');
cd(`public sealed class DrLevel : Luban.BeanBase {
  public readonly int Id, NextLevel, LevelFigureX, LevelFigureY;
  public readonly int DifficultyTier, OutOfPlaceNumber;
  public readonly string LevelFigure;
  public readonly List<int> MDistanceRange;
  public DrLevel(JSONNode b) {
    Id=b["id"]; NextLevel=b["next_level"]; LevelFigure=b["level_figure"];
    LevelFigureX=b["level_figure_x"]; LevelFigureY=b["level_figure_y"];
    DifficultyTier=b["difficulty_tier"]; OutOfPlaceNumber=b["out_of_place_number"];
    MDistanceRange=new List<int>();
    foreach(JSONNode e in b["m_distance_range"].Children) MDistanceRange.Add(e); } }
// JSON: [{"id":100001,"next_level":100002,"level_figure":"level_01",
//         "level_figure_x":2,"level_figure_y":2,"difficulty_tier":1,
//         "out_of_place_number":1,"m_distance_range":[2,4]}]`);

h2('5.3 Tables 配置集合');
p('Tables 汇聚所有配置表，负责 JSON 加载和跨表引用解析。在 datamgr 初始化时创建，后续通过 datamgr.Instance.Tables 访问。');
cd(`public partial class Tables {
  public TbChapter TbChapter { get; } public TbLevel TbLevel { get; }
  public Tables(Func<string,JSONNode> loader) {
    TbChapter=new TbChapter(loader("tbchapter"));
    TbLevel=new TbLevel(loader("tblevel")); ResolveRef(); }
  void ResolveRef(){TbChapter.ResolveRef(this);TbLevel.ResolveRef(this);} }`);

// ==================== S6: 算法 ====================
h1('六、核心算法');
h2('6.1 拼图打乱算法');
p('确定性种子（levelId*1000 + isHardFlag）保证同一关卡结果一致。Fisher-Yates 洗牌随机排列碎片，普通模式保留约 30% 原位，困难模式通过 3 轮贪心优化最大化曼哈顿距离。');
h2('6.2 连通组检测算法');
p('DFS 从当前碎片出发，沿 adjacent 标志为 true 的方向递归扩展。坐标映射通过碎片名称 GridCell_x_y 解析，构建坐标到碎片的字典。组内碎片保持正确相对位置。');
h2('6.3 位置交换算法');
p('计算拖拽组目标位置 → 识别被覆盖碎片和空出位置 → 按网格顺序排序后建立映射 → DOTween Sequence 并行动画（0.25s）→ 完成回调刷新索引、边框和胜利检测。');
h2('6.4 胜利检测算法');
p('遍历碎片，基于实际 anchoredPosition 检测四方向是否存在名称匹配邻居。全部匹配触发胜利序列。');
h2('6.5 边框刷新算法');
p('分两组独立处理：非拖拽组和拖拽组。每组只在组内搜索邻居（位置阈值 1.5px）。正确连接隐藏边框，角贴图根据连接状态自动切换。');

// ==================== S7: 持久化 ====================
h1('七、数据持久化');
h2('7.1 存档系统');
p('JSON 格式存储于 Application.persistentDataPath/playerData.json。含当前关卡、章节、体力值、上次更新时间戳、已解锁关卡数组。通过事件自动保存。');
h2('7.2 体力系统');
p('上限 100，每关消耗 10。每 15 分钟恢复 1 点，离线累积。LitJson 序列化，onpowerChange 事件驱动 UI 刷新。');

// ==================== S8: 环境 ====================
h1('八、运行环境');
p('硬件：ARM64/x86_64 1.5GHz+、2GB+ RAM、100MB 存储、1280x720+ 分辨率。\n软件：iOS 13.0+ / Android 8.0+、Unity 2022.3 LTS。\n依赖：DOTween、LitJson、Luban。');

doc.end();

stream.on('finish', () => {
  const b = fs.readFileSync(outPath);
  const m = b.toString().match(/\/Type\s*\/Page[^s]/g);
  console.log('Total pages:', m ? m.length : '?');
  fs.writeFileSync('D:\\project\\pintu\\拼图游戏_软著鉴别材料_前30后30.pdf',
    b); // placeholder, will be replaced
});
stream.on('error', err => console.error('Error:', err));
