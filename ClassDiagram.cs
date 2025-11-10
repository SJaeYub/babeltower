// Babel Tower - 핵심 클래스 구조
// C# Unity 기반

// ═══════════════════════════════════════════════════════════
// 📦 CORE SYSTEMS
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 게임 전체를 관리하는 싱글톤 매니저
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public Player CurrentPlayer { get; set; }
    public InventorySystem Inventory { get; private set; }
    public QuestSystem Quests { get; private set; }
    
    public void SaveGame();
    public void LoadGame();
    public void ChangeScene(string sceneName);
}

/// <summary>
/// 씬별 관리자 베이스 클래스
/// </summary>
public abstract class SceneManager : MonoBehaviour
{
    protected virtual void OnSceneLoaded();
    protected virtual void OnSceneUnloaded();
}

// ═══════════════════════════════════════════════════════════
// 👤 CHARACTER SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 모든 캐릭터(플레이어/몬스터)의 베이스 클래스
/// </summary>
public abstract class Character : MonoBehaviour
{
    // 기본 스탯
    public string CharacterName { get; set; }
    public int Level { get; protected set; }
    
    public float MaxHP { get; protected set; }
    public float CurrentHP { get; protected set; }
    public float MaxMP { get; protected set; }
    public float CurrentMP { get; protected set; }
    
    public float Attack { get; protected set; }
    public float Defense { get; protected set; }
    public float MoveSpeed { get; protected set; }
    public float AttackSpeed { get; protected set; }
    
    public float CriticalChance { get; protected set; }
    public float CriticalDamage { get; protected set; }
    
    // 상태
    public bool IsAlive => CurrentHP > 0;
    public bool IsDead => CurrentHP <= 0;
    
    // 메서드
    public abstract void Move(Vector2 direction);
    public abstract void Attack(Character target);
    public virtual void TakeDamage(float damage);
    public virtual void Heal(float amount);
    public virtual void RestoreMP(float amount);
    public virtual void Die();
}

/// <summary>
/// 플레이어 캐릭터
/// </summary>
public class Player : Character
{
    public PlayerClass Class { get; private set; }
    public int Experience { get; set; }
    public int Gold { get; set; }
    
    public Equipment EquippedWeapon { get; set; }
    public Equipment EquippedArmor { get; set; }
    public Equipment EquippedHelmet { get; set; }
    public Equipment EquippedGloves { get; set; }
    public Equipment EquippedBoots { get; set; }
    public Equipment EquippedRing { get; set; }
    
    public List<Skill> Skills { get; private set; }
    
    public void Initialize(PlayerClass playerClass);
    public void LevelUp();
    public void EquipItem(Equipment equipment);
    public void UnequipItem(EquipmentSlot slot);
    public void UseSkill(int skillIndex, Vector2 target);
    
    public override void Move(Vector2 direction);
    public override void Attack(Character target);
}

/// <summary>
/// 플레이어 직업 Enum
/// </summary>
public enum PlayerClass
{
    Warrior,    // 전사
    Mage,       // 마법사
    Rogue,      // 도적
    Archer      // 궁수
}

/// <summary>
/// 몬스터 캐릭터
/// </summary>
public class Monster : Character
{
    public MonsterType Type { get; set; }
    public int ExpReward { get; set; }
    public int GoldReward { get; set; }
    public List<LootDrop> LootTable { get; set; }
    
    public MonsterAI AI { get; private set; }
    
    public override void Move(Vector2 direction);
    public override void Attack(Character target);
    public void DropLoot();
}

public enum MonsterType
{
    Normal,     // 일반
    Elite,      // 정예
    Boss        // 보스
}

/// <summary>
/// 몬스터 AI
/// </summary>
public class MonsterAI : MonoBehaviour
{
    public Monster Owner { get; set; }
    public float DetectionRange { get; set; }
    public float AttackRange { get; set; }
    
    private Character target;
    
    public void UpdateAI();
    private void Idle();
    private void Patrol();
    private void Chase(Character target);
    private void AttackTarget();
}

// ═══════════════════════════════════════════════════════════
// ⚔️ COMBAT SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 스킬 베이스 클래스
/// </summary>
public abstract class Skill : ScriptableObject
{
    public string SkillName;
    public string Description;
    public Sprite Icon;
    
    public float Cooldown;
    public float ManaCost;
    public float DamageMultiplier;
    public SkillType Type;
    
    public abstract void Cast(Character caster, Vector2 target);
}

public enum SkillType
{
    Melee,      // 근접
    Ranged,     // 원거리
    AOE,        // 범위
    Buff,       // 버프
    Debuff      // 디버프
}

/// <summary>
/// 전사 스킬들
/// </summary>
public class ChargeSkill : Skill { }
public class WhirlwindSkill : Skill { }
public class ShieldBlockSkill : Skill { }
public class WarCrySkill : Skill { }

/// <summary>
/// 마법사 스킬들
/// </summary>
public class FireballSkill : Skill { }
public class FrozenOrbSkill : Skill { }
public class LightningSkill : Skill { }
public class MeteorSkill : Skill { }

/// <summary>
/// 도적 스킬들
/// </summary>
public class BackstabSkill : Skill { }
public class SmokeBombSkill : Skill { }
public class PoisonStrikeSkill : Skill { }
public class ShadowStealthSkill : Skill { }

/// <summary>
/// 궁수 스킬들
/// </summary>
public class PiercingShotSkill : Skill { }
public class MultiShotSkill : Skill { }
public class ExplosiveArrowSkill : Skill { }
public class SnipeSkill : Skill { }

/// <summary>
/// 데미지 계산 시스템
/// </summary>
public static class DamageCalculator
{
    public static float CalculateDamage(Character attacker, Character defender, float baseMultiplier = 1f)
    {
        float baseDamage = attacker.Attack - defender.Defense * 0.5f;
        baseDamage = Mathf.Max(baseDamage, 1f);
        
        bool isCritical = UnityEngine.Random.value < attacker.CriticalChance;
        float critMultiplier = isCritical ? attacker.CriticalDamage : 1f;
        
        return baseDamage * baseMultiplier * critMultiplier;
    }
}

/// <summary>
/// 발사체 (화살, 마법탄 등)
/// </summary>
public class Projectile : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public Character Owner;
    public GameObject HitEffect;
    
    private void Update();
    private void OnTriggerEnter2D(Collider2D collision);
}

// ═══════════════════════════════════════════════════════════
// 🎒 ITEM & INVENTORY SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 아이템 베이스 클래스
/// </summary>
public abstract class Item : ScriptableObject
{
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public ItemRarity Rarity;
    public int SellPrice;
    public int BuyPrice;
    public int StackLimit;
}

public enum ItemRarity
{
    Common,     // 일반
    Rare,       // 희귀
    Epic,       // 영웅
    Legendary   // 전설
}

/// <summary>
/// 장비 아이템
/// </summary>
public class Equipment : Item
{
    public EquipmentSlot Slot;
    public PlayerClass RequiredClass;
    public int RequiredLevel;
    public int EnhancementLevel;  // 강화 수치 +0 ~ +10
    
    // 장비 스탯
    public float BonusHP;
    public float BonusMP;
    public float BonusAttack;
    public float BonusDefense;
    public float BonusCritChance;
    public float BonusCritDamage;
    
    public void Enhance();
}

public enum EquipmentSlot
{
    Weapon,
    Helmet,
    Armor,
    Gloves,
    Boots,
    Ring
}

/// <summary>
/// 소비 아이템
/// </summary>
public class ConsumableItem : Item
{
    public ConsumableType Type;
    public float EffectValue;
    
    public void Use(Player player);
}

public enum ConsumableType
{
    HealthPotion,
    ManaPotion,
    Buff
}

/// <summary>
/// 재료 아이템
/// </summary>
public class MaterialItem : Item
{
    public MaterialType Type;
}

public enum MaterialType
{
    Ore,            // 광석
    Leather,        // 가죽
    Essence,        // 정수
    EnhancementStone // 강화석
}

/// <summary>
/// 인벤토리 시스템
/// </summary>
public class InventorySystem
{
    public const int INVENTORY_SIZE = 24; // 6x4
    
    private List<ItemStack> items;
    
    public bool AddItem(Item item, int quantity = 1);
    public bool RemoveItem(Item item, int quantity = 1);
    public bool HasItem(Item item, int quantity = 1);
    public ItemStack GetItem(int index);
    public void SortInventory();
}

/// <summary>
/// 아이템 스택 (개수 관리)
/// </summary>
public class ItemStack
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
}

// ═══════════════════════════════════════════════════════════
// 🗺️ DUNGEON SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 던전 매니저
/// </summary>
public class DungeonManager : SceneManager
{
    public DungeonDifficulty CurrentDifficulty { get; set; }
    public float TimeElapsed { get; private set; }
    public int MonstersKilled { get; private set; }
    
    private DungeonGenerator generator;
    private List<Monster> spawnedMonsters;
    
    public void GenerateDungeon();
    public void SpawnMonsters();
    public void CompleteDungeon();
    public void ExitDungeon();
}

public enum DungeonDifficulty
{
    Easy,
    Normal,
    Hard,
    Hell
}

/// <summary>
/// 던전 생성기
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public int MinRooms = 5;
    public int MaxRooms = 10;
    public Vector2Int RoomSizeMin = new Vector2Int(8, 8);
    public Vector2Int RoomSizeMax = new Vector2Int(15, 15);
    
    private List<Room> rooms;
    
    public void Generate();
    private void CreateRooms();
    private void ConnectRooms();
    private void PlaceSpawnPoints();
    private void PlaceExit();
}

/// <summary>
/// 던전 방
/// </summary>
public class Room
{
    public Vector2Int Position { get; set; }
    public Vector2Int Size { get; set; }
    public List<Vector2Int> SpawnPoints { get; set; }
    public RoomType Type { get; set; }
}

public enum RoomType
{
    Normal,
    Start,
    Exit,
    Treasure
}

/// <summary>
/// 몬스터 스포너
/// </summary>
public class MonsterSpawner : MonoBehaviour
{
    public List<Monster> MonsterPrefabs;
    public Transform SpawnPoint;
    
    public Monster SpawnMonster(MonsterType type, int level);
}

/// <summary>
/// 루트 드랍
/// </summary>
public class LootDrop
{
    public Item Item;
    public float DropChance; // 0.0 ~ 1.0
    public int MinQuantity;
    public int MaxQuantity;
}

/// <summary>
/// 루트 시스템
/// </summary>
public class LootSystem
{
    public static List<Item> GenerateLoot(List<LootDrop> lootTable)
    {
        List<Item> drops = new List<Item>();
        
        foreach (var loot in lootTable)
        {
            if (UnityEngine.Random.value < loot.DropChance)
            {
                drops.Add(loot.Item);
            }
        }
        
        return drops;
    }
    
    public static ItemRarity RollRarity()
    {
        float roll = UnityEngine.Random.value;
        
        if (roll < 0.01f) return ItemRarity.Legendary;  // 1%
        if (roll < 0.05f) return ItemRarity.Epic;       // 4%
        if (roll < 0.30f) return ItemRarity.Rare;       // 25%
        return ItemRarity.Common;                        // 70%
    }
}

// ═══════════════════════════════════════════════════════════
// 🗼 BABEL TOWER SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 바벨탑 매니저
/// </summary>
public class BabelTowerManager : SceneManager
{
    public int CurrentFloor { get; private set; }
    public int HighestFloor { get; private set; }
    public float TimeLimit { get; private set; }
    public float TimeRemaining { get; private set; }
    
    private List<Monster> currentMonsters;
    
    public void StartFloor(int floor);
    public void CompleteFloor();
    public void FailFloor();
    public void NextFloor();
    
    private void SpawnFloorMonsters();
    private void SpawnBoss();
    private float CalculateTimeLimit(int floor);
}

/// <summary>
/// 타워 층 데이터
/// </summary>
public class TowerFloor
{
    public int FloorNumber { get; set; }
    public bool IsBossFloor => FloorNumber % 5 == 0;
    public float TimeLimit { get; set; }
    public List<MonsterSpawnData> Monsters { get; set; }
}

public class MonsterSpawnData
{
    public Monster MonsterPrefab;
    public int Count;
    public int Level;
}

// ═══════════════════════════════════════════════════════════
// 🏘️ TOWN SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 마을 매니저
/// </summary>
public class TownManager : SceneManager
{
    public BlacksmithNPC Blacksmith { get; private set; }
    public ShopNPC Shop { get; private set; }
    public Portal DungeonPortal { get; private set; }
    public Portal TowerPortal { get; private set; }
}

/// <summary>
/// NPC 베이스 클래스
/// </summary>
public abstract class NPC : MonoBehaviour
{
    public string NPCName;
    public Sprite Portrait;
    
    public abstract void Interact(Player player);
}

/// <summary>
/// 대장장이 NPC
/// </summary>
public class BlacksmithNPC : NPC
{
    public void CraftEquipment(Recipe recipe, Player player);
    public bool EnhanceEquipment(Equipment equipment, Player player);
    public List<MaterialItem> Dismantle(Equipment equipment);
    
    public override void Interact(Player player);
}

/// <summary>
/// 제작 레시피
/// </summary>
public class Recipe
{
    public Equipment Result;
    public List<MaterialRequirement> Materials;
    public int GoldCost;
}

public class MaterialRequirement
{
    public MaterialItem Material;
    public int Quantity;
}

/// <summary>
/// 상점 NPC
/// </summary>
public class ShopNPC : NPC
{
    public List<Item> ShopInventory;
    
    public void BuyItem(Item item, int quantity, Player player);
    public void SellItem(Item item, int quantity, Player player);
    
    public override void Interact(Player player);
}

/// <summary>
/// 포탈
/// </summary>
public class Portal : MonoBehaviour
{
    public string TargetScene;
    
    public void EnterPortal();
}

// ═══════════════════════════════════════════════════════════
// 🎮 INPUT & CAMERA SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 플레이어 입력 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Player Player { get; set; }
    public float MouseMoveThreshold = 0.1f;
    
    private void Update();
    private void HandleMovement();
    private void HandleAttack();
    private void HandleSkills();
    private void HandleInventory();
}

/// <summary>
/// 디아블로 스타일 카메라
/// </summary>
public class IsometricCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(0, 0, -10);
    public float SmoothSpeed = 0.125f;
    public float IsoAngle = 45f;
    
    private void LateUpdate();
}

// ═══════════════════════════════════════════════════════════
// 💾 SAVE/LOAD SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 저장 데이터
/// </summary>
[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public ProgressData progressData;
    public InventoryData inventoryData;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public PlayerClass playerClass;
    public int level;
    public int experience;
    public int gold;
    public float currentHP;
    public float currentMP;
}

[System.Serializable]
public class ProgressData
{
    public int highestBabelFloor;
    public int completedDungeons;
    public float totalPlaytime;
    public List<string> unlockedDifficulties;
}

[System.Serializable]
public class InventoryData
{
    public List<ItemSaveData> items;
    public List<EquipmentSaveData> equippedItems;
}

/// <summary>
/// 저장/로드 매니저
/// </summary>
public class SaveManager
{
    private const string SAVE_KEY = "BabelTowerSave";
    
    public static void SaveGame(SaveData data);
    public static SaveData LoadGame();
    public static bool HasSaveData();
    public static void DeleteSave();
}

// ═══════════════════════════════════════════════════════════
// 🎨 UI SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// UI 매니저
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public PlayerHUD HUD;
    public InventoryUI InventoryPanel;
    public ShopUI ShopPanel;
    public BlacksmithUI BlacksmithPanel;
    public DamageTextPool DamageTextPool;
    
    public void ShowPanel(UIPanel panel);
    public void HidePanel(UIPanel panel);
    public void ShowDamageText(Vector3 position, float damage, bool isCritical);
}

/// <summary>
/// 플레이어 HUD
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    public Slider HPBar;
    public Slider MPBar;
    public Text GoldText;
    public SkillCooldownUI[] SkillSlots;
    
    public void UpdateHP(float current, float max);
    public void UpdateMP(float current, float max);
    public void UpdateGold(int gold);
}

/// <summary>
/// 스킬 쿨다운 UI
/// </summary>
public class SkillCooldownUI : MonoBehaviour
{
    public Image SkillIcon;
    public Image CooldownOverlay;
    public Text CooldownText;
    public KeyCode Hotkey;
    
    public void UpdateCooldown(float remaining, float total);
}

/// <summary>
/// 인벤토리 UI
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public List<ItemSlotUI> ItemSlots;
    public List<EquipmentSlotUI> EquipmentSlots;
    
    public void Refresh();
    public void OnItemClicked(int slotIndex);
}

/// <summary>
/// 데미지 텍스트
/// </summary>
public class DamageText : MonoBehaviour
{
    public Text TextField;
    public float Duration = 1f;
    public float FloatSpeed = 2f;
    
    public void Show(float damage, bool isCritical);
}

// ═══════════════════════════════════════════════════════════
// 🔊 AUDIO SYSTEM
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 오디오 매니저
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public AudioSource BGMSource;
    public AudioSource SFXSource;
    
    public void PlayBGM(AudioClip clip);
    public void PlaySFX(AudioClip clip);
    public void StopBGM();
    public void SetBGMVolume(float volume);
    public void SetSFXVolume(float volume);
}

// ═══════════════════════════════════════════════════════════
// 🎲 UTILITY & HELPERS
// ═══════════════════════════════════════════════════════════

/// <summary>
/// 오브젝트 풀링
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool;
    private T prefab;
    private Transform parent;
    
    public ObjectPool(T prefab, int initialSize, Transform parent = null);
    public T Get();
    public void Return(T obj);
}

/// <summary>
/// 확장 메서드
/// </summary>
public static class Extensions
{
    public static Vector2 ToIsometric(this Vector3 worldPos);
    public static Vector3 ToWorld(this Vector2 isoPos);
}

/// <summary>
/// 게임 상수
/// </summary>
public static class GameConstants
{
    public const float TILE_SIZE = 1f;
    public const int MAX_PLAYER_LEVEL = 50;
    public const int MAX_ENHANCEMENT_LEVEL = 10;
    
    public static readonly int[] ENHANCEMENT_SUCCESS_RATES = 
        { 100, 100, 80, 80, 60, 60, 40, 40, 20, 20 };
}
