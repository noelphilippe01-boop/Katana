using Katana.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float stoppingDistance = 0.35f;

        Camera cam;
        Vector3? destination;
        GameObject clickMarker;

        void Awake()
        {
            cam = Camera.main;
            CreateClickMarker();
        }

        void Update()
        {
            if (cam == null)
                cam = Camera.main;

            HandleKeyboard();
            HandleMouseClick();
            MoveTowardDestination();
        }

        void HandleKeyboard()
        {
            var move = ReadKeyboardMove();
            if (move.sqrMagnitude < 0.01f)
                return;

            destination = null;
            HideMarker();

            var worldMove = ToWorldDirection(move.normalized);
            transform.position += worldMove * (moveSpeed * Time.deltaTime);
        }

        static Vector3 ReadKeyboardMove()
        {
            var move = Vector3.zero;
            var keyboard = Keyboard.current;

            if (keyboard != null)
            {
                // Lit les touches affichees sur le clavier actif (AZERTY: Z/Q/S/D).
                if (IsLayoutKeyPressed(keyboard, "z", Key.W)) move.z += 1f;
                if (IsLayoutKeyPressed(keyboard, "s", Key.S)) move.z -= 1f;
                if (IsLayoutKeyPressed(keyboard, "q", Key.A)) move.x -= 1f;
                if (IsLayoutKeyPressed(keyboard, "d", Key.D)) move.x += 1f;
                return move;
            }

            // Legacy Input: KeyCode = positions physiques US (= cluster ZQSD sur AZERTY).
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

        static bool IsForwardPressed()
        {
            var keyboard = Keyboard.current;
            return keyboard != null
                ? IsLayoutKeyPressed(keyboard, "z", Key.W)
                : Input.GetKey(KeyCode.W);
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

            transform.position += direction.normalized * (moveSpeed * Time.deltaTime);
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

        void OnGUI()
        {
            var keyboard = Keyboard.current;
            var wLegacy = Input.GetKey(KeyCode.W);
            GUI.Label(new Rect(12f, 12f, 520f, 100f),
                $"Katana debug\nPosition: {transform.position:F1}\nZ AZERTY: {IsForwardPressed()}\nW legacy: {wLegacy}\nInput: {(keyboard != null ? "New" : "Old")}");
        }
    }
}
