using Katana.Characters;
using Katana.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaSceneColors
    {
        [MenuItem("Katana/Apply Scene Colors")]
        public static void ApplySceneColors()
        {
            EnsureBootstrapInScene();
            var ground = GameObject.Find("Ground");
            var player = GameObject.Find("Player");

            if (ground == null && player == null)
            {
                Debug.LogWarning("Katana: objets 'Ground' et 'Player' introuvables.");
                return;
            }

            AssetDatabase.SaveAssets();

            if (ground != null)
                KatanaMaterials.ApplyToRenderer(ground, KatanaMaterials.GetOrCreateGroundMaterial());

            if (player != null)
                KatanaMaterials.ApplyToRenderer(player, KatanaMaterials.GetOrCreatePlayerMaterial());

            StyleMainCamera();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: sol vert foncé + joueur bleu appliqués. Sauvegardez la scène (Ctrl+S).");
        }

        static void EnsureBootstrapInScene()
        {
            var managers = GameObject.Find("--- MANAGERS ---") ?? new GameObject("--- MANAGERS ---");

            if (managers.GetComponent<SceneVisualBootstrap>() == null)
                managers.AddComponent<SceneVisualBootstrap>();
            if (managers.GetComponent<NavMeshBootstrap>() == null)
                managers.AddComponent<NavMeshBootstrap>();
            if (managers.GetComponent<SceneLandmarks>() == null)
                managers.AddComponent<SceneLandmarks>();
            if (managers.GetComponent<MoveDestinationMarker>() == null)
                managers.AddComponent<MoveDestinationMarker>();
        }

        static void StyleMainCamera()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
        }
    }
}
