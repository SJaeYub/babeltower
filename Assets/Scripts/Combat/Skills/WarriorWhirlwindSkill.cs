using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Warrior_Whirlwind", menuName = "Skills/Warrior/Whirlwind")]
    public class WarriorWhirlwindSkill : Skill
    {
        [Header("Whirlwind Settings")]
        [SerializeField] private float radius = 3f;
        [SerializeField] private int hitCount = 3;
        [SerializeField] private float hitInterval = 0.2f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(WhirlwindCoroutine(caster));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator WhirlwindCoroutine(BabelTower.Character.Character caster)
        {
            Rigidbody2D rb = caster.GetComponent<Rigidbody2D>();
            Vector2 originalVelocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, radius);

                foreach (var hit in hits)
                {
                    if (hit.transform == caster.transform) continue;

                    BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                    if (target != null && target.IsAlive)
                    {
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
                        }
                    }
                }

                yield return new WaitForSeconds(hitInterval);
            }

            rb.linearVelocity = originalVelocity;
        }
    }
}
