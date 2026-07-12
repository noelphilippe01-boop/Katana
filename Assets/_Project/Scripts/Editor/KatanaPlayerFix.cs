using Katana.Characters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaPlayerFix
    {
        [MenuItem("Katana/Fix Player Movement")]
        public static void FixPlayerMovement()
        {
            CleanupManagers();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Katana: Player introuvable.");
                return;
            }

            foreach (var agent in player.GetComponents<NavMeshAgent>())
                Object.DestroyImmediate(agent);

            foreach (var capsule in player.GetComponents<CapsuleCollider>())
                Object.DestroyImmediate(capsule);

            var meshCollider = player.GetComponent<MeshCollider>();
            if (meshCollider != null)
                Object.DestroyImmediate(meshCollider);

            foreach (var cc in player.GetComponents<CharacterController>())
                Object.DestroyImmediate(cc);

            foreach (var old in player.GetComponents<PlayerInputHandler>())
                Object.DestroyImmediate(old);

            foreach (var old in player.GetComponents<PlayerMovementController>())
                Object.DestroyImmediate(old);

            if (player.GetComponent<PlayerController>() == null)
                player.AddComponent<PlayerController>();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: PlayerController simple (Input classique). Reglez Input Handling sur BOTH. Ctrl+S puis Play.");
        }

        static void CleanupManagers()
        {
            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers == null)
                return;

            foreach (var movement in managers.GetComponents<PlayerMovementController>())
                Object.DestroyImmediate(movement);

            foreach (var input in managers.GetComponents<PlayerInputHandler>())
                Object.DestroyImmediate(input);

            foreach (var playerCtrl in managers.GetComponents<PlayerController>())
                Object.DestroyImmediate(playerCtrl);

            foreach (var controller in managers.GetComponents<CharacterController>())
                Object.DestroyImmediate(controller);

            foreach (var agent in managers.GetComponents<NavMeshAgent>())
                Object.DestroyImmediate(agent);
        }
    }
}
