using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class FloatingDamageText : MonoBehaviour
    {
        [SerializeField] float lifetime = 0.85f;
        [SerializeField] float riseSpeed = 1.4f;

        string label;
        Color color;
        int fontSize;
        float spawnTime;

        public static void Spawn(Vector3 worldPosition, DamageInfo damage)
        {
            var floater = new GameObject("FloatingDamage");
            floater.transform.position = worldPosition + Vector3.up * 1.3f;

            var text = floater.AddComponent<FloatingDamageText>();
            text.label = damage.IsCritical ? $"{damage.Amount:0}!" : $"{damage.Amount:0}";
            text.fontSize = damage.IsCritical ? 20 : 15;
            text.color = ColorFor(damage);
            text.spawnTime = Time.time;
        }

        static Color ColorFor(DamageInfo damage)
        {
            if (damage.IsCritical)
                return new Color(1f, 0.92f, 0.2f);

            return damage.DamageType switch
            {
                DamageType.Magic => new Color(0.75f, 0.45f, 1f),
                DamageType.Fire => new Color(1f, 0.45f, 0.15f),
                DamageType.Cold => new Color(0.45f, 0.75f, 1f),
                _ => Color.white
            };
        }

        void Update()
        {
            transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
            if (Time.time - spawnTime >= lifetime)
                Destroy(gameObject);
        }

        void OnGUI()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            var screen = camera.WorldToScreenPoint(transform.position);
            if (screen.z < 0f)
                return;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color }
            };

            var rect = new Rect(screen.x - 40f, Screen.height - screen.y - 12f, 80f, 24f);
            GUI.Label(rect, label, style);
        }
    }
}
