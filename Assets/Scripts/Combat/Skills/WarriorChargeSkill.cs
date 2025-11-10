using UnityEngine;

namespace BabelTower.Combat.Skills
{
    /// <summary>
    /// 전사 스킬 1: 돌진 (Charge)
    /// 전방으로 빠르게 돌진하며 경로상의 적에게 데미지
    /// </summary>
    [CreateAssetMenu(fileName = "Warrior_Charge", menuName = "Skills/Warrior/Charge")]
    public class WarriorChargeSkill : Skill
    {
        [Header("Charge Settings")]
        [SerializeField] private float chargeDistance = 5f;
        [SerializeField] private float chargeSpeed = 20f;
        [SerializeField] private float chargeDuration = 0.3f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            // 방향 계산
            Vector2 direction = (targetPosition - (Vector2)caster.transform.position).normalized;

            // 돌진 시작
            caster.StartCoroutine(ChargeCoroutine(caster, direction));

            // 시전 이펙트
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator ChargeCoroutine(BabelTower.Character.Character caster, Vector2 direction)
        {
            float elapsed = 0f;
            Vector2 startPos = caster.transform.position;
            Vector2 targetPos = startPos + direction * chargeDistance;

            while (elapsed < chargeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / chargeDuration;

                // 이동
                Vector2 newPos = Vector2.Lerp(startPos, targetPos, progress);
                caster.transform.position = newPos;

                // 경로상의 적 탐지 및 데미지
                Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, 1f);
                foreach (var hit in hits)
                {
                    if (hit.transform == caster.transform) continue;

                    BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                    if (target != null && target.IsAlive)
                    {
                        // 플레이어면 몬스터만, 몬스터면 플레이어만 공격
                        if ((caster is BabelTower.Character.Player && target is BabelTower.Character.Monster) ||
                            (caster is BabelTower.Character.Monster && target is BabelTower.Character.Player))
                        {
                            float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier);
                            bool isCritical = DamageCalculator.RollCritical(caster);
                            
                            if (isCritical)
                            {
                                damage *= caster.CriticalDamage;
                            }

                            target.TakeDamage(damage, isCritical);
                            SpawnHitEffect(target.transform.position);

                            // 넉백 효과
                            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                            if (targetRb != null)
                            {
                                targetRb.AddForce(direction * 5f, ForceMode2D.Impulse);
                            }
                        }
                    }
                }

                yield return null;
            }
        }
    }
}
