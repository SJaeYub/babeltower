using UnityEngine;
using System.Collections.Generic;
using BabelTower.Combat;

namespace BabelTower.Character
{
    /// <summary>
    /// 플레이어 직업
    /// </summary>
    public enum PlayerClass
    {
        Warrior,    // 전사
        Mage,       // 마법사
        Rogue,      // 도적
        Archer      // 궁수
    }

    /// <summary>
    /// 플레이어 캐릭터
    /// </summary>
    public class Player : Character
    {
        [Header("Player Specific")]
        [SerializeField] private PlayerClass playerClass = PlayerClass.Warrior;
        [SerializeField] private int experience;
        [SerializeField] private int experienceToNextLevel = 100;
        [SerializeField] private int gold;

        [Header("Skills")]
        [SerializeField] private List<Skill> skills = new List<Skill>();
        private float[] skillCooldowns = new float[4];

        [Header("Input")]
        private Vector2 moveInput;
        private Vector2 lastMoveDirection = Vector2.down;

        // Properties
        public PlayerClass Class => playerClass;
        public int Experience => experience;
        public int ExperienceToNextLevel => experienceToNextLevel;
        public int Gold => gold;
        public List<Skill> Skills => skills;

        // Singleton
        public static Player Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // 싱글톤 설정
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected override void InitializeCharacter()
        {
            base.InitializeCharacter();
            InitializeClassStats();
            LoadSkills();
        }

        private void Update()
        {
            if (IsDead) return;

            HandleInput();
            UpdateSkillCooldowns();
        }

        private void FixedUpdate()
        {
            if (IsDead) return;

            // 물리 기반 이동
            rb.linearVelocity = moveInput * moveSpeed;

            // 애니메이션 업데이트
            UpdateAnimation();
        }

        /// <summary>
        /// 입력 처리
        /// </summary>
        private void HandleInput()
        {
            // 이동 입력
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            // 마지막 이동 방향 저장 (멈췄을 때 방향 유지)
            if (moveInput.magnitude > 0.1f)
            {
                lastMoveDirection = moveInput;
            }

            // 기본 공격 (마우스 좌클릭)
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PerformAttack(mousePos);
            }

            // 스킬 입력 (Q, W, E, R)
            if (Input.GetKeyDown(KeyCode.Q)) UseSkill(0);
            if (Input.GetKeyDown(KeyCode.W)) UseSkill(1);
            if (Input.GetKeyDown(KeyCode.E)) UseSkill(2);
            if (Input.GetKeyDown(KeyCode.R)) UseSkill(3);

            // 인벤토리 (I)
            if (Input.GetKeyDown(KeyCode.I))
            {
                // TODO: 인벤토리 열기
            }
        }

        /// <summary>
        /// 이동
        /// </summary>
        public override void Move(Vector2 direction)
        {
            moveInput = direction;
        }

        /// <summary>
        /// 기본 공격
        /// </summary>
        public override void PerformAttack(Vector2 targetPosition)
        {
            if (!CanAttack()) return;

            lastAttackTime = Time.time;

            // 공격 방향으로 회전
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            lastMoveDirection = direction;

            // 애니메이션
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // 공격 범위 내 적 탐지
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

            foreach (var hit in hits)
            {
                Monster monster = hit.GetComponent<Monster>();
                if (monster != null && monster.IsAlive)
                {
                    // 데미지 계산
                    float damage = Combat.DamageCalculator.CalculateDamage(this, monster);
                    bool isCritical = Random.value < criticalChance;
                    
                    if (isCritical)
                    {
                        damage *= criticalDamage;
                    }

                    monster.TakeDamage(damage, isCritical);

                    // 클래스별 특수 효과
                    ApplyClassSpecificEffect(monster);
                    
                    break; // 하나만 공격
                }
            }
        }

        /// <summary>
        /// 스킬 사용
        /// </summary>
        public void UseSkill(int skillIndex)
        {
            if (skillIndex < 0 || skillIndex >= skills.Count) return;
            if (skills[skillIndex] == null) return;
            if (skillCooldowns[skillIndex] > 0) return;

            Skill skill = skills[skillIndex];

            // 마나 확인
            if (!ConsumeMP(skill.ManaCost))
            {
                Debug.Log("Not enough mana!");
                return;
            }

            // 스킬 시전
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            skill.Cast(this, mousePos);

            // 쿨다운 설정
            skillCooldowns[skillIndex] = skill.Cooldown;

            // 애니메이션
            if (animator != null)
            {
                animator.SetTrigger($"Skill{skillIndex + 1}");
            }
        }

        /// <summary>
        /// 스킬 쿨다운 업데이트
        /// </summary>
        private void UpdateSkillCooldowns()
        {
            for (int i = 0; i < skillCooldowns.Length; i++)
            {
                if (skillCooldowns[i] > 0)
                {
                    skillCooldowns[i] -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// 마나 회복
        /// </summary>
        public void RestoreMana(float amount)
        {
            currentMP += amount;
            currentMP = Mathf.Min(currentMP, maxMP);
            InvokeManaChanged(currentMP);
        }

        /// <summary>
        /// 경험치 획득
        /// </summary>
        public void GainExperience(int amount)
        {
            experience += amount;

            if (experience >= experienceToNextLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// 레벨업
        /// </summary>
        private void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

            // 스탯 증가
            maxHP += 10f;
            maxMP += 5f;
            attack += 2f;
            defense += 1f;

            // 체력/마나 회복
            currentHP = maxHP;
            currentMP = maxMP;

            InvokeHealthChanged(currentHP);
            InvokeManaChanged(currentMP);

            Debug.Log($"Level Up! Now level {level}");

            // TODO: 레벨업 이펙트
        }

        /// <summary>
        /// 골드 획득
        /// </summary>
        public void GainGold(int amount)
        {
            gold += amount;
            Debug.Log($"Gained {amount} gold. Total: {gold}");
        }

        /// <summary>
        /// 골드 소비
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (gold < amount) return false;

            gold -= amount;
            return true;
        }

        /// <summary>
        /// 직업별 스탯 초기화
        /// </summary>
        private void InitializeClassStats()
        {
            switch (playerClass)
            {
                case PlayerClass.Warrior:
                    maxHP = 150f;
                    maxMP = 50f;
                    attack = 15f;
                    defense = 10f;
                    moveSpeed = 4f;
                    attackSpeed = 1.2f;
                    attackRange = 2f;
                    break;

                case PlayerClass.Mage:
                    maxHP = 80f;
                    maxMP = 150f;
                    attack = 20f;
                    defense = 3f;
                    moveSpeed = 4.5f;
                    attackSpeed = 0.8f;
                    attackRange = 5f;
                    break;

                case PlayerClass.Rogue:
                    maxHP = 100f;
                    maxMP = 80f;
                    attack = 18f;
                    defense = 5f;
                    moveSpeed = 6f;
                    attackSpeed = 1.8f;
                    attackRange = 1.5f;
                    criticalChance = 0.25f;
                    criticalDamage = 2f;
                    break;

                case PlayerClass.Archer:
                    maxHP = 90f;
                    maxMP = 70f;
                    attack = 17f;
                    defense = 4f;
                    moveSpeed = 5f;
                    attackSpeed = 1.5f;
                    attackRange = 7f;
                    break;
            }

            currentHP = maxHP;
            currentMP = maxMP;
        }

        /// <summary>
        /// 스킬 로드
        /// </summary>
        private void LoadSkills()
        {
            // TODO: Resources 또는 ScriptableObject에서 스킬 로드
            // 지금은 빈 리스트로 초기화
            skills = new List<Skill>(4);
        }

        /// <summary>
        /// 클래스별 특수 효과
        /// </summary>
        private void ApplyClassSpecificEffect(Monster target)
        {
            switch (playerClass)
            {
                case PlayerClass.Warrior:
                    // 전사: 추가 넉백
                    break;

                case PlayerClass.Mage:
                    // 마법사: 마법 폭발
                    break;

                case PlayerClass.Rogue:
                    // 도적: 출혈 효과
                    break;

                case PlayerClass.Archer:
                    // 궁수: 관통
                    break;
            }
        }

        /// <summary>
        /// 애니메이션 업데이트
        /// </summary>
        private void UpdateAnimation()
        {
            if (animator == null) return;

            // 이동 애니메이션
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }

        /// <summary>
        /// 스킬 쿨다운 확인
        /// </summary>
        public float GetSkillCooldown(int index)
        {
            if (index < 0 || index >= skillCooldowns.Length) return 0;
            return Mathf.Max(0, skillCooldowns[index]);
        }
    }
}