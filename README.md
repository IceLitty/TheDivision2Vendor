# TheDivision2Vendor
该工具可以获取到每周商人的内容并根据内置文本翻译成中文，并加以注释。

下载请点击右侧或此处的 [Release](https://github.com/IceLitty/TheDivision2Vendor/releases) 标签，选择最新版本即可：

- `D2Vendor.zip` 标准软件包版本，仅提供软件本身，通用库请看 [v1.13](https://github.com/IceLitty/TheDivision2Vendor/releases/tag/v1.13) 更新内容。

- `D2Vendor_Full.zip` 完整打包版本，提供包含运行时环境的所有内容，任何Windows-x86/64系统环境皆可运行。

操作方式包含在内置说明里。

每周数据来源：https://rubenalamina.mx/

当前匹配游戏版本：1.10

## 功能列表：

- 从数据源获取每周商人数据并翻译（并存档，以供查看历史数据）

- 格式化输出商人数据内容（装备、武器、模组、所在槽位、数值上限与当前占比）

- 根据特定条件输出仅符合条件的“推荐道具”

- 输出全部的天赋/装备组信息

- 软件自检查更新与检查游戏服务器健康状况

- 提供在标题栏显示每周刷新剩余时间、隐藏商人出现状态及剩余时间

![最佳筛选功能](https://user-images.githubusercontent.com/6522057/85621302-a69f5c00-b697-11ea-9b68-0d5d389bfd52.png)
![第三方命令行兼容](https://user-images.githubusercontent.com/6522057/85621297-a4d59880-b697-11ea-993e-70764ff8a7f0.png)

## 设置文件：

key | 功用 | 默认值 | 可选值
---|---|---|---
`checkUpdate` | 软件是否自检查更新 | `true` | `true/false`
`checkServerStatus` | 每分钟是否刷新监测游戏服务器状态 | `true` | `true/false`
`checkTransUpdateDates` | 翻译文件更新频率，负数为关闭，0为每次都检查，该值表示间隔天数 | `1` | 任意整数
`barLength` | 数值条方块字符长度 | `1` | 任意数值，建议范围`1`~`2`
`bestFilterThreshold` | 最佳属性装备筛选最低百分比条件 | `0.95` | 任意数值，可选范围`0`~`1`
`bestFilterUpToMax` | 筛选备选条件，当数值距上限低于此数则判断通过 | `-1` | 任意数值，负数关闭此功能
`bestFilterUpToMaxPercent` | 筛选备选条件，当数值(百分比)距上限低于此数则判断通过 | `1` | 任意数值，负数关闭此功能
`ignoreSetsMainAttrIsUtility` | 是否忽略装备组主属性为电时的推荐 | `true` | `true/false`

## Todo List：

- [ ] 长期：引入所有的装备组特殊天赋名称和汉化。

- [ ] 寻找新的TU10模组上限数值，现数值已发现多处不匹配。

## 目前已知bug：

- 放大窗口后再缩小窗口会发现每行最右侧有文本残留，这是因为：1.首先确保WindowsCMD和第三方（例如cmder）可以正常显示输出内容；2.为了使刷新页面不花眼/闪屏于是使用了覆写刷新方案，而不是清空再写等方案。

- 查看推荐装备分类时，简介列表内显示序号为原文件序号，详细框显示为当前序号，这个作为特性，不修。
