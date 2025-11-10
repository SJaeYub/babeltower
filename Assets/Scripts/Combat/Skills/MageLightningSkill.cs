using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Mage_Lightning", menuName = "Skills/Mage/Lightning")]
    public class MageLightningSkill : Skill
    {
        [Header("Lightning Settings")]
        [SerializeField] private int chainCount = 3;
        [SerializeField] private float chainRange = 4f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, chainRange);
            BabelTower.Character.Character firstTarget = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                if (target != null && target.IsAlive && target != caster)
                {
                    if ((caster is BabelTower.Character.Player && target is BabelTower.Character.Monster) ||
                        (caster is BabelTower.Character.Monster && target is BabelTower.Character.Player))
                    {
                        float distance = Vector2.Distance(caster.transform.position, target.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            firstTarget = target;
                        }
                    }
                }
            }

            if (firstTarget != null)
            {
                ChainLightning(caster, firstTarget, chainCount);
            }

            SpawnCastEffect(caster.transform.position);
        }

        private void ChainLightning(BabelTower.Character.Character caster, BabelTower.Character.Character target, int remaining)
        {
            if (target == null || !target.IsAlive || remaining <= 0) return;

            float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier);
            target.TakeDamage(damage);
            SpawnHitEffect(target.transform.position);

            if (remaining > 1)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, chainRange);
                foreach (var hit in hits)
                {
                    BabelTower.Character.Character nextTarget = hit.GetComponent<BabelTower.Character.Character>();
                    if (nextTarget != null && nextTarget.IsAlive && nextTarget != target && nextTarget != caster)
                    {
                        if ((caster is BabelTower.Character.Player && nextTarget is BabelTower.Character.Monster) ||
                            (caster is BabelTower.Character.Monster && nextTarget is BabelTower.Character.Player))
                        {
                            ChainLightning(caster, nextTarget, remaining - 1);
                            break;
                        }
                    }
                }
            }
        }
    }
}
