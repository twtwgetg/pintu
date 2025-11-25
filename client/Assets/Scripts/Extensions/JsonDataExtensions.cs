using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Game.Extensions
{
    /// <summary>
    /// JsonData扩展方法类，为LitJson的JsonData提供安全的数据访问扩展
    /// </summary>
    public static class JsonDataExtensions
    {
        /// <summary>
        /// 检查JsonData对象是否包含指定的键
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <returns>如果包含键则返回true，否则返回false</returns>
        private static bool ContainsKey(this JsonData data, string key)
        {
            if (data == null || !data.IsObject)
                return false;

            // 通过IDictionary接口访问Keys属性
            try
            {
                IDictionary dict = data as IDictionary;
                if (dict != null)
                {
                    foreach (string k in dict.Keys)
                    {
                        if (k == key)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全地获取int值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>int值或默认值</returns>
        public static int GetInt(this JsonData data, string key, int defaultValue = 0)
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsInt)
                        return (int)value;
                    
                    if (value.IsString)
                    {
                        int result;
                        if (int.TryParse(value.ToString(), out result))
                            return result;
                    }
                    
                    if (value.IsDouble)
                        return (int)(double)value;
                    
                    if (value.IsLong)
                        return (int)(long)value;
                    
                    if (value.IsBoolean)
                        return (bool)value ? 1 : 0;
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 安全地获取string值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>string值或默认值</returns>
        public static string GetString(this JsonData data, string key, string defaultValue = "")
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsString)
                        return (string)value;
                    
                    if (value.IsInt)
                        return ((int)value).ToString();
                    
                    if (value.IsLong)
                        return ((long)value).ToString();
                    
                    if (value.IsDouble)
                        return ((double)value).ToString();
                    
                    if (value.IsBoolean)
                        return ((bool)value).ToString();
                        
                    // 对于其他类型，尝试转换为字符串
                    return value.ToString();
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 安全地获取bool值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>bool值或默认值</returns>
        public static bool GetBool(this JsonData data, string key, bool defaultValue = false)
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsBoolean)
                        return (bool)value;
                    
                    if (value.IsInt)
                        return (int)value != 0;
                    
                    if (value.IsLong)
                        return (long)value != 0L;
                    
                    if (value.IsDouble)
                        return Math.Abs((double)value) > double.Epsilon;
                    
                    if (value.IsString)
                    {
                        string str = ((string)value).ToLower();
                        if (str == "true" || str == "1")
                            return true;
                        if (str == "false" || str == "0")
                            return false;
                    }
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 安全地获取float值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>float值或默认值</returns>
        public static float GetFloat(this JsonData data, string key, float defaultValue = 0f)
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsDouble)
                        return (float)(double)value;
                    
                    if (value.IsInt)
                        return (float)(int)value;
                    
                    if (value.IsLong)
                        return (float)(long)value;
                    
                    if (value.IsString)
                    {
                        float result;
                        if (float.TryParse((string)value, out result))
                            return result;
                    }
                    
                    if (value.IsBoolean)
                        return (bool)value ? 1f : 0f;
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 安全地获取double值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>double值或默认值</returns>
        public static double GetDouble(this JsonData data, string key, double defaultValue = 0.0)
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsDouble)
                        return (double)value;
                    
                    if (value.IsInt)
                        return (double)(int)value;
                    
                    if (value.IsLong)
                        return (double)(long)value;
                    
                    if (value.IsString)
                    {
                        double result;
                        if (double.TryParse((string)value, out result))
                            return result;
                    }
                    
                    if (value.IsBoolean)
                        return (bool)value ? 1.0 : 0.0;
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 安全地获取long值，如果转换失败则返回默认值
        /// </summary>
        /// <param name="data">JsonData对象</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>long值或默认值</returns>
        public static long GetLong(this JsonData data, string key, long defaultValue = 0L)
        {
            if (data == null || !data.IsObject) return defaultValue;

            try
            {
                // 检查键是否存在
                if (data.ContainsKey(key))
                {
                    JsonData value = data[key];
                    if (value.IsLong)
                        return (long)value;
                    
                    if (value.IsInt)
                        return (long)(int)value;
                    
                    if (value.IsDouble)
                        return (long)(double)value;
                    
                    if (value.IsString)
                    {
                        long result;
                        if (long.TryParse((string)value, out result))
                            return result;
                    }
                    
                    if (value.IsBoolean)
                        return (bool)value ? 1L : 0L;
                }
            }
            catch
            {
                // 转换失败，返回默认值
            }
            
            return defaultValue;
        }
    }
}