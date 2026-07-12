using Unity.Cinemachine;
using UnityEngine;

namespace Katana.CameraSystems
{
    public class IsometricCameraController : MonoBehaviour
    {
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] Transform target;

        void LateUpdate()
        {
            if (cinemachineCamera != null && target != null)
                cinemachineCamera.Follow = target;
        }
    }
}
