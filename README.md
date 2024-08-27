# Pathfinding Dedicated Server for '**The Last Rollback**'

## Introduction

 스파르타 내일배움캠프 Node.js 5기 게임서버 최종 프로젝트
 12팀 'The Last Rollback' 의 'The Last Rollback' 게임 개발 프로젝트에서 사용되는 Dedicated Server 입니다.

- [The Last Rollback](https://github.com/eliotjang/the-last-rollback-server) 프로젝트 repo
- [Brosure](https://www.notion.so/eliotjang/12-Rules-for-Life-3d4bbae7340d4a98bd97ac411c45a1de)  브로셔

<br> 

## Service Architecture
![image](https://github.com/user-attachments/assets/1eb36a53-7021-4609-9f00-9d2d432527a0)

<br>

## Brosure Documents
- [데디 서버 플로우](https://eliotjang.notion.site/c7de917bf02144be934b05cef0e32bd8?pvs=4) 
- [개발 과정](https://eliotjang.notion.site/Dedicated-Server-247a2325e5b6440c933b521d063184f8?pvs=4)

<br>

### Used libraries
- [DotRecast](https://github.com/ikpil/DotRecast?tab=readme-ov-file) by [ikpil](https://github.com/ikpil)  
- [protobuf-net](https://github.com/protobuf-net/protobuf-net)

<br>

## Screenshots

![스크린샷 2024-08-15 173331](https://github.com/user-attachments/assets/f7f128e6-dcfd-468f-b1d3-68984c853e32)
![스크린샷 2024-08-16 001408](https://github.com/user-attachments/assets/b7bad15b-26fa-4a08-8a64-9b5bd06127d5)
![image](https://github.com/user-attachments/assets/d114c6ae-2d79-4178-8b19-7fce2cde9860)

## Directory Structure
```

├─Assets
├─Properties
└─Src
    ├─Constants
    ├─Data
    │  └─Abstracts
    ├─Handlers
    │  └─abstracts
    ├─Libs
    │  ├─DotRecast.Core
    │  │  ├─Buffers
    │  │  ├─Collections
    │  │  ├─Compression
    │  │  └─Numerics
    │  ├─DotRecast.Detour
    │  │  ├─bin
    │  │  │  └─Debug
    │  │  │      ├─net6.0
    │  │  │      ├─net7.0
    │  │  │      ├─net8.0
    │  │  │      └─netstandard2.1
    │  │  └─Io
    │  ├─DotRecast.Detour.Crowd
    │  ├─DotRecast.Detour.Dynamic
    │  │  ├─Colliders
    │  │  └─Io
    │  ├─DotRecast.Detour.TileCache
    │  │  └─Io
    │  │      └─Compress
    │  └─DotRecast.Recast
    │      └─Geom
    ├─Nav
    │  ├─Config
    │  ├─Crowds
    │  │  └─Agents
    │  │      └─Models
    │  │          └─Base
    │  └─QueryFilters
    ├─Network
    ├─Sessions
    └─Utils
        └─FileLoader

```

## Credit

| 이름           | email    | github              |
|----------------|-------------------------------|-----------------------------|
|장성원          | eliotjang2@gmail.com | [https://github.com/eliotjang](https://github.com/eliotjang) |
|김동균          | donkim0122@gmail.com | [https://github.com/donkim1212](https://github.com/donkim1212) |
|윤동협          | []() | [https://github.com/ydh1503](https://github.com/ydh1503) |
|박지호          | []() | [https://github.com/Hoji1998](https://github.com/Hoji1998) |
|양현언          | []() | [https://github.com/HyuneonY](https://github.com/HyuneonY) |
|황정민          | []() | [https://github.com/mimihimesama](https://github.com/mimihimesama) |

<br>

