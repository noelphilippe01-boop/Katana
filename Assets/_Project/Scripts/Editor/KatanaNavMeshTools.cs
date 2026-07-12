using Katana.Core;
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
        [MenuItem("Katana/Bake NavMesh")]
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

            var managers = GameObject.Find("--- MANAGERS ---") ?? new GameObject("--- MANAGERS ---");
            if (managers.GetComponent<NavMeshBootstrap>() == null)
                managers.AddComponent<NavMeshBootstrap>();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<NavMeshAgent>(out var agent))
            {
                if (NavMesh.SamplePosition(player.transform.position, out var hit, 5f, NavMesh.AllAreas))
                    agent.Warp(hit.position);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: NavMesh bake terminé.");
        }
    }
}
