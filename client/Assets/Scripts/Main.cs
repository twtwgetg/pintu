using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class Main : MonoBehaviour
{
    public delegate object registfun(object parm);
    // Start is called before the first frame update
    public static Main inst;
    public Material matboader;
    // 添加游戏暂停状态变量
    private static bool isGamePaused = false;
    
    // CDN 图片下载
    private static string imagesCacheDir = null;
    private const string FallbackImageBaseUrl = "https://www.haoyouqu.net/api/pintu-res/image/";
    private const int ImageDownloadTimeoutSeconds = 12;
    private const int CdnDownloadRetryCount = 1;
    private const int FallbackDownloadRetryCount = 2;

    /// <summary>
    /// 获取图片缓存目录，不存在则创建
    /// </summary>
    private static string GetImagesCacheDir()
    {
        if (string.IsNullOrEmpty(imagesCacheDir))
        {
            imagesCacheDir = Path.Combine(Application.persistentDataPath, "images");
            if (!Directory.Exists(imagesCacheDir))
            {
                Directory.CreateDirectory(imagesCacheDir);
                Debug.Log($"Created images cache directory: {imagesCacheDir}");
            }
        }
        return imagesCacheDir;
    }

    /// <summary>
    /// 根据 URL 生成缓存文件路径
    /// </summary>
    private static string GetCacheFilePath(string url)
    {
        // 使用 URL 的哈希作为文件名，避免路径过长或特殊字符问题
        byte[] hashBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(url));
        string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        // 根据 URL 后缀判断文件扩展名
        string ext = ".png"; // 默认 png
        if (url.ToLower().EndsWith(".jpg") || url.ToLower().EndsWith(".jpeg"))
            ext = ".jpg";
        else if (url.ToLower().EndsWith(".webp"))
            ext = ".webp";
        else if (url.ToLower().EndsWith(".gif"))
            ext = ".gif";

        return Path.Combine(GetImagesCacheDir(), hash + ext);
    }

    /// <summary>
    /// 从本地文件加载 Texture2D
    /// </summary>
    private static Texture2D LoadTextureFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            byte[] data = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(texture, data))
            {
                Debug.Log($"Texture loaded from disk cache: {filePath}");
                return texture;
            }
            else
            {
                Debug.LogWarning($"Failed to decode image from disk cache: {filePath}");
                Destroy(texture);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading texture from disk: {filePath} - {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 保存图片数据到本地缓存
    /// </summary>
    private static void SaveTextureToDisk(string url, byte[] data)
    {
        try
            {
            string filePath = GetCacheFilePath(url);
            File.WriteAllBytes(filePath, data);
            Debug.Log($"Texture saved to disk cache: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving texture to disk: {url} - {e.Message}");
        }
        }

    private static string GetFallbackImageUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        string pathPart = url;
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            pathPart = uri.AbsolutePath;
        }

        int slashIndex = pathPart.LastIndexOf('/');
        string fileName = slashIndex >= 0 ? pathPart.Substring(slashIndex + 1) : pathPart;
        if (string.IsNullOrEmpty(fileName)) return null;

        for (int i = 0; i < fileName.Length; i++)
        {
            char c = fileName[i];
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '-' && c != '.')
            {
                return null;
            }
        }

        string lowerName = fileName.ToLower();
        if (!lowerName.EndsWith(".png") && !lowerName.EndsWith(".jpg") && !lowerName.EndsWith(".jpeg") && !lowerName.EndsWith(".webp"))
        {
            return null;
        }

        return FallbackImageBaseUrl + UnityWebRequest.EscapeURL(fileName);
    }

    private static bool TryCreateTextureFromBytes(byte[] data, string cacheKey, out Texture2D texture)
    {
        texture = null;
        if (data == null || data.Length == 0) return false;

        Texture2D loadedTexture = new Texture2D(2, 2);
        if (ImageConversion.LoadImage(loadedTexture, data))
        {
            SaveTextureToDisk(cacheKey, data);
            texture = loadedTexture;
            return true;
        }

        Destroy(loadedTexture);
        return false;
    }

    private static IEnumerator LoadTextureFromFallback(string fallbackUrl, string cacheKey, System.Action<Texture2D> callback)
    {
        if (string.IsNullOrEmpty(fallbackUrl))
        {
            callback?.Invoke(null);
            yield break;
        }

        for (int attempt = 1; attempt <= FallbackDownloadRetryCount; attempt++)
        {
            Debug.Log($"Downloading texture from fallback ({attempt}/{FallbackDownloadRetryCount}): {fallbackUrl}");
            using (var www = UnityWebRequest.Get(fallbackUrl))
            {
                www.timeout = ImageDownloadTimeoutSeconds;
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Fallback texture load failed ({attempt}/{FallbackDownloadRetryCount}): {fallbackUrl} - {www.error}");
                    continue;
                }

                if (TryCreateTextureFromBytes(www.downloadHandler.data, cacheKey, out Texture2D texture))
                {
                    callback?.Invoke(texture);
                    Debug.Log($"Fallback texture loaded and cached to disk: {fallbackUrl}");
                    yield break;
                }

                Debug.LogWarning($"Fallback texture load failed ({attempt}/{FallbackDownloadRetryCount}): {fallbackUrl} - failed to decode image data");
            }
        }

        Debug.LogError($"Texture load failed after fallback retries: {fallbackUrl}");
        callback?.Invoke(null);
    }

    /// <summary>
    /// 从 CDN URL 异步加载 Texture2D，带磁盘缓存（使用 UnityWebRequest）
    /// 优先级：磁盘缓存 > CDN 下载
    /// 不保留内存缓存，加载完由调用方自行管理生命周期
    /// </summary>
    public static IEnumerator LoadTextureFromCDN(string url, System.Action<Texture2D> callback)
    {
        // 1. 从磁盘缓存加载
        string cacheFilePath = GetCacheFilePath(url);
        Texture2D diskTexture = LoadTextureFromFile(cacheFilePath);
        if (diskTexture != null)
        {
            callback?.Invoke(diskTexture);
            yield break;
        }

        // 2. 从 CDN 下载（使用 UnityWebRequest）
        for (int attempt = 1; attempt <= CdnDownloadRetryCount; attempt++)
        {
            Debug.Log($"Downloading texture from CDN ({attempt}/{CdnDownloadRetryCount}): {url}");
            using (var www = UnityWebRequest.Get(url))
            {
            www.timeout = ImageDownloadTimeoutSeconds;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"CDN texture load failed ({attempt}/{CdnDownloadRetryCount}): {url} - {www.error}");
                continue;
            }

            byte[] data = www.downloadHandler.data;
            Texture2D texture = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(texture, data))
            {
                // 保存到磁盘缓存
                SaveTextureToDisk(url, data);
                callback?.Invoke(texture);
                Debug.Log($"CDN texture loaded and cached to disk: {url}");
                yield break;
            }
            else
            {
                Debug.LogWarning($"CDN texture load failed ({attempt}/{CdnDownloadRetryCount}): {url} - failed to decode image data");
                continue;
            }
        }
        }
    
    // 添加公共属性来访问暂停状态
        yield return LoadTextureFromFallback(GetFallbackImageUrl(url), url, callback);
    }

    public static bool IsPaused
    {
        get { return isGamePaused; }
        set
        {
            isGamePaused = value;
        }
    }

    internal static void SendEvent(string v)
    {
        DispEvent(v);
    }

    public static void InitGame()
    {
        try
        {
            // 获取持久化数据路径
            string persistentDataPath = Application.persistentDataPath;

            // 检查目录是否存在
            if (Directory.Exists(persistentDataPath))
            {
                // 删除目录下的所有文件
                string[] files = Directory.GetFiles(persistentDataPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                    Debug.Log("已删除文件: " + file);
                }

                // 删除目录下的所有子目录
                string[] directories = Directory.GetDirectories(persistentDataPath);
                foreach (string directory in directories)
                {
                    Directory.Delete(directory, true);
                    Debug.Log("已删除目录: " + directory);
                }

                Debug.Log("游戏数据初始化完成，所有持久化数据已清除。");
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("初始化完成", "游戏数据已成功初始化，所有持久化数据已清除。", "确定");
#endif
            }
            else
            {
                Debug.LogWarning("持久化数据目录不存在: " + persistentDataPath);
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("初始化失败", "持久化数据目录不存在。", "确定");
#endif
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("初始化游戏数据时发生错误: " + e.Message);
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("初始化失败", "初始化过程中发生错误，请查看控制台日志。", "确定");
#endif
        }
    }

    // 当前关卡和波次信息
    private static int currentLevel = 1;
    private static int currentWave = 0;
    
    // 添加技能待释放状态标记
    private static bool isSkillPending = false;
    
    // 点击效果预制体
    public GameObject clickEffectPrefab; 
    private void Awake()
    {
        inst = this;
        
        // 初始化DOTween
        DOTween.Init();
        Debug.Log("Main.Awake: DOTween已初始化");
        
        // 注册游戏重新开始事件
        RegistEvent("game_restart", (object parm) =>
        {
            RestartLevel();
            return null;
        });
    }
    
    void Start()
    {
#if UNITY_WEBGL
#else
        float bl = ((float)Screen.width) / Screen.height;
        Screen.SetResolution((int)(1280*bl),1280,false);
#endif
        DispEvent("login_begin");
    }
    
    static Dictionary<string, List<registfun>> evs = new();
    public static object DispEvent(string ev, object parm = null)
    {

        if (evs.ContainsKey(ev))
        {
            for (int i = 0; i < evs[ev].Count; i++)
            {
                var p = evs[ev][i](parm);
                if (p != null)
                {
                    return p;
                }
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
        if (evs.ContainsKey(ev))
        {
            //Debug.LogError("已经注册消息" + ev);
        }
        else
        {
            evs[ev] = new List<registfun>();
        }
        evs[ev].Add(fun);
    }
    public static void UnRegistEvent(string ev, registfun fun)
    {
        if (evs.ContainsKey(ev))
        {
            evs[ev].Remove(fun);
        }
    }
    
    // 添加暂停游戏的方法
    public static void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("游戏已暂停");
    }
    
    // 添加恢复游戏的方法
    public static void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("游戏已恢复");
    }
    
    // 添加获取游戏暂停状态的方法
    public static bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    // 添加重新开始关卡的方法
    public static void RestartLevel()
    {
        Debug.Log("重新开始关卡");
        
        // 检查并消耗power
        if (!PlayerData.gd.hasEnoughpower(10))
        {
            Debug.Log("power不足，无法重新开始游戏");
            // 可以在这里添加power不足的提示
            return;
        }
        
        // 消耗10点power
        PlayerData.gd.消耗power(10);
      
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }
}
