using Katana.CameraSystems;
using Katana.Characters;
using Katana.Combat;
using Katana.Core;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaSceneSetup
    {
        const string ScenePath = "Assets/_Project/Scenes/GameWorld.unity";

        [MenuItem("Katana/Setup GameWorld Scene")]
        public static void SetupGameWorld()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "GameWorld";

            CreateLighting();
            var ground = CreateGround();
            var player = CreatePlayer();
            var cameraRig = CreateCameraRig(player.transform);
            StyleMainCamera();
            CreateManagers(player.transform, cameraRig);

            EnsureScenesFolder();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), ScenePath);
            BakeNavMesh(ground);
            AddSceneToBuildSettings(ScenePath);

            Debug.Log("Katana: GameWorld ready. Clic sol = deplacer, clic ennemi = attaquer, ZQSD = deplacer.");
        }

        static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var s in scenes)
            {
                if (s.path == scenePath)
                    return;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Scenes"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project"))
                    AssetDatabase.CreateFolder("Assets", "_Project");
                AssetDatabase.CreateFolder("Assets/_Project", "Scenes");
            }
        }

        static void CreateLighting()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        static GameObject CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            ground.isStatic = true;
            KatanaMaterials.ApplyToRenderer(ground, KatanaMaterials.GetOrCreateGroundMaterial());
            return ground;
        }

        [MenuItem("Katana/Style Ground (existing scene)")]
        public static void StyleGroundInScene() => KatanaSceneColors.ApplySceneColors();

        static GameObject CreatePlayer()
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

            player.AddComponent<CharacterFacing>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerCombat>();
            player.AddComponent<PlayerInventory>();
            player.AddComponent<FacingMarker>();
            KatanaMaterials.ApplyToRenderer(player, KatanaMaterials.GetOrCreatePlayerMaterial());

            return player;
        }

        static CinemachineCamera CreateCameraRig(Transform player)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var cameraGo = new GameObject("Main Camera");
                mainCamera = cameraGo.AddComponent<Camera>();
                cameraGo.tag = "MainCamera";
                cameraGo.AddComponent<AudioListener>();
            }

            var brain = mainCamera.GetComponent<CinemachineBrain>();
            if (brain == null)
                mainCamera.gameObject.AddComponent<CinemachineBrain>();

            var cameraTarget = CameraFollowTarget.EnsureOn(player);

            var cmGo = new GameObject("CM_Isometric");
            var cmCamera = cmGo.AddComponent<CinemachineCamera>();
            cmCamera.Follow = cameraTarget;
            cmCamera.LookAt = null;
            cmCamera.Target.TrackingTarget = cameraTarget;
            cmCamera.Target.LookAtTarget = null;

            var follow = cmGo.AddComponent<CinemachineFollow>();
            follow.FollowOffset = new Vector3(10f, 15f, -10f).normalized * 20f;
            follow.TrackerSettings.BindingMode = BindingMode.WorldSpace;
            follow.TrackerSettings.PositionDamping = Vector3.zero;
            cmGo.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            return cmCamera;
        }

        static void StyleMainCamera()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
        }

        static void CreateManagers(Transform player, CinemachineCamera cmCamera)
        {
            var managers = new GameObject("--- MANAGERS ---");
            managers.AddComponent<GameBootstrapper>();
            managers.AddComponent<SceneVisualBootstrap>();
            managers.AddComponent<NavMeshBootstrap>();
            managers.AddComponent<SceneLandmarks>();
            managers.AddComponent<MoveDestinationMarker>();
            managers.AddComponent<CombatHud>();
            managers.AddComponent<EnemySpawner>();

            var cameraController = managers.AddComponent<IsometricCameraController>();
            var so = new SerializedObject(cameraController);
            so.FindProperty("cinemachineCamera").objectReferenceValue = cmCamera;
            so.FindProperty("target").objectReferenceValue = CameraFollowTarget.EnsureOn(player);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void BakeNavMesh(GameObject ground)
        {
            var surface = ground.GetComponent<NavMeshSurface>();
            if (surface == null)
                surface = ground.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            surface.BuildNavMesh();
        }
    }
}
