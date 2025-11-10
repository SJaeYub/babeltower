using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Warrior_WarCry", menuName = "Skills/Warrior/WarCry")]
    public class WarriorWarCrySkill : Skill
    {
        [Header("War Cry Settings")]
        [SerializeField] private float duration = 5f;
        [SerializeField] private float attackBoost = 0.3f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            caster.StartCoroutine(WarCryCoroutine(caster));
            SpawnCastEffect(caster.transform.position);
        }

        private System.Collections.IEnumerator WarCryCoroutine(BabelTower.Character.Character caster)
        {
            Debug.Log($"War Cry activated! Attack increased by {attackBoost * 100}%");
            yield return new WaitForSeconds(duration);
        }
    }
}
