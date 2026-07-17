using Katana.CameraSystems;
using Katana.Combat;
using Katana.Characters;
using Katana.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaCombatTools
    {
        [MenuItem("Katana/Setup Combat (current scene)")]
        public static void SetupCombatInScene()
        {
            if (SceneManager.GetActiveScene().name == GameScenes.MainMenu)
            {
                Debug.LogWarning("Katana: Setup Combat ignore sur MainMenu. Ouvre GameWorld.unity.");
                return;
            }

            EnsureTags();
            EnsureNavMeshBootstrap();
            EnsureSpawnSafeZone();
            EnsurePlayerCombat();
            EnsureCombatSystems();
            EnsureEnemySpawner();
            UpgradeExistingEnemies();
            KatanaCameraTools.FixCameraInScene();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Combat pret (3 armes: katana cleave, arc, baton mage).");
        }

        [MenuItem("Katana/Add Test Enemy")]
        public static void AddTestEnemy()
        {
            EnsureTags();
            var position = new Vector3(Random.Range(-8f, 8f), 1f, Random.Range(-8f, 8f));
            EnemyFactory.Create(position);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Ennemi ajoute.");
        }

        [MenuItem("Katana/Cleanup Scene (current scene)")]
        public static void CleanupCurrentScene()
        {
            RemoveMissingScripts();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Scene nettoyee (scripts manquants retires).");
        }

        static void RemoveMissingScripts()
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
                RemoveMissingScriptsRecursive(root);
        }

        static void RemoveMissingScriptsRecursive(GameObject go)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            foreach (Transform child in go.transform)
                RemoveMissingScriptsRecursive(child.gameObject);
        }

        static void EnsureTags()
        {
            EnsureTag("Player");
            EnsureTag("Enemy");
        }

        static void EnsureTag(string tag)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (assets == null || assets.Length == 0)
                return;

            var tagManager = new SerializedObject(assets[0]);
            var tags = tagManager.FindProperty("tags");
            for (var i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                    return;
            }

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }

        static void EnsurePlayerCombat()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Katana: Player introuvable.");
                return;
            }

            if (player.GetComponent<PlayerController>() == null)
                player.AddComponent<PlayerController>();

            if (player.GetComponent<CharacterFacing>() == null)
                player.AddComponent<CharacterFacing>();

            if (player.GetComponent<WeaponLoadout>() == null)
                player.AddComponent<WeaponLoadout>();

            if (player.GetComponent<PlayerStats>() == null)
                player.AddComponent<PlayerStats>();

            if (player.GetComponent<PlayerCombat>() == null)
                player.AddComponent<PlayerCombat>();

            if (player.GetComponent<PlayerInventory>() == null)
                player.AddComponent<PlayerInventory>();

            if (player.GetComponent<FacingMarker>() == null)
                player.AddComponent<FacingMarker>();

            if (player.GetComponent<PlayerHealth>() == null)
                player.AddComponent<PlayerHealth>();

            if (player.GetComponent<AttackRangeIndicator>() == null)
                player.AddComponent<AttackRangeIndicator>();
        }

        static void EnsureSpawnSafeZone()
        {
            if (Object.FindAnyObjectByType<SpawnSafeZone>() != null)
                return;

            SpawnSafeZone.CreateAt(Vector3.zero);
        }

        static void EnsureNavMeshBootstrap()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
            {
                Debug.LogWarning("Katana: --- MANAGERS --- introuvable.");
                return;
            }

            if (managers.GetComponent<NavMeshRuntimeBootstrap>() == null)
                managers.AddComponent<NavMeshRuntimeBootstrap>();
        }

        static void EnsureCombatSystems()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
            {
                Debug.LogWarning("Katana: --- MANAGERS --- introuvable.");
                return;
            }

            if (managers.GetComponent<CombatHud>() == null)
                managers.AddComponent<CombatHud>();

            if (managers.GetComponent<EnemyAggroSystem>() == null)
                managers.AddComponent<EnemyAggroSystem>();

            if (managers.GetComponent<PauseMenuController>() == null)
                managers.AddComponent<PauseMenuController>();

            if (managers.GetComponent<DamageFloaterSystem>() == null)
                managers.AddComponent<DamageFloaterSystem>();
        }

        static void EnsureEnemySpawner()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
            {
                Debug.LogWarning("Katana: --- MANAGERS --- introuvable.");
                return;
            }

            if (managers.GetComponent<EnemySpawner>() != null)
                return;

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                Object.DestroyImmediate(enemy);

            managers.AddComponent<EnemySpawner>();
        }

        static void UpgradeExistingEnemies()
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (enemy.GetComponent<WorldHealthBar>() == null)
                    enemy.AddComponent<WorldHealthBar>();

                if (enemy.GetComponent<LootDropper>() == null)
                    enemy.AddComponent<LootDropper>();

                if (enemy.GetComponent<FacingMarker>() == null)
                    enemy.AddComponent<FacingMarker>();

                if (enemy.GetComponent<CharacterFacing>() == null)
                    enemy.AddComponent<CharacterFacing>();

                var ai = enemy.GetComponent<EnemyAI>() ?? enemy.AddComponent<EnemyAI>();
                ai.Configure(8f);

                if (enemy.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
                {
                    var agent = enemy.AddComponent<UnityEngine.AI.NavMeshAgent>();
                    agent.height = 2f;
                    agent.radius = 0.4f;
                }
            }
        }
    }
}
