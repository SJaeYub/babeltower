# Babel Tower - 2단계 구현 완료

## ✅ 구현 완료 내용

### 1. 캐릭터 시스템
- ✅ Character.cs - 캐릭터 베이스 클래스
- ✅ Player.cs - 플레이어 캐릭터 (4개 직업)
- ✅ Monster.cs - 몬스터 캐릭터 + AI

### 2. 전투 시스템
- ✅ DamageCalculator - 데미지 계산
- ✅ Skill - 스킬 베이스 클래스
- ✅ Projectile - 발사체 시스템
- ✅ MeleeAttackHitbox - 근접 공격 판정

### 3. 카메라 시스템
- ✅ IsometricCamera - 디아블로 스타일 쿼터뷰

### 4. 매니저
- ✅ GameManager - 게임 전체 관리

### 5. 테스트 도구
- ✅ TestSceneSetup - 씬 자동 설정

---

## 🎮 Unity 설정 가이드

### 1단계: 프로젝트 설정

1. **새 Unity 프로젝트 생성** (2D 템플릿)
   - Unity 버전: 2021.3 LTS 이상
   - 프로젝트 이름: BabelTower

2. **레이어 설정**
   - Edit > Project Settings > Tags and Layers
   - 추가할 레이어:
     - Layer 6: Player
     - Layer 7: Enemy
     - Layer 8: Projectile

3. **Physics 2D 설정**
   - Edit > Project Settings > Physics 2D
   - Layer Collision Matrix 설정:
     - Player vs Enemy: ✅ (충돌 허용)
     - Player vs Projectile: ❌ (자신의 발사체는 무시)
     - Enemy vs Projectile: ✅ (적에게 맞음)

### 2단계: 스크립트 임포트

1. `/home/claude/BabelTower/Scripts/` 폴더의 모든 스크립트를
   Unity 프로젝트의 `Assets/Scripts/` 폴더로 복사

2. 스크립트 구조:
```
Assets/
└── Scripts/
    ├── Character/
    │   ├── Character.cs
    │   ├── Player.cs
    │   └── Monster.cs
    ├── Combat/
    │   └── CombatSystem.cs
    ├── Manager/
    │   └── GameManager.cs
    ├── IsometricCamera.cs
    └── TestSceneSetup.cs
```

### 3단계: 프리팹 생성

#### A. 플레이어 프리팹

1. **GameObject 생성**
   - Hierarchy > Create Empty
   - 이름: Player

2. **컴포넌트 추가**
   - Rigidbody2D
     - Body Type: Dynamic
     - Gravity Scale: 0
     - Constraints: Freeze Rotation Z
   - Circle Collider 2D
     - Radius: 0.4
   - Sprite Renderer
     - Sprite: 업로드한 플레이어 스프라이트 사용
     - Order in Layer: 10
   - Animator (옵션)
   - Player.cs 스크립트

3. **Player 스크립트 설정**
   - Character Name: "Hero"
   - Level: 1
   - Max HP: 100
   - Max MP: 50
   - Attack: 10
   - Defense: 5
   - Move Speed: 5
   - Attack Speed: 1
   - Attack Range: 1.5
   - Target Layer: Enemy

4. **태그 & 레이어**
   - Tag: Player
   - Layer: Player

5. **프리팹 저장**
   - Assets/Prefabs/ 폴더에 드래그

#### B. 몬스터 프리팹

1. **GameObject 생성**
   - Hierarchy > Create Empty
   - 이름: Monster_Goblin

2. **컴포넌트 추가**
   - Rigidbody2D (설정 동일)
   - Circle Collider 2D
   - Sprite Renderer
     - Sprite: 업로드한 고블린 스프라이트
     - Order in Layer: 10
   - Monster.cs 스크립트

3. **Monster 스크립트 설정**
   - Character Name: "Goblin"
   - Level: 1
   - Max HP: 50
   - Attack: 8
   - Defense: 3
   - Move Speed: 3
   - Attack Range: 1.5
   - Detection Range: 5
   - Chase Speed: 3
   - Target Layer: Player

4. **태그 & 레이어**
   - Tag: Enemy
   - Layer: Enemy

5. **프리팹 저장**

### 4단계: 테스트 씬 생성

1. **새 씬 생성**
   - File > New Scene
   - 이름: TestScene
   - 저장: Assets/Scenes/TestScene.unity

2. **GameManager 추가**
   - Hierarchy > Create Empty
   - 이름: GameManager
   - GameManager.cs 스크립트 추가

3. **TestSceneSetup 추가**
   - Hierarchy > Create Empty
   - 이름: SceneSetup
   - TestSceneSetup.cs 스크립트 추가
   - 설정:
     - Player Prefab: 생성한 플레이어 프리팹
     - Player Class: Warrior
     - Monster Prefab: 생성한 몬스터 프리팹
     - Monster Count: 5
     - Spawn Radius: 10

4. **메인 카메라 설정**
   - Main Camera 선택
   - Projection: Orthographic
   - Size: 6
   - Background: 적절한 색상
   - IsometricCamera.cs 스크립트 추가
   - 설정:
     - Auto Find Player: ✅
     - Smooth Speed: 0.125
     - Iso Angle: 45

5. **그라운드 추가** (옵션)
   - Hierarchy > 2D Object > Sprite
   - 이름: Ground
   - Scale을 크게 조정하여 바닥 생성

---

## 🎯 조작 방법

### 기본 조작
- **이동**: WASD 또는 방향키
- **기본 공격**: 마우스 좌클릭
- **스킬 1-4**: Q, W, E, R
- **인벤토리**: I
- **일시정지**: ESC

### 카메라 조작
- **줌 인/아웃**: 마우스 휠

---

## 🧪 테스트 방법

### 1. 기본 테스트
1. Unity에서 TestScene 열기
2. Play 버튼 클릭
3. 확인사항:
   - 플레이어가 중앙에 생성되는가?
   - 몬스터가 랜덤 위치에 생성되는가?
   - 카메라가 플레이어를 따라가는가?

### 2. 이동 테스트
- WASD로 이동
- 플레이어가 부드럽게 움직이는지 확인
- 애니메이션이 방향에 맞게 재생되는지 확인

### 3. 전투 테스트
- 몬스터 근처로 이동
- 마우스 좌클릭으로 공격
- 몬스터 HP가 감소하는지 확인
- 몬스터가 사망하면 사라지는지 확인

### 4. AI 테스트
- 몬스터의 탐지 범위 밖에서 관찰
- 몬스터가 Idle/Patrol 상태인지 확인
- 탐지 범위 안으로 들어가기
- 몬스터가 추적하는지 확인
- 공격 범위 안에서 공격하는지 확인

---

## 🐛 문제 해결

### Q1: 플레이어/몬스터가 생성되지 않음
- Prefab이 제대로 할당되었는지 확인
- 콘솔에 에러 메시지가 있는지 확인

### Q2: 충돌이 안됨
- Layer 설정이 올바른지 확인
- Physics 2D Matrix에서 충돌 설정 확인
- Collider가 활성화되어 있는지 확인

### Q3: 카메라가 따라가지 않음
- IsometricCamera의 Target이 설정되었는지 확인
- Auto Find Player가 체크되어 있는지 확인
- Player 태그가 올바르게 설정되었는지 확인

### Q4: 공격이 안됨
- Attack Range 설정 확인
- Target Layer 설정 확인
- 공격 애니메이션 트리거 확인

---

## 📊 성능 최적화 팁

1. **Object Pooling** (다음 단계에서 구현)
   - 몬스터, 발사체, 이펙트 등

2. **Rigidbody2D 설정**
   - Sleeping Mode: Start Awake
   - Interpolate: Interpolate (부드러운 움직임)

3. **Collider 최적화**
   - 복잡한 Polygon Collider 대신 Circle/Box 사용
   - Trigger로 설정 가능한 것은 Trigger 사용

---

## 🎨 스프라이트 활용

업로드하신 25개 에셋을 다음과 같이 활용하세요:

### 캐릭터
- `player_character_4dir.png` → Player 스프라이트

### 몬스터
- `goblin.png` → 고블린 몬스터
- `orc_warrior.png` → 오크 전사
- `green_slime.png` → 슬라임

### VFX
- `cyan_slash.png` → 공격 이펙트
- `purple_red_explosion.png` → 스킬 이펙트
- `green_heal_common.png` → 회복 이펙트

### UI
- `red_heart.png` → HP 아이콘
- `golden_star_icon.png` → 경험치/레벨

---

## 🔜 다음 단계 (3단계)

3단계에서는 다음을 구현합니다:
- ✅ 4개 직업별 스킬 시스템
- ✅ 스킬 이펙트
- ✅ UI (HP/MP 바, 스킬 쿨다운)
- ✅ 데미지 텍스트

2단계 완료를 확인하신 후 3단계로 진행하시면 됩니다!

---

**현재 진행도**: 2/6 단계 완료 (33%)
