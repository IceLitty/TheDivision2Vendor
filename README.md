# TheDivision2Vendor
该工具可以获取到每周商人的内容并根据内置文本翻译成中文，以及加以注释。

下载请点击顶部或此处的 [Release](https://github.com/IceLitty/TheDivision2Vendor/releases) 标签，选择对应版本即可：

- `D2Vendor.zip` 标准发布版本，解压到文件夹后，每次使用运行D2Vendor.exe即可。
- `D2VendorStand.zip` 打包单一可执行文件版本，不会在目录下生成或需要额外文件，缺点是下载的资源文件等全部保存在用户temp目录下，被清理后会丢失数据。

操作方式包含在内置说明里。

每周数据来源：https://rubenalamina.mx/

当前匹配游戏版本：1.08

## 目前已知bug：

- v1.1 已解决 ~~在多内容行中，左右选择后的页面刷新方式不会根据当前选择所在位置来进行调整，导致回到顶部。
  暂时解决方法可以上下选择一下即可，或多用用PageUp/PageDown来进行大致选择。~~

- 放大窗口后再缩小窗口会发现每行最右侧有文本残留，这是因为：1.首先确保WindowsCMD和第三方（例如cmder）可以正常显示输出内容；2.为了使刷新页面不花眼/闪屏于是使用了覆写刷新方案，而不是清空再写等方案。

- v1.3 已解决 ~~查看往期数据选择推荐装备分类、直接q退回主菜单皆未将左右移动锁定去除。~~

- 查看推荐装备分类时，简介列表内显示序号为原文件序号，详细框显示为当前序号，这个作为特性，不修。

- 未知情况下崩溃会导致进程残留，暂时没解决方案。

- 放弃替换掉按键输入while无限循环方案，已无此问题 ~~当输入按键响应的写入在未完成的情况下继续接受更多输入响应事件，会导致行列错位俗称花屏，由于CMD的刷新方法限制无法解决，遇到后等输入响应事件都结束后再次输入按键刷新窗口即可正常。~~

- v1.3 已解决 ~~缩放窗口自动刷新、标题栏倒计时有时无法使用~~

- 在1.3版本放大默认窗口大小后，在多列条件下显示的第5列时左右切换虽正常响应但未刷新。

- v1.6 已解决 ~~在周二刷新时间前的当天获取数据仍认为是新的一周数据。~~

- 还有的另说，先凑合用

## 功能列表：

- 从数据源获取每周商人数据并翻译。

- 格式化输出商人数据内容（装备、武器、模组）。

- 根据特定条件输出仅符合条件的“推荐道具”

- 输出全部的天赋/装备组信息

## Todo List：

- [ ] 由于数据源没完整实现模组的每周更新，故未制作模组相关内容。

- [x] v1.2 引入天赋解释文本，利于例如我这种光看天赋名字记不住效果的人。

- [ ] 引入所有的装备组特殊天赋名称和汉化。

- [ ] 翻译所有的装备/武器名称并将其按槽位归类（[工作量](https://docs.google.com/spreadsheets/d/e/2PACX-1vTJEX5DerCvOj3a_m36TRy1gPBAUvrduOIdmXI9j1Y0MpQk1wIXaZ9KOcPa7HzXzp_N5qGmjDj6yEfL/pubhtml#)太大，谁来救救孩子）。

- [x] v1.3 将天赋和套装效果做个输出。

- [ ] 完善全部模组名称、词条名称、词条属性最大值。

- [x] v1.7 使用v3api https://api.github.com/repos/IceLitty/TheDivision2Vendor/releases 来检测更新

- [ ] 其他的还没想到。
