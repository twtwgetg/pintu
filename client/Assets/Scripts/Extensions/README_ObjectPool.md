# 对象池系统使用说明

## 概述
本项目实现了通用的对象池系统，用于减少频繁创建和销毁对象带来的性能开销，特别是在需要频繁显示伤害数字、提示信息等场景。

## 组件说明

### 1. ObjectPoolManager<T>
通用对象池管理器，可以池化任何继承自Component的类型。

**主要功能：**
- 自动创建和回收对象
- 支持预加载
- 支持自动扩容
- 支持最大池大小限制

### 2. TipsPoolManager
专门用于管理提示信息(Tips)的对象池管理器。

**主要功能：**
- 管理普通提示和城墙提示两种类型的对象池
- 提供获取和回收提示对象的接口
- 支持运行时初始化

### 3. TipsPoolInitializer
用于在运行时初始化Tips对象池的组件。

**主要功能：**
- 动态创建Tips预制件
- 初始化TipsPoolManager
- 预加载对象池

## 使用方法

### 1. 基本对象池使用
```csharp
// 创建对象池
ObjectPoolManager<GameObject> pool = new ObjectPoolManager<GameObject>(prefab, initialSize, poolParent, autoExpand, maxSize);

// 获取对象
GameObject obj = pool.GetObject();

// 回收对象
pool.ReturnObject(obj);
```

### 2. Tips对象池使用
```csharp
// 获取普通提示对象
GameObject normalTips = TipsPoolManager.Instance.GetNormalTips();

// 获取城墙提示对象
GameObject wallTips = TipsPoolManager.Instance.GetWallTips();

// 回收对象（在动画完成后）
TipsPoolManager.Instance.ReturnNormalTips(tipsObject);
TipsPoolManager.Instance.ReturnWallTips(tipsObject);
```

## 注意事项
1. 对象在回收时会自动重置部分属性（如TextMeshPro的文本内容）
2. 对象池支持最大大小限制，避免内存无限增长
3. 对象池会在对象返回时自动将其设为非激活状态
4. 当池中无可用对象且未达到最大限制时，会自动创建新对象