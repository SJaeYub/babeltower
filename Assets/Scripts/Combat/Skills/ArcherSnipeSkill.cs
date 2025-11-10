using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Archer_Snipe", menuName = "Skills/Archer/Snipe")]
    public class ArcherSnipeSkill : Skill
    {
        [Header("Snipe Settings")]
        [SerializeField] private float chargeTime = 2f;
        [SerializeField] private float snipeRange = 15f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(SnipeCoroutine(caster, targetPosition));
        }

        private System.Collections.IEnumerator SnipeCoroutine(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            yield return new WaitForSeconds(chargeTime);

            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, snipeRange);
            BabelTower.Character.Character target = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                BabelTower.Character.Character t = hit.GetComponent<BabelTower.Character.Character>();
                if (t != null && t.IsAlive && t != caster)
                {
                    float distance = Vector2.Distance(caster.transform.position, t.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = t;
                    }
                }
            }

            if (target != null)
            {
                float damage = DamageCalculator.CalculateDamage(caster, target, damageMultiplier * 3f);
                target.TakeDamage(damage, true);
                SpawnHitEffect(target.transform.position);
            }
        }
    }
}
