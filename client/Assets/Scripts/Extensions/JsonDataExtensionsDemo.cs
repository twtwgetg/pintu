using UnityEngine;
using LitJson;
using Game.Extensions;

/// <summary>
/// JsonDataExtensions使用示例脚本
/// 演示如何使用扩展方法安全地访问JSON数据
/// </summary>
public class JsonDataExtensionsDemo : MonoBehaviour
{
    void Start()
    {
        // 创建一个示例JSON数据
        string jsonString = @"{
            'id': 1001,
            'name': '张三',
            'age': 25,
            'height': 175.5,
            'isStudent': false,
            'score': 95.5
        }";
        
        // 解析JSON
        JsonData jsonData = JsonMapper.ToObject(jsonString.Replace('\'', '"'));
        
        // 使用扩展方法安全地获取数据
        int id = jsonData.GetInt("id", 0);
        string name = jsonData.GetString("name", "未知");
        int age = jsonData.GetInt("age", 0);
        float height = jsonData.GetFloat("height", 0f);
        bool isStudent = jsonData.GetBool("isStudent", true);
        double score = jsonData.GetDouble("score", 0.0);
        
        // 输出结果
        Debug.Log($"ID: {id}");
        Debug.Log($"姓名: {name}");
        Debug.Log($"年龄: {age}");
        Debug.Log($"身高: {height}cm");
        Debug.Log($"是否学生: {isStudent}");
        Debug.Log($"分数: {score}");
        
        // 测试错误处理 - 访问不存在的键
        int notExistInt = jsonData.GetInt("notExistKey", -1);
        string notExistString = jsonData.GetString("notExistKey", "默认值");
        
        Debug.Log($"不存在的int键值: {notExistInt}");
        Debug.Log($"不存在的string键值: {notExistString}");
        
        // 测试类型不匹配的情况
        string ageAsString = jsonData.GetString("age", "转换失败");
        int heightAsInt = jsonData.GetInt("height", -1);
        
        Debug.Log($"年龄作为字符串: {ageAsString}");
        Debug.Log($"身高作为整数: {heightAsInt}");
    }
}