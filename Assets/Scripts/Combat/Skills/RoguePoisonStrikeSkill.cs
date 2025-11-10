using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Rogue_PoisonStrike", menuName = "Skills/Rogue/PoisonStrike")]
    public class RoguePoisonStrikeSkill : Skill
    {
        [Header("Poison Settings")]
        [SerializeField] private float dotDuration = 5f;
        [SerializeField] private float dotInterval = 1f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, range);
            
            foreach (var hit in hits)
            {
                BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                if (target != null && target.IsAlive && target != caster)
                {
                    if ((caster is BabelTower.Character.Player && target is BabelTower.Character.Monster) ||
                        (caster is BabelTower.Character.Monster && target is BabelTower.Character.Player))
                    {
                        float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier);
                        target.TakeDamage(damage);

                        caster.StartCoroutine(ApplyPoison(caster, target));
                        
                        SpawnHitEffect(target.transform.position);
                        break;
                    }
                }
            }

            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator ApplyPoison(BabelTower.Character.Character caster, BabelTower.Character.Character target)
        {
            float elapsed = 0f;

            while (elapsed < dotDuration && target != null && target.IsAlive)
            {
                yield return new WaitForSeconds(dotInterval);
                elapsed += dotInterval;

                float dotDamage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier * 0.2f);
                target.TakeDamage(dotDamage);
            }
        }
    }
}
