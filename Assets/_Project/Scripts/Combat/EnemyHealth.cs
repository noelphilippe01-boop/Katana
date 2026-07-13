using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealth = 30f;
        [SerializeField] float hitFlashDuration = 0.12f;

        float currentHealth;
        float hitFlashTimer;
        Color originalColor;
        Renderer targetRenderer;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsAlive => currentHealth > 0f;

        void Awake()
        {
            currentHealth = maxHealth;
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer != null)
                originalColor = targetRenderer.material.color;
        }

        void Update()
        {
            if (hitFlashTimer <= 0f || targetRenderer == null)
                return;

            hitFlashTimer -= Time.deltaTime;
            if (hitFlashTimer <= 0f)
                targetRenderer.material.color = originalColor;
        }

        public void ApplyDamage(DamageInfo damage)
        {
            if (!IsAlive)
                return;

            currentHealth = Mathf.Max(0f, currentHealth - damage.Amount);
            FlashHit();

            var info = damage;
            info.Target = gameObject;
            GameEventBus.RaiseDamageDealt(info);

            if (!IsAlive)
            {
                GetComponent<LootDropper>()?.DropLoot();
                GameEventBus.RaiseEnemyKilled(gameObject);
                Destroy(gameObject, 0.15f);
            }
        }

        void FlashHit()
        {
            if (targetRenderer == null)
                return;

            targetRenderer.material.color = Color.white;
            hitFlashTimer = hitFlashDuration;
        }
    }
}
