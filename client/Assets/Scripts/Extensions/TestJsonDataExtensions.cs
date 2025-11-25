using UnityEngine;
using LitJson;
using Game.Extensions;

/// <summary>
/// 测试JsonDataExtensions扩展方法的脚本
/// </summary>
public class TestJsonDataExtensions : MonoBehaviour
{
    void Start()
    {
        TestJsonDataExtensionsMethods();
    }
    
    void TestJsonDataExtensionsMethods()
    {
        Debug.Log("=== 开始测试JsonDataExtensions ===");
        
        // 测试正常的JSON数据
        string testJson1 = @"{
            'intVal': 42,
            'stringVal': 'Hello World',
            'boolVal': true,
            'floatVal': 3.14,
            'doubleVal': 2.718281828,
            'longVal': 123456789012345
        }";
        
        JsonData data1 = JsonMapper.ToObject(testJson1.Replace('\'', '"'));
        
        // 测试正常获取
        int intVal = data1.GetInt("intVal", -1);
        string stringVal = data1.GetString("stringVal", "default");
        bool boolVal = data1.GetBool("boolVal", false);
        float floatVal = data1.GetFloat("floatVal", -1f);
        double doubleVal = data1.GetDouble("doubleVal", -1.0);
        long longVal = data1.GetLong("longVal", -1L);
        
        Debug.Log($"正常获取测试:");
        Debug.Log($"  intVal: {intVal} (期望: 42)");
        Debug.Log($"  stringVal: {stringVal} (期望: Hello World)");
        Debug.Log($"  boolVal: {boolVal} (期望: True)");
        Debug.Log($"  floatVal: {floatVal} (期望: 3.14)");
        Debug.Log($"  doubleVal: {doubleVal} (期望: 2.718281828)");
        Debug.Log($"  longVal: {longVal} (期望: 123456789012345)");
        
        // 测试默认值
        int defaultInt = data1.GetInt("nonExistentKey", 999);
        string defaultString = data1.GetString("nonExistentKey", "默认字符串");
        bool defaultBool = data1.GetBool("nonExistentKey", true);
        
        Debug.Log($"默认值测试:");
        Debug.Log($"  defaultInt: {defaultInt} (期望: 999)");
        Debug.Log($"  defaultString: {defaultString} (期望: 默认字符串)");
        Debug.Log($"  defaultBool: {defaultBool} (期望: True)");
        
        // 测试类型转换
        string intAsString = data1.GetString("intVal", "转换失败");
        int stringAsInt = data1.GetInt("stringVal", -1);
        bool intAsBool = data1.GetBool("intVal", false);
        
        Debug.Log($"类型转换测试:");
        Debug.Log($"  intAsString: {intAsString} (期望: 42)");
        Debug.Log($"  stringAsInt: {stringAsInt} (期望: -1)");
        Debug.Log($"  intAsBool: {intAsBool} (期望: True)");
        
        // 测试null数据
        JsonData nullData = null;
        int nullInt = nullData.GetInt("anyKey", 123);
        Debug.Log($"null数据测试:");
        Debug.Log($"  nullInt: {nullInt} (期望: 123)");
        
        Debug.Log("=== 测试完成 ===");
    }
}