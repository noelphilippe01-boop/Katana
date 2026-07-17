using Katana.CameraSystems;
using Katana.Combat;
using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(CharacterFacing))]
    public class PlayerController : MonoBehaviour
    {
        const float MinWalkableNormalY = 0.55f;

        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float stoppingDistance = 0.35f;
        [SerializeField] float bobAmplitude = 0.035f;
        [SerializeField] float bobFrequency = 10f;
        [SerializeField] float openGroundHeight = 1f;
        [SerializeField] float attackPickScreenRadius = 56f;

        Camera cam;
        CharacterFacing facing;
        PlayerStats stats;
        GameObject clickMarker;
        float bobPhase;
        bool isMoving;
        Vector3? moveDestination;
        float markerHeight = 0.05f;

        public bool IsMoving { get; private set; }

        float CurrentMoveSpeed => stats != null ? stats.MoveSpeed : moveSpeed;

        void Awake()
        {
            cam = Camera.main;
            facing = GetComponent<CharacterFacing>();
            stats = GetComponent<PlayerStats>();
            CreateClickMarker();
        }

        void Update()
        {
            if (cam == null)
                cam = Camera.main;

            isMoving = false;
            HandleMouseMovement();
            MoveTowardStoredDestination();
            ApplyMovementBobbing();
            IsMoving = isMoving;
        }

        void HandleMouseMovement()
        {
            if (cam == null || !Input.GetMouseButton(0))
                return;

            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!TryResolveMovePoint(ray, out var targetPoint, out var enemyHit))
                return;

            if (Input.GetMouseButtonDown(0))
            {
                var attackTarget = enemyHit ?? CombatTargetQuery.FindNearestEnemyNearScreenPoint(
                    cam,
                    Input.mousePosition,
                    attackPickScreenRadius);

                if (attackTarget != null)
                {
                    GameEventBus.RaiseTargetSelected(attackTarget);
                    targetPoint = FlattenToWalkHeight(attackTarget.transform.position);
                }
                else
                    GetComponent<PlayerCombat>()?.DisengageCombat();
            }

            moveDestination = targetPoint;
            ShowMarker(new Vector3(targetPoint.x, markerHeight, targetPoint.z));
        }

        bool TryResolveMovePoint(Ray ray, out Vector3 point, out GameObject enemyHit)
        {
            enemyHit = null;
            var hits = Physics.RaycastAll(ray, 500f);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (IsSelf(hit.transform))
                    continue;

                if (Input.GetMouseButtonDown(0) && hit.collider.CompareTag("Enemy"))
                    enemyHit = hit.collider.gameObject;

                if (hit.normal.y < MinWalkableNormalY)
                    continue;

                point = FlattenToWalkHeight(hit.point);
                return true;
            }

            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (!groundPlane.Raycast(ray, out var distance))
            {
                point = default;
                return false;
            }

            point = FlattenToWalkHeight(ray.GetPoint(distance));
            return true;
        }

        Vector3 FlattenToWalkHeight(Vector3 worldPoint)
        {
            if (SpawnSafeZone.TryGet(out var zone) && zone.TrySnapToPlatformSurface(worldPoint, out var platformPoint))
                return platformPoint;

            worldPoint.y = openGroundHeight;
            return worldPoint;
        }

        float ResolveGroundHeight(Vector3 position)
        {
            if (SpawnSafeZone.TryGet(out var zone) && zone.IsOnPlatformFootprint(position))
                return zone.PlayerGroundHeight;

            return openGroundHeight;
        }

        void MoveTowardStoredDestination()
        {
            if (!moveDestination.HasValue)
                return;

            if (MoveTowardPoint(moveDestination.Value))
                ClearDestination();
        }

        bool MoveTowardPoint(Vector3 flatTarget)
        {
            var groundHeight = ResolveGroundHeight(flatTarget);
            flatTarget.y = groundHeight;
            var direction = flatTarget - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= stoppingDistance * stoppingDistance)
                return true;

            var step = direction.normalized * (CurrentMoveSpeed * Time.deltaTime);
            transform.position += step;

            var combat = GetComponent<PlayerCombat>();
            if (combat == null || !combat.IsTargetInRange())
                facing.FaceDirection(direction);

            isMoving = true;
            return false;
        }

        void ClearDestination()
        {
            moveDestination = null;
            HideMarker();
        }

        void ApplyMovementBobbing()
        {
            if (isMoving)
                bobPhase += Time.deltaTime * bobFrequency;

            var groundHeight = ResolveGroundHeight(transform.position);
            var bobOffset = isMoving ? Mathf.Sin(bobPhase) * bobAmplitude : 0f;
            var position = transform.position;
            transform.position = new Vector3(position.x, groundHeight + bobOffset, position.z);
        }

        void Start()
        {
            if (SpawnSafeZone.TryGet(out var zone))
                markerHeight = zone.PlatformTopY + 0.02f;

            CameraFollowTarget.EnsureOn(transform, ResolveGroundHeight(transform.position));
        }

        bool IsSelf(Transform hitTransform) =>
            hitTransform == transform || hitTransform.IsChildOf(transform);

        void CreateClickMarker()
        {
            clickMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            clickMarker.name = "ClickMarker";
            Destroy(clickMarker.GetComponent<Collider>());
            clickMarker.transform.localScale = new Vector3(1.2f, 0.05f, 1.2f);
            clickMarker.SetActive(false);
        }

        void ShowMarker(Vector3 point)
        {
            if (clickMarker == null)
                return;

            clickMarker.transform.position = point;
            clickMarker.SetActive(true);
        }

        void HideMarker()
        {
            if (clickMarker != null)
                clickMarker.SetActive(false);
        }
    }
}
