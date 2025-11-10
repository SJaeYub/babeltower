using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Archer_PiercingShot", menuName = "Skills/Archer/PiercingShot")]
    public class ArcherPiercingShotSkill : Skill
    {
        [Header("Piercing Settings")]
        [SerializeField] private float arrowSpeed = 20f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)caster.transform.position).normalized;
            
            GameObject arrow = new GameObject("PiercingArrow");
            arrow.transform.position = caster.transform.position;
            
            Projectile proj = arrow.AddComponent<Projectile>();
            proj.piercing = true;
            proj.maxHits = 999;
            
            float damage = DamageCalculator.CalculateDamage(caster, caster, damageMultiplier);
            proj.Launch(direction, caster, damage);
            proj.speed = arrowSpeed;

            SpawnCastEffect(caster.transform.position);
        }
    }
}
