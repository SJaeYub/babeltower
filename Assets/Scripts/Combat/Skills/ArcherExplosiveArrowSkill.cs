using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Archer_ExplosiveArrow", menuName = "Skills/Archer/ExplosiveArrow")]
    public class ArcherExplosiveArrowSkill : Skill
    {
        [Header("Explosive Settings")]
        [SerializeField] private float explosionRadius = 2.5f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(ExplosiveArrowCoroutine(caster, targetPosition));
        }

        private System.Collections.IEnumerator ExplosiveArrowCoroutine(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            yield return new WaitForSeconds(0.3f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, explosionRadius);
            
            foreach (var hit in hits)
            {
                BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                if (target != null && target.IsAlive && target != caster)
                {
                    float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier);
                    target.TakeDamage(damage);
                }
            }

            SpawnHitEffect(targetPosition);
        }
    }
}
