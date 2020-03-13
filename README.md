# TheDivision2Vendor
该工具可以获取到每周商人的内容并根据内置文本翻译成中文，以及加以注释。

下载请点击顶部或此处的 [Release](https://github.com/IceLitty/TheDivision2Vendor/releases) 标签，选择对应版本即可：

- `D2Vendor.zip` 标准发布版本，解压到文件夹后，每次使用运行D2Vendor.exe即可。
- `D2VendorStand.zip` 打包单一可执行文件版本，不会在目录下生成或需要额外文件，缺点是下载的资源文件等全部保存在用户temp目录下，被清理后会丢失数据。

操作方式包含在内置说明里。

每周数据来源：https://rubenalamina.mx/

## 目前已知bug：

- v1.1 已解决 ~~在多内容行中，左右选择后的页面刷新方式不会根据当前选择所在位置来进行调整，导致回到顶部。
  暂时解决方法可以上下选择一下即可，或多用用PageUp/PageDown来进行大致选择。~~

- 放大窗口后再缩小窗口会发现每行最右侧有文本残留，这是因为：1.首先确保WindowsCMD和第三方（例如cmder）可以正常显示输出内容；2.为了使刷新页面不花眼/闪屏于是使用了覆写刷新方案，而不是清空再写等方案。

- 还有的另说，先凑合用

## 功能列表：

- 从数据源获取每周商人数据并翻译。

- 格式化输出商人数据内容。

- 根据特定条件输出仅符合条件的“推荐道具”

## Todo List：

- [ ] 由于数据源没完整实现模组的每周更新，故未制作模组相关内容。

- [x] 引入天赋解释文本，利于例如我这种光看天赋名字记不住效果的人。

- [ ] 引入所有的装备组特殊天赋名称和汉化。

- [ ] 翻译所有的装备/武器名称并将其按槽位归类（[工作量](https://docs.google.com/spreadsheets/d/e/2PACX-1vTJEX5DerCvOj3a_m36TRy1gPBAUvrduOIdmXI9j1Y0MpQk1wIXaZ9KOcPa7HzXzp_N5qGmjDj6yEfL/pubhtml#)太大，谁来救救孩子）。

- [ ] 其他的还没想到。

