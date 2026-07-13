using UnityEngine;

namespace Katana.CameraSystems
{
    public class CameraFollowTarget : MonoBehaviour
    {
        [SerializeField] float stableHeight = 1f;

        Transform playerRoot;

        void Awake()
        {
            playerRoot = transform.parent;
        }

        void LateUpdate()
        {
            if (playerRoot == null)
                return;

            var position = playerRoot.position;
            transform.position = new Vector3(position.x, stableHeight, position.z);
        }

        public static Transform EnsureOn(Transform player, float height = 1f)
        {
            var existing = player.Find("CameraTarget");
            if (existing != null)
                return existing;

            var targetObject = new GameObject("CameraTarget");
            targetObject.transform.SetParent(player);
            targetObject.transform.localPosition = Vector3.zero;

            var followTarget = targetObject.AddComponent<CameraFollowTarget>();
            followTarget.stableHeight = height;
            return targetObject.transform;
        }
    }
}
