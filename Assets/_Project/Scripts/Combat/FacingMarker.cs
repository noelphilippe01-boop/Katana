using UnityEngine;

namespace Katana.Combat
{
    public class FacingMarker : MonoBehaviour
    {
        [SerializeField] Color poleColor = new(0.95f, 0.9f, 0.2f);
        [SerializeField] Color tipColor = new(1f, 0.45f, 0.1f);
        [SerializeField] float heightOffset = 1.05f;

        void Awake()
        {
            var root = new GameObject("FacingMarker").transform;
            root.SetParent(transform);
            root.localPosition = new Vector3(0f, heightOffset, 0f);

            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "FacingPole";
            pole.transform.SetParent(root);
            pole.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            pole.transform.localScale = new Vector3(0.07f, 0.18f, 0.07f);
            CombatVisuals.RemoveCollider(pole);
            CombatVisuals.ApplyColor(pole.GetComponent<Renderer>(), poleColor);

            var tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "FacingTip";
            tip.transform.SetParent(root);
            tip.transform.localPosition = new Vector3(0f, 0.42f, 0.28f);
            tip.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            CombatVisuals.RemoveCollider(tip);
            CombatVisuals.ApplyColor(tip.GetComponent<Renderer>(), tipColor);
        }
    }
}
