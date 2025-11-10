using UnityEngine;

namespace BabelTower
{
    /// <summary>
    /// 디아블로 스타일 쿼터뷰 카메라
    /// </summary>
    public class IsometricCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindPlayer = true;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private float isoAngle = 45f;

        [Header("Bounds (Optional)")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        [Header("Zoom")]
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float zoomSpeed = 1f;
        private float currentZoom = 6f;

        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();

            // 플레이어 자동 찾기
            if (autoFindPlayer && target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            // 초기 카메라 각도 설정
            SetIsometricAngle();

            // 초기 줌 설정
            if (cam != null)
            {
                cam.orthographicSize = currentZoom;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 부드러운 카메라 추적
            FollowTarget();

            // 줌 처리
            HandleZoom();
        }

        /// <summary>
        /// 타겟 추적
        /// </summary>
        private void FollowTarget()
        {
            Vector3 desiredPosition = target.position + offset;

            // 바운드 제한
            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            // 부드러운 이동
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        /// <summary>
        /// 줌 처리
        /// </summary>
        private void HandleZoom()
        {
            if (cam == null) return;

            // 마우스 휠로 줌
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                currentZoom -= scroll * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, smoothSpeed);
            }
        }

        /// <summary>
        /// 아이소메트릭 각도 설정
        /// </summary>
        private void SetIsometricAngle()
        {
            transform.rotation = Quaternion.Euler(isoAngle, 0, 0);
        }

        /// <summary>
        /// 타겟 설정
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// 즉시 타겟 위치로 이동
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = desiredPosition;
        }

        /// <summary>
        /// 바운드 설정
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            useBounds = true;
            minBounds = min;
            maxBounds = max;
        }

        /// <summary>
        /// 기즈모로 바운드 표시
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!useBounds) return;

            Gizmos.color = Color.yellow;
            
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            
            Gizmos.DrawWireCube(center, size);
        }
    }
}
