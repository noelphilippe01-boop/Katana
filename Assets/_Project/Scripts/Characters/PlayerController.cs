using Katana.CameraSystems;
using Katana.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    [RequireComponent(typeof(CharacterFacing))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float stoppingDistance = 0.35f;
        [SerializeField] float bobAmplitude = 0.035f;
        [SerializeField] float bobFrequency = 10f;
        [SerializeField] float groundHeight = 1f;

        Camera cam;
        CharacterFacing facing;
        Vector3? destination;
        GameObject clickMarker;
        float bobPhase;
        bool isMoving;

        void Awake()
        {
            cam = Camera.main;
            facing = GetComponent<CharacterFacing>();
            CreateClickMarker();
        }

        void Update()
        {
            if (cam == null)
                cam = Camera.main;

            isMoving = false;
            HandleKeyboard();
            HandleMouseClick();
            MoveTowardDestination();
            ApplyMovementBobbing();
        }

        void HandleKeyboard()
        {
            var move = ReadKeyboardMove();
            if (move.sqrMagnitude < 0.01f)
                return;

            destination = null;
            HideMarker();
            GameEventBus.RaiseTargetSelected(null);

            var worldMove = ToWorldDirection(move.normalized);
            transform.position += worldMove * (moveSpeed * Time.deltaTime);
            facing.FaceDirection(worldMove);
            isMoving = true;
        }

        static Vector3 ReadKeyboardMove()
        {
            var move = Vector3.zero;
            var keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (IsLayoutKeyPressed(keyboard, "z", Key.W)) move.z += 1f;
                if (IsLayoutKeyPressed(keyboard, "s", Key.S)) move.z -= 1f;
                if (IsLayoutKeyPressed(keyboard, "q", Key.A)) move.x -= 1f;
                if (IsLayoutKeyPressed(keyboard, "d", Key.D)) move.x += 1f;
                return move;
            }

            if (Input.GetKey(KeyCode.W)) move.z += 1f;
            if (Input.GetKey(KeyCode.S)) move.z -= 1f;
            if (Input.GetKey(KeyCode.A)) move.x -= 1f;
            if (Input.GetKey(KeyCode.D)) move.x += 1f;

            return move;
        }

        static bool IsLayoutKeyPressed(Keyboard keyboard, string layoutKeyName, Key usPhysicalFallback)
        {
            var keyControl = keyboard.FindKeyOnCurrentKeyboardLayout(layoutKeyName);
            if (keyControl == null)
                return keyboard[usPhysicalFallback].isPressed;

            return keyControl.isPressed;
        }

        void HandleMouseClick()
        {
            if (!Input.GetMouseButtonDown(0) || cam == null)
                return;

            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 500f))
                return;

            if (hit.collider.transform == transform)
                return;

            if (hit.collider.CompareTag("Enemy"))
            {
                var enemy = hit.collider.gameObject;
                destination = enemy.transform.position;
                ShowMarker(enemy.transform.position);
                GameEventBus.RaiseTargetSelected(enemy);
                return;
            }

            GameEventBus.RaiseTargetSelected(null);
            destination = hit.point;
            ShowMarker(hit.point);
            GameEventBus.RaisePlayerMoveRequested(hit.point);
        }

        void MoveTowardDestination()
        {
            if (!destination.HasValue)
                return;

            var flatTarget = new Vector3(destination.Value.x, transform.position.y, destination.Value.z);
            var direction = flatTarget - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                destination = null;
                HideMarker();
                return;
            }

            var step = direction.normalized * (moveSpeed * Time.deltaTime);
            transform.position += step;
            facing.FaceDirection(direction);
            isMoving = true;
        }

        void ApplyMovementBobbing()
        {
            if (isMoving)
                bobPhase += Time.deltaTime * bobFrequency;

            var bobOffset = isMoving ? Mathf.Sin(bobPhase) * bobAmplitude : 0f;
            var position = transform.position;
            transform.position = new Vector3(position.x, groundHeight + bobOffset, position.z);
        }

        void Start()
        {
            CameraFollowTarget.EnsureOn(transform, groundHeight);
        }

        Vector3 ToWorldDirection(Vector3 localDir)
        {
            if (cam == null)
                return new Vector3(localDir.x, 0f, localDir.z);

            var forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
            return forward * localDir.z + right * localDir.x;
        }

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

            clickMarker.transform.position = new Vector3(point.x, 0.05f, point.z);
            clickMarker.SetActive(true);
        }

        void HideMarker()
        {
            if (clickMarker != null)
                clickMarker.SetActive(false);
        }
    }
}
