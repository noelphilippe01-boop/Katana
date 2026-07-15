using Katana.Characters;
using Katana.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Katana.Combat
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float aggroRange = 8f;
        [SerializeField] float deAggroRange = 100f;
        [SerializeField] float attackRange = 1.6f;
        [SerializeField] float attackDamage = 6f;
        [SerializeField] float attacksPerSecond = 0.9f;
        [SerializeField] float moveSpeed = 3.5f;

        NavMeshAgent agent;
        CharacterFacing facing;
        EnemyHealth health;
        Transform player;
        float attackCooldown;
        bool isAggroed;

        public float AggroRange => aggroRange;
        public float DeAggroRange => deAggroRange;
        public bool IsAggroed => isAggroed;

        public void Configure(float aggroRangeOverride)
        {
            if (aggroRangeOverride > 0f)
                aggroRange = aggroRangeOverride;
        }

        public void ForceAggro() => isAggroed = true;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
            facing = GetComponent<CharacterFacing>();
            if (facing == null)
                facing = gameObject.AddComponent<CharacterFacing>();

            agent.speed = moveSpeed;
            agent.stoppingDistance = attackRange * 0.85f;
            agent.angularSpeed = 720f;
            agent.acceleration = 16f;
        }

        void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        void Update()
        {
            if (player == null || health == null || !health.IsAlive)
                return;

            if (SpawnSafeZone.IsPlayerInside())
            {
                StopPursuit();
                return;
            }

            var distance = FlatDistance(transform.position, player.position);
            UpdateAggroState(distance);

            if (!isAggroed)
            {
                StopPursuit();
                return;
            }

            agent.isStopped = false;

            var toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.01f)
                facing.FaceDirection(toPlayer);

            if (distance <= attackRange)
            {
                if (agent.hasPath)
                    agent.ResetPath();
                TryAttack();
                return;
            }

            agent.SetDestination(player.position);
        }

        void UpdateAggroState(float distanceToPlayer)
        {
            if (isAggroed)
            {
                if (distanceToPlayer > deAggroRange)
                    isAggroed = false;
                return;
            }

            if (distanceToPlayer <= aggroRange)
                isAggroed = true;
        }

        void TryAttack()
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown > 0f)
                return;

            attackCooldown = 1f / attacksPerSecond;

            if (SpawnSafeZone.IsPlayerInside())
                return;

            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null || !playerHealth.IsAlive)
                return;

            var damage = CombatDamageCalculator.RollAttack(
                gameObject,
                player.gameObject,
                attackDamage,
                0f,
                1f,
                DamageType.Physical);

            playerHealth.ApplyDamage(damage);
        }

        void StopPursuit()
        {
            if (agent.hasPath)
                agent.ResetPath();

            agent.isStopped = true;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = isAggroed
                ? new Color(1f, 0.2f, 0.2f, 0.85f)
                : new Color(1f, 0.35f, 0.2f, 0.85f);
            Gizmos.DrawWireSphere(transform.position, isAggroed ? deAggroRange : aggroRange);
        }

        static float FlatDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }
    }
}
