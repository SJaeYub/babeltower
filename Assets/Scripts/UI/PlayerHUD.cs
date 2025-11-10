using UnityEngine;
using UnityEngine.UI;
using BabelTower.Character;

namespace BabelTower.UI
{
    /// <summary>
    /// 플레이어 HUD (HP, MP, 스킬 쿨다운 등)
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Health & Mana")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider mpBar;
        [SerializeField] private Text hpText;
        [SerializeField] private Text mpText;

        [Header("Character Info")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text goldText;
        [SerializeField] private Slider expBar;

        [Header("Skills")]
        [SerializeField] private SkillSlotUI[] skillSlots;

        [Header("References")]
        private Player player;

        private void Start()
        {
            // 플레이어 찾기
            player = Player.Instance;
            
            if (player != null)
            {
                SubscribeToPlayerEvents();
                UpdateAllUI();
            }
        }

        private void Update()
        {
            if (player == null) return;

            // 스킬 쿨다운 업데이트
            UpdateSkillCooldowns();
        }

        /// <summary>
        /// 플레이어 이벤트 구독
        /// </summary>
        private void SubscribeToPlayerEvents()
        {
            player.OnHealthChanged += UpdateHP;
            player.OnManaChanged += UpdateMP;
        }

        /// <summary>
        /// HP 업데이트
        /// </summary>
        public void UpdateHP(float currentHP)
        {
            if (hpBar != null)
            {
                hpBar.value = currentHP / player.MaxHP;
            }

            if (hpText != null)
            {
                hpText.text = $"{Mathf.Ceil(currentHP)}/{Mathf.Ceil(player.MaxHP)}";
            }
        }

        /// <summary>
        /// MP 업데이트
        /// </summary>
        public void UpdateMP(float currentMP)
        {
            if (mpBar != null)
            {
                mpBar.value = currentMP / player.MaxMP;
            }

            if (mpText != null)
            {
                mpText.text = $"{Mathf.Ceil(currentMP)}/{Mathf.Ceil(player.MaxMP)}";
            }
        }

        /// <summary>
        /// 레벨 업데이트
        /// </summary>
        public void UpdateLevel()
        {
            if (levelText != null)
            {
                levelText.text = $"Lv.{player.Level}";
            }

            if (expBar != null)
            {
                expBar.value = (float)player.Experience / player.ExperienceToNextLevel;
            }
        }

        /// <summary>
        /// 골드 업데이트
        /// </summary>
        public void UpdateGold()
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {player.Gold}";
            }
        }

        /// <summary>
        /// 스킬 쿨다운 업데이트
        /// </summary>
        private void UpdateSkillCooldowns()
        {
            for (int i = 0; i < skillSlots.Length && i < 4; i++)
            {
                if (skillSlots[i] != null)
                {
                    float cooldown = player.GetSkillCooldown(i);
                    skillSlots[i].UpdateCooldown(cooldown);
                }
            }
        }

        /// <summary>
        /// 모든 UI 업데이트
        /// </summary>
        public void UpdateAllUI()
        {
            UpdateHP(player.CurrentHP);
            UpdateMP(player.CurrentMP);
            UpdateLevel();
            UpdateGold();
        }

        private void OnDestroy()
        {
            if (player != null)
            {
                player.OnHealthChanged -= UpdateHP;
                player.OnManaChanged -= UpdateMP;
            }
        }
    }

    /// <summary>
    /// 스킬 슬롯 UI
    /// </summary>
    [System.Serializable]
    public class SkillSlotUI : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Text cooldownText;
        [SerializeField] private Text hotkeyText;
        [SerializeField] private KeyCode hotkey;

        private float maxCooldown;

        public void Initialize(Sprite icon, KeyCode key)
        {
            if (skillIcon != null)
            {
                skillIcon.sprite = icon;
            }

            hotkey = key;
            if (hotkeyText != null)
            {
                hotkeyText.text = key.ToString();
            }
        }

        public void UpdateCooldown(float remaining)
        {
            if (remaining <= 0)
            {
                // 쿨다운 없음
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = 0;
                }

                if (cooldownText != null)
                {
                    cooldownText.gameObject.SetActive(false);
                }
            }
            else
            {
                // 쿨다운 중
                if (cooldownOverlay != null)
                {
                    // maxCooldown은 스킬에서 가져와야 함
                    cooldownOverlay.fillAmount = remaining / maxCooldown;
                }

                if (cooldownText != null)
                {
                    cooldownText.gameObject.SetActive(true);
                    cooldownText.text = Mathf.Ceil(remaining).ToString();
                }
            }
        }

        public void SetMaxCooldown(float max)
        {
            maxCooldown = max;
        }
    }
}
