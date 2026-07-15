using System;
using UnityEngine;

namespace Katana.Core
{
    public static class GameEventBus
    {
        public static event Action<Vector3> PlayerMoveRequested;
        public static event Action<GameObject> TargetSelected;
        public static event Action<DamageInfo> DamageDealt;
        public static event Action<GameObject> EnemyKilled;
        public static event Action<ItemPickupEvent> ItemPickedUp;
        public static event Action<WeaponProfile> WeaponChanged;

        public static void RaisePlayerMoveRequested(Vector3 destination) =>
            PlayerMoveRequested?.Invoke(destination);

        public static void RaiseTargetSelected(GameObject target) =>
            TargetSelected?.Invoke(target);

        public static void RaiseDamageDealt(DamageInfo info) =>
            DamageDealt?.Invoke(info);

        public static void RaiseEnemyKilled(GameObject enemy) =>
            EnemyKilled?.Invoke(enemy);

        public static void RaiseItemPickedUp(ItemPickupEvent pickup) =>
            ItemPickedUp?.Invoke(pickup);

        public static void RaiseWeaponChanged(WeaponProfile weapon) =>
            WeaponChanged?.Invoke(weapon);
    }

    public struct DamageInfo
    {
        public GameObject Source;
        public GameObject Target;
        public float Amount;
        public bool IsCritical;
        public DamageType DamageType;
    }

    public struct ItemPickupEvent
    {
        public string ItemId;
        public int Quantity;
    }
}
