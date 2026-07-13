using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.CameraSystems
{
    public class IsometricCameraController : MonoBehaviour
    {
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] Transform target;
        [SerializeField] float pitch = 45f;
        [SerializeField] float pitchAtMinZoom = 33f;
        [SerializeField] float pitchAtMaxZoom = 55f;
        [SerializeField] float yaw = -35f;
        [SerializeField] float zoomDistance = 20f;
        [SerializeField] float minZoom = 11f;
        [SerializeField] float maxZoom = 34f;
        [SerializeField] float zoomStep = 1.4f;
        [SerializeField] float zoomSmoothing = 8f;

        CinemachineFollow follow;
        float targetZoom;
        float currentZoom;

        void Awake()
        {
            if (cinemachineCamera == null)
                cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();

            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = CameraFollowTarget.EnsureOn(player.transform);
            }

            targetZoom = zoomDistance;
            currentZoom = zoomDistance;
            EnsureCinemachineSetup();
        }

        void LateUpdate()
        {
            if (cinemachineCamera == null || target == null)
                return;

            if (follow == null)
                follow = cinemachineCamera.GetComponent<CinemachineFollow>();

            cinemachineCamera.Target.TrackingTarget = target;
            cinemachineCamera.Target.LookAtTarget = null;
            cinemachineCamera.Follow = target;
            cinemachineCamera.LookAt = null;

            HandleZoomInput();

            if (follow != null)
            {
                currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSmoothing * Time.deltaTime);
                var currentPitch = GetPitchForZoom(currentZoom);
                follow.FollowOffset = ComputeFollowOffset(currentZoom, currentPitch);
                cinemachineCamera.transform.rotation = Quaternion.Euler(currentPitch, yaw, 0f);
            }
        }

        void EnsureCinemachineSetup()
        {
            if (cinemachineCamera == null || target == null)
                return;

            var hardLookAt = cinemachineCamera.GetComponent<CinemachineHardLookAt>();
            if (hardLookAt != null)
                Destroy(hardLookAt);

            cinemachineCamera.Target.TrackingTarget = target;
            cinemachineCamera.Target.LookAtTarget = null;
            cinemachineCamera.Follow = target;
            cinemachineCamera.LookAt = null;

            follow = cinemachineCamera.GetComponent<CinemachineFollow>();
            if (follow == null)
                follow = cinemachineCamera.gameObject.AddComponent<CinemachineFollow>();

            follow.FollowOffset = ComputeFollowOffset(zoomDistance, pitch);
            follow.TrackerSettings.BindingMode = BindingMode.WorldSpace;
            follow.TrackerSettings.PositionDamping = Vector3.zero;

            cinemachineCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        float GetPitchForZoom(float distance)
        {
            if (distance <= zoomDistance)
            {
                var t = Mathf.InverseLerp(minZoom, zoomDistance, distance);
                return Mathf.Lerp(pitchAtMinZoom, pitch, t);
            }

            var tFar = Mathf.InverseLerp(zoomDistance, maxZoom, distance);
            return Mathf.Lerp(pitch, pitchAtMaxZoom, tFar);
        }

        void HandleZoomInput()
        {
            var scroll = ReadScrollDelta();
            if (Mathf.Abs(scroll) < 0.01f)
                return;

            targetZoom = Mathf.Clamp(targetZoom - scroll * zoomStep, minZoom, maxZoom);
        }

        static float ReadScrollDelta()
        {
            var mouse = Mouse.current;
            if (mouse != null)
            {
                var scroll = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                    return Mathf.Sign(scroll);
            }

            return Input.mouseScrollDelta.y;
        }

        Vector3 ComputeFollowOffset(float distance, float currentPitch)
        {
            var rotation = Quaternion.Euler(currentPitch, yaw, 0f);
            return rotation * (Vector3.back * distance);
        }
    }
}
