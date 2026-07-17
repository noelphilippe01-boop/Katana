using Katana.CameraSystems;
using Katana.Combat;
using Katana.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaMenuTools
    {
        const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        const string GameWorldScenePath = "Assets/_Project/Scenes/GameWorld.unity";

        [MenuItem("Katana/Setup MainMenu Scene")]
        public static void SetupMainMenuScene()
        {
            EnsureScenesFolder();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            CreateMainMenuCamera();
            CreateMainMenuManagers();

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), MainMenuScenePath);
            ConfigureBuildSettings();

            Debug.Log("Katana: MainMenu pret. Build Settings mis a jour (MainMenu en premier).");
        }

        [MenuItem("Katana/Fix MainMenu Scene")]
        public static void FixMainMenuScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.name != "MainMenu")
            {
                Debug.LogWarning("Katana: ouvre MainMenu.unity puis relance Fix MainMenu Scene.");
                return;
            }

            RemoveCombatObjectsFromMenu();
            CreateMainMenuManagersIfMissing();
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Katana: MainMenu nettoye (composants combat retires).");
        }

        [MenuItem("Katana/Configure Build Settings (Menu + GameWorld)")]
        public static void ConfigureBuildSettingsMenuItem() => ConfigureBuildSettings();

        static void RemoveCombatObjectsFromMenu()
        {
            var spawnPlatform = GameObject.Find("PlayerSpawnPlatform");
            if (spawnPlatform != null)
                Object.DestroyImmediate(spawnPlatform);

            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
                return;

            RemoveComponent<NavMeshRuntimeBootstrap>(managers);
            RemoveComponent<EnemySpawner>(managers);
            RemoveComponent<DamageFloaterSystem>(managers);
            RemoveComponent<PauseMenuController>(managers);
            RemoveComponent<EnemyAggroSystem>(managers);
            RemoveComponent<CombatHud>(managers);
            RemoveComponent<GameBootstrapper>(managers);
            RemoveComponent<SceneLandmarks>(managers);
            RemoveComponent<SceneVisualBootstrap>(managers);
            RemoveComponent<IsometricCameraController>(managers);
        }

        static void CreateMainMenuManagersIfMissing()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
                managers = new GameObject("--- MANAGERS ---");

            if (managers.GetComponent<MainMenuBootstrapper>() == null)
                managers.AddComponent<MainMenuBootstrapper>();

            if (managers.GetComponent<MainMenuUI>() == null)
                managers.AddComponent<MainMenuUI>();

            if (managers.GetComponent<SceneFlowController>() == null)
                managers.AddComponent<SceneFlowController>();
        }

        static void RemoveComponent<T>(GameObject go) where T : Component
        {
            foreach (var component in go.GetComponents<T>())
                Object.DestroyImmediate(component);
        }

        static void CreateMainMenuCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.1f, 0.16f);

            cameraObject.AddComponent<AudioListener>();
        }

        static void CreateMainMenuManagers()
        {
            var managers = new GameObject("--- MANAGERS ---");
            managers.AddComponent<MainMenuBootstrapper>();
            managers.AddComponent<MainMenuUI>();
            managers.AddComponent<SceneFlowController>();
        }

        static void ConfigureBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

            AddSceneIfExists(scenes, MainMenuScenePath);
            AddSceneIfExists(scenes, GameWorldScenePath);

            if (scenes.Count == 0)
            {
                Debug.LogWarning("Katana: aucune scene trouvee pour le build.");
                return;
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        static void AddSceneIfExists(System.Collections.Generic.List<EditorBuildSettingsScene> scenes, string scenePath)
        {
            if (!System.IO.File.Exists(scenePath))
                return;

            foreach (var existing in scenes)
            {
                if (existing.path == scenePath)
                    return;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
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
    }
}
