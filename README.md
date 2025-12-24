---

# 🕹️ FpsGame – Unity-based FPS project (Beta)

## 📌 Project Overview
This is a Unity-based first-person shooter (FPS) game project.

---

## 🎯 Development Goals
- Implement core FPS mechanics (player movement, weapon system, enemy AI)
- Expand from FSM to Behavior Tree and ML-Agents PPO to gain **game AI design and reinforcement learning experience**

- Apply client-server architecture to gain **a sense of real-world service architecture**

---

## 📁 Directory Structure
```
FpsGame/
├── frontend[client]/ # Unity-based client
│   └── Assets/
│       └── Scripts/ # Game logic, including Player, Weapon, Enemy, BT, ML-Agent
│
└── backend[server]/ # Node.js-based API server
    ├── controllers/
    ├── models/ (Player.js, Match.js, Score.js, etc.)
    ├── routes/
    └── app.js
```

---

## 🚀 Branch Strategy
```
main → Deployment
develop → Integrated Development
feature/~~~ → Feature Development
```

---

## 🧩 Main Code Structure
- `PlayerMove.cs`, `PlayerRotate.cs`: Movement, Jump, and Dash Logic
- `WeaponController.cs`, `BombAction.cs`: Weapon System
- `ZombieFSM.cs` / `ZombieBTAgent.cs`: Enemy AI (FSM + Behavior Tree)

- `ZombiePPOAgent.cs`: ML-Agents PPO-based Reinforcement Learning Agent
- `server.js`: Backend Initialization and API Endpoint

---

## ✨ Current Implementation (v0.2-beta)

### ✅ Login & Lobby
- Signup / Login Function (MongoDB Atlas + Render Deployment Server Integration)

- Lobby UI: Map Selection Panel + Details Panel
- Multi-map Selection and Switching

### ✅ Game Scene
- **Player Controls**
    - Movement, Jumping, Dash, Camera Rotation
- **Weapon System**
    - Firing, Reloading, Ammo Management
- **Enemy AI**
    - Extended from FSM-based to Behavior Tree

    - NavMesh-based Pathfinding and State Transition
    - **Unity ML-Agents Applying Reinforcement Learning to PPO**
    - Reward function design (player detection, successful attack, survival, etc.)

    - Incorporating learned policies into NPC behavior → Implementing AI that adapts to player movements
    - Comparison and integration of rule-based (FSM/BT) and learning-based (PPO) AI

### ✅ Options Menu
- Continue/Retry/End Game Functions

---

## 🧠 AI System
- **FSM → Behavior Tree → ML-Agents PPO**
    - FSM: Simple state transition-based AI
    - Behavior Tree: ScriptableObject-based condition/action node management
    - ML-Agents PPO: Reinforcement learning-based NPC behavior learning and adaptation
    - Implementing diverse tactical patterns by combining rule-based and learning-based AI

---

## ⚙️ Tech Stack
| Configuration | Technology |
|------------|------|
| Game Engine | Unity (URP) |
| Language | C# |
| AI/ML | Unity ML-Agents (PPO), PyTorch |
| Backend | Node.js, Express |
| DB | MongoDB Atlas |
| Deployment | Render |
| Collaboration | Git, GitHub, GitFlow |

---

## 📦 Release Information
Initial Beta Release Completed

🔗 [v0.1-beta Release](https://github.com/m97j/FpsGame/releases/tag/v0.1-beta)

---

## 🖥️ How to Run
1. Download `.zip` from the release page
2. Unzip and run `FpsGame.exe`
3. Runs on Windows 64-bit.

---

## 🎥 [Demo Video](https://youtu.be/98fkWuGhLA0)

---

## 📬 Inquiries and Feedback
Please submit feature suggestions, bug reports, and other requests to **Issues**.
- contact
- email: mmnkjiae@gmail.com

---
