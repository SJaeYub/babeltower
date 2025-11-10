using UnityEngine;
using UnityEngine.SceneManagement;

namespace BabelTower.Manager
{
    /// <summary>
    /// 게임 전체를 관리하는 싱글톤 매니저
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private bool isPaused = false;

        // 참조
        private Character.Player player;

        private void Awake()
        {
            // 싱글톤 패턴
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Update()
        {
            // ESC로 일시정지
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            // 게임 설정
            Application.targetFrameRate = 60;
            
            Debug.Log("GameManager Initialized");
        }

        /// <summary>
        /// 씬 로드
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            isPaused = false;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public void StartNewGame(Character.PlayerClass playerClass)
        {
            // TODO: 플레이어 데이터 초기화
            LoadScene("TownScene");
        }

        /// <summary>
        /// 일시정지 토글
        /// </summary>
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            // TODO: 일시정지 UI 표시
            Debug.Log($"Game {(isPaused ? "Paused" : "Resumed")}");
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 플레이어 참조 설정
        /// </summary>
        public void SetPlayer(Character.Player newPlayer)
        {
            player = newPlayer;
        }

        public Character.Player GetPlayer()
        {
            return player;
        }

        public bool IsPaused => isPaused;
    }
}
