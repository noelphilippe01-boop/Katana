using UnityEngine;

namespace Katana.Core
{
    [DefaultExecutionOrder(-250)]
    public class SpawnSafeZone : MonoBehaviour
    {
        public static SpawnSafeZone Instance { get; private set; }

        [SerializeField] float safeRadius = 5f;
        [SerializeField] float platformRadius = 4.5f;
        [SerializeField] float platformHeight = 0.35f;
        [SerializeField] Color platformColor = new(0.42f, 0.62f, 0.88f);
        [SerializeField] Color rimColor = new(0.55f, 0.78f, 0.95f);

        public float SafeRadius => safeRadius;
        public float PlatformRadius => platformRadius;
        public float PlatformTopY => platformHeight;
        public float PlayerSpawnHeight => platformHeight + 1f;
        public float PlayerGroundHeight => PlayerSpawnHeight;

        public Vector3 Center => new(transform.position.x, 0f, transform.position.z);
        public Vector3 PlayerSpawnPosition => new(transform.position.x, PlayerSpawnHeight, transform.position.z);

        public static bool TryGet(out SpawnSafeZone zone)
        {
            zone = Instance;
            return zone != null;
        }

        public static bool IsPlayerInside()
        {
            if (Instance == null)
                return false;

            var player = GameObject.FindGameObjectWithTag("Player");
            return player != null && Instance.Contains(player.transform.position);
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsurePlatformVisual();
            SanitizePlatformColliders();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Start() => SnapPlayerToSpawnIfNeeded();

        public bool Contains(Vector3 worldPosition)
        {
            var flat = worldPosition;
            flat.y = 0f;
            return Vector3.Distance(Center, flat) <= safeRadius;
        }

        public bool IsOnPlatformFootprint(Vector3 worldPosition)
        {
            var flat = worldPosition;
            flat.y = 0f;
            return Vector3.Distance(Center, flat) <= platformRadius;
        }

        public bool TrySnapToPlatformSurface(Vector3 worldPoint, out Vector3 surfacePoint)
        {
            if (!IsOnPlatformFootprint(worldPoint))
            {
                surfacePoint = default;
                return false;
            }

            surfacePoint = new Vector3(worldPoint.x, PlatformTopY, worldPoint.z);
            return true;
        }

        public static SpawnSafeZone CreateAt(Vector3 center, Transform parent = null)
        {
            var existing = Object.FindAnyObjectByType<SpawnSafeZone>();
            if (existing != null)
                return existing;

            var root = new GameObject("PlayerSpawnPlatform");
            if (parent != null)
                root.transform.SetParent(parent);

            root.transform.position = new Vector3(center.x, 0f, center.z);
            var zone = root.AddComponent<SpawnSafeZone>();
            zone.EnsurePlatformVisual();
            zone.SanitizePlatformColliders();
            return zone;
        }

        public void EnsurePlatformVisual()
        {
            if (transform.Find("SpawnPlatform") != null)
                return;

            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "SpawnPlatform";
            disc.transform.SetParent(transform);
            disc.transform.localPosition = new Vector3(0f, platformHeight * 0.5f, 0f);
            disc.transform.localScale = new Vector3(platformRadius * 2f, platformHeight, platformRadius * 2f);
            SceneVisualBootstrap.ApplyColor(disc, platformColor, 0.35f);

            var rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "SpawnPlatformRim";
            rim.transform.SetParent(transform);
            rim.transform.localPosition = new Vector3(0f, platformHeight + 0.025f, 0f);
            rim.transform.localScale = new Vector3((platformRadius + 0.15f) * 2f, 0.05f, (platformRadius + 0.15f) * 2f);
            SceneVisualBootstrap.ApplyColor(rim, rimColor, 0.5f);

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "SafeZoneRing";
            ring.transform.SetParent(transform);
            ring.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            ring.transform.localScale = new Vector3(safeRadius * 2f, 0.02f, safeRadius * 2f);
            SceneVisualBootstrap.ApplyColor(ring, new Color(0.35f, 0.55f, 0.75f, 0.45f), 0.15f);
        }

        void SanitizePlatformColliders()
        {
            RemoveCollider(transform.Find("SpawnPlatformRim"));
            RemoveCollider(transform.Find("SafeZoneRing"));
            RemoveCollider(transform.Find("SpawnPlatform"));
            EnsureWalkSurfaceCollider();
        }

        void EnsureWalkSurfaceCollider()
        {
            var walkSurface = transform.Find("PlatformWalkSurface");
            if (walkSurface == null)
            {
                var go = new GameObject("PlatformWalkSurface");
                walkSurface = go.transform;
                walkSurface.SetParent(transform);
            }

            walkSurface.localPosition = new Vector3(0f, platformHeight, 0f);
            walkSurface.localRotation = Quaternion.identity;
            walkSurface.localScale = Vector3.one;

            foreach (var collider in walkSurface.GetComponents<Collider>())
                Destroy(collider);

            var box = walkSurface.gameObject.AddComponent<BoxCollider>();
            box.size = new Vector3(platformRadius * 2f, 0.05f, platformRadius * 2f);
            box.center = Vector3.zero;
        }

        static void RemoveCollider(Transform target)
        {
            if (target == null)
                return;

            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);
        }

        void SnapPlayerToSpawnIfNeeded()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            if (Contains(player.transform.position) || player.transform.position.sqrMagnitude <= safeRadius * safeRadius)
                player.transform.position = PlayerSpawnPosition;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.35f, 0.65f, 0.95f, 0.35f);
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0.1f, transform.position.z), safeRadius);
        }
    }
}
