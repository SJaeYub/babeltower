using UnityEngine;
using System;

namespace BabelTower.Character
{
    /// <summary>
    /// 모든 캐릭터(플레이어/몬스터)의 베이스 클래스
    /// </summary>
    public abstract class Character : MonoBehaviour
    {
        [Header("Basic Info")]
        [SerializeField] protected string characterName = "Character";
        [SerializeField] protected int level = 1;

        [Header("Stats")]
        [SerializeField] protected float maxHP = 100f;
        [SerializeField] protected float currentHP;
        [SerializeField] protected float maxMP = 100f;
        [SerializeField] protected float currentMP;

        [SerializeField] protected float attack = 10f;
        [SerializeField] protected float defense = 5f;
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float attackSpeed = 1f;

        [SerializeField] protected float criticalChance = 0.1f;  // 10%
        [SerializeField] protected float criticalDamage = 1.5f;  // 150%

        [Header("Components")]
        protected Rigidbody2D rb;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;

        [Header("Combat")]
        [SerializeField] protected float attackRange = 1.5f;
        [SerializeField] protected LayerMask targetLayer;
        protected float lastAttackTime;

        // Properties
        public string CharacterName => characterName;
        public int Level => level;
        public float MaxHP => maxHP;
        public float CurrentHP => currentHP;
        public float MaxMP => maxMP;
        public float CurrentMP => currentMP;
        public float Attack => attack;
        public float Defense => defense;
        public float MoveSpeed => moveSpeed;
        public float AttackSpeed => attackSpeed;
        public float CriticalChance => criticalChance;
        public float CriticalDamage => criticalDamage;

        public bool IsAlive => currentHP > 0;
        public bool IsDead => currentHP <= 0;

        // Events
        public event Action<float> OnHealthChanged;
        public event Action<float> OnManaChanged;
        public event Action OnDeath;
        public event Action<float, bool> OnDamageTaken; // damage, isCritical

        // Protected 이벤트 호출 메서드
        protected void InvokeHealthChanged(float health)
        {
            OnHealthChanged?.Invoke(health);
        }

        protected void InvokeManaChanged(float mana)
        {
            OnManaChanged?.Invoke(mana);
        }

        protected void InvokeDeath()
        {
            OnDeath?.Invoke();
        }

        protected void InvokeDamageTaken(float damage, bool isCritical)
        {
            OnDamageTaken?.Invoke(damage, isCritical);
        }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            currentHP = maxHP;
            currentMP = maxMP;
        }

        protected virtual void Start()
        {
            InitializeCharacter();
        }

        protected virtual void InitializeCharacter()
        {
            // 자식 클래스에서 구현
        }

        /// <summary>
        /// 캐릭터 이동
        /// </summary>
        public abstract void Move(Vector2 direction);

        /// <summary>
        /// 기본 공격
        /// </summary>
        public abstract void PerformAttack(Vector2 targetPosition);

        /// <summary>
        /// 데미지 받기
        /// </summary>
        public virtual void TakeDamage(float damage, bool isCritical = false)
        {
            if (IsDead) return;

            currentHP -= damage;
            currentHP = Mathf.Max(currentHP, 0);

            OnHealthChanged?.Invoke(currentHP);
            OnDamageTaken?.Invoke(damage, isCritical);

            // 데미지 텍스트 표시
            ShowDamageText(damage, isCritical);

            // 피격 애니메이션
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            if (IsDead)
            {
                Die();
            }
        }

        /// <summary>
        /// 체력 회복
        /// </summary>
        public virtual void Heal(float amount)
        {
            if (IsDead) return;

            currentHP += amount;
            currentHP = Mathf.Min(currentHP, maxHP);

            OnHealthChanged?.Invoke(currentHP);
        }

        /// <summary>
        /// 마나 회복
        /// </summary>
        public virtual void RestoreMP(float amount)
        {
            if (IsDead) return;

            currentMP += amount;
            currentMP = Mathf.Min(currentMP, maxMP);

            OnManaChanged?.Invoke(currentMP);
        }

        /// <summary>
        /// 마나 소모
        /// </summary>
        public virtual bool ConsumeMP(float amount)
        {
            if (currentMP < amount) return false;

            currentMP -= amount;
            OnManaChanged?.Invoke(currentMP);
            return true;
        }

        /// <summary>
        /// 사망 처리
        /// </summary>
        public virtual void Die()
        {
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            OnDeath?.Invoke();

            // 충돌 비활성화
            if (rb != null)
            {
                rb.simulated = false;
            }

            // 일정 시간 후 제거
            Destroy(gameObject, 2f);
        }

        /// <summary>
        /// 공격 가능 여부 확인
        /// </summary>
        protected bool CanAttack()
        {
            return Time.time >= lastAttackTime + (1f / attackSpeed);
        }

        /// <summary>
        /// 데미지 텍스트 표시
        /// </summary>
        protected virtual void ShowDamageText(float damage, bool isCritical)
        {
            // UIManager를 통해 데미지 텍스트 표시
            if (BabelTower.UI.UIManager.Instance != null)
            {
                Vector3 textPosition = transform.position + Vector3.up * 0.5f;
                BabelTower.UI.UIManager.Instance.ShowDamageText(textPosition, damage, isCritical);
            }
        }

        /// <summary>
        /// 스탯 업데이트 (장비 착용 등)
        /// </summary>
        public virtual void UpdateStats()
        {
            // 자식 클래스에서 구현
        }

        /// <summary>
        /// 디버그용 기즈모
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            // 공격 범위 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}