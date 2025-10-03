
## 🕹️ FpsGame – Unity 기반 FPS 프로젝트 (Beta)

## 📌 프로젝트 개요

> Unity 기반 1인칭 슈팅 게임 프로젝트  
> 단순한 FPS 구조에서 나아가 **스토리 중심 RPG 요소**, **AI 기반 NPC 대화 시스템**, **서버 기반 진행 저장/복구** 기능을 포함한 확장형 게임 아키텍처를 연구 및 개발하고 있습니다.
> **모듈화된 디렉토리 구조와 GitFlow 전략**을 통해 구조적 확장성과 협업 효율성을 고려하여 설계되었습니다.  
> 클라이언트와 서버를 명확히 분리하고, 실제 환경에서의 역할 구분과 API 연동을 염두에 두었습니다.

## 🎯 개발 목적 및 기획 의도

본 프로젝트는 FPS 장르의 기본기를 익히는 동시에,  
**MMORPG 요소를 접목한 확장형 게임 시스템**을 기획하고 있습니다.
특히 RPG 핵심 요소인 *npc및 스토리, 아이템 강화, 상점 시스템, 계정 기반 성장 구조* 등을 현재 게임에 적용하는 방향으로 발전시키고자 합니다.

## 🎥 [시연 영상](https://youtu.be/98fkWuGhLA0)

## 📁 디렉토리 구조

```
FpsGame/
├── frontend/         # Unity 기반 클라이언트
│   └── Assets/
│       └── Scripts/  # Player, Weapon, Enemy 등 게임 로직
└── backend/          # Node.js 기반 API 서버
    ├── controllers/
    ├── models/   (Player.js, NPCState.js, Quest.js, DialogueLog.js, ...)
    ├── routes/
    └── app.js
```

## 🚀 브랜치 전략

```
main → 배포용
develop → 통합 개발
feature/~~~ → 기능 개발
```

## ✨ 현재 구현 기능 (v0.1-beta)

#### ✅ 로그인 씬
- UI 구성: ID, 비밀번호 입력 및 버튼 인터랙션 구성  
- 회원가입 및 로그인 기능 구현  
- **Render 플랫폼을 통한 백엔드 배포** 및 **MongoAtlas 기반 CloudDB 연동**  
  → 클라이언트-서버 통신을 통해 실제 데이터 기반 인증 처리 완료

---

#### ✅ 로비 씬
- 전체 UI는 Grid 기반 맵 선택 패널과 디테일 패널로 구성  
  - 맵 목록은 카드 형태의 GridList로 출력  
  - 각 맵 클릭 시 **스토리 방식의 설명이 출력되는 디테일 패널**이 등장  
  - 디테일 패널 내 **Play / Close 버튼**으로 인터랙션 제어  
    - Play 클릭 시 해당 맵 기반 게임 씬으로 전환  
    - Close 클릭 시 디테일 패널이 사라지고 GridList가 다시 보임

- 하단 메뉴 버튼(Shop, Settings, Inventory, Friends)은 UI만 구현된 상태이며  
  각 버튼 클릭 시 열리는 창(detail_shop 등)은 향후 개발 예정

---

#### ✅ 게임 씬
  - 전체 배경은 **산업형 컨테이너 지형 / 창고 환경으로 구성**  
  - Hierarchy상에는 Player, MainCamera, GameManager, Enemy(Zombie1), UI(Canvas), Map 등이 배치됨  
  - **Player 이동 / 회전 로직 개선:**  
    - 제자리 회전 구현  
    - 카메라 위치 정렬 및 상하/좌우 회전 분리 구조 적용  
  - **적 오브젝트 상태:**  
    - Enemy FSM이 구성되어 있으며 추후 AI 상태 전이 확장 예정  
    - 현재는 플레이어 위치를 따라오는 로직 기반으로 이동  
    - 플레이어가 점프할 경우 Enemy가 공중에서 움직이는 문제 발생 → 로직 수정 예정
  - **오브젝트 배치:**  
    - Map_v2를 기반으로 지형 요소 배치  
    - Bullet Impact, Light 등 다양한 이펙트 연계 구조 적용 준비 중

---

#### ✅ 게임 씬 옵션 메뉴
- 게임 화면 우상단에 **톱니바퀴 아이콘 클릭 시 옵션 패널 활성화**  
- 옵션 패널에는 다음 버튼들이 포함됨:
  - **계속하기** : 옵션 창 닫고 게임 복귀  
  - **다시하기** : 게임 씬을 다시 로드하여 재시작  
  - **게임 종료** : 게임 씬 종료 후 **로비 씬의 기본 화면으로 자동 전환**

---

## ✨ 추가될 내용 (v0.2-beta)

### 🎭 NPC 상호작용
- 단순 대사 스크립트 기반이 아닌 **AI 엔진(persona-chat-engine 연동)** 을 통한 자연어 대화
- 플레이어 신뢰도 / NPC 관계도 / 퀘스트 진행 상황에 따라 **동적으로 변화하는 대사**
- 특정 조건 달성 시 **게임 환경 변경(플래그/델타값 적용)**

### 🌐 서버 통신
- **game-server(Node.js, Express)** 와 연결하여 다음을 처리:
  - 로그인 및 세션 관리
  - NPC 상태 / 플레이어 진행 상황 실시간 업데이트
  - AI-server와의 중계 역할 수행
- 클라이언트(UI/씬)는 **server API 호출**을 통해 대사/퀘스트/환경변화를 동기화

### 🧠 AI 기능 (persona-chat-engine)
- 대화 입력을 **ai-server(FastAPI)** 로 전달 후,  
  Hugging Face Spaces에 올린 **fine-tuned 모델(hf-serve)** 과 연동
- 대화 맥락 + NPC 성격을 기반으로 한 응답 생성
- AI가 반환한 결과를 **game-server** 에서 처리 →  
  `trust`, `questStage`, `flags` 등의 상태값에 반영 후 Unity UI에 반영

### 💾 자동 저장
- 15분마다 / 로딩 화면 / 게임 종료 시 진행 상태를 자동 저장
- MongoDB 기반의 `Player`, `NPCState`, `Quest`, `DialogueLog` 스키마 활용
- 이후 접속 시 **이어하기(continue play)** 지원

---

## 🏗️ 아키텍처 개요

```mermaid
flowchart LR
  Client[Unity Client] --REST--> GameServer[Node.js Game Server]
  GameServer --RPC--> AiServer[FastAPI AI-Server]
  AiServer --API--> HFServe[Hugging Face Model]
  GameServer --> MongoDB[(Game DB)]
  GameServer --> Redis[(Cache)]
````

* **Unity Client**

  * `NPCDialogueManager.cs`: 대화 입력/출력
  * `SessionSaveManager.cs`: 세이브/로드 트리거
  * **ScriptableObject** 를 사용해 NPC 데이터/Quest 상태를 모듈화하여 관리

* **Game Server (Node.js)**

  * `models/` : Player, NPCState, Quest, DialogueLog, Inventory 등 저장 스키마
  * `controllers/` : Dialogue/Event/Quest 처리
  * `services/` : AI 호출, 저장관리, 이벤트 처리

* **AI Server (persona-chat-engine)**

  * 입력 prompt 구성 + 후처리 → Hugging Face 모델 호출

---

## 📌 기술 포인트

* **Unity 아키텍처**

  * ScriptableObject 기반 NPC 데이터 관리 (상태/대사 분리)
  * OOP 구조 → 이벤트 기반 설계 (Dialogue/Quest 트리거)
  * `NPCDialogueManager`와 `SessionSaveManager`를 통한 **클라이언트-서버 동기화**

* **서버 아키텍처**

  * RESTful API 기반 구조
  * MongoDB로 상태 저장 (장기적 진행 관리)
  * Redis 캐시를 활용한 단기 메모리 최적화
  * **AI-server → hf-serve 모델 연동**을 통한 분리형 구조

---

## 📦 릴리즈 정보

최초 베타 릴리즈가 완료되었습니다!  
🔗 [v0.1-beta Release 페이지 바로가기](https://github.com/m97j/FpsGame/releases/tag/v0.1-beta)

## 🖥️ 실행 방법

1. 릴리즈 페이지에서 `.zip` 파일 다운로드  
2. 압축 해제 후 모든 파일을 **같은 디렉토리**에 위치시킵니다  
3. `FpsGame.exe` 실행 → 게임 시작!

※ Windows 64비트 환경에서 실행 가능

## 🔭 향후 개발 계획 (v1.x >)

- 🗺️ **다중 맵 시스템**: ~~로비에서 맵 선택 가능~~[구현 완료], 맵별 무기 드롭 테이블 설정
- 🛒 **상점 시스템**: 로비 상점 및 게임 내 히든 상점 구현, 아이템 구매/판매 기능 (Inventory/Item DB와 연동)
- 🎲 **랜덤 아이템 등장**: 상점마다 판매 아이템이 매 게임마다 랜덤 결정
- 💎 **아이템 강화 시스템**: 무기/방어구 강화, 등급별 능력치 적용
- 📦 **계정 기반 저장**: 플레이어의 인벤토리, 재화, 강화 상태를 백엔드에 저장
- 🎭 **npc 기반 스토리 추가**: ai기반 npc대사 동적 생성 (RAG 기반 NPC 지식 확장 (persona-chat-engine RAG 모듈 활용))
- 🧠 **히든 요소**: 특정 조건 만족 시 등장하는 공간/장비/엔딩루트 등 탐색 요소 추가
- 🌐 **멀티플레이 모드**: 로컬 및 네트워크 기반 2인 플레이 지원 (Game-server에서 세션 관리 확장)
- 📱 **모바일 대응 UI**: 터치 및 컨트롤러 입력 대응

## ⚙️ 기술 스택

| 구성       | 기술 |
|------------|------|
| 게임 엔진  | Unity (URP) |
| 언어       | C# |
| 백엔드     | Node.js, Express |
| DB         | MongoDB Atlas |
| 배포       | Render |
| 협업       | Git, GitHub, GitFlow |
| UI/그래픽  | Shader Graph, Unity Animation Controller |

## 🧩 주요 코드 구성

- `PlayerMovement.cs` : 이동, 점프, 대시 로직
- `WeaponController.cs`, `GunData.cs` : 총기 발사 및 재장전
- `EnemyFSM.cs`, `EnemyState.cs` : 적 AI 상태 머신
- `server.js` : 백엔드 초기화 및 API 엔드포인트 구성

## 🤝 협업 및 확장성 고려

본 프로젝트는 **클라이언트-서버 구조를 명확히 분리**하여,  
실제 게임 개발 환경에서의 협업을 염두에 두고 설계되었습니다.  
백엔드와의 연동은 RESTful API 기반으로 구성되어 있으며,  
**계정 기반 저장 및 상태 동기화 기능**을 확장할 수 있도록 준비 중입니다.

## 📬 문의 및 피드백

기능 제안, 오류 제보, 기타 요청은 **Issues**로 남겨주세요.  
새로운 기능 구현이나 협업에 관심 있는 분들의 PR도 언제든 환영합니다!

---




