using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class EnemyAggroSystem : MonoBehaviour
    {
        [SerializeField] float nearbyAggroRadius = 8f;

        void OnEnable() => GameEventBus.DamageDealt += OnDamageDealt;
        void OnDisable() => GameEventBus.DamageDealt -= OnDamageDealt;

        void OnDamageDealt(DamageInfo info)
        {
            if (info.Source == null || !info.Source.CompareTag("Player"))
                return;

            if (info.Target == null || !info.Target.CompareTag("Enemy"))
                return;

            PullAggroAt(info.Target.transform.position, info.Target);
        }

        void PullAggroAt(Vector3 center, GameObject primaryEnemy)
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                var ai = enemy.GetComponent<EnemyAI>();
                if (ai == null)
                    continue;

                if (enemy == primaryEnemy || IsWithinFlatRadius(center, enemy.transform.position, nearbyAggroRadius))
                    ai.ForceAggro();
            }
        }

        static bool IsWithinFlatRadius(Vector3 center, Vector3 target, float radius)
        {
            center.y = 0f;
            target.y = 0f;
            return Vector3.Distance(center, target) <= radius;
        }
    }
}
