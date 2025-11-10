using UnityEngine;
using BabelTower.Character;

namespace BabelTower
{
    /// <summary>
    /// 테스트용 씬 설정
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerClass playerClass = PlayerClass.Warrior;
        [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero;

        [Header("Monsters")]
        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private int monsterCount = 5;
        [SerializeField] private float spawnRadius = 10f;

        [Header("Camera")]
        [SerializeField] private GameObject cameraPrefab;

        private void Start()
        {
            SetupScene();
        }

        /// <summary>
        /// 씬 설정
        /// </summary>
        private void SetupScene()
        {
            // 플레이어 생성
            SpawnPlayer();

            // 몬스터 생성
            SpawnMonsters();

            // 카메라 설정
            SetupCamera();
        }

        /// <summary>
        /// 플레이어 생성
        /// </summary>
        private void SpawnPlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab not assigned!");
                return;
            }

            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            playerObj.tag = "Player";
            playerObj.layer = LayerMask.NameToLayer("Player");

            Player player = playerObj.GetComponent<Player>();
            if (player != null)
            {
                // 직업 설정은 인스펙터에서
                Debug.Log($"Player spawned as {playerClass}");
            }
        }

        /// <summary>
        /// 몬스터 생성
        /// </summary>
        private void SpawnMonsters()
        {
            if (monsterPrefab == null)
            {
                Debug.LogWarning("Monster prefab not assigned!");
                return;
            }

            for (int i = 0; i < monsterCount; i++)
            {
                // 랜덤 위치 생성
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = new Vector3(randomPos.x, randomPos.y, 0);

                // 몬스터 생성
                GameObject monsterObj = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                monsterObj.tag = "Enemy";
                monsterObj.layer = LayerMask.NameToLayer("Enemy");

                Monster monster = monsterObj.GetComponent<Monster>();
                if (monster != null)
                {
                    // 레벨 랜덤 설정
                    monster.SetLevel(Random.Range(1, 5));
                }
            }

            Debug.Log($"Spawned {monsterCount} monsters");
        }

        /// <summary>
        /// 카메라 설정
        /// </summary>
        private void SetupCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                if (cameraPrefab != null)
                {
                    GameObject camObj = Instantiate(cameraPrefab);
                    mainCam = camObj.GetComponent<Camera>();
                }
                else
                {
                    Debug.LogError("No main camera found!");
                    return;
                }
            }

            // IsometricCamera 컴포넌트 추가 또는 설정
            IsometricCamera isoCam = mainCam.GetComponent<IsometricCamera>();
            if (isoCam == null)
            {
                isoCam = mainCam.gameObject.AddComponent<IsometricCamera>();
            }

            // 플레이어 찾아서 타겟 설정
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                isoCam.SetTarget(player.transform);
                isoCam.SnapToTarget();
            }

            Debug.Log("Camera setup complete");
        }

        /// <summary>
        /// 기즈모로 스폰 범위 표시
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerSpawnPosition, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerSpawnPosition, spawnRadius);
        }
    }
}
