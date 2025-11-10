# Babel Tower - 3단계 구현 완료

## ✅ 구현 완료 내용

### 1. 스킬 시스템 (총 16개 스킬)

#### 전사 스킬 (Warrior)
- ✅ **돌진 (Charge)** - 전방으로 돌진하며 경로상 적 공격 (Q)
- ✅ **회전베기 (Whirlwind)** - 360도 범위 공격, 3회 타격 (W)
- ✅ **방패막기 (Shield Block)** - 3초간 피해 50% 감소 (E)
- ✅ **전쟁의 함성 (War Cry)** - 5초간 공격력 30% 증가 (R)

#### 마법사 스킬 (Mage)
- ✅ **파이어볼 (Fireball)** - 화염 발사체, 단일 대상 고데미지 (Q)
- ✅ **프로즌 오브 (Frozen Orb)** - 느린 얼음 구체, 범위 지속 데미지 (W)
- ✅ **라이트닝 (Lightning)** - 연쇄 번개, 최대 3회 전이 (E)
- ✅ **메테오 (Meteor)** - 1.5초 후 운석 낙하, 광역 폭발 (R)

#### 도적 스킬 (Rogue)
- ✅ **백스탭 (Backstab)** - 적 뒤로 순간이동 후 2배 데미지 (Q)
- ✅ **연막탄 (Smoke Bomb)** - 3초간 은신 + 이속 증가 (W)
- ✅ **독살 (Poison Strike)** - 독 데미지 + 5초 지속 피해 (E)
- ✅ **그림자 은신 (Shadow Stealth)** - 5초간 완전 은신, 다음 공격 치명타 (R)

#### 궁수 스킬 (Archer)
- ✅ **관통 사격 (Piercing Shot)** - 직선상 모든 적 관통 (Q)
- ✅ **멀티샷 (Multi Shot)** - 3발의 화살 부채꼴 발사 (W)
- ✅ **폭발 화살 (Explosive Arrow)** - 착탄 시 범위 폭발 (E)
- ✅ **저격 (Snipe)** - 2초 차징, 초장거리 3배 데미지 (R)

### 2. UI 시스템
- ✅ **PlayerHUD** - HP/MP 바, 레벨, 골드 표시
- ✅ **SkillSlotUI** - 스킬 아이콘 및 쿨다운 표시
- ✅ **DamageText** - 데미지 숫자 표시 (일반/치명타)
- ✅ **DamageTextPool** - Object Pooling 최적화
- ✅ **UIManager** - UI 통합 관리

### 3. 전투 이펙트
- ✅ 데미지 텍스트 (위로 떠오르며 페이드)
- ✅ 치명타 강조 (노란색, 큰 폰트)
- ✅ 회복 표시 (초록색, + 기호)

---

## 📁 생성된 파일 목록

```
BabelTower/Scripts/
├── Combat/
│   └── Skills/
│       ├── WarriorChargeSkill.cs
│       ├── WarriorWhirlwindSkill.cs
│       ├── WarriorShieldBlockSkill.cs
│       ├── WarriorWarCrySkill.cs
│       └── AllSkills.cs (마법사/도적/궁수 스킬 12개)
│
└── UI/
    ├── PlayerHUD.cs
    └── DamageTextSystem.cs
```

**총 7개 파일 추가**

---

## 🎮 Unity 설정 가이드

### 1단계: ScriptableObject 스킬 생성

모든 스킬은 ScriptableObject로 생성되어야 합니다.

#### 전사 스킬 생성 예시:

1. **Project 창에서 우클릭**
   - Create > Skills > Warrior > Charge

2. **생성된 에셋 설정**
   - Skill Name: "돌진"
   - Description: "전방으로 돌진하며 경로상의 적 공격"
   - Cooldown: 8
   - Mana Cost: 20
   - Damage Multiplier: 1.5
   - Range: 5
   - Skill Type: Melee

3. **아이콘 설정**
   - Icon: 업로드한 스프라이트 중 적절한 것 선택

4. **이펙트 설정** (옵션)
   - Cast Effect: 시전 이펙트 프리팹
   - Hit Effect: 적중 이펙트 프리팹

#### 모든 스킬 생성:

**전사 (Warrior):**
- Warrior_Charge (쿨다운: 8초, 마나: 20, 배율: 1.5x)
- Warrior_Whirlwind (쿨다운: 12초, 마나: 35, 배율: 2.0x)
- Warrior_ShieldBlock (쿨다운: 15초, 마나: 25, 배율: 0x - 방어 스킬)
- Warrior_WarCry (쿨다운: 20초, 마나: 40, 배율: 0x - 버프 스킬)

**마법사 (Mage):**
- Mage_Fireball (쿨다운: 5초, 마나: 25, 배율: 2.0x)
- Mage_FrozenOrb (쿨다운: 10초, 마나: 40, 배율: 1.5x)
- Mage_Lightning (쿨다운: 8초, 마나: 35, 배율: 1.8x)
- Mage_Meteor (쿨다운: 15초, 마나: 60, 배율: 3.0x)

**도적 (Rogue):**
- Rogue_Backstab (쿨다운: 6초, 마나: 30, 배율: 2.5x)
- Rogue_SmokeBomb (쿨다운: 12초, 마나: 25, 배율: 0x)
- Rogue_PoisonStrike (쿨다운: 8초, 마나: 30, 배율: 1.5x)
- Rogue_ShadowStealth (쿨다운: 20초, 마나: 40, 배율: 0x)

**궁수 (Archer):**
- Archer_PiercingShot (쿨다운: 6초, 마나: 25, 배율: 1.5x)
- Archer_MultiShot (쿨다운: 10초, 마나: 35, 배율: 1.2x)
- Archer_ExplosiveArrow (쿨다운: 12초, 마나: 45, 배율: 2.0x)
- Archer_Snipe (쿨다운: 15초, 마나: 50, 배율: 3.0x)

### 2단계: 플레이어 프리팹에 스킬 할당

1. **Player 프리팹 열기**

2. **Player 컴포넌트 찾기**
   - Skills 리스트 확장
   - Size: 4

3. **스킬 할당**
   - Element 0: 생성한 스킬 Q
   - Element 1: 생성한 스킬 W
   - Element 2: 생성한 스킬 E
   - Element 3: 생성한 스킬 R

### 3단계: HUD 캔버스 생성

1. **Canvas 생성**
   - Hierarchy > UI > Canvas
   - 이름: PlayerHUD_Canvas
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler 설정:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920x1080

2. **HP 바 생성**
   - Canvas 하위 > UI > Slider
   - 이름: HPBar
   - 위치: 왼쪽 상단 (Anchor: Top-Left)
   - Position: X=150, Y=-50
   - Size: Width=300, Height=30
   - Fill Area > Fill: 색상을 빨간색으로

3. **MP 바 생성**
   - HPBar 복제
   - 이름: MPBar
   - Position: X=150, Y=-90
   - Fill 색상: 파란색

4. **HP/MP 텍스트 추가**
   - Slider 하위 > UI > Text
   - 이름: HPText / MPText
   - Alignment: Center-Middle
   - 색상: 흰색
   - Font Size: 16

5. **스킬 슬롯 생성**
   - Canvas 하위 > UI > Image
   - 이름: SkillSlot_Q
   - 위치: 하단 중앙
   - Position: X=-200, Y=80
   - Size: 64x64
   - 색상: 반투명 회색

6. **스킬 슬롯 복제**
   - SkillSlot_Q 복제 3개
   - 이름: SkillSlot_W, SkillSlot_E, SkillSlot_R
   - X 위치를 80씩 증가 (-120, -40, 40)

7. **각 스킬 슬롯에 자식 추가:**
   ```
   SkillSlot_Q/
   ├── Icon (Image) - 스킬 아이콘
   ├── Cooldown (Image) - 검은색 반투명, Fill Amount
   ├── CooldownText (Text) - 숫자 표시
   └── HotkeyText (Text) - "Q" 표시
   ```

8. **PlayerHUD 스크립트 추가**
   - Canvas에 PlayerHUD.cs 컴포넌트 추가
   - 각 UI 요소 할당:
     - HP Bar → HPBar
     - MP Bar → MPBar
     - HP Text → HPText
     - MP Text → MPText
     - Skill Slots → 4개 스킬 슬롯

### 4단계: 데미지 텍스트 설정

1. **UIManager 오브젝트 생성**
   - Hierarchy > Create Empty
   - 이름: UIManager
   - UIManager.cs 스크립트 추가

2. **DamageTextCanvas 생성**
   - Hierarchy > UI > Canvas
   - 이름: DamageTextCanvas
   - Render Mode: World Space
   - Canvas Scaler 추가
     - Dynamic Pixels Per Unit: 10

3. **DamageTextPool 추가**
   - UIManager에 DamageTextPool.cs 컴포넌트 추가
   - World Canvas: DamageTextCanvas 할당

4. **데미지 텍스트 프리팹 생성** (옵션)
   - Canvas 하위 > UI > Text
   - 이름: DamageText
   - Font: Arial (또는 원하는 폰트)
   - Font Size: 24
   - Alignment: Center-Middle
   - Color: White
   - DamageText.cs 스크립트 추가
   - CanvasGroup 컴포넌트 추가
   - Prefabs 폴더로 드래그하여 프리팹 생성
   - DamageTextPool의 Damage Text Prefab에 할당

---

## 🧪 테스트 방법

### 1. 스킬 테스트

1. **게임 실행**
2. **스킬 사용**
   - Q 키: 첫 번째 스킬 시전
   - W 키: 두 번째 스킬 시전
   - E 키: 세 번째 스킬 시전
   - R 키: 네 번째 스킬 시전

3. **확인사항**
   - 마나가 소모되는가?
   - 쿨다운이 표시되는가?
   - 스킬 효과가 작동하는가?
   - 데미지가 적용되는가?

### 2. UI 테스트

1. **HP/MP 바**
   - 몬스터에게 맞으면 HP 감소
   - 스킬 사용 시 MP 감소
   - 바와 텍스트가 동기화되는가?

2. **스킬 쿨다운**
   - 스킬 사용 시 쿨다운 표시
   - 숫자가 카운트다운되는가?
   - Fill Amount가 줄어드는가?

3. **데미지 텍스트**
   - 공격 시 데미지 숫자 표시
   - 치명타는 노란색 + 큰 폰트
   - 위로 떠오르며 페이드 아웃

### 3. 직업별 스킬 테스트

#### 전사 테스트
- 돌진: 전방으로 빠르게 이동하며 공격
- 회전베기: 주변 360도 공격
- 방패막기: 피해 감소 확인
- 전쟁의 함성: 공격력 증가 확인

#### 마법사 테스트
- 파이어볼: 발사체가 날아가는가?
- 프로즌 오브: 느리게 이동하며 지속 데미지
- 라이트닝: 적 간 연쇄 공격
- 메테오: 1.5초 후 폭발

#### 도적 테스트
- 백스탭: 적 뒤로 순간이동
- 연막탄: 투명해지는가?
- 독살: 지속 데미지 확인
- 그림자 은신: 완전 투명화

#### 궁수 테스트
- 관통 사격: 여러 적 관통
- 멀티샷: 3발 발사
- 폭발 화살: 범위 폭발
- 저격: 2초 후 고데미지

---

## 🎨 스프라이트 활용

업로드하신 에셋을 스킬 이펙트로 활용:

### 스킬 이펙트
- `cyan_slash.png` → 전사 돌진, 궁수 관통사격
- `purple_red_explosion.png` → 마법사 파이어볼, 메테오
- `golden_fireworks.png` → 치명타 이펙트
- `white_smoke.png` → 도적 연막탄
- `green_heal_common.png` → 회복 이펙트 (추후)

### UI 아이콘
- 스킬 아이콘으로 사용 가능
- HP/MP 아이콘

---

## 🐛 문제 해결

### Q1: 스킬이 시전되지 않음
- 마나가 충분한지 확인
- 쿨다운이 끝났는지 확인
- Player 컴포넌트에 스킬이 할당되었는지 확인
- ScriptableObject가 제대로 생성되었는지 확인

### Q2: UI가 표시되지 않음
- Canvas가 활성화되어 있는지 확인
- Camera 설정이 올바른지 확인 (Screen Space Overlay)
- PlayerHUD 스크립트에 모든 참조가 할당되었는지 확인

### Q3: 데미지 텍스트가 안보임
- DamageTextCanvas가 World Space인지 확인
- DamageTextPool이 초기화되었는지 확인
- UIManager가 씬에 있는지 확인

### Q4: 쿨다운이 표시되지 않음
- SkillSlotUI의 MaxCooldown이 설정되었는지 확인
- Cooldown Overlay의 Fill Amount가 1로 시작하는지 확인
- Image Type이 Filled인지 확인

---

## ⚡ 성능 최적화

### Object Pooling 적용
- ✅ DamageText (이미 구현됨)
- ⏳ Projectile (다음 단계)
- ⏳ Effect (다음 단계)

### UI 최적화
- Canvas를 적절히 분리
- Batch를 고려한 아틀라스 사용
- Text 대신 TextMeshPro 사용 권장

---

## 🔜 다음 단계 (4단계)

4단계에서는 **던전 시스템**을 구현합니다:
- ✅ 랜덤 맵 생성 (BSP 알고리즘)
- ✅ 몬스터 스폰 시스템
- ✅ 아이템 드랍 시스템
- ✅ 던전 입장/퇴장
- ✅ 5-10분 던전 타이머

---

**현재 진행도**: 3/6 단계 완료 (50%)

모든 스킬이 정상 작동하는지 확인하신 후 4단계로 진행하시면 됩니다!
