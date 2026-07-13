using UnityEngine;

namespace Katana.Combat
{
    public static class EnemyFactory
    {
        static Material enemyMaterial;

        public static GameObject Create(Vector3 position, string enemyName = null)
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

            enemy.AddComponent<EnemyHealth>();
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
