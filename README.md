[TOC]

## VR创新鼠标

### 方案设计

#### 现有操作映射

鼠标常见规定操作及其作用：

|         动作         |               作用               |
| :------------------: | :------------------------------: |
|         移动         |      移动鼠标，光标跟随移动      |
|       单击左键       | 选择某个对象，触发按钮，切换窗口 |
|       双击左键       | 启动程序，打开窗口，选中文本词组 |
|       单击右键       |        弹出对象的快捷菜单        |
| 拖动（按住左键移动） |           移动对象位置           |
|       滚动滚轮       |           上下翻动页面           |
|       按下滚轮       |         打开自动翻页模式         |
|       三击左键       |         选中全部文本内容         |


将以上动作分解成基础操作：

| 操作 |     方式     |           变量           |
| :--: | :----------: | :----------------------: |
| 滚动 | 滚轮，轨迹球 |  1或2个角度值double[2]   |
| 点击 |     按键     |        布尔值bool        |
| 遥感 |    陀螺仪    | 6自由度空间向量double[6] |
| 移动 |     红外     |  2个水平坐标值double[2]  |
| 触摸 |    触感屏    |         矩阵信息         |

部分鼠标添加了额外按钮和功能，例如下图的[罗技Anywhere2无线鼠标](https://www.logitech.com.cn/zh-cn/articles/11892)：

<img src="https://i.loli.net/2021/10/08/ks6alHNEI3dcMrh.png" alt="image-20211008183229130" style="zoom:67%;" />

#### VR操控需求

VR场景中是三维空间六个自由度的运动，主要需要的功能主要包括：

前进后退，左右旋转，俯仰旋转，上下平移，左右平移，放大缩小

（翻滚角这个旋转自由度基本不会用到）

现常见的二维鼠标操作映射解决方案：

|   功能   |       操作       |
| :------: | :--------------: |
| 左右旋转 |     拖动光标     |
| 俯仰旋转 |     拖动光标     |
| 前后平移 |   双击左键前进   |
| 上下平移 |        无        |
| 左右平移 |        无        |
| 放大缩小 | 触控板上二指操作 |


#### 目标操作映射

经过上述调研，我们计划添加的操作映射：

|             操作             |        作用        |
| :--------------------------: | :----------------: |
|          转动轨迹球          | 左右旋转，俯仰旋转 |
| 移动鼠标（类似自动翻页模式） | 上下平移，左右平移 |



### 计划步骤

#### 背景调研

调研鼠标主要类型、鼠标操控方式、VR场景操控需求并进行总结。

#### 方案设计

转动轨迹球控制视角左右俯仰旋转，鼠标在桌面上平移控制上下左右移动。

#### 初步效果验证

在unity仿真平台上模拟上述方案，查看效果是否符合想象、是否优于现有鼠标VR控制方式。

#### 初步硬件实现

购买轨迹球鼠标，保留原鼠标在平面上移动的二维坐标值输入，增加轨迹球二维角度值输入，用于控制VR视角，即左右转动轨迹球时人物左右旋转、前后转动轨迹球时人物俯仰旋转，可以灵活地使得人物转向任一方向。

保留鼠标原有二维移动功能的同时，用轨迹球操控视角，尝试体验给出使用感受反馈，便于进一步改进和拓展。

#### 改造组装鼠标

尝试将轨迹球加到普通鼠标上，可能涉及到电路板的焊接和驱动控制的编写，为拓展内容。

#### 实现效果展示

使用组装好的VR创新鼠标，按照设计的操控方式，实现在Unity中的VR常见的自由平移、视角旋转，对比体现设计的创新性和实用性。



### 完成的工作

#### 环境配置

安装配置Unity仿真平台，使用GitHub上面的VRTK库场景

#### 开始代码编写

[教程链接点这里](http://c.biancheng.net/unity3d/)

学习C#语言的基本语法

熟悉Unity面向对象编程框架，通过属性添加components的方式

熟悉Unity搭建场景、添加脚本、关联物体的方式

<img src="https://i.loli.net/2021/11/02/6k7KexPhdyr8uEi.png" alt="image-20211102022530578" style="zoom:67%;" />

搭建了一个3D场景，通过wsad前后左右移动，通过鼠标平移旋转视角，做了一个简易demo

#### 初步效果验证

在VRTK农场场景的基础上编写Player控制代码

<img src="https://i.loli.net/2021/11/02/vXaOi6TostlHw8f.png" alt="image-20211102021342527" style="zoom: 50%;" />

按照方案计划，鼠标在桌面平移控制人物前后左右移动，且为了方便操作会保持该速度运动（即惯性），按左键速度清零，有最大速度限制

由于现在还没有轨迹球，初步使用WASD四个键模拟轨迹球输入，进行视角旋转，操作上轨迹球应该会更加灵活效果更好一些

[效果演示视频链接](https://gitee.com/beatrix/vrmouse/blob/master/Images/%E6%BC%94%E7%A4%BA%E8%A7%86%E9%A2%91.mp4)

项目已经上传到[Gitee仓库](https://gitee.com/beatrix/vrmouse)

#### tips:

①Unity Hub许可证失效：先登录再激活许可证，否则手动激活 每隔一段时间 许可证文件会自动失效

②穿模问题：注意人物Charcter Controller中心高度需要在平面之上，否则即使没有勾选Use Gravity也会逐渐下沉穿模、掉到平面下面

③注意是每帧刷新一次，需要设置最大速度，防止发生瞬移的情况



## VRTK场景

本项目场景修改自[Extend Reality Ltd](https://github.com/ExtendRealityLtd)的开源项目[VRTK](https://github.com/ExtendRealityLtd/VRTK)

## License

Code released under the [MIT License][License].

## Disclaimer

These materials are not sponsored by or affiliated with Unity Technologies or its affiliates. "Unity" is a trademark or registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.

[VRTK-Image]: https://raw.githubusercontent.com/ExtendRealityLtd/related-media/main/github/readme/vrtk.png
[Unity]: https://unity3d.com/
[Made With VRTK]: https://www.vrtk.io/madewith.html
[License]: LICENSE.md
[Tilia]: https://www.vrtk.io/tilia.html
[VRTK.Prefabs]: https://github.com/ExtendRealityLtd/VRTK.Prefabs
[Unity Hub]: https://docs.unity3d.com/Manual/GettingStartedUnityHub.html

[License-Badge]: https://img.shields.io/github/license/ExtendRealityLtd/VRTK.svg
[Backlog-Badge]: https://img.shields.io/badge/project-backlog-78bdf2.svg

[Discord-Badge]: https://img.shields.io/badge/discord--7289DA.svg?style=social&logo=discord
[Videos-Badge]: https://img.shields.io/badge/youtube--e52d27.svg?style=social&logo=youtube
[Twitter-Badge]: https://img.shields.io/badge/twitter--219eeb.svg?style=social&logo=twitter

[License]: LICENSE.md
[Backlog]: http://tracker.vrtk.io

[Discord]: https://discord.com/invite/bRNS6hr
[Videos]: http://videos.vrtk.io
[Twitter]: https://twitter.com/VR_Toolkit
[Bowling Tutorial]: https://github.com/ExtendRealityLtd/VRTK.Tutorials.VRBowling

[Quick Outline]: https://github.com/chrisnolet/QuickOutline

[Contributing guidelines]: https://github.com/ExtendRealityLtd/.github/blob/master/CONTRIBUTING.md
[project coding conventions]: https://github.com/ExtendRealityLtd/.github/blob/master/CONVENTIONS/UNITY3D.md
