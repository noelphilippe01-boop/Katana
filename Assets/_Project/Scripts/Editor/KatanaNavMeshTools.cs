using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaNavMeshTools
    {
        [MenuItem("Katana/Bake NavMesh (editor only)")]
        public static void BakeNavMesh()
        {
            var ground = GameObject.Find("Ground");
            if (ground == null)
            {
                Debug.LogWarning("Katana: objet 'Ground' introuvable.");
                return;
            }

            var surface = ground.GetComponent<NavMeshSurface>();
            if (surface == null)
                surface = ground.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: NavMesh bake pour usage futur (IA ennemis).");
        }
    }
}
