using UnityEngine;
using System.Collections.Generic;

namespace BabelTower.Character
{
    /// <summary>
    /// 몬스터 타입
    /// </summary>
    public enum MonsterType
    {
        Normal,     // 일반
        Elite,      // 정예
        Boss        // 보스
    }

    /// <summary>
    /// 몬스터 캐릭터
    /// </summary>
    public class Monster : Character
    {
        [Header("Monster Specific")]
        [SerializeField] private MonsterType monsterType = MonsterType.Normal;
        [SerializeField] private int expReward = 10;
        [SerializeField] private int goldReward = 5;

        [Header("AI")]
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float chaseSpeed = 3f;
        [SerializeField] private float attackCooldown = 1.5f;

        private MonsterAI ai;
        private Transform target;

        // Properties
        public MonsterType Type => monsterType;
        public int ExpReward => expReward;
        public int GoldReward => goldReward;

        protected override void Awake()
        {
            base.Awake();
            ai = gameObject.AddComponent<MonsterAI>();
        }

        protected override void InitializeCharacter()
        {
            base.InitializeCharacter();
            
            // 타입별 스탯 조정
            AdjustStatsByType();

            // AI 초기화
            if (ai != null)
            {
                ai.Initialize(this, detectionRange, attackRange);
            }
        }

        private void Update()
        {
            if (IsDead) return;

            // AI 업데이트
            if (ai != null)
            {
                ai.UpdateAI();
            }
        }

        /// <summary>
        /// 이동
        /// </summary>
        public override void Move(Vector2 direction)
        {
            if (IsDead) return;

            rb.linearVelocity = direction.normalized * chaseSpeed;

            // 애니메이션
            if (animator != null)
            {
                animator.SetFloat("Speed", direction.magnitude);
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
            }
        }

        /// <summary>
        /// 기본 공격
        /// </summary>
        public override void PerformAttack(Vector2 targetPosition)
        {
            if (!CanAttack()) return;

            lastAttackTime = Time.time;

            // 애니메이션
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // 플레이어 공격
            if (Player.Instance != null)
            {
                float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
                
                if (distance <= attackRange)
                {
                    float damage = Combat.DamageCalculator.CalculateDamage(this, Player.Instance);
                    bool isCritical = Random.value < criticalChance;
                    
                    if (isCritical)
                    {
                        damage *= criticalDamage;
                    }

                    Player.Instance.TakeDamage(damage, isCritical);
                }
            }
        }

        /// <summary>
        /// 사망 처리
        /// </summary>
        public override void Die()
        {
            // 보상 드랍
            DropRewards();

            base.Die();
        }

        /// <summary>
        /// 보상 드랍
        /// </summary>
        private void DropRewards()
        {
            // 경험치 및 골드 지급
            if (Player.Instance != null)
            {
                Player.Instance.GainExperience(expReward);
                Player.Instance.GainGold(goldReward);
            }

            // 아이템 드랍 (TODO: Phase 3에서 구현 예정)
            // if (BabelTower.Item.ItemDropSystem.Instance != null)
            // {
            //     BabelTower.Item.ItemDropSystem.Instance.DropItemsFromMonster(this, transform.position);
            // }
        }

        /// <summary>
        /// 타입별 스탯 조정
        /// </summary>
        private void AdjustStatsByType()
        {
            float multiplier = 1f;

            switch (monsterType)
            {
                case MonsterType.Normal:
                    multiplier = 1f;
                    break;

                case MonsterType.Elite:
                    multiplier = 2f;
                    expReward = Mathf.RoundToInt(expReward * 3f);
                    goldReward = Mathf.RoundToInt(goldReward * 3f);
                    break;

                case MonsterType.Boss:
                    multiplier = 5f;
                    expReward = Mathf.RoundToInt(expReward * 10f);
                    goldReward = Mathf.RoundToInt(goldReward * 10f);
                    break;
            }

            maxHP *= multiplier;
            currentHP = maxHP;
            attack *= multiplier;
            defense *= multiplier;
        }

        /// <summary>
        /// 레벨 설정
        /// </summary>
        public void SetLevel(int newLevel)
        {
            level = newLevel;
            
            // 레벨에 따른 스탯 조정
            maxHP = 50f + (newLevel * 10f);
            attack = 5f + (newLevel * 2f);
            defense = 2f + (newLevel * 1f);
            
            currentHP = maxHP;

            // 보상 조정
            expReward = 10 + (newLevel * 5);
            goldReward = 5 + (newLevel * 2);

            // 타입별 추가 조정
            AdjustStatsByType();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // 탐지 범위 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }

    /// <summary>
    /// 몬스터 AI
    /// </summary>
    public class MonsterAI : MonoBehaviour
    {
        private enum AIState
        {
            Idle,
            Patrol,
            Chase,
            Attack
        }

        private Monster owner;
        private AIState currentState = AIState.Idle;
        private Transform target;
        private Vector2 patrolTarget;
        private float detectionRange;
        private float attackRange;
        private float stateTimer;

        public void Initialize(Monster monster, float detectRange, float atkRange)
        {
            owner = monster;
            detectionRange = detectRange;
            attackRange = atkRange;
            currentState = AIState.Idle;
        }

        public void UpdateAI()
        {
            if (owner == null || owner.IsDead) return;

            // 플레이어 탐지
            DetectPlayer();

            // 상태별 행동
            switch (currentState)
            {
                case AIState.Idle:
                    IdleState();
                    break;

                case AIState.Patrol:
                    PatrolState();
                    break;

                case AIState.Chase:
                    ChaseState();
                    break;

                case AIState.Attack:
                    AttackState();
                    break;
            }

            stateTimer += Time.deltaTime;
        }

        /// <summary>
        /// 플레이어 탐지
        /// </summary>
        private void DetectPlayer()
        {
            if (Player.Instance == null) return;

            float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);

            if (distance <= detectionRange)
            {
                target = Player.Instance.transform;

                if (distance <= attackRange)
                {
                    ChangeState(AIState.Attack);
                }
                else if (currentState != AIState.Attack)
                {
                    ChangeState(AIState.Chase);
                }
            }
            else
            {
                target = null;
                if (currentState == AIState.Chase || currentState == AIState.Attack)
                {
                    ChangeState(AIState.Idle);
                }
            }
        }

        /// <summary>
        /// 대기 상태
        /// </summary>
        private void IdleState()
        {
            owner.Move(Vector2.zero);

            // 일정 시간 후 순찰
            if (stateTimer > 2f)
            {
                ChangeState(AIState.Patrol);
            }
        }

        /// <summary>
        /// 순찰 상태
        /// </summary>
        private void PatrolState()
        {
            // 목표 지점이 없으면 랜덤 생성
            if (patrolTarget == Vector2.zero || stateTimer > 3f)
            {
                patrolTarget = (Vector2)transform.position + Random.insideUnitCircle * 3f;
                stateTimer = 0;
            }

            // 목표로 이동
            Vector2 direction = (patrolTarget - (Vector2)transform.position);
            
            if (direction.magnitude > 0.5f)
            {
                owner.Move(direction.normalized);
            }
            else
            {
                ChangeState(AIState.Idle);
            }
        }

        /// <summary>
        /// 추적 상태
        /// </summary>
        private void ChaseState()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            Vector2 direction = (target.position - transform.position);
            owner.Move(direction.normalized);
        }

        /// <summary>
        /// 공격 상태
        /// </summary>
        private void AttackState()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            // 정지
            owner.Move(Vector2.zero);

            // 공격
            owner.PerformAttack(target.position);

            // 범위를 벗어나면 추적
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > attackRange * 1.2f)
            {
                ChangeState(AIState.Chase);
            }
        }

        /// <summary>
        /// 상태 변경
        /// </summary>
        private void ChangeState(AIState newState)
        {
            currentState = newState;
            stateTimer = 0;
        }
    }
}
