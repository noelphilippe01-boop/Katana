using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class LootPickup : MonoBehaviour
    {
        [SerializeField] float pickupRadius = 1.1f;

        string itemId;
        int quantity;
        Transform player;

        public static LootPickup Spawn(Vector3 position, string itemId, int quantity)
        {
            var loot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            loot.name = $"Loot_{itemId}";
            loot.transform.position = new Vector3(position.x, 0.35f, position.z);
            loot.transform.localScale = Vector3.one * 0.45f;
            CombatVisuals.ApplyColor(loot.GetComponent<Renderer>(), new Color(1f, 0.82f, 0.15f));

            var collider = loot.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.75f;

            var pickup = loot.AddComponent<LootPickup>();
            pickup.itemId = itemId;
            pickup.quantity = quantity;
            return pickup;
        }

        void Update()
        {
            if (player == null)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                    player = playerObject.transform;
                return;
            }

            var flatPlayer = new Vector3(player.position.x, 0f, player.position.z);
            var flatSelf = new Vector3(transform.position.x, 0f, transform.position.z);
            if (Vector3.Distance(flatPlayer, flatSelf) <= pickupRadius)
                Collect();
        }

        void Collect()
        {
            GameEventBus.RaiseItemPickedUp(new ItemPickupEvent
            {
                ItemId = itemId,
                Quantity = quantity
            });
            Destroy(gameObject);
        }
    }
}
