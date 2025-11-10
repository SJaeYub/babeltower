using UnityEngine;

namespace BabelTower.Combat
{
    /// <summary>
    /// 데미지 계산 시스템
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// 기본 데미지 계산
        /// </summary>
        public static float CalculateDamage(BabelTower.Character.Character attacker, BabelTower.Character.Character defender, float multiplier = 1f)
        {
            // 기본 데미지 = 공격력 - (방어력 * 0.5)
            float baseDamage = attacker.Attack - (defender.Defense * 0.5f);
            
            // 최소 데미지는 1
            baseDamage = Mathf.Max(baseDamage, 1f);

            // 배율 적용
            float finalDamage = baseDamage * multiplier;

            return finalDamage;
        }

        /// <summary>
        /// 치명타 데미지 계산
        /// </summary>
        public static float CalculateCriticalDamage(BabelTower.Character.Character attacker, BabelTower.Character.Character defender, float multiplier = 1f)
        {
            float baseDamage = CalculateDamage(attacker, defender, multiplier);
            return baseDamage * attacker.CriticalDamage;
        }

        /// <summary>
        /// 치명타 판정
        /// </summary>
        public static bool RollCritical(BabelTower.Character.Character attacker)
        {
            return Random.value < attacker.CriticalChance;
        }

        /// <summary>
        /// 스킬 데미지 계산
        /// </summary>
        public static float CalculateSkillDamage(BabelTower.Character.Character attacker, BabelTower.Character.Character defender, float skillMultiplier)
        {
            bool isCritical = RollCritical(attacker);
            
            if (isCritical)
            {
                return CalculateCriticalDamage(attacker, defender, skillMultiplier);
            }
            else
            {
                return CalculateDamage(attacker, defender, skillMultiplier);
            }
        }
    }

    /// <summary>
    /// 스킬 베이스 클래스
    /// </summary>
    public abstract class Skill : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName = "Skill";
        public string description = "Skill Description";
        public Sprite icon;

        [Header("Stats")]
        public float cooldown = 5f;
        public float manaCost = 20f;
        public float damageMultiplier = 1.5f;

        [Header("Type")]
        public SkillType skillType = SkillType.Melee;
        public TargetType targetType = TargetType.Single;

        [Header("Range & Area")]
        public float range = 3f;
        public float areaRadius = 0f;

        [Header("Effects")]
        public GameObject castEffect;
        public GameObject hitEffect;

        // Properties
        public string SkillName => skillName;
        public string Description => description;
        public Sprite Icon => icon;
        public float Cooldown => cooldown;
        public float ManaCost => manaCost;
        public float DamageMultiplier => damageMultiplier;

        /// <summary>
        /// 스킬 시전
        /// </summary>
        public abstract void Cast(BabelTower.Character.Character caster, Vector2 targetPosition);

        /// <summary>
        /// 시전 이펙트 생성
        /// </summary>
        protected void SpawnCastEffect(Vector3 position)
        {
            if (castEffect != null)
            {
                GameObject effect = Instantiate(castEffect, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }

        /// <summary>
        /// 적중 이펙트 생성
        /// </summary>
        protected void SpawnHitEffect(Vector3 position)
        {
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
    }

    /// <summary>
    /// 스킬 타입
    /// </summary>
    public enum SkillType
    {
        Melee,      // 근접
        Ranged,     // 원거리
        AOE,        // 범위
        Buff,       // 버프
        Debuff      // 디버프
    }

    /// <summary>
    /// 타겟 타입
    /// </summary>
    public enum TargetType
    {
        Single,     // 단일 대상
        Multiple,   // 다수 대상
        Area        // 범위
    }

    /// <summary>
    /// 발사체 (화살, 마법탄 등)
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")]
        public float damage = 10f;
        public float speed = 10f;
        public float lifetime = 5f;

        [Header("References")]
        public BabelTower.Character.Character owner;
        public GameObject hitEffect;

        [Header("Collision")]
        public LayerMask targetLayer;
        public bool piercing = false;
        public int maxHits = 1;

        private Vector2 direction;
        private int hitCount = 0;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // 이동
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        /// <summary>
        /// 발사
        /// </summary>
        public void Launch(Vector2 dir, BabelTower.Character.Character caster, float dmg)
        {
            direction = dir.normalized;
            owner = caster;
            damage = dmg;

            // 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 자신은 무시
            if (collision.transform == owner.transform) return;

            // 타겟 레이어 확인
            if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

            BabelTower.Character.Character target = collision.GetComponent<BabelTower.Character.Character>();
            if (target != null && target.IsAlive)
            {
                // 데미지 적용
                target.TakeDamage(damage);

                // 적중 이펙트
                SpawnHitEffect();

                hitCount++;

                // 관통하지 않으면 파괴
                if (!piercing || hitCount >= maxHits)
                {
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// 적중 이펙트 생성
        /// </summary>
        private void SpawnHitEffect()
        {
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
    }

    /// <summary>
    /// 근접 공격 판정
    /// </summary>
    public class MeleeAttackHitbox : MonoBehaviour
    {
        public BabelTower.Character.Character owner;
        public float damage;
        public float duration = 0.2f;
        public LayerMask targetLayer;

        private void Start()
        {
            Destroy(gameObject, duration);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform == owner.transform) return;

            if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

            BabelTower.Character.Character target = collision.GetComponent<BabelTower.Character.Character>();
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
