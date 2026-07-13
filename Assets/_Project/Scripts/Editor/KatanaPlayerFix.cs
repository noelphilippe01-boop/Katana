using Katana.CameraSystems;
using Katana.Characters;
using Katana.Combat;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaPlayerFix
    {
        [MenuItem("Katana/Fix Player Setup")]
        public static void FixPlayerSetup()
        {
            CleanupManagers();
            EnsurePlayerComponents();
            KatanaCameraTools.FixCameraInScene();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Joueur et camera mis a jour. Ctrl+S puis Play.");
        }

        [MenuItem("Katana/Fix Player Movement", false, 1)]
        public static void FixPlayerMovement() => FixPlayerSetup();

        static void EnsurePlayerComponents()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Katana: Player introuvable.");
                return;
            }

            RemoveLegacyPhysics(player);

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

            CameraFollowTarget.EnsureOn(player.transform);
        }

        static void RemoveLegacyPhysics(GameObject player)
        {
            foreach (var agent in player.GetComponents<NavMeshAgent>())
                Object.DestroyImmediate(agent);

            foreach (var capsule in player.GetComponents<CapsuleCollider>())
                Object.DestroyImmediate(capsule);

            var meshCollider = player.GetComponent<MeshCollider>();
            if (meshCollider != null)
                Object.DestroyImmediate(meshCollider);

            foreach (var cc in player.GetComponents<CharacterController>())
                Object.DestroyImmediate(cc);
        }

        static void CleanupManagers()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
                return;

            foreach (var playerCtrl in managers.GetComponents<PlayerController>())
                Object.DestroyImmediate(playerCtrl);

            foreach (var controller in managers.GetComponents<CharacterController>())
                Object.DestroyImmediate(controller);

            foreach (var agent in managers.GetComponents<NavMeshAgent>())
                Object.DestroyImmediate(agent);
        }
    }
}
