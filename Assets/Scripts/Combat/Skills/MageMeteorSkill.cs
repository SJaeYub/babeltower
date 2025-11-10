using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Mage_Meteor", menuName = "Skills/Mage/Meteor")]
    public class MageMeteorSkill : Skill
    {
        [Header("Meteor Settings")]
        [SerializeField] private float delay = 1.5f;
        [SerializeField] private float impactRadius = 3f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(MeteorCoroutine(caster, targetPosition));
            SpawnCastEffect(targetPosition);
        }

        private System.Collections.IEnumerator MeteorCoroutine(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            yield return new WaitForSeconds(delay);

            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, impactRadius);
            
            foreach (var hit in hits)
            {
                BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                if (target != null && target.IsAlive && target != caster)
                {
                    if ((caster is BabelTower.Character.Player && target is BabelTower.Character.Monster) ||
                        (caster is BabelTower.Character.Monster && target is BabelTower.Character.Player))
                    {
                        float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier);
                        bool isCritical = DamageCalculator.RollCritical(caster);
                        
                        if (isCritical) damage *= caster.CriticalDamage;
                        
                        target.TakeDamage(damage, isCritical);
                    }
                }
            }

            SpawnHitEffect(targetPosition);
        }
    }
}
