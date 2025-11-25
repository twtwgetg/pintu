using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
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
    JsonData data;
    public GameData(JsonData _data)
    {
        data = _data;
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
