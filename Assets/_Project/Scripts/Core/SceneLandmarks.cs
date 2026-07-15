using UnityEngine;

namespace Katana.Core
{
    [DefaultExecutionOrder(-250)]
    public class SceneLandmarks : MonoBehaviour
    {
        [SerializeField] int gridSpacing = 5;
        [SerializeField] int gridHalfExtent = 40;
        [SerializeField] float pillarHeight = 0.35f;
        [SerializeField] float pillarSize = 0.45f;

        void Awake()
        {
            EnsureSpawnPlatform();
            RemoveLegacySpawnPillar();

            if (GameObject.Find("ReferenceMarkers") != null)
                return;

            CreateLandmarks();
        }

        void EnsureSpawnPlatform() => SpawnSafeZone.CreateAt(Vector3.zero, null);

        void RemoveLegacySpawnPillar()
        {
            var markers = GameObject.Find("ReferenceMarkers");
            if (markers == null)
                return;

            foreach (Transform child in markers.transform)
            {
                if (child.name != "Marker_0_0")
                    continue;

                if (Vector3.Distance(child.position, new Vector3(0f, 1.5f, 0f)) < 0.25f)
                    Destroy(child.gameObject);
            }
        }

        void CreateLandmarks()
        {
            var root = new GameObject("ReferenceMarkers");

            for (var x = -gridHalfExtent; x <= gridHalfExtent; x += gridSpacing)
            {
                for (var z = -gridHalfExtent; z <= gridHalfExtent; z += gridSpacing)
                {
                    if (x == 0 && z == 0)
                        continue;

                    var isDark = ((x / gridSpacing) + (z / gridSpacing)) % 2 == 0;
                    var color = isDark
                        ? new Color(0.22f, 0.28f, 0.2f)
                        : new Color(0.38f, 0.48f, 0.34f);

                    CreatePillar(root.transform, new Vector3(x, pillarHeight * 0.5f, z), pillarSize, pillarHeight, color);
                }
            }

            CreatePillar(root.transform, new Vector3(-45f, 1f, -45f), 1.2f, 2f, new Color(0.85f, 0.35f, 0.2f));
            CreatePillar(root.transform, new Vector3(45f, 1f, -45f), 1.2f, 2f, new Color(0.85f, 0.75f, 0.2f));
            CreatePillar(root.transform, new Vector3(-45f, 1f, 45f), 1.2f, 2f, new Color(0.3f, 0.75f, 0.85f));
            CreatePillar(root.transform, new Vector3(45f, 1f, 45f), 1.2f, 2f, new Color(0.75f, 0.3f, 0.75f));
        }

        static void CreatePillar(Transform parent, Vector3 position, float size, float height, Color color)
        {
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = $"Marker_{position.x}_{position.z}";
            pillar.transform.SetParent(parent);
            pillar.transform.position = position;
            pillar.transform.localScale = new Vector3(size, height, size);
            SceneVisualBootstrap.ApplyColor(pillar, color, 0.2f);
        }
    }
}
