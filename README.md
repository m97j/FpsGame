# 🕹️ FpsGame – Unity 기반 FPS 프로젝트 (Beta)

---

## 📌 프로젝트 개요

FPS 장르의 3D 액션 게임으로, 플레이어가 다양한 무기와 스킬을 활용하여 적을 처치하는 컨셉입니다.  
Unity 기반으로 개발되었으며, 모듈화된 디렉토리 구조와 GitFlow 전략을 활용한 것이 특징입니다.
클라이언트(frontend)와 백엔드(backend)를 명확하게 분리하여, 구조적 확장성과 협업성을 염두에 두고 설계되었습니다.

---

## 📂 디렉토리 구조

```

FpsGame/
├── README.md              # 프로젝트 개요 및 사용법 문서
├── .gitignore             # Git에서 추적하지 않을 파일/폴더 목록
├── frontend/              # Unity 기반 클라이언트 프로젝트 폴더
│   └── Assets/            # Unity의 리소스 및 코드 파일 모음
│       └── Scripts/       # C# 스크립트 디렉토리 (Player, Weapon, Enemy 등 게임 로직 포함)
└── backend/               # Node.js 기반 백엔드 API 서버
    ├── controllers/       # 라우터에서 호출되는 로직 정의 
    ├── models/            # MongoDB 모델 정의 (Schema) 파일
    ├── routes/            # API 엔드포인트 라우팅 정의
    ├── app.js             # 서버의 진입점. Express 앱 초기화 및 미들웨어 설정
    ├── package-lock.json  # 의존성 트리 고정 파일
    └── package.json       # 프로젝트 메타정보 및 의존성 정의


````

---
## 🚀 브랜치 전략

```plaintext
main → 배포용
develop → 통합 개발
feature/~~~ → 기능 개발
```

예시 흐름:
```
main
└── develop
    └── feature/front_beta
```

병합 순서:
```bash
feature → develop → main
```

---

## ✨ 현재 기능 (Beta)

- ✅ 캐릭터 이동 (WASD), 점프, 대시  
- ✅ 1인칭 총기 조작: 발사, 재장전 애니메이션  
- ✅ 적 AI 시스템: 감지→추적→피격 판정 (FSM)  
- ✅ UI 구성: 체력 및 탄약 HUD  
- ✅ 백엔드: Express 서버 + MongoDB (Atlas) 연결 완료  
- ✅ GitFlow 브랜치 전략 적용

---

## 🔮 향후 로드맵 (v1.x)

* 🗺️ 다중 맵 선택 기능 (맵 로비 인터페이스)
* 🌐 2인용 로컬/네트워크 멀티플레이 모드
* 🛠️ 계정별 데이터 관리: 보유 재화, 인벤토리 아이템 저장
* 💾 스킬 및 무기 업그레이드 시스템 도입
* 📱 모바일 및 컨트롤러 대응 UI 업데이트

---

## ⚙️ 기술 스택

| 구성      | 기술                                       |
| ------- | ---------------------------------------- |
| 게임 엔진   | Unity (URP)                              |
| 언어      | C#                                       |
| 백엔드     | Node.js, Express                         |
| DB      | MongoDB Atlas                            |
| 배포      | Render                                   |
| 협업      | Git, GitHub, GitFlow                     |
| UI/ 그래픽 | Shader Graph, Unity Animation Controller |

---

## 🧩 주요 코드 구성

* `PlayerMovement.cs`: 이동, 점프, 대시 로직
* `WeaponController.cs`, `GunData.cs`: 총기 발사와 재장전 시스템
* `EnemyFSM.cs`, `EnemyState.cs`: 적 AI 상태 머신
* `server.js`: 백엔드 초기화 및 API 엔드포인트 준비 (향후 확장 예정)

---

## 📮 문의 및 피드백

기능 제안, 오류 제보, 기여 요청은 **Issues**로 남겨주세요.
새로운 기능 구현이나 협업에 관심 있는 분들의 PR도 언제든 환영합니다!

---

