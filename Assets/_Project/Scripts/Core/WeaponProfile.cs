using System;
using UnityEngine;

namespace Katana.Core
{
    [Serializable]
    public struct WeaponProfile
    {
        public string Id;
        public string DisplayName;
        public WeaponAttackStyle AttackStyle;
        public float AttackDamage;
        [Range(0f, 1f)] public float CriticalChance;
        public float CriticalMultiplier;
        public float AttackRange;
        public float EffectRadius;
        public float AttacksPerSecond;
        public float DefenseBonus;
        public float MoveSpeedBonus;
        public float LifeStealPercent;
        public DamageType DamageType;

        public float CriticalHitDamage => AttackDamage * CriticalMultiplier;
        public float CriticalBonusPercent => (CriticalMultiplier - 1f) * 100f;

        public static WeaponProfile Katana => new()
        {
            Id = "katana",
            DisplayName = "Katana",
            AttackStyle = WeaponAttackStyle.MeleeCleave,
            AttackDamage = 10f,
            CriticalChance = 0.2f,
            CriticalMultiplier = 2f,
            AttackRange = 1.8f,
            EffectRadius = 2.4f,
            AttacksPerSecond = 1.5f,
            DefenseBonus = 3f,
            MoveSpeedBonus = 0f,
            LifeStealPercent = 0f,
            DamageType = DamageType.Physical
        };

        public static WeaponProfile Arc => new()
        {
            Id = "arc",
            DisplayName = "Arc",
            AttackStyle = WeaponAttackStyle.RangedSingle,
            AttackDamage = 9f,
            CriticalChance = 0.25f,
            CriticalMultiplier = 2f,
            AttackRange = 11f,
            EffectRadius = 0f,
            AttacksPerSecond = 1.25f,
            DefenseBonus = 0f,
            MoveSpeedBonus = 0.25f,
            LifeStealPercent = 0f,
            DamageType = DamageType.Physical
        };

        public static WeaponProfile BatonMage => new()
        {
            Id = "baton_mage",
            DisplayName = "Baton de mage",
            AttackStyle = WeaponAttackStyle.RangedArea,
            AttackDamage = 7f,
            CriticalChance = 0.15f,
            CriticalMultiplier = 1.8f,
            AttackRange = 9f,
            EffectRadius = 3.2f,
            AttacksPerSecond = 0.9f,
            DefenseBonus = 1f,
            MoveSpeedBonus = -0.25f,
            LifeStealPercent = 0f,
            DamageType = DamageType.Magic
        };
    }
}
