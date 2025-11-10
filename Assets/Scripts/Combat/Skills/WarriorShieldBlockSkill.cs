using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Warrior_ShieldBlock", menuName = "Skills/Warrior/ShieldBlock")]
    public class WarriorShieldBlockSkill : Skill
    {
        [Header("Shield Settings")]
        [SerializeField] private float duration = 3f;
        [SerializeField] private float damageReduction = 0.5f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(ShieldBlockCoroutine(caster));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator ShieldBlockCoroutine(BabelTower.Character.Character caster)
        {
            GameObject shieldEffect = null;
            if (castEffect != null)
            {
                shieldEffect = Object.Instantiate(castEffect, caster.transform);
                shieldEffect.transform.localPosition = Vector3.zero;
            }

            yield return new WaitForSeconds(duration);

            if (shieldEffect != null)
            {
                Object.Destroy(shieldEffect);
            }
        }
    }
}
