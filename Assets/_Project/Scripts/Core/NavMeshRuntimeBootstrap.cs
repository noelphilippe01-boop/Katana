using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    [DefaultExecutionOrder(-150)]
    public class NavMeshRuntimeBootstrap : MonoBehaviour
    {
        public static bool IsReady { get; private set; }
        public static event Action Ready;

        [SerializeField] string groundObjectName = "Ground";

        void Awake()
        {
            IsReady = false;
            TryBuildNavMesh();
        }

        void Start()
        {
            if (IsReady)
                return;

            TryBuildNavMesh();
        }

        public static void EnsureReady(Action onReady)
        {
            if (IsReady)
            {
                onReady?.Invoke();
                return;
            }

            void Handler()
            {
                Ready -= Handler;
                onReady?.Invoke();
            }

            Ready += Handler;
        }

        void TryBuildNavMesh()
        {
            if (SceneManager.GetActiveScene().name != GameScenes.GameWorld)
            {
                IsReady = false;
                return;
            }

            BuildNavMesh();
        }

        void BuildNavMesh()
        {
            var ground = GameObject.Find(groundObjectName);
            if (ground == null)
            {
                Debug.LogWarning("Katana: Ground introuvable dans GameWorld — NavMesh non genere.");
                IsReady = false;
                return;
            }

            var surface = ground.GetComponent<NavMeshSurface>();
            if (surface == null)
                surface = ground.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();

            IsReady = HasValidNavMesh();
            if (IsReady)
                Ready?.Invoke();
            else
                Debug.LogWarning("Katana: NavMesh bake termine mais aucune zone walkable detectee.");
        }

        static bool HasValidNavMesh() =>
            NavMesh.SamplePosition(Vector3.zero, out _, 80f, NavMesh.AllAreas);
    }
}
