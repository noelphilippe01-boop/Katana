using Katana.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace Katana.Combat
{
    public static class EnemyFactory
    {
        static Material enemyMaterial;

        public static GameObject Create(Vector3 position, string enemyName = null, float aggroRange = 8f)
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = string.IsNullOrWhiteSpace(enemyName) ? "Enemy_Dummy" : enemyName;
            enemy.tag = "Enemy";
            enemy.transform.position = new Vector3(position.x, 1f, position.z);

            var renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (enemyMaterial == null)
                    enemyMaterial = CreateEnemyMaterial();
                renderer.material = enemyMaterial;
            }

            var agent = enemy.AddComponent<NavMeshAgent>();
            agent.height = 2f;
            agent.radius = 0.4f;
            agent.baseOffset = 0f;

            if (UnityEngine.AI.NavMesh.SamplePosition(enemy.transform.position, out var hit, 8f, UnityEngine.AI.NavMesh.AllAreas))
                agent.Warp(hit.position);

            enemy.AddComponent<CharacterFacing>();
            enemy.AddComponent<EnemyHealth>();
            var ai = enemy.AddComponent<EnemyAI>();
            ai.Configure(aggroRange);
            enemy.AddComponent<TargetHighlight>();
            enemy.AddComponent<WorldHealthBar>();
            enemy.AddComponent<LootDropper>();
            enemy.AddComponent<FacingMarker>();
            return enemy;
        }

        static Material CreateEnemyMaterial()
        {
            var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
            return shader != null
                ? new Material(shader) { color = new Color(0.85f, 0.2f, 0.18f) }
                : new Material(Shader.Find("Diffuse")) { color = new Color(0.85f, 0.2f, 0.18f) };
        }
    }
}
