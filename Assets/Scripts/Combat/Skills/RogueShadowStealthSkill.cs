using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Rogue_ShadowStealth", menuName = "Skills/Rogue/ShadowStealth")]
    public class RogueShadowStealthSkill : Skill
    {
        [Header("Stealth Settings")]
        [SerializeField] private float duration = 5f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(StealthCoroutine(caster));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator StealthCoroutine(BabelTower.Character.Character caster)
        {
            SpriteRenderer sr = caster.GetComponent<SpriteRenderer>();
            Color originalColor = sr.color;
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);

            yield return new WaitForSeconds(duration);

            sr.color = originalColor;
        }
    }
}
