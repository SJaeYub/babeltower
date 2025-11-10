# Babel Tower - Unity 완전 구현 가이드 (2~3단계)

## 📋 목차
1. [프로젝트 초기 설정](#1-프로젝트-초기-설정)
2. [스크립트 임포트](#2-스크립트-임포트)
3. [레이어 및 태그 설정](#3-레이어-및-태그-설정)
4. [플레이어 프리팹 생성](#4-플레이어-프리팹-생성)
5. [몬스터 프리팹 생성](#5-몬스터-프리팹-생성)
6. [스킬 생성 (ScriptableObject)](#6-스킬-생성)
7. [UI 구성](#7-ui-구성)
8. [테스트 씬 설정](#8-테스트-씬-설정)
9. [테스트 및 디버깅](#9-테스트-및-디버깅)

---

## 1. 프로젝트 초기 설정

### 1-1. Unity 프로젝트 생성

1. **Unity Hub 실행**
2. **New Project 클릭**
3. **프로젝트 설정:**
   - Template: **2D (Universal Render Pipeline)** 또는 **2D Core**
   - Project Name: `BabelTower`
   - Location: 원하는 경로
   - Unity Version: **2021.3 LTS** 이상
4. **Create Project 클릭**

### 1-2. 기본 폴더 구조 생성

Project 창에서 다음 폴더들을 생성하세요:

```
Assets/
├── Scenes/
├── Scripts/
│   ├── Character/
│   ├── Combat/
│   │   └── Skills/
│   ├── Manager/
│   └── UI/
├── Prefabs/
│   ├── Characters/
│   ├── UI/
│   └── Effects/
├── Sprites/
│   ├── Characters/
│   ├── Effects/
│   └── UI/
├── Skills/
│   ├── Warrior/
│   ├── Mage/
│   ├── Rogue/
│   └── Archer/
└── Materials/
```

**폴더 생성 방법:**
- Project 창에서 우클릭 > Create > Folder

---

## 2. 스크립트 임포트

### 2-1. 스크립트 파일 복사

1. `/home/claude/BabelTower/Scripts/` 폴더의 모든 `.cs` 파일을
2. Unity 프로젝트의 `Assets/Scripts/` 폴더로 복사

**복사할 파일 목록:**
```
Character/
├── Character.cs
├── Player.cs
└── Monster.cs

Combat/
├── CombatSystem.cs
└── Skills/
    ├── WarriorChargeSkill.cs
    ├── WarriorWhirlwindSkill.cs
    ├── WarriorShieldBlockSkill.cs
    ├── WarriorWarCrySkill.cs
    └── AllSkills.cs

Manager/
└── GameManager.cs

UI/
├── PlayerHUD.cs
└── DamageTextSystem.cs

(루트)
├── IsometricCamera.cs
└── TestSceneSetup.cs
```

### 2-2. 컴파일 확인

- Unity로 돌아가면 자동으로 스크립트 컴파일
- **Console 창** (Window > General > Console) 확인
- 에러가 없어야 함

**주의사항:**
- `using BabelTower.XXX` namespace가 올바른지 확인
- 모든 스크립트가 올바른 폴더에 있는지 확인

---

## 3. 레이어 및 태그 설정

### 3-1. 레이어 설정

1. **Edit > Project Settings > Tags and Layers**
2. **Layers 섹션에서 다음 추가:**

| Layer # | Name |
|---------|------|
| 6 | Player |
| 7 | Enemy |
| 8 | Projectile |
| 9 | Ground |

**설정 방법:**
- Layer 6 클릭 > "Player" 입력
- Layer 7 클릭 > "Enemy" 입력
- Layer 8 클릭 > "Projectile" 입력
- Layer 9 클릭 > "Ground" 입력

### 3-2. 태그 설정

**Tags 섹션에서 다음 추가:**
- `Player`
- `Enemy`

**추가 방법:**
- Tags 섹션의 + 버튼 클릭
- 태그 이름 입력

### 3-3. Physics 2D 충돌 설정

1. **Edit > Project Settings > Physics 2D**
2. **Layer Collision Matrix 설정:**

| Layer | Player | Enemy | Projectile |
|-------|--------|-------|------------|
| **Player** | ❌ | ✅ | ❌ |
| **Enemy** | ✅ | ❌ | ✅ |
| **Projectile** | ❌ | ✅ | ❌ |

**설정 방법:**
- 체크박스를 클릭하여 충돌 허용/차단
- ✅ = 충돌 허용
- ❌ = 충돌 차단

---

## 4. 플레이어 프리팹 생성

### 4-1. 스프라이트 준비

1. **업로드한 스프라이트를 Unity로 가져오기:**
   - `player_character_4dir.png` → `Assets/Sprites/Characters/`에 복사
   
2. **스프라이트 설정:**
   - 스프라이트 선택
   - Inspector > Texture Type: **Sprite (2D and UI)**
   - Pixels Per Unit: **100** (또는 16/32 - 스프라이트 크기에 따라)
   - Filter Mode: **Point (no filter)** (픽셀 아트용)
   - Compression: **None**
   - Apply 클릭

3. **스프라이트 슬라이싱** (4방향 스프라이트인 경우):
   - Sprite Editor 클릭 (Install 필요시 설치)
   - Slice > Grid By Cell Size
   - 각 방향별로 분리
   - Apply

### 4-2. 플레이어 GameObject 생성

1. **Hierarchy에서 빈 GameObject 생성:**
   - 우클릭 > Create Empty
   - 이름: `Player`

2. **Transform 설정:**
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

### 4-3. 컴포넌트 추가

#### A. Sprite Renderer

1. **Add Component > Rendering > Sprite Renderer**
2. **설정:**
   - Sprite: `player_character_4dir` (또는 Down 방향 스프라이트)
   - Color: White
   - Sorting Layer: **Default**
   - Order in Layer: **10**

#### B. Rigidbody2D

1. **Add Component > Physics 2D > Rigidbody2D**
2. **설정:**
   - Body Type: **Dynamic**
   - Material: None
   - Simulated: ✅
   - Use Auto Mass: ❌
   - Mass: **1**
   - Linear Drag: **0**
   - Angular Drag: **0.05**
   - Gravity Scale: **0** (탑뷰이므로 중력 없음)
   - Constraints:
     - Freeze Rotation: **Z 체크** ✅

#### C. Collider 2D

1. **Add Component > Physics 2D > Circle Collider 2D**
2. **설정:**
   - Is Trigger: ❌
   - Radius: **0.4** (캐릭터 크기에 맞게 조정)
   - Offset: (0, 0)

#### D. Animator (옵션)

1. **Add Component > Animation > Animator**
2. **설정:**
   - Controller: (나중에 생성)
   - Apply Root Motion: ❌

### 4-4. Player 스크립트 추가

1. **Add Component > Scripts > Player**
2. **Inspector에서 설정:**

```
Player (Script)
├── [Character Name]: "Hero"
├── [Level]: 1
├── [Max HP]: 100
├── [Max MP]: 50
├── [Attack]: 10
├── [Defense]: 5
├── [Move Speed]: 5
├── [Attack Speed]: 1
├── [Critical Chance]: 0.1
├── [Critical Damage]: 1.5
├── [Attack Range]: 1.5
├── [Target Layer]: Enemy (레이어 선택)
└── [Player Class]: Warrior (드롭다운)
```

### 4-5. 태그 및 레이어 설정

- **Tag**: Player
- **Layer**: Player

### 4-6. 프리팹 저장

1. **Hierarchy에서 Player 선택**
2. **Assets/Prefabs/Characters/ 폴더로 드래그**
3. **이름 확인: Player.prefab**

---

## 5. 몬스터 프리팹 생성

### 5-1. 고블린 몬스터 생성

#### 스프라이트 준비
- `goblin.png` → `Assets/Sprites/Characters/`에 복사
- 플레이어와 동일하게 설정

#### GameObject 생성

1. **Create Empty > 이름: Monster_Goblin**
2. **컴포넌트 추가:**

**A. Sprite Renderer**
```
- Sprite: goblin
- Order in Layer: 10
```

**B. Rigidbody2D**
```
- Body Type: Dynamic
- Gravity Scale: 0
- Freeze Rotation Z: ✅
```

**C. Circle Collider 2D**
```
- Radius: 0.4
```

**D. Monster 스크립트**
```
Monster (Script)
├── [Character Name]: "Goblin"
├── [Level]: 1
├── [Max HP]: 50
├── [Attack]: 8
├── [Defense]: 3
├── [Move Speed]: 3
├── [Attack Range]: 1.5
├── [Target Layer]: Player
├── [Monster Type]: Normal
├── [Exp Reward]: 10
├── [Gold Reward]: 5
├── [Detection Range]: 5
└── [Chase Speed]: 3
```

**E. 태그 및 레이어**
- Tag: Enemy
- Layer: Enemy

#### 프리팹 저장
- `Assets/Prefabs/Characters/Monster_Goblin.prefab`

### 5-2. 다른 몬스터 생성 (옵션)

동일한 방법으로:
- **Monster_Orc** (`orc_warrior.png`)
- **Monster_Slime** (`green_slime.png`)

---

## 6. 스킬 생성 (ScriptableObject)

### 6-1. 전사 스킬 생성

#### 스킬 1: 돌진 (Charge)

1. **Project 창에서:**
   - Assets/Skills/Warrior 폴더로 이동
   - 우클릭 > **Create > Skills > Warrior > Charge**

2. **생성된 에셋 선택 (Warrior_Charge)**

3. **Inspector에서 설정:**
```
Warrior Charge Skill (ScriptableObject)
├── [Skill Name]: "돌진"
├── [Description]: "전방으로 돌진하며 경로상의 적 공격"
├── [Icon]: (스프라이트 할당 - 옵션)
├── [Cooldown]: 8
├── [Mana Cost]: 20
├── [Damage Multiplier]: 1.5
├── [Skill Type]: Melee
├── [Target Type]: Single
├── [Range]: 5
├── [Area Radius]: 0
├── [Charge Distance]: 5
├── [Charge Speed]: 20
├── [Charge Duration]: 0.3
├── [Cast Effect]: (프리팹 - 옵션)
└── [Hit Effect]: (프리팹 - 옵션)
```

#### 스킬 2: 회전베기 (Whirlwind)

- **Create > Skills > Warrior > Whirlwind**
```
├── [Skill Name]: "회전베기"
├── [Description]: "주변 360도 범위 공격"
├── [Cooldown]: 12
├── [Mana Cost]: 35
├── [Damage Multiplier]: 2.0
├── [Skill Type]: AOE
├── [Radius]: 3
├── [Hit Count]: 3
└── [Hit Interval]: 0.2
```

#### 스킬 3: 방패막기 (Shield Block)

- **Create > Skills > Warrior > ShieldBlock**
```
├── [Skill Name]: "방패막기"
├── [Description]: "3초간 피해 50% 감소"
├── [Cooldown]: 15
├── [Mana Cost]: 25
├── [Skill Type]: Buff
├── [Duration]: 3
└── [Damage Reduction]: 0.5
```

#### 스킬 4: 전쟁의 함성 (War Cry)

- **Create > Skills > Warrior > WarCry**
```
├── [Skill Name]: "전쟁의 함성"
├── [Description]: "5초간 공격력 30% 증가"
├── [Cooldown]: 20
├── [Mana Cost]: 40
├── [Skill Type]: Buff
├── [Duration]: 5
└── [Attack Boost]: 0.3
```

### 6-2. 마법사 스킬 생성

동일한 방법으로:

1. **Mage_Fireball**
```
├── [Skill Name]: "파이어볼"
├── [Cooldown]: 5
├── [Mana Cost]: 25
├── [Damage Multiplier]: 2.0
├── [Projectile Speed]: 15
└── [Explosion Radius]: 1.5
```

2. **Mage_FrozenOrb**
```
├── [Skill Name]: "프로즌 오브"
├── [Cooldown]: 10
├── [Mana Cost]: 40
├── [Damage Multiplier]: 1.5
├── [Orb Speed]: 5
├── [Tick Interval]: 0.5
└── [Duration]: 3
```

3. **Mage_Lightning**
```
├── [Skill Name]: "라이트닝"
├── [Cooldown]: 8
├── [Mana Cost]: 35
├── [Damage Multiplier]: 1.8
├── [Chain Count]: 3
└── [Chain Range]: 4
```

4. **Mage_Meteor**
```
├── [Skill Name]: "메테오"
├── [Cooldown]: 15
├── [Mana Cost]: 60
├── [Damage Multiplier]: 3.0
├── [Delay]: 1.5
└── [Impact Radius]: 3
```

### 6-3. 도적 & 궁수 스킬 생성

**도적 스킬:**
- Rogue_Backstab (쿨다운: 6, 마나: 30, 배율: 2.5x)
- Rogue_SmokeBomb (쿨다운: 12, 마나: 25)
- Rogue_PoisonStrike (쿨다운: 8, 마나: 30, 배율: 1.5x)
- Rogue_ShadowStealth (쿨다운: 20, 마나: 40)

**궁수 스킬:**
- Archer_PiercingShot (쿨다운: 6, 마나: 25, 배율: 1.5x)
- Archer_MultiShot (쿨다운: 10, 마나: 35, 배율: 1.2x)
- Archer_ExplosiveArrow (쿨다운: 12, 마나: 45, 배율: 2.0x)
- Archer_Snipe (쿨다운: 15, 마나: 50, 배율: 3.0x)

### 6-4. 플레이어 프리팹에 스킬 할당

1. **Player 프리팹 열기** (더블클릭)
2. **Player 스크립트 찾기**
3. **Skills 리스트 확장:**
   - Size: **4**
   - Element 0: Warrior_Charge
   - Element 1: Warrior_Whirlwind
   - Element 2: Warrior_ShieldBlock
   - Element 3: Warrior_WarCry

---

## 7. UI 구성

### 7-1. Canvas 생성

1. **Hierarchy > 우클릭 > UI > Canvas**
2. **이름: PlayerHUD_Canvas**
3. **Canvas 설정:**
```
Canvas
├── Render Mode: Screen Space - Overlay
├── Pixel Perfect: ✅
└── Sort Order: 0

Canvas Scaler
├── UI Scale Mode: Scale With Screen Size
├── Reference Resolution: 1920 x 1080
├── Screen Match Mode: Match Width Or Height
└── Match: 0.5
```

### 7-2. HP 바 생성

1. **Canvas 우클릭 > UI > Slider**
2. **이름: HPBar**
3. **RectTransform:**
```
├── Anchor Preset: Top-Left
├── Anchor: Min(0, 1), Max(0, 1)
├── Pivot: (0, 1)
├── Pos X: 20
├── Pos Y: -20
├── Width: 300
└── Height: 30
```

4. **Slider 설정:**
```
├── Interactable: ❌
├── Transition: None
├── Min Value: 0
├── Max Value: 1
└── Value: 1
```

5. **자식 오브젝트 수정:**

**Background:**
```
└── Color: 검은색 (50% 투명도)
```

**Fill Area > Fill:**
```
├── Color: 빨간색 (255, 0, 0)
└── Image Type: Filled
```

**Handle Slide Area:**
```
└── (비활성화 또는 삭제)
```

6. **HP 텍스트 추가:**
   - HPBar 우클릭 > UI > Text
   - 이름: HPText
```
Text
├── Text: "100/100"
├── Font: Arial
├── Font Size: 16
├── Alignment: Center-Middle
├── Color: 흰색
├── Best Fit: ✅
```

### 7-3. MP 바 생성

1. **HPBar 복제 (Ctrl+D)**
2. **이름: MPBar**
3. **RectTransform:**
```
├── Pos Y: -60
```

4. **Fill 색상 변경:**
```
└── Color: 파란색 (0, 100, 255)
```

5. **텍스트 변경:**
```
└── Text: "50/50"
```

### 7-4. 레벨/골드 텍스트

1. **Canvas 우클릭 > UI > Text**
2. **이름: LevelText**
```
RectTransform
├── Anchor: Top-Left
├── Pos X: 20
├── Pos Y: -100
├── Width: 100
└── Height: 30

Text
├── Text: "Lv.1"
├── Font Size: 20
└── Color: 노란색
```

3. **골드 텍스트 생성:**
```
Text - GoldText
├── Anchor: Top-Right
├── Pos X: -20
├── Text: "Gold: 0"
```

### 7-5. 스킬 슬롯 생성

#### 기본 슬롯

1. **Canvas 우클릭 > UI > Image**
2. **이름: SkillSlot_Q**
3. **RectTransform:**
```
├── Anchor: Bottom-Center
├── Anchor: Min(0.5, 0), Max(0.5, 0)
├── Pivot: (0.5, 0)
├── Pos X: -180
├── Pos Y: 20
├── Width: 80
└── Height: 80
```

4. **Image 설정:**
```
├── Source Image: (None 또는 배경 이미지)
├── Color: 회색 (128, 128, 128, 200)
└── Image Type: Simple
```

#### 자식 오브젝트 추가

**A. 스킬 아이콘**
```
SkillSlot_Q/SkillIcon (Image)
├── Anchor: Stretch
├── Left, Top, Right, Bottom: 5
├── Color: 흰색
└── Preserve Aspect: ✅
```

**B. 쿨다운 오버레이**
```
SkillSlot_Q/CooldownOverlay (Image)
├── Anchor: Stretch
├── Offsets: 0
├── Color: 검은색 (0, 0, 0, 180)
├── Image Type: Filled
├── Fill Method: Radial 360
├── Fill Origin: Top
└── Fill Amount: 0
```

**C. 쿨다운 텍스트**
```
SkillSlot_Q/CooldownText (Text)
├── Anchor: Middle-Center
├── Text: ""
├── Font Size: 32
├── Alignment: Center-Middle
├── Color: 흰색
└── Best Fit: ✅
```

**D. 단축키 텍스트**
```
SkillSlot_Q/HotkeyText (Text)
├── Anchor: Bottom-Right
├── Pivot: (1, 0)
├── Pos X: -5
├── Pos Y: 5
├── Width: 30
├── Height: 20
├── Text: "Q"
├── Font Size: 16
└── Color: 노란색
```

#### 나머지 슬롯 복제

1. **SkillSlot_Q 복제 3번 (Ctrl+D)**
2. **이름 및 위치 변경:**

```
SkillSlot_W
├── Pos X: -60
└── HotkeyText: "W"

SkillSlot_E
├── Pos X: 60
└── HotkeyText: "E"

SkillSlot_R
├── Pos X: 180
└── HotkeyText: "R"
```

### 7-6. PlayerHUD 스크립트 추가

1. **PlayerHUD_Canvas 선택**
2. **Add Component > Scripts > Player HUD**
3. **참조 할당:**

```
Player HUD (Script)
├── [HP Bar]: HPBar (Slider)
├── [MP Bar]: MPBar (Slider)
├── [HP Text]: HPText (Text)
├── [MP Text]: MPText (Text)
├── [Level Text]: LevelText
├── [Gold Text]: GoldText
├── [Exp Bar]: (옵션)
└── [Skill Slots]: (Array)
    ├── Size: 4
    ├── Element 0: SkillSlot_Q
    ├── Element 1: SkillSlot_W
    ├── Element 2: SkillSlot_E
    └── Element 3: SkillSlot_R
```

### 7-7. 데미지 텍스트 Canvas

1. **Hierarchy > 우클릭 > UI > Canvas**
2. **이름: DamageTextCanvas**
3. **설정:**
```
Canvas
├── Render Mode: World Space
└── Sort Order: 100

Canvas Scaler
└── Dynamic Pixels Per Unit: 10
```

### 7-8. 데미지 텍스트 프리팹

1. **DamageTextCanvas 우클릭 > UI > Text**
2. **이름: DamageText**
3. **설정:**
```
Text
├── Text: "999"
├── Font: Arial
├── Font Size: 24
├── Alignment: Center-Middle
├── Color: 흰색
└── Rich Text: ✅

RectTransform
├── Width: 100
└── Height: 50
```

4. **Add Component:**
   - **Damage Text** 스크립트
   - **Canvas Group** 컴포넌트

5. **프리팹 저장:**
   - `Assets/Prefabs/UI/DamageText.prefab`로 드래그

### 7-9. UIManager 생성

1. **Hierarchy > Create Empty**
2. **이름: UIManager**
3. **Add Component:**
   - **UI Manager** 스크립트
   - **Damage Text Pool** 스크립트

4. **Damage Text Pool 설정:**
```
Damage Text Pool
├── [Damage Text Prefab]: DamageText (프리팹)
├── [Pool Size]: 20
└── [World Canvas]: DamageTextCanvas
```

---

## 8. 테스트 씬 설정

### 8-1. 씬 생성

1. **File > New Scene**
2. **저장: Assets/Scenes/TestScene.unity**

### 8-2. 카메라 설정

1. **Main Camera 선택**
2. **설정:**
```
Camera
├── Projection: Orthographic
├── Size: 6
├── Background: 회색 (50, 50, 50)
└── Culling Mask: Everything
```

3. **Add Component > Isometric Camera**
```
Isometric Camera
├── [Auto Find Player]: ✅
├── [Offset]: (0, 0, -10)
├── [Smooth Speed]: 0.125
├── [Iso Angle]: 45
├── [Use Bounds]: ❌
├── [Min Zoom]: 3
└── [Max Zoom]: 10
```

### 8-3. Ground 생성 (바닥)

1. **Hierarchy > 2D Object > Sprite > Square**
2. **이름: Ground**
3. **Transform:**
```
├── Position: (0, 0, 0)
└── Scale: (50, 50, 1)
```

4. **Sprite Renderer:**
```
├── Color: 어두운 녹색 (50, 100, 50)
├── Sorting Layer: Default
└── Order in Layer: 0
```

5. **Layer: Ground**

### 8-4. GameManager 추가

1. **Hierarchy > Create Empty**
2. **이름: GameManager**
3. **Add Component > Game Manager**
4. **Tag: GameController**

### 8-5. TestSceneSetup 추가

1. **Hierarchy > Create Empty**
2. **이름: SceneSetup**
3. **Add Component > Test Scene Setup**
4. **설정:**
```
Test Scene Setup
├── [Player Prefab]: Player (프리팹)
├── [Player Class]: Warrior
├── [Player Spawn Position]: (0, 0, 0)
├── [Monster Prefab]: Monster_Goblin (프리팹)
├── [Monster Count]: 5
├── [Spawn Radius]: 10
└── [Camera Prefab]: (None - Main Camera 사용)
```

### 8-6. 씬에 UI 추가

1. **PlayerHUD_Canvas 프리팹화**
   - Canvas를 `Assets/Prefabs/UI/`로 드래그

2. **씬에 배치**
   - 프리팹을 다시 씬으로 드래그

3. **DamageTextCanvas도 씬에 배치**

4. **UIManager 확인**
   - 모든 참조가 올바른지 확인

---

## 9. 테스트 및 디버깅

### 9-1. 첫 테스트 실행

1. **씬 저장 (Ctrl+S)**
2. **Play 버튼 클릭 ▶**

### 9-2. 확인 사항

#### ✅ 초기 생성
- [ ] 플레이어가 중앙에 생성되는가?
- [ ] 몬스터가 랜덤 위치에 생성되는가?
- [ ] UI가 표시되는가?
- [ ] 카메라가 플레이어를 바라보는가?

**문제 해결:**
- 생성 안됨: TestSceneSetup의 프리팹 할당 확인
- UI 안보임: Canvas 활성화 확인

#### ✅ 이동 테스트
- [ ] WASD로 이동 가능한가?
- [ ] 캐릭터가 부드럽게 움직이는가?
- [ ] 카메라가 따라오는가?

**문제 해결:**
- 이동 안됨: Rigidbody2D 설정 확인
- 카메라 안따라옴: IsometricCamera의 Target 확인

#### ✅ 전투 테스트
- [ ] 마우스 클릭으로 공격하는가?
- [ ] 몬스터가 데미지를 받는가?
- [ ] 데미지 텍스트가 표시되는가?
- [ ] HP 바가 줄어드는가?

**문제 해결:**
- 공격 안됨: Layer 설정 확인
- 데미지 안들어감: Physics 2D Matrix 확인
- 텍스트 안보임: UIManager, DamageTextPool 확인

#### ✅ 스킬 테스트
- [ ] Q/W/E/R로 스킬 사용되는가?
- [ ] MP가 소모되는가?
- [ ] 쿨다운이 표시되는가?
- [ ] 스킬 효과가 작동하는가?

**문제 해결:**
- 스킬 안나감: Player 프리팹의 Skills 할당 확인
- MP 부족: 초기 MP 값 확인
- 쿨다운 안보임: SkillSlotUI 참조 확인

#### ✅ AI 테스트
- [ ] 몬스터가 플레이어를 탐지하는가?
- [ ] 추적해서 다가오는가?
- [ ] 공격 범위에서 공격하는가?

**문제 해결:**
- AI 작동 안함: Monster 스크립트 에러 확인
- 탐지 안됨: Detection Range 증가

### 9-3. Console 창 확인

**Window > General > Console**

**일반적인 에러:**

1. **NullReferenceException**
   - 원인: 참조가 할당되지 않음
   - 해결: Inspector에서 모든 필수 참조 할당

2. **MissingReferenceException**
   - 원인: 삭제된 오브젝트 참조
   - 해결: 참조 다시 할당

3. **Layer/Tag not found**
   - 원인: 레이어/태그가 설정되지 않음
   - 해결: Project Settings에서 생성

### 9-4. 성능 확인

**Window > Analysis > Profiler**

- **FPS 확인:** 60 FPS 유지되는가?
- **CPU Usage:** 정상 범위인가?
- **GC Alloc:** 너무 많은 메모리 할당이 없는가?

### 9-5. 디버깅 팁

#### Debug.Log 활용
```csharp
// Character.cs의 TakeDamage에 추가
Debug.Log($"{characterName} took {damage} damage!");
```

#### Gizmo 활용
- 공격 범위, 탐지 범위가 Scene 뷰에 표시됨
- GameObject 선택 시 확인 가능

#### Inspector에서 실시간 확인
- Play 모드에서 값 변경 가능
- HP, MP, 스킬 쿨다운 등 실시간 확인

---

## 10. 빌드 및 최종 테스트

### 10-1. 빌드 설정

1. **File > Build Settings**
2. **Add Open Scenes** (TestScene 추가)
3. **Platform: Windows/Mac/Linux**
4. **Architecture: x86_64**

### 10-2. Player Settings

1. **Company Name:** 입력
2. **Product Name:** BabelTower
3. **Default Icon:** 설정 (옵션)
4. **Resolution:**
   - Default Resolution: 1920x1080
   - Fullscreen Mode: Windowed

### 10-3. 빌드 실행

1. **Build And Run** 클릭
2. **저장 위치 선택**
3. **빌드 완료 후 자동 실행**

---

## 📋 체크리스트

완전히 구현했는지 최종 확인:

### 프로젝트 설정
- [ ] Unity 프로젝트 생성 완료
- [ ] 폴더 구조 생성 완료
- [ ] 스크립트 모두 임포트
- [ ] 컴파일 에러 없음

### 기본 설정
- [ ] 레이어 6개 설정 (Player, Enemy, Projectile, Ground)
- [ ] 태그 2개 설정 (Player, Enemy)
- [ ] Physics 2D 충돌 매트릭스 설정

### 프리팹
- [ ] Player 프리팹 완성
- [ ] Monster_Goblin 프리팹 완성
- [ ] 스킬 16개 ScriptableObject 생성
- [ ] Player에 스킬 4개 할당

### UI
- [ ] PlayerHUD Canvas 완성
- [ ] HP/MP 바 작동
- [ ] 스킬 슬롯 4개 생성
- [ ] DamageText 프리팹 완성
- [ ] UIManager 설정 완료

### 씬
- [ ] TestScene 생성
- [ ] 카메라 설정 (Isometric)
- [ ] Ground 생성
- [ ] GameManager 배치
- [ ] TestSceneSetup 설정
- [ ] UI Canvas 배치

### 테스트
- [ ] 플레이어 생성 확인
- [ ] 몬스터 생성 확인
- [ ] 이동 테스트 통과
- [ ] 전투 테스트 통과
- [ ] 스킬 테스트 통과
- [ ] UI 작동 확인
- [ ] AI 작동 확인
- [ ] 에러 없음

---

## 🚨 문제 해결 가이드

### 자주 발생하는 문제

#### 1. 캐릭터가 생성되지 않음
**원인:**
- 프리팹이 할당되지 않음
- 스크립트 에러

**해결:**
1. TestSceneSetup의 Prefab 할당 확인
2. Console에서 에러 확인

#### 2. 충돌이 작동하지 않음
**원인:**
- Layer 설정 오류
- Collider 없음
- Physics Matrix 설정 오류

**해결:**
1. GameObject의 Layer 확인
2. Collider2D 컴포넌트 확인
3. Edit > Project Settings > Physics 2D 확인

#### 3. UI가 표시되지 않음
**원인:**
- Canvas 비활성화
- Camera 설정 오류

**해결:**
1. Canvas가 Active인지 확인
2. Canvas Render Mode 확인
3. Event Camera 설정 (Screen Space - Camera인 경우)

#### 4. 스킬이 작동하지 않음
**원인:**
- ScriptableObject 미할당
- 마나 부족
- 쿨다운 중

**해결:**
1. Player 프리팹의 Skills 배열 확인
2. 초기 MP 값 증가
3. Console에서 로그 확인

#### 5. 데미지 텍스트가 안보임
**원인:**
- UIManager 없음
- DamageTextPool 미설정
- Canvas가 World Space가 아님

**해결:**
1. UIManager GameObject 확인
2. DamageTextPool 컴포넌트 확인
3. DamageTextCanvas가 World Space인지 확인

---

## 📚 추가 리소스

### Unity 학습 자료
- Unity Learn: https://learn.unity.com
- Unity Documentation: https://docs.unity3d.com

### 픽셀 아트 도구
- Aseprite (유료)
- Piskel (무료, 웹 기반)

### 이펙트 제작
- Particle System 활용
- Sprite Animation

---

**구현 완료 후 4단계(던전 시스템)로 진행하시면 됩니다!**
