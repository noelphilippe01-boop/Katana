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

        public static GameObject FindNearestEnemyAtPoint(Vector3 worldPoint, float pickRadius)
        {
            worldPoint.y = 0f;
            var maxDistSq = pickRadius * pickRadius;
            GameObject nearest = null;
            var nearestDistSq = float.MaxValue;

            ForEachAliveEnemy(health =>
            {
                var delta = health.transform.position - worldPoint;
                delta.y = 0f;
                var distSq = delta.sqrMagnitude;
                if (distSq > maxDistSq || distSq >= nearestDistSq)
                    return;

                nearestDistSq = distSq;
                nearest = health.gameObject;
            });

            return nearest;
        }

        public static GameObject FindNearestEnemyInRange(Vector3 origin, float range) =>
            FindNearestEnemyAtPoint(origin, range);

        public static GameObject FindNearestEnemyNearScreenPoint(Camera camera, Vector2 screenPoint, float maxScreenDistance)
        {
            if (camera == null)
                return null;

            var maxDistSq = maxScreenDistance * maxScreenDistance;
            GameObject nearest = null;
            var nearestDistSq = float.MaxValue;

            ForEachAliveEnemy(health =>
            {
                var screenPos = camera.WorldToScreenPoint(health.transform.position + Vector3.up * 0.5f);
                if (screenPos.z <= 0f)
                    return;

                var delta = new Vector2(screenPos.x, screenPos.y) - screenPoint;
                var distSq = delta.sqrMagnitude;
                if (distSq > maxDistSq || distSq >= nearestDistSq)
                    return;

                nearestDistSq = distSq;
                nearest = health.gameObject;
            });

            return nearest;
        }
    }
}
