using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;

public class opend
{
    JsonData _data;
    public opend(JsonData data)
    {
        _data = data;
    }

    internal bool Opened(int id)
    {
        bool ret = false;
        
        for(int i = 0; i < _data.Count; i++)
        {
            if(int.Parse(_data[i].ToString()) == id)
            {
                ret = true;
                break;
            }
        }
        return ret;
    }
    internal void Open(int id)
    {
        if(!Opened(id))
        {
            _data.Add(id.ToString());
        }
    }
}
public class GameData
{
    internal int levelid
    {
        get
        {
            if(data.Has("levelid"))
            {
                return int.Parse( data["levelid"].ToString());
            }
            return 100001;
        }
        set
        {
            data["levelid"] = value.ToString();
            Main.DispEvent("onLevelChange");
        }
    }
    
    // power相关字段和方法
    internal int power
    {
        get
        {
            int currentpower = int.Parse(data.Has("power") ? data["power"].ToString() : "100");
            int maxpower = 100;
            
            // 计算自动恢复的power
            long lastUpdateTime = lastpowerUpdateTime;
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            long elapsedSeconds = now - lastUpdateTime;
            
            int recoveredpower = (int)(elapsedSeconds / (15 * 60)); // 每15分钟恢复1点power
            if (recoveredpower > 0)
            {
                currentpower = Math.Min(maxpower, currentpower + recoveredpower);
                data["power"] = Math.Min(100, currentpower).ToString();
                lastpowerUpdateTime = now;
            }
            
            return currentpower;
        }
        set
        {
            data["power"] = Math.Min(100, value).ToString();
            lastpowerUpdateTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            Main.DispEvent("onpowerChange");
        }
    }
    
    internal long lastpowerUpdateTime
    {
        get
        {
            if(data.Has("lastpowerUpdateTime"))
            {
                return long.Parse(data["lastpowerUpdateTime"].ToString());
            }
            return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        }
        set
        {
            data["lastpowerUpdateTime"] = value.ToString();
        }
    }
    
    // 检查是否有足够的power
    public bool hasEnoughpower(int cost = 10)
    {
        return power >= cost;
    }
    
    // 消耗power
    public bool 消耗power(int cost = 10)
    {
        if(hasEnoughpower(cost))
        {
            power -= cost;
            return true;
        }
        return false;
    }
    
    opend op;

    JsonData data;
    internal int currChapter
    {
        get
        {
            if (data.Has("currChapter"))
            {
                return int.Parse(data["currChapter"].ToString());
            }
            return 1;
        }
        set
        {
            data["currChapter"] = value.ToString();
            Main.DispEvent("onChapterChange");
        }
    }

    public GameData(JsonData _data)
    {
        data = _data;
        op = new opend(openeddata);
    }
    JsonData openeddata
    {
        get
        {
            if (!data.Has("opened"))
            {
                var x = new JsonData();
                x.SetJsonType(JsonType.Array);
                data["opened"] = x;
            }
            if (!data["opened"].IsArray)
            {
                var x = new JsonData();
                x.SetJsonType(JsonType.Array);
                data["opened"] = x;
            }
            return data["opened"];
        }

    }
    public bool isOpened(int id)
    {
        return op.Opened(id);
    }
    public void Open(int id)
    {
        op.Open(id);
        Main.DispEvent("onLevelChange");
    }
    public JsonData getData()
    {
        return data;
    }
}
public class PlayerData : MonoBehaviour
{
    bool chapterOpend()
    {
        bool ret = true;
        int cid = gd.currChapter;
        var sst = datamgr.Instance.GetChapter(cid);
        for (int i = 0; i < sst.LevelId.Count; i++)
        {
            var xt = sst.LevelId[i];
            if (!gd.isOpened(xt))
            {
                ret = false;
                break;
            }
        }
        return ret;
    }
    // Start is called before the first frame update
    public static GameData gd;
    private void Awake()
    {
        loadData();
        Main.RegistEvent("onLevelChange", (x) =>
        {
            if (chapterOpend())
            {
                gd.currChapter++;
            }

            saveData();
            return 1;
        });
        Main.RegistEvent("onpowerChange", (x) =>
        {
            saveData();
            return null;
        });
    }
   
    void loadData()
    {
        var pa = Application.persistentDataPath+"/playerData.json";
        if(File.Exists(pa))
        {
            var json = File.ReadAllText(pa);
            JsonData data = null;
            try
            {
               data = JsonMapper.ToObject<JsonData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                data = new JsonData();
            }
            if(data==null){
                data = new JsonData();
            }
            gd = new GameData(data);
        }
        else
        {
            gd = new GameData(new JsonData());
        }

    }
    public void saveData()
    {
        var pa = Application.persistentDataPath+"/playerData.json";
        var json = JsonMapper.ToJson(gd.getData());
        File.WriteAllText(pa,json);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
