using UnityEngine;

namespace BabelTower.Combat.Skills
{
    [CreateAssetMenu(fileName = "Mage_Fireball", menuName = "Skills/Mage/Fireball")]
    public class MageFireballSkill : Skill
    {
        [Header("Fireball Settings")]
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float projectileSpeed = 15f;
        [SerializeField] private float explosionRadius = 1.5f;

        public override void Cast(BabelTower.Character.Character caster, Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)caster.transform.position).normalized;
            
            GameObject fireball = CreateFireball(caster.transform.position, direction, caster);
            
            SpawnCastEffect(caster.transform.position);
        }

        private GameObject CreateFireball(Vector3 startPos, Vector2 direction, BabelTower.Character.Character caster)
        {
            GameObject fireball;

            if (fireballPrefab != null)
            {
                fireball = Object.Instantiate(fireballPrefab, startPos, Quaternion.identity);
            }
            else
            {
                fireball = new GameObject("Fireball");
                fireball.transform.position = startPos;
                
                SpriteRenderer sr = fireball.AddComponent<SpriteRenderer>();
                sr.sprite = CreateCircleSprite(Color.red);
                sr.sortingOrder = 50;
                
                CircleCollider2D col = fireball.AddComponent<CircleCollider2D>();
                col.radius = 0.3f;
                col.isTrigger = true;
            }

            Projectile proj = fireball.GetComponent<Projectile>();
            if (proj == null)
            {
                proj = fireball.AddComponent<Projectile>();
            }

            float damage = DamageCalculator.CalculateDamage(caster, caster, damageMultiplier);
            
            proj.owner = caster;
            proj.damage = damage;
            proj.speed = projectileSpeed;
            proj.hitEffect = hitEffect;
            proj.piercing = false;
            
            proj.Launch(direction, caster, damage);

            return fireball;
        }

        private Sprite CreateCircleSprite(Color color)
        {
            Texture2D tex = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                    pixels[y * 32 + x] = distance < 12 ? color : Color.clear;
                }
            }
            
            tex.SetPixels(pixels);
            tex.Apply();
            
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
    }
}
