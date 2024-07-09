# BouncingRhythm

> Rhythm Game Editor Plugin in Unity

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/Toukaya/IntelliScheduler.git) [![License](https://img.shields.io/badge/license-GPL3.0-blue.svg)](LICENSE)

## Features

- Associate any game with custom beats
- Using Event
- A cool Editor Interface
- **Tap to Recording**

## Install

Clone the whole project and Run it. (I Only test on Unity 2022.3.31f1 and it's ok)

### Usage

if you want to use "Hit to Record Mode":

* Open the "Make Chart Scene"
* Open the Gameobject with "ChartGenerator" On it
* hit play.
* hit spacebar to Start the music and game.
* There are currently four key: D F J K stands for four kinds of keys.
* 

## Screenshot

<p align="center"><img src="Resources/BouncingRhythm.png" style="width: 100%;" /></p>


## 开发环境

- Unity2022.3.31f1
- 

## TODO

- **周视图** (May do)
  - 实现一周的显示布局
  - 显示和排列一周内的事件
- **事件自动安排算法**
  - 开发能够自动优化和安排事件的算法
- **搜索和过滤模块**
  - 设计并实现高效的搜索算法
  - 允许用户自定义事件过滤器
- **NLP集成**
  - 集成LLM API到应用中
  - 调用API生成自然语言处理结果
  - 将API结果集成到应用界面中
- **Google Calendar API 集成**
  - 设置Google Cloud Console，创建API密钥和OAuth 2.0客户端ID
  - 集成Google Calendar API，并实现认证过程
  - 通过API获取和显示用户的日历数据
- **日历设置和个性化模块**
  - 用户界面主题和样式的偏好设置
  - 时间格式、语言等偏好设置
  - 显示当前时区和切换时区界面
  - 处理跨时区事件的显示

## 协议

该项目基于[GNU GPL-3.0 LICENSE](LICENSE)开源。
