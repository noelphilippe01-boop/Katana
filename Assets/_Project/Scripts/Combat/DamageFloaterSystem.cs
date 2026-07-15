using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class DamageFloaterSystem : MonoBehaviour
    {
        void OnEnable() => GameEventBus.DamageDealt += OnDamageDealt;
        void OnDisable() => GameEventBus.DamageDealt -= OnDamageDealt;

        static void OnDamageDealt(DamageInfo damage)
        {
            if (damage.Target == null)
                return;

            FloatingDamageText.Spawn(damage.Target.transform.position, damage);
        }
    }
}
