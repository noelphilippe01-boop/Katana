using Katana.Combat;
using Katana.Characters;
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
            EnsureTags();
            EnsurePlayerCombat();
            EnsureCombatHud();
            EnsureEnemySpawner();
            UpgradeExistingEnemies();
            KatanaCameraTools.FixCameraInScene();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Combat avance pret (spawn, loot, barres PV, repere orientation).");
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

            if (player.GetComponent<PlayerCombat>() == null)
                player.AddComponent<PlayerCombat>();

            if (player.GetComponent<PlayerInventory>() == null)
                player.AddComponent<PlayerInventory>();

            if (player.GetComponent<FacingMarker>() == null)
                player.AddComponent<FacingMarker>();
        }

        static void EnsureCombatHud()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
            {
                Debug.LogWarning("Katana: --- MANAGERS --- introuvable.");
                return;
            }

            if (managers.GetComponent<CombatHud>() == null)
                managers.AddComponent<CombatHud>();
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
            }
        }
    }
}
