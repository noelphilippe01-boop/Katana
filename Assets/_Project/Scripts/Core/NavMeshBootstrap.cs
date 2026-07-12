using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Katana.Core
{
    [DefaultExecutionOrder(-100)]
    public class NavMeshBootstrap : MonoBehaviour
    {
        [SerializeField] string groundObjectName = "Ground";

        void Awake()
        {
            var ground = GameObject.Find(groundObjectName);
            if (ground == null)
            {
                Debug.LogWarning("Katana: Ground introuvable — NavMesh non généré.");
                return;
            }

            var surface = ground.GetComponent<NavMeshSurface>();
            if (surface == null)
                surface = ground.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<NavMeshAgent>(out var agent))
            {
                if (NavMesh.SamplePosition(player.transform.position, out var hit, 5f, NavMesh.AllAreas))
                    agent.Warp(hit.position);
            }
        }
    }
}
