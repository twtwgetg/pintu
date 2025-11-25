using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

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
    opend op;

    JsonData data;
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
    // Start is called before the first frame update
    public static GameData gd;
    private void Awake()
    {
        loadData();
        Main.RegistEvent("onLevelChange", (x) =>
        {
            saveData();
            return 1;
        });
    }
   
    void loadData()
    {
        var pa = Application.persistentDataPath+"/playerData.json";
        if(File.Exists(pa))
        {
            var json = File.ReadAllText(pa);
            JsonData data = JsonMapper.ToObject<JsonData>(json);
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
