using UnityEngine;

namespace Katana.Core
{
    public class SceneVisualBootstrap : MonoBehaviour
    {
        [SerializeField] Color groundColor = new(0.28f, 0.38f, 0.26f);
        [SerializeField] Color playerColor = new(0.15f, 0.45f, 0.95f);
        [SerializeField] Color skyColor = new(0.12f, 0.14f, 0.18f);

        void Awake()
        {
            var ground = GameObject.Find("Ground");
            var player = GameObject.Find("Player");

            if (ground != null)
                ApplyColor(ground, groundColor, 0.1f);

            if (player != null)
                ApplyColor(player, playerColor, 0.6f);

            if (Camera.main != null)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = skyColor;
            }
        }

        public static void ApplyColor(GameObject go, Color color, float smoothness)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null)
                return;

            var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                return;

            var material = new Material(shader) { color = color };

            if (material.HasProperty("_Glossiness"))
                material.SetFloat("_Glossiness", smoothness);
            else if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);

            renderer.material = material;
        }
    }
}
