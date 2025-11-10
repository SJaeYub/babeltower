using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Archer_MultiShot", menuName = "Skills/Archer/MultiShot")]
    public class ArcherMultiShotSkill : Skill
    {
        [Header("Multi Shot Settings")]
        [SerializeField] private int arrowCount = 3;
        [SerializeField] private float spreadAngle = 30f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Vector2 baseDirection = (targetPosition - (Vector2)caster.transform.position).normalized;
            float startAngle = -spreadAngle / 2f;
            float angleStep = spreadAngle / (arrowCount - 1);

            for (int i = 0; i < arrowCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector2 direction = Quaternion.Euler(0, 0, angle) * baseDirection;
                
                Debug.Log($"Arrow {i} fired at angle {angle}");
            }

            SpawnCastEffect(caster.transform.position);
        }
    }
}
