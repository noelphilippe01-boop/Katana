using Katana.Combat;
using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CharacterFacing))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerCombat : MonoBehaviour, ICombatant
    {
        GameObject combatTarget;
        float attackCooldown;
        CharacterFacing facing;
        PlayerController movement;
        PlayerStats stats;
        PlayerHealth playerHealth;

        public GameObject SelectedTarget => combatTarget;
        public bool IsCombatEngaged => combatTarget != null;
        public float AttackRange => stats != null ? stats.AttackRange : 1.8f;
        public float EffectRadius => stats != null ? stats.EffectRadius : 0f;
        public WeaponAttackStyle AttackStyle => stats != null ? stats.AttackStyle : WeaponAttackStyle.MeleeCleave;

        void Awake()
        {
            facing = GetComponent<CharacterFacing>();
            movement = GetComponent<PlayerController>();
            stats = GetComponent<PlayerStats>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        void OnEnable()
        {
            GameEventBus.TargetSelected += OnTargetSelected;
            GameEventBus.EnemyKilled += OnEnemyKilled;
        }

        void OnDisable()
        {
            GameEventBus.TargetSelected -= OnTargetSelected;
            GameEventBus.EnemyKilled -= OnEnemyKilled;
        }

        void OnTargetSelected(GameObject target)
        {
            if (target != null && target.CompareTag("Enemy"))
                EngageTarget(target);
        }

        void OnEnemyKilled(GameObject enemy)
        {
            if (combatTarget == enemy)
                combatTarget = null;
        }

        public void EngageTarget(GameObject target)
        {
            if (target == null || !target.CompareTag("Enemy"))
                return;

            combatTarget = target;
        }

        public void ClearCombatTarget() => combatTarget = null;

        public bool IsTargetInRange()
        {
            if (combatTarget == null)
                return false;

            return FlatDistance(transform.position, combatTarget.transform.position) <= AttackRange;
        }

        void Update()
        {
            if (combatTarget == null)
                return;

            var health = combatTarget.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
                combatTarget = null;
        }

        void LateUpdate()
        {
            if (combatTarget == null || stats == null)
                return;

            var health = combatTarget.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
                return;

            if (movement != null && movement.IsMoving)
                return;

            if (!IsTargetInRange())
                return;

            var toTarget = combatTarget.transform.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.01f)
                facing.FaceDirection(toTarget);

            attackCooldown -= Time.deltaTime;
            if (attackCooldown > 0f)
                return;

            attackCooldown = 1f / stats.AttacksPerSecond;
            PerformAttack(health);
        }

        void PerformAttack(EnemyHealth primaryTarget)
        {
            switch (stats.AttackStyle)
            {
                case WeaponAttackStyle.MeleeCleave:
                    PerformCleaveAttack();
                    break;
                case WeaponAttackStyle.RangedSingle:
                    ApplyDamageToEnemy(primaryTarget);
                    break;
                case WeaponAttackStyle.RangedArea:
                    PerformAreaAttack(primaryTarget.transform.position);
                    break;
            }
        }

        void PerformCleaveAttack()
        {
            var cleaveRadius = Mathf.Max(stats.EffectRadius, stats.AttackRange);
            var playerPos = transform.position;

            CombatTargetQuery.ForEachAliveEnemy(health =>
            {
                if (!CombatTargetQuery.IsWithinFlatRadius(playerPos, health.transform.position, cleaveRadius))
                    return;

                ApplyDamageToEnemy(health);
            });
        }

        void PerformAreaAttack(Vector3 center)
        {
            var radius = stats.EffectRadius;
            if (radius <= 0f)
            {
                var primary = combatTarget?.GetComponent<EnemyHealth>();
                if (primary != null && primary.IsAlive)
                    ApplyDamageToEnemy(primary);
                return;
            }

            CombatTargetQuery.ForEachAliveEnemy(health =>
            {
                if (!CombatTargetQuery.IsWithinFlatRadius(center, health.transform.position, radius))
                    return;

                ApplyDamageToEnemy(health);
            });
        }

        void ApplyDamageToEnemy(EnemyHealth health)
        {
            var damage = CombatDamageCalculator.RollAttack(
                gameObject,
                health.gameObject,
                stats.AttackDamage,
                stats.CriticalChance,
                stats.CriticalMultiplier,
                stats.DamageType);

            health.ApplyDamage(damage);

            var heal = stats.ApplyLifeSteal(damage.Amount);
            if (heal > 0f)
                playerHealth?.Heal(heal);
        }

        public void RequestAttack()
        {
            if (combatTarget == null || stats == null)
                return;

            var health = combatTarget.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive || !IsTargetInRange())
                return;

            if (movement != null && movement.IsMoving)
                return;

            PerformAttack(health);
        }

        public void RequestAbility(int slotIndex) { }

        static float FlatDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }
    }
}
