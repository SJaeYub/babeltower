using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Mage_FrozenOrb", menuName = "Skills/Mage/FrozenOrb")]
    public class MageFrozenOrbSkill : Skill
    {
        [Header("Frozen Orb Settings")]
        [SerializeField] private float orbSpeed = 5f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private float duration = 3f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)caster.transform.position).normalized;
            caster.StartCoroutine(FrozenOrbCoroutine(caster, direction));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator FrozenOrbCoroutine(BabelTower.Character.Character caster, Vector2 direction)
        {
            Vector3 orbPos = caster.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                orbPos += (Vector3)(direction * orbSpeed * Time.deltaTime);

                Collider2D[] hits = Physics2D.OverlapCircleAll(orbPos, areaRadius);
                foreach (var hit in hits)
                {
                    BabelTower.Character.Character target = hit.GetComponent<BabelTower.Character.Character>();
                    if (target != null && target.IsAlive && target != caster)
                    {
                        if ((caster is BabelTower.Character.Player && target is BabelTower.Character.Monster) ||
                            (caster is BabelTower.Character.Monster && target is BabelTower.Character.Player))
                        {
                            float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier * 0.3f);
                            target.TakeDamage(damage);
                        }
                    }
                }

                yield return new WaitForSeconds(tickInterval);
            }
        }
    }
}
