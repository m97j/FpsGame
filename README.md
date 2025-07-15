
# 🕹️ FpsGame

## 📌 프로젝트 소개
FPS 장르의 3D 액션 게임으로, 플레이어가 다양한 무기와 스킬을 활용하여 적을 처치하는 컨셉입니다.  
Unity 기반으로 개발되었으며, 모듈화된 디렉토리 구조와 GitFlow 전략을 활용한 것이 특징입니다.

---

## 📁 폴더 구조

```
FpsGame/
├── README.md
├── .gitignore
└── front/
    ├── Assets/
    └── Scripts/
```

---

## ⚙️ 기술 스택

- **Engine**: Unity (URP 적용)
- **Language**: C#
- **Version Control**: Git & GitHub (GitFlow 전략 적용)
- **Design**: Shader Graph, Animation Controller
- **Other**: Visual Studio, GitHub Desktop

---

## ✨ 주요 기능

- 🔫 1인칭 총기 조작 및 애니메이션
- 👣 캐릭터 이동 (점프, 대시 등)
- 🎯 적 AI 및 피격 판정 시스템
- 🧠 스킬 시스템 구현 예정
- 🌐 GitFlow 기반 브랜치 관리

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

## 📦 설치 및 실행 방법

1. Unity Hub에서 해당 프로젝트 추가
2. Unity 2022.x 버전으로 열기
3. 플레이 버튼 클릭 후 테스트

---

## 📈 기획 & 일정

- [o] 기본 캐릭터 움직임 구현
- [o] 총기 모델링 & 애니메이션
- [o] 적 AI 및 사운드 추가
- [o] UI 시스템 구축
- [x] 버전 1.0 배포 준비

---


## 📮 문의

이 프로젝트에 대한 문의나 제안이 있다면 Issues에 남겨주세요.

---


