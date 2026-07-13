using UnityEditor;
using UnityEngine;

namespace Katana.Editor
{
    public static class KatanaMaterials
    {
        const string MaterialsFolder = "Assets/_Project/Art/Materials";
        const string GroundPath = MaterialsFolder + "/Mat_Ground.mat";
        const string PlayerPath = MaterialsFolder + "/Mat_Player.mat";
        const string EnemyPath = MaterialsFolder + "/Mat_Enemy.mat";

        public static Material GetOrCreateGroundMaterial()
        {
            EnsureFolder();
            var existing = AssetDatabase.LoadAssetAtPath<Material>(GroundPath);
            if (existing != null)
                return existing;

            var mat = CreateStandardMaterial(new Color(0.28f, 0.38f, 0.26f), 0.1f);
            AssetDatabase.CreateAsset(mat, GroundPath);
            return mat;
        }

        public static Material GetOrCreatePlayerMaterial()
        {
            EnsureFolder();
            var existing = AssetDatabase.LoadAssetAtPath<Material>(PlayerPath);
            if (existing != null)
                return existing;

            var mat = CreateStandardMaterial(new Color(0.15f, 0.45f, 0.95f), 0.6f);
            AssetDatabase.CreateAsset(mat, PlayerPath);
            return mat;
        }

        public static Material GetOrCreateEnemyMaterial()
        {
            EnsureFolder();
            var existing = AssetDatabase.LoadAssetAtPath<Material>(EnemyPath);
            if (existing != null)
                return existing;

            var mat = CreateStandardMaterial(new Color(0.85f, 0.2f, 0.18f), 0.45f);
            AssetDatabase.CreateAsset(mat, EnemyPath);
            return mat;
        }

        static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Art"))
                AssetDatabase.CreateFolder("Assets/_Project", "Art");
            if (!AssetDatabase.IsValidFolder(MaterialsFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Art", "Materials");
        }

        static Material CreateStandardMaterial(Color color, float glossiness)
        {
            var mat = new Material(Shader.Find("Standard"))
            {
                color = color
            };
            mat.SetFloat("_Glossiness", glossiness);
            return mat;
        }

        public static void ApplyToRenderer(GameObject go, Material material)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }
    }
}
