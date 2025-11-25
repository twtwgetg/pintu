using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用对象池管理器
/// </summary>
/// <typeparam name="T">要池化的对象类型</typeparam>
public class ObjectPoolManager<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform poolParent;
    private readonly Queue<T> pooledObjects;
    private readonly bool autoExpand;
    private readonly int maxSize;
    private int currentCount;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="prefab">要池化的预制体</param>
    /// <param name="initialSize">初始池大小</param>
    /// <param name="poolParent">池中对象的父物体</param>
    /// <param name="autoExpand">是否自动扩容</param>
    /// <param name="maxSize">最大池大小（-1表示无限制）</param>
    public ObjectPoolManager(T prefab, int initialSize, Transform poolParent = null, bool autoExpand = true, int maxSize = -1)
    {
        this.prefab = prefab;
        this.poolParent = poolParent;
        this.autoExpand = autoExpand;
        this.maxSize = maxSize;
        this.pooledObjects = new Queue<T>();
        this.currentCount = 0;

        // 创建初始对象
        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    /// <summary>
    /// 从对象池中获取一个对象
    /// </summary>
    /// <returns>对象实例</returns>
    public T GetObject()
    {
        T obj;
        if (pooledObjects.Count > 0)
        {
            obj = pooledObjects.Dequeue();
        }
        else if (autoExpand && (maxSize == -1 || currentCount < maxSize))
        {
            obj = CreateObject();
        }
        else
        {
            return null;
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 将对象返回到对象池
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void ReturnObject(T obj)
    {
        if (obj == null) return;

        // 如果池已满且有大小限制，则销毁对象
        if (maxSize != -1 && currentCount > maxSize)
        {
            Object.Destroy(obj.gameObject);
            currentCount--;
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(poolParent);
        pooledObjects.Enqueue(obj);
    }

    /// <summary>
    /// 创建新对象并加入池中
    /// </summary>
    /// <returns>新创建的对象</returns>
    private T CreateObject()
    {
        T obj = Object.Instantiate(prefab);
        obj.gameObject.SetActive(false);
        if (poolParent != null)
        {
            obj.transform.SetParent(poolParent);
        }
        pooledObjects.Enqueue(obj);
        currentCount++;
        return obj;
    }

    /// <summary>
    /// 预加载指定数量的对象
    /// </summary>
    /// <param name="count">预加载数量</param>
    public void Preload(int count)
    {
        int spaceAvailable = maxSize == -1 ? count : Mathf.Min(count, maxSize - currentCount);
        for (int i = 0; i < spaceAvailable; i++)
        {
            CreateObject();
        }
    }

    /// <summary>
    /// 获取当前池中可用对象数量
    /// </summary>
    public int AvailableCount => pooledObjects.Count;
    
    /// <summary>
    /// 获取当前池中总对象数量
    /// </summary>
    public int TotalCount => currentCount;
}

/// <summary>
/// GameObject专用对象池管理器
/// </summary>
public class GameObjectPoolManager
{
    private readonly GameObject prefab;
    private readonly Transform poolParent;
    private readonly Queue<GameObject> pooledObjects;
    private readonly bool autoExpand;
    //private readonly int maxSize;
    private int currentCount;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="prefab">要池化的预制体</param>
    /// <param name="initialSize">初始池大小</param>
    /// <param name="poolParent">池中对象的父物体</param>
    /// <param name="autoExpand">是否自动扩容</param>
    /// <param name="maxSize">最大池大小（-1表示无限制）</param>
    public GameObjectPoolManager(GameObject prefab, int initialSize, Transform poolParent = null, bool autoExpand = true, int maxSize = -1)
    {
        this.prefab = prefab;
        this.poolParent = poolParent;
        this.autoExpand = autoExpand;
        //this.maxSize = maxSize;
        this.pooledObjects = new Queue<GameObject>();
        this.currentCount = 0;

        // 创建初始对象
        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    /// <summary>
    /// 从对象池中获取一个对象
    /// </summary>
    /// <returns>对象实例</returns>
    public GameObject GetObject()
    {
        GameObject obj;
        if (pooledObjects.Count > 0)
        {
            obj = pooledObjects.Dequeue();
        }
        else
        {
            obj = CreateObject();
        }

        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 将对象返回到对象池
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        // 如果池已满且有大小限制，则销毁对象
        //if (maxSize != -1 && currentCount > maxSize)
        //{
        //    Object.Destroy(obj);
        //    currentCount--;
        //    return;
        //}

        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        pooledObjects.Enqueue(obj);
    }

    /// <summary>
    /// 创建新对象并加入池中
    /// </summary>
    /// <returns>新创建的对象</returns>
    private GameObject CreateObject()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.SetActive(false);
        if (poolParent != null)
        {
            obj.transform.SetParent(poolParent);
        }
        pooledObjects.Enqueue(obj);
        currentCount++;
        return obj;
    }

    /// <summary>
    /// 预加载指定数量的对象
    /// </summary>
    /// <param name="count">预加载数量</param>
    public void Preload(int count)
    {
        int spaceAvailable = count  ;
        for (int i = 0; i < spaceAvailable; i++)
        {
            CreateObject();
        }
    }

    /// <summary>
    /// 获取当前池中可用对象数量
    /// </summary>
    public int AvailableCount => pooledObjects.Count;
    
    /// <summary>
    /// 获取当前池中总对象数量
    /// </summary>
    public int TotalCount => currentCount;
}