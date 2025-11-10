using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BabelTower.UI
{
    /// <summary>
    /// 데미지 텍스트 표시
    /// </summary>
    public class DamageText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Text textComponent;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private float fadeSpeed = 1f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = Color.yellow;
        [SerializeField] private Color healColor = Color.green;

        private float elapsed;
        private Vector3 startPosition;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (textComponent == null)
            {
                textComponent = GetComponent<Text>();
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// 데미지 텍스트 초기화
        /// </summary>
        public void Initialize(float damage, bool isCritical, bool isHeal = false)
        {
            if (textComponent != null)
            {
                textComponent.text = Mathf.Ceil(damage).ToString();
                
                if (isHeal)
                {
                    textComponent.color = healColor;
                    textComponent.text = "+" + textComponent.text;
                }
                else if (isCritical)
                {
                    textComponent.color = criticalColor;
                    textComponent.fontSize = 32;
                    textComponent.text += "!";
                }
                else
                {
                    textComponent.color = normalColor;
                    textComponent.fontSize = 24;
                }
            }

            startPosition = transform.position;
            elapsed = 0f;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            // 위로 떠오르기
            transform.position = startPosition + Vector3.up * (floatSpeed * elapsed);

            // 페이드 아웃
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - (elapsed / duration);
            }

            // 지속시간 종료
            if (elapsed >= duration)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 데미지 텍스트 풀 (Object Pooling)
    /// </summary>
    public class DamageTextPool : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private Canvas worldCanvas;

        private Queue<GameObject> pool;

        private static DamageTextPool instance;
        public static DamageTextPool Instance => instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePool();
        }

        /// <summary>
        /// 풀 초기화
        /// </summary>
        private void InitializePool()
        {
            pool = new Queue<GameObject>();

            // Canvas 찾기 또는 생성
            if (worldCanvas == null)
            {
                GameObject canvasObj = new GameObject("DamageTextCanvas");
                worldCanvas = canvasObj.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;
                
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;
            }

            // 프리팹이 없으면 기본 생성
            if (damageTextPrefab == null)
            {
                damageTextPrefab = CreateDefaultDamageTextPrefab();
            }

            // 풀 채우기
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewDamageText();
            }
        }

        /// <summary>
        /// 새 데미지 텍스트 생성
        /// </summary>
        private void CreateNewDamageText()
        {
            GameObject obj = Instantiate(damageTextPrefab, worldCanvas.transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        /// <summary>
        /// 데미지 텍스트 가져오기
        /// </summary>
        public GameObject GetDamageText()
        {
            if (pool.Count == 0)
            {
                CreateNewDamageText();
            }

            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 데미지 텍스트 반환
        /// </summary>
        public void ReturnDamageText(GameObject obj)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        /// <summary>
        /// 데미지 표시
        /// </summary>
        public void ShowDamage(Vector3 worldPosition, float damage, bool isCritical, bool isHeal = false)
        {
            GameObject obj = GetDamageText();
            obj.transform.position = worldPosition;

            DamageText damageText = obj.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.Initialize(damage, isCritical, isHeal);
            }
        }

        /// <summary>
        /// 기본 데미지 텍스트 프리팹 생성
        /// </summary>
        private GameObject CreateDefaultDamageTextPrefab()
        {
            GameObject prefab = new GameObject("DamageText");
            
            // Text 컴포넌트
            Text text = prefab.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            // RectTransform
            RectTransform rt = prefab.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 50);
            
            // DamageText 스크립트
            prefab.AddComponent<DamageText>();
            
            // CanvasGroup
            prefab.AddComponent<CanvasGroup>();

            return prefab;
        }
    }

    /// <summary>
    /// UI 매니저
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private PlayerHUD playerHUD;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Damage Text")]
        [SerializeField] private DamageTextPool damageTextPool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 데미지 텍스트 표시
        /// </summary>
        public void ShowDamageText(Vector3 position, float damage, bool isCritical = false, bool isHeal = false)
        {
            if (damageTextPool != null)
            {
                damageTextPool.ShowDamage(position, damage, isCritical, isHeal);
            }
        }

        /// <summary>
        /// 인벤토리 토글
        /// </summary>
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }

        /// <summary>
        /// 일시정지 메뉴 토글
        /// </summary>
        public void TogglePauseMenu()
        {
            if (pausePanel != null)
            {
                bool isActive = !pausePanel.activeSelf;
                pausePanel.SetActive(isActive);
                Time.timeScale = isActive ? 0f : 1f;
            }
        }

        /// <summary>
        /// 패널 표시
        /// </summary>
        public void ShowPanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// 패널 숨기기
        /// </summary>
        public void HidePanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}
