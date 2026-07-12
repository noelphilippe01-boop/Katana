using Katana.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] LayerMask groundLayer = ~0;
        [SerializeField] LayerMask targetLayer = ~0;

        Camera mainCamera;

        void Awake() => mainCamera = Camera.main;

        void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                HandleClick(mouse.position.ReadValue());

            if (keyboard == null)
                return;

            if (keyboard.zKey.isPressed) MoveDirection(Vector3.forward);
            if (keyboard.sKey.isPressed) MoveDirection(Vector3.back);
            if (keyboard.qKey.isPressed) MoveDirection(Vector3.left);
            if (keyboard.dKey.isPressed) MoveDirection(Vector3.right);
        }

        void HandleClick(Vector2 screenPosition)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera == null)
                return;

            var ray = mainCamera.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out var hit, 200f, targetLayer) && hit.collider.CompareTag("Enemy"))
            {
                GameEventBus.RaiseTargetSelected(hit.collider.gameObject);
                return;
            }

            if (Physics.Raycast(ray, out hit, 200f, groundLayer))
                GameEventBus.RaisePlayerMoveRequested(hit.point);
        }

        void MoveDirection(Vector3 localDir)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera == null)
                return;

            var camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            var camRight = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
            var worldDir = camForward * localDir.z + camRight * localDir.x;
            GameEventBus.RaisePlayerMoveRequested(transform.position + worldDir * 3f);
        }
    }
}
