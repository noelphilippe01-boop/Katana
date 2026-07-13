using UnityEngine;

namespace Katana.Combat
{
    static class CombatVisuals
    {
        public static void ApplyColor(Renderer renderer, Color color)
        {
            if (renderer == null)
                return;

            var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
                renderer.material = new Material(shader) { color = color };
        }

        public static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
