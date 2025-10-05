---

# 🕹️ FpsGame – Unity 기반 FPS 프로젝트 (Beta)

## 📌 프로젝트 개요
Unity 기반 1인칭 슈팅(FPS) 게임 프로젝트입니다.  
본 프로젝트는 **FPS 장르의 기본기 학습**과 함께,  
**AI 기반 NPC 행동 설계(FSM → Behavior Tree → ML-Agents PPO)**, **다중 맵 플레이**, **클라이언트-서버 구조**를 경험하기 위해 제작되었습니다.  

> 단순한 기능 구현을 넘어서, **모듈화된 디렉토리 구조**와 **GitFlow 전략**을 적용하여  
> 협업과 확장성을 고려한 구조적 설계를 목표로 했습니다.

---

## 🎯 개발 목적
- FPS 장르의 핵심 메커니즘(플레이어 이동, 무기 시스템, 적 AI) 구현  
- FSM에서 Behavior Tree, 그리고 ML-Agents PPO까지 확장하여 **게임 AI 설계 및 강화학습 경험** 확보  
- 클라이언트-서버 구조를 적용해 **실제 서비스 아키텍처 감각** 습득  
- 포트폴리오용으로 **완성도 있는 단일 FPS 게임**을 구축  

---

## 🎥 [시연 영상](https://youtu.be/98fkWuGhLA0)

---

## 📁 디렉토리 구조
```
FpsGame/
├── frontend/         # Unity 기반 클라이언트
│   └── Assets/
│       └── Scripts/  # Player, Weapon, Enemy, BT, ML-Agent 등 게임 로직
└── backend/          # Node.js 기반 API 서버
    ├── controllers/
    ├── models/   (Player.js, Match.js, Score.js ...)
    ├── routes/
    └── app.js
```

---

## 🚀 브랜치 전략
```
main → 배포용
develop → 통합 개발
feature/~~~ → 기능 개발
```

---

## ✨ 현재 구현 기능 (v0.2-beta)

### ✅ 로그인 & 로비
- 회원가입 / 로그인 기능 (MongoDB Atlas + Render 배포 서버 연동)  
- 로비 UI: 맵 선택 패널 + 디테일 패널  
- 다중 맵 선택 및 전환 가능  

### ✅ 게임 씬
- **플레이어 조작**
  - 이동, 점프, 대시, 카메라 회전  
- **무기 시스템**
  - 발사, 재장전, 탄약 관리  
- **적 AI**
  - FSM 기반에서 Behavior Tree로 확장  
  - NavMesh 기반 경로 탐색 및 상태 전환  
  - **Unity ML-Agents PPO 강화학습 적용**
    - 보상 함수 설계(플레이어 탐지, 공격 성공, 생존 등)  
    - 학습된 정책을 NPC 행동에 반영 → 플레이어 움직임에 적응하는 AI 구현  
    - 규칙 기반(FSM/BT)과 학습 기반(PPO) AI 비교·통합  

### ✅ 옵션 메뉴
- 계속하기 / 다시하기 / 게임 종료 기능  

---

## 🧠 AI 시스템
- **FSM → Behavior Tree → ML-Agents PPO**
  - FSM: 단순 상태 전이 기반 AI  
  - Behavior Tree: ScriptableObject 기반 조건/행동 노드 관리  
  - ML-Agents PPO: 강화학습 기반 NPC 행동 학습 및 적응  
  - 규칙형 AI와 학습형 AI를 혼합하여 다양한 전술 패턴 구현  

---

## 📦 릴리즈 정보
최초 베타 릴리즈 완료  
🔗 [v0.2-beta Release 페이지](https://github.com/m97j/FpsGame/releases/tag/v0.2-beta)

---

## 🖥️ 실행 방법
1. 릴리즈 페이지에서 `.zip` 다운로드  
2. 압축 해제 후 `FpsGame.exe` 실행  
3. Windows 64비트 환경에서 실행 가능  

---

## 🔭 향후 개발 계획
- ScriptableObject를 활용한 데이터 관리  
- 🗺️ 다중 맵 플레이 안정화  
- 🎮 Behavior Tree + PPO 기반 적 AI 패턴 다양화  
- 📊 멀티플레이 기반 점수/랭킹 시스템 (서버 연동)  
- 🧩 강화학습 보상 함수 고도화 및 멀티에이전트 학습 적용  

※ RPG 장르, LLM 기반 NPC 대화 생성 및 게임 환경 동적 변화, 대규모 서버 연동, DRL 적용 등은 추후 별도 UE5 프로젝트에서 진행 예정  

---

## ⚙️ 기술 스택
| 구성       | 기술 |
|------------|------|
| 게임 엔진  | Unity (URP) |
| 언어       | C# |
| AI/ML      | Unity ML-Agents (PPO), PyTorch |
| 백엔드     | Node.js, Express |
| DB         | MongoDB Atlas |
| 배포       | Render |
| 협업       | Git, GitHub, GitFlow |

---

## 🧩 주요 코드 구성
- `PlayerMove.cs`, `PlayerRotate.cs` : 이동, 점프, 대시 로직  
- `WeaponController.cs`, `BombAction.cs` : 무기 시스템  
- `ZombieFSM.cs` / `ZombieBTAgent.cs` : 적 AI (FSM + Behavior Tree)  
- `ZombiePPOAgent.cs` : ML-Agents PPO 기반 강화학습 에이전트  
- `server.js` : 백엔드 초기화 및 API 엔드포인트  

---

## 🤝 협업 및 확장성 고려
- 클라이언트-서버 구조 분리  
- GitFlow 기반 브랜치 전략  
- AI 모듈화: FSM/BT/ML-Agent 구조 병행 가능  

---

## 📬 문의 및 피드백
기능 제안, 오류 제보, 기타 요청은 **Issues**로 남겨주세요.  
- contact  
    - email : mmnkjiae@gmail.com  

---
