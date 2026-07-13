using UnityEngine;

namespace Katana.Characters
{
    public class CharacterFacing : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 14f;

        Quaternion? targetRotation;

        public void FaceDirection(Vector3 flatDirection)
        {
            flatDirection.y = 0f;
            if (flatDirection.sqrMagnitude < 0.001f)
                return;

            targetRotation = Quaternion.LookRotation(flatDirection.normalized);
        }

        void LateUpdate()
        {
            if (!targetRotation.HasValue)
                return;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation.Value,
                rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation.Value) < 0.5f)
                transform.rotation = targetRotation.Value;
        }
    }
}
