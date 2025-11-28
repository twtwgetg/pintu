# Excel文件配置指南

## ✅ 当前进展
通过修改Defines/table.xml，我们已经成功让Luban找到了：
- ✅ 文件：`fc-g-关卡.xlsx`
- ✅ Sheet：`GameLevel`
- ❌ 问题：GameLevel Sheet中缺少`name`列

## 📋 需要解决的问题
错误信息：`bean:'GameLevel' 缺失 列:'name'`

## 🔧 Excel文件内部结构要求

### GameLevel Sheet必须包含以下结构：

#### 第1行 (##var行)：
```
##var | id | name | battle_scene | next_level | wave_id | card_slot_A | card_slot_B | card_slot_C | main_bgm | battle_bgm
```

#### 第2行 (##type行)：
```
##type | int | string | string | int | int[] | int | int | int | int | int
```

#### 第3行及以后 (数据行)：
```
1 | GameLevel_name_1 | Scenes/BattleScene_2D_1.unity | 2 | [100101,100102] | 1 | 2 | 3 | 1 | 3
2 | GameLevel_name_2 | Scenes/BattleScene_2D_1.unity | 3 | [100101,100102] | 1 | 2 | 3 | 1 | 3
```

### 关键点：
1. **必须有name列** - 这是Luban的硬性要求
2. **第1行必须是##var** - 定义列变量名
3. **第2行必须是##type** - 定义列数据类型
4. **第3行开始是数据** - 实际的配置数据

## 🎯 建议的修改步骤：

### 1. 打开Excel文件
打开：`需求文档及配置表\MiniTemplate\Datas\fc-g-关卡.xlsx`

### 2. 检查GameLevel Sheet
确保GameLevel Sheet的第1行包含name列：
```
##var | id | name | battle_scene | next_level | ...
```

### 3. 如果要添加Wave表
创建一个名为"Wave"的Sheet，包含：
```
##var | id | name | wave_count | monster_ids | ...
##type | int | string | int | int[] | ...
101 | Wave_1 | 5 | [1001,1002] | ...
```

### 4. 保存并测试
保存Excel文件后运行：`.\gen.bat`

## 🚀 添加Wave表的配置

修改完GameLevel后，可以在table.xml中添加Wave表：
```xml
<!-- 关卡表 -->
<table name="TbGameLevel" value="GameLevel" input="fc-g-关卡.xlsx#GameLevel" readSchemaFromFile="true" />
<!-- 波次表 -->
<table name="TbWave" value="Wave" input="fc-g-关卡.xlsx#Wave" readSchemaFromFile="true" />
```

## 📝 期望结果
成功后会生成：
- `tbgamelevel.json` - GameLevel数据
- `tbwave.json` - Wave数据（如果添加了Wave表）
- `GameLevel.cs` - GameLevel类（使用Sheet名称作为类名）
- `TbGameLevel.cs` - GameLevel表管理类
- 更新的`Tables.cs`

## 🔍 验证方法
成功的标志：
- 看到"bye~"信息
- 没有错误信息
- 生成对应的json和cs文件