using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public static class CombatTargetQuery
    {
        public static void ForEachAliveEnemy(System.Action<EnemyHealth> action)
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                var health = enemy.GetComponent<EnemyHealth>();
                if (health == null || !health.IsAlive)
                    continue;

                action(health);
            }
        }

        public static bool IsWithinFlatRadius(Vector3 center, Vector3 target, float radius)
        {
            center.y = 0f;
            target.y = 0f;
            return Vector3.Distance(center, target) <= radius;
        }
    }
}
