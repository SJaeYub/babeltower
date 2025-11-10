using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Rogue_SmokeBomb", menuName = "Skills/Rogue/SmokeBomb")]
    public class RogueSmokeBombSkill : Skill
    {
        [Header("Smoke Bomb Settings")]
        [SerializeField] private float duration = 3f;
        [SerializeField] private float speedBoost = 2f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(SmokeBombCoroutine(caster));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator SmokeBombCoroutine(BabelTower.Character.Character caster)
        {
            SpriteRenderer sr = caster.GetComponent<SpriteRenderer>();
            Color originalColor = sr.color;
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);

            yield return new WaitForSeconds(duration);

            sr.color = originalColor;
        }
    }
}
