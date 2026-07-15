using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public static class CombatDamageCalculator
    {
        public static DamageInfo RollAttack(
            GameObject source,
            GameObject target,
            float baseDamage,
            float criticalChance,
            float criticalMultiplier,
            DamageType damageType)
        {
            var isCritical = Random.value <= criticalChance;
            var amount = isCritical ? baseDamage * criticalMultiplier : baseDamage;

            return new DamageInfo
            {
                Source = source,
                Target = target,
                Amount = amount,
                IsCritical = isCritical,
                DamageType = damageType
            };
        }
    }
}
