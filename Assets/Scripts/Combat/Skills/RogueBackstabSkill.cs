using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Rogue_Backstab", menuName = "Skills/Rogue/Backstab")]
    public class RogueBackstabSkill : Skill
    {
        [Header("Backstab Settings")]
        [SerializeField] private float teleportDistance = 2f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, 3f);
            BabelTower.Character.Character nearestEnemy = null;
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
                            nearestEnemy = target;
                        }
                    }
                }
            }

            if (nearestEnemy != null)
            {
                Vector2 direction = (nearestEnemy.transform.position - caster.transform.position).normalized;
                Vector2 behindPosition = (Vector2)nearestEnemy.transform.position - direction * teleportDistance;
                
                SpawnCastEffect(caster.transform.position);
                caster.transform.position = behindPosition;
                SpawnHitEffect(behindPosition);

                float damage = DamageCalculator.CalculateDamage(caster, nearestEnemy, damageMultiplier);
                damage *= 2f;
                nearestEnemy.TakeDamage(damage, true);
            }
        }
    }
}
