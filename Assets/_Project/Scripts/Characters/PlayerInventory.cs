using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    public class PlayerInventory : MonoBehaviour
    {
        int gold;

        public int Gold => gold;

        void OnEnable() => GameEventBus.ItemPickedUp += OnItemPickedUp;
        void OnDisable() => GameEventBus.ItemPickedUp -= OnItemPickedUp;

        void OnItemPickedUp(ItemPickupEvent pickup)
        {
            if (pickup.ItemId != "gold")
                return;

            gold += pickup.Quantity;
        }
    }
}
