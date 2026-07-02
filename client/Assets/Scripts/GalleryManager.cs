using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class GalleryInfo
{
    public string id, name, theme, description, cover;
    public int version;
    public string cdnOwner, cdnRepo;
}

public class GalleryChapter
{
    public int id, cols, rows, nextChapter;
    public string title, figure;
    public List<int> levelIds;
}

public class GalleryLevel
{
    public int id, cols, rows, difficulty, outOfPlace, nextLevel;
    public string figure;
    public List<int> distRange;
}

public static class GalleryManager
{
    private static GalleryInfo _info;
    private static Dictionary<int, GalleryChapter> _chapters = new();
    private static Dictionary<int, GalleryLevel> _levels = new();
    private static List<GalleryChapter> _chapterList = new();
    private static string _activeId;
    private static bool _ready;

    public static bool IsReady => _ready;
    public static GalleryInfo Info => _info;
    public static string ActiveId => _activeId;

    private static string PersistentRoot => Application.persistentDataPath + "/galleries";
    private static string StreamingRoot
    {
        get
        {
            string p = Application.streamingAssetsPath + "/galleries";
#if UNITY_ANDROID && !UNITY_EDITOR
            return p; // handled by WWW/UnityWebRequest on Android
#else
            return p;
#endif
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInit()
    {
        Init();
    }

    public static void Init()
    {
        if (_ready) return;
        EnsureDefaultGallery();
        LoadGallery("default");
    }

    private static void EnsureDefaultGallery()
    {
        string src = StreamingRoot + "/default";
        string dst = PersistentRoot + "/default";
        if (Directory.Exists(dst)) return;

        Directory.CreateDirectory(PersistentRoot);
        CopyDirectory(src, dst);
        Debug.Log($"[Gallery] Copied default gallery: {src} → {dst}");
    }

    public static bool LoadGallery(string id)
    {
        string path = $"{PersistentRoot}/{id}/gallery.json";
        if (!File.Exists(path))
        {
            Debug.LogError($"[Gallery] Not found: {path}");
            return false;
        }

        var root = JSON.Parse(File.ReadAllText(path));
        _info = new GalleryInfo
        {
            id = root["id"],
            name = root["name"],
            theme = root["theme"],
            description = root["description"],
            version = root["version"].AsInt,
            cover = root["cover"],
            cdnOwner = root["cdn"]["owner"],
            cdnRepo = root["cdn"]["repo"]
        };

        _chapters.Clear();
        _chapterList.Clear();
        var chArr = root["chapters"].AsArray;
        foreach (JSONNode ch in chArr.Children)
        {
            var c = new GalleryChapter
            {
                id = ch["id"].AsInt,
                title = ch["title"],
                figure = ch["figure"],
                cols = ch["cols"].AsInt,
                rows = ch["rows"].AsInt,
                nextChapter = ch["nextChapter"].AsInt,
                levelIds = new List<int>()
            };
            foreach (JSONNode lid in ch["levels"].AsArray.Children)
                c.levelIds.Add(lid.AsInt);
            _chapters[c.id] = c;
            _chapterList.Add(c);
        }

        _levels.Clear();
        var lvArr = root["levels"].AsArray;
        foreach (JSONNode lv in lvArr.Children)
        {
            var l = new GalleryLevel
            {
                id = lv["id"].AsInt,
                figure = lv["figure"],
                cols = lv["cols"].AsInt,
                rows = lv["rows"].AsInt,
                difficulty = lv["difficulty"].AsInt,
                outOfPlace = lv["outOfPlace"].AsInt,
                nextLevel = lv["nextLevel"].AsInt,
                distRange = new List<int>()
            };
            var dr = lv["distRange"].AsArray;
            if (dr != null)
                foreach (JSONNode d in dr.Children) l.distRange.Add(d.AsInt);
            _levels[l.id] = l;
        }

        _activeId = id;
        _ready = true;
        Debug.Log($"[Gallery] Loaded '{id}' ({_chapterList.Count} chapters, {_levels.Count} levels)");
        return true;
    }

    public static string GetFigureUrl(string figureName)
    {
        if (_info == null) return null;
        return $"https://cdn.jsdelivr.net/gh/{_info.cdnOwner}/{_info.cdnRepo}/{figureName}";
    }

    public static GalleryChapter GetChapter(int id)
    {
        _chapters.TryGetValue(id, out var c);
        return c;
    }

    public static GalleryLevel GetLevel(int id)
    {
        _levels.TryGetValue(id, out var l);
        return l;
    }

    public static List<GalleryChapter> GetChapters() => _chapterList;

    public static int GetChapterCount() => _chapterList.Count;
    public static int GetLevelCount() => _levels.Count;

    private static void CopyDirectory(string src, string dst)
    {
        Directory.CreateDirectory(dst);
        foreach (var f in Directory.GetFiles(src))
            File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), true);
        foreach (var d in Directory.GetDirectories(src))
            CopyDirectory(d, Path.Combine(dst, Path.GetFileName(d)));
    }
}
