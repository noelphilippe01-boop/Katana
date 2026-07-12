using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float stoppingDistance = 0.3f;
        [SerializeField] float gravity = -20f;

        CharacterController controller;
        Vector3? destination;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            SnapToGround();
            GameEventBus.PlayerMoveRequested += OnMoveRequested;
        }

        void OnDestroy() => GameEventBus.PlayerMoveRequested -= OnMoveRequested;

        void OnMoveRequested(Vector3 target) => destination = target;

        void Update()
        {
            var motion = Vector3.zero;

            if (destination.HasValue)
            {
                var flatTarget = new Vector3(destination.Value.x, transform.position.y, destination.Value.z);
                var direction = flatTarget - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude <= stoppingDistance * stoppingDistance)
                    destination = null;
                else
                    motion += direction.normalized * moveSpeed;
            }

            if (controller.isGrounded && motion.y < 0f)
                motion.y = -1f;
            else
                motion.y += gravity;

            controller.Move(motion * Time.deltaTime);
        }

        void SnapToGround()
        {
            var origin = transform.position + Vector3.up * 2f;
            if (Physics.Raycast(origin, Vector3.down, out var hit, 10f))
            {
                var y = hit.point.y + controller.height * 0.5f;
                controller.enabled = false;
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
                controller.enabled = true;
            }
        }
    }
}
