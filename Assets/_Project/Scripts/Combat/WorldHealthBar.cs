using UnityEngine;

namespace Katana.Combat
{
    public class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] float heightOffset = 1.45f;
        [SerializeField] Vector3 barSize = new(1.2f, 0.08f, 0.06f);
        [SerializeField] Vector3 fillSize = new(1.15f, 0.06f, 0.05f);

        EnemyHealth health;
        Transform barRoot;
        Transform fillTransform;
        Camera cam;

        void Awake()
        {
            health = GetComponent<EnemyHealth>();
            cam = Camera.main;

            barRoot = new GameObject("HealthBar").transform;
            barRoot.SetParent(transform);

            var background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.name = "HealthBarBackground";
            background.transform.SetParent(barRoot);
            background.transform.localScale = barSize;
            CombatVisuals.RemoveCollider(background);
            CombatVisuals.ApplyColor(background.GetComponent<Renderer>(), new Color(0.35f, 0.08f, 0.08f));

            var fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fill.name = "HealthBarFill";
            fill.transform.SetParent(barRoot);
            fill.transform.localScale = fillSize;
            CombatVisuals.RemoveCollider(fill);
            CombatVisuals.ApplyColor(fill.GetComponent<Renderer>(), new Color(0.2f, 0.85f, 0.25f));
            fillTransform = fill.transform;
        }

        void LateUpdate()
        {
            if (health == null || barRoot == null)
                return;

            if (!health.IsAlive)
            {
                barRoot.gameObject.SetActive(false);
                return;
            }

            barRoot.gameObject.SetActive(true);
            barRoot.position = transform.position + Vector3.up * heightOffset;

            if (cam == null)
                cam = Camera.main;

            if (cam != null)
                barRoot.rotation = Quaternion.LookRotation(barRoot.position - cam.transform.position);

            var ratio = Mathf.Clamp01(health.CurrentHealth / health.MaxHealth);
            fillTransform.localScale = new Vector3(fillSize.x * ratio, fillSize.y, fillSize.z);
            fillTransform.localPosition = new Vector3(-fillSize.x * 0.5f * (1f - ratio), 0f, -0.01f);
        }
    }
}
