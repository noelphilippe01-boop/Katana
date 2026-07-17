using Katana.CameraSystems;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Editor
{
    public static class KatanaCameraTools
    {
        [MenuItem("Katana/Fix Camera (current scene)")]
        public static void FixCameraInScene()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Katana: Player introuvable.");
                return;
            }

            var cmCamera = Object.FindAnyObjectByType<CinemachineCamera>();
            if (cmCamera == null)
            {
                Debug.LogWarning("Katana: CinemachineCamera introuvable.");
                return;
            }

            var cameraTarget = CameraFollowTarget.EnsureOn(player.transform);

            var hardLookAt = cmCamera.GetComponent<CinemachineHardLookAt>();
            if (hardLookAt != null)
                Object.DestroyImmediate(hardLookAt);

            cmCamera.Follow = cameraTarget;
            cmCamera.LookAt = null;
            cmCamera.Target.TrackingTarget = cameraTarget;
            cmCamera.Target.LookAtTarget = null;

            var follow = cmCamera.GetComponent<CinemachineFollow>();
            if (follow == null)
                follow = cmCamera.gameObject.AddComponent<CinemachineFollow>();

            follow.FollowOffset = new Vector3(10f, 15f, -10f).normalized * 40f;
            follow.TrackerSettings.BindingMode = BindingMode.WorldSpace;
            follow.TrackerSettings.PositionDamping = Vector3.zero;

            cmCamera.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            var managers = GameObject.Find("--- MANAGERS ---");
            if (managers != null)
            {
                var controller = managers.GetComponent<IsometricCameraController>();
                if (controller == null)
                    controller = managers.AddComponent<IsometricCameraController>();

                var so = new SerializedObject(controller);
                so.FindProperty("cinemachineCamera").objectReferenceValue = cmCamera;
                so.FindProperty("target").objectReferenceValue = cameraTarget;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Katana: Camera stable, centree sur le joueur, zoom molette.");
        }
    }
}
