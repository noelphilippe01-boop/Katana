using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class TargetHighlight : MonoBehaviour
    {
        [SerializeField] float ringDiameter = 1.6f;
        [SerializeField] float ringHeight = 0.06f;
        [SerializeField] Color ringColor = new(1f, 0.25f, 0.2f, 0.85f);

        GameObject ring;

        void Awake()
        {
            ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "SelectionRing";
            ring.transform.SetParent(transform);
            ring.transform.localPosition = new Vector3(0f, -0.45f, 0f);
            ring.transform.localScale = new Vector3(ringDiameter, ringHeight, ringDiameter);
            Destroy(ring.GetComponent<Collider>());

            var renderer = ring.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                    renderer.material = new Material(shader) { color = ringColor };
            }

            ring.SetActive(false);
        }

        void OnEnable() => GameEventBus.TargetSelected += OnTargetSelected;
        void OnDisable() => GameEventBus.TargetSelected -= OnTargetSelected;

        void OnTargetSelected(GameObject target) =>
            ring.SetActive(target != null && target == gameObject);
    }
}
