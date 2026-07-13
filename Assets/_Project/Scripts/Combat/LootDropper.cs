using UnityEngine;

namespace Katana.Combat
{
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] string itemId = "gold";
        [SerializeField] int minQuantity = 1;
        [SerializeField] int maxQuantity = 4;

        public void DropLoot()
        {
            var quantity = Random.Range(minQuantity, maxQuantity + 1);
            LootPickup.Spawn(transform.position, itemId, quantity);
        }
    }
}
