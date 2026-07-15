using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(WeaponLoadout))]
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] float baseDefense = 5f;
        [SerializeField] float baseMaxHealth = 100f;
        [SerializeField] float baseMoveSpeed = 6f;

        WeaponLoadout loadout;

        public float BaseDefense => baseDefense;
        public float BaseMaxHealth => baseMaxHealth;
        public float BaseMoveSpeed => baseMoveSpeed;

        public WeaponAttackStyle AttackStyle => loadout.Current.AttackStyle;
        public float AttackDamage => loadout.Current.AttackDamage;
        public float CriticalChance => loadout.Current.CriticalChance;
        public float CriticalMultiplier => loadout.Current.CriticalMultiplier;
        public float CriticalHitDamage => loadout.Current.CriticalHitDamage;
        public float AttackRange => loadout.Current.AttackRange;
        public float EffectRadius => loadout.Current.EffectRadius;
        public float AttacksPerSecond => loadout.Current.AttacksPerSecond;
        public float Defense => baseDefense + loadout.Current.DefenseBonus;
        public float MoveSpeed => baseMoveSpeed + loadout.Current.MoveSpeedBonus;
        public float LifeStealPercent => loadout.Current.LifeStealPercent;
        public DamageType DamageType => loadout.Current.DamageType;
        public WeaponProfile CurrentWeapon => loadout.Current;
        public WeaponLoadout Loadout => loadout;

        void Awake() => loadout = GetComponent<WeaponLoadout>();

        public float MitigateIncomingDamage(float rawDamage) =>
            Mathf.Max(1f, rawDamage * (100f / (100f + Defense)));

        public float ApplyLifeSteal(float damageDealt)
        {
            if (LifeStealPercent <= 0f)
                return 0f;

            return damageDealt * (LifeStealPercent / 100f);
        }
    }
}
