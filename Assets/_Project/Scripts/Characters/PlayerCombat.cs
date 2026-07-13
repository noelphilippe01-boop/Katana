using Katana.Combat;
using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CharacterFacing))]
    public class PlayerCombat : MonoBehaviour, ICombatant
    {
        [SerializeField] float attackRange = 1.8f;
        [SerializeField] float attackDamage = 10f;
        [SerializeField] float attacksPerSecond = 1.5f;

        GameObject selectedTarget;
        float attackCooldown;
        CharacterFacing facing;

        public GameObject SelectedTarget => selectedTarget;

        void Awake() => facing = GetComponent<CharacterFacing>();

        void OnEnable() => GameEventBus.TargetSelected += OnTargetSelected;
        void OnDisable() => GameEventBus.TargetSelected -= OnTargetSelected;

        void OnTargetSelected(GameObject target) => selectedTarget = target;

        void Update()
        {
            if (selectedTarget == null)
                return;

            var health = selectedTarget.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
            {
                selectedTarget = null;
                return;
            }

            var toTarget = selectedTarget.transform.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.01f)
                facing.FaceDirection(toTarget);

            if (FlatDistance(transform.position, selectedTarget.transform.position) > attackRange)
                return;

            attackCooldown -= Time.deltaTime;
            if (attackCooldown > 0f)
                return;

            attackCooldown = 1f / attacksPerSecond;
            health.ApplyDamage(new DamageInfo
            {
                Source = gameObject,
                Target = selectedTarget,
                Amount = attackDamage,
                IsCritical = false
            });
        }

        public void RequestAttack()
        {
            if (selectedTarget == null)
                return;

            var health = selectedTarget.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
                return;

            if (FlatDistance(transform.position, selectedTarget.transform.position) > attackRange)
                return;

            health.ApplyDamage(new DamageInfo
            {
                Source = gameObject,
                Target = selectedTarget,
                Amount = attackDamage,
                IsCritical = false
            });
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
