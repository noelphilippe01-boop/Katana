using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    public class MoveDestinationMarker : MonoBehaviour
    {
        [SerializeField] float markerDiameter = 1.2f;
        [SerializeField] float markerHeight = 0.08f;
        [SerializeField] Color markerColor = new(1f, 0.85f, 0.2f, 0.9f);

        GameObject marker;
        Transform player;

        void Awake()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;

            marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "MoveMarker";
            marker.transform.SetParent(transform);
            marker.transform.localScale = new Vector3(markerDiameter, markerHeight, markerDiameter);
            Destroy(marker.GetComponent<Collider>());

            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    var mat = new Material(shader) { color = markerColor };
                    if (mat.HasProperty("_Glossiness"))
                        mat.SetFloat("_Glossiness", 0.8f);
                    renderer.material = mat;
                }
            }

            marker.SetActive(false);
            GameEventBus.PlayerMoveRequested += OnMoveRequested;
        }

        void OnDestroy() => GameEventBus.PlayerMoveRequested -= OnMoveRequested;

        void OnMoveRequested(Vector3 destination)
        {
            marker.transform.position = new Vector3(destination.x, markerHeight, destination.z);
            marker.SetActive(true);
        }

        void LateUpdate()
        {
            if (!marker.activeSelf || player == null)
                return;

            var playerFlat = new Vector3(player.position.x, 0f, player.position.z);
            var markerFlat = new Vector3(marker.transform.position.x, 0f, marker.transform.position.z);

            if (Vector3.Distance(playerFlat, markerFlat) <= 0.35f)
                marker.SetActive(false);
        }
    }
}
