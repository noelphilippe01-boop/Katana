using Katana.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Combat
{
    public class FloatingDamageText : MonoBehaviour
    {
        [SerializeField] float lifetime = 0.85f;
        [SerializeField] float riseSpeed = 1.4f;

        Text labelText;
        Color color;
        float spawnTime;
        Camera cam;

        public static void Spawn(Vector3 worldPosition, DamageInfo damage)
        {
            var floater = new GameObject("FloatingDamage");
            floater.transform.position = worldPosition + Vector3.up * 1.3f;

            var text = floater.AddComponent<FloatingDamageText>();
            text.color = ColorFor(damage);
            text.spawnTime = Time.time;
            text.Build(
                damage.IsCritical ? $"{damage.Amount:0}!" : $"{damage.Amount:0}",
                damage.IsCritical ? 20 : 15);
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

        void Build(string label, int fontSize)
        {
            cam = Camera.main;

            var canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform);
            canvasGo.transform.localPosition = Vector3.zero;

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rect = canvasGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120f, 40f);
            rect.localScale = Vector3.one * 0.012f;

            labelText = KatanaUiFactory.CreateText(canvasGo.transform, "Value", label, fontSize, TextAnchor.MiddleCenter, FontStyle.Bold);
            labelText.color = color;
        }

        void Update()
        {
            transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

            if (cam == null)
                cam = Camera.main;

            if (cam != null)
                transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

            if (Time.time - spawnTime >= lifetime)
                Destroy(gameObject);
        }
    }
}
