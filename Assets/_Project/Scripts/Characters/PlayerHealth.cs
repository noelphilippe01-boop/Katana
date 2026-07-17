using Katana.Combat;
using Katana.Core;
using UnityEngine;

namespace Katana.Characters
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        float currentHealth;
        Vector3 respawnPosition;
        PlayerStats stats;

        public float MaxHealth => stats != null ? stats.BaseMaxHealth : 100f;
        public float CurrentHealth => currentHealth;
        public bool IsAlive => currentHealth > 0f;

        void Awake()
        {
            stats = GetComponent<PlayerStats>();
            currentHealth = MaxHealth;
            respawnPosition = transform.position;
        }

        void Start()
        {
            if (SpawnSafeZone.TryGet(out var zone))
                SetRespawnPoint(zone.PlayerSpawnPosition);
        }

        void Update()
        {
            RegenerateHealth();
        }

        void RegenerateHealth()
        {
            if (!IsAlive || stats == null || stats.HealthRegenerationPerSecond <= 0f)
                return;

            if (currentHealth >= MaxHealth)
                return;

            Heal(stats.HealthRegenerationPerSecond * Time.deltaTime);
        }

        public void SetRespawnPoint(Vector3 position) => respawnPosition = position;

        public void ApplyDamage(DamageInfo damage)
        {
            if (!IsAlive || stats == null)
                return;

            var mitigated = stats.MitigateIncomingDamage(damage.Amount);
            currentHealth = Mathf.Max(0f, currentHealth - mitigated);

            var info = damage;
            info.Amount = mitigated;
            info.Target = gameObject;
            GameEventBus.RaiseDamageDealt(info);

            if (!IsAlive)
                Respawn();
        }

        public void Heal(float amount)
        {
            if (!IsAlive)
                return;

            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        }

        void Respawn()
        {
            currentHealth = MaxHealth;
            transform.position = respawnPosition;
            Debug.Log("Katana: joueur tombe — respawn.");
        }
    }
}
