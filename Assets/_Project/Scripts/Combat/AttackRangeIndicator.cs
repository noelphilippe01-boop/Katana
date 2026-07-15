using Katana.Characters;
using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    [RequireComponent(typeof(PlayerCombat))]
    public class AttackRangeIndicator : MonoBehaviour
    {
        [SerializeField] int segments = 56;
        [SerializeField] float lineWidth = 0.07f;
        [SerializeField] Color inRangeColor = new(0.2f, 0.95f, 0.35f, 0.85f);
        [SerializeField] Color outOfRangeColor = new(0.95f, 0.85f, 0.2f, 0.75f);
        [SerializeField] Color effectRadiusColor = new(0.55f, 0.35f, 0.95f, 0.75f);
        [SerializeField] Color cleaveRadiusColor = new(0.95f, 0.45f, 0.2f, 0.75f);

        PlayerCombat combat;
        LineRenderer ring;
        LineRenderer effectRing;
        LineRenderer aimLine;
        Transform visualRoot;

        void Awake()
        {
            combat = GetComponent<PlayerCombat>();

            visualRoot = new GameObject("AttackRangeVisual").transform;
            visualRoot.SetParent(transform);

            ring = CreateLine("RangeRing", lineWidth);
            effectRing = CreateLine("EffectRing", lineWidth * 0.9f);
            aimLine = CreateLine("AimLine", lineWidth * 0.85f);
            aimLine.loop = false;
            aimLine.positionCount = 2;
        }

        void LateUpdate()
        {
            if (combat == null || ring == null)
                return;

            var target = combat.SelectedTarget;
            if (target == null)
            {
                ring.enabled = false;
                effectRing.enabled = false;
                aimLine.enabled = false;
                return;
            }

            var playerFlat = new Vector3(transform.position.x, 0.08f, transform.position.z);
            visualRoot.position = playerFlat;

            var range = combat.AttackRange;
            var inRange = combat.IsTargetInRange();
            var rangeColor = inRange ? inRangeColor : outOfRangeColor;
            UpdateRing(ring, playerFlat, range, rangeColor, segments);

            ring.enabled = true;
            aimLine.enabled = true;
            aimLine.startColor = rangeColor;
            aimLine.endColor = rangeColor;

            var targetFlat = new Vector3(target.transform.position.x, 0.08f, target.transform.position.z);
            aimLine.SetPosition(0, playerFlat);
            aimLine.SetPosition(1, targetFlat);

            UpdateEffectRing(targetFlat, inRange);
        }

        void UpdateEffectRing(Vector3 targetFlat, bool inRange)
        {
            var style = combat.AttackStyle;
            var effectRadius = combat.EffectRadius;

            if (effectRadius <= 0f || style == WeaponAttackStyle.RangedSingle)
            {
                effectRing.enabled = false;
                return;
            }

            if (!inRange)
            {
                effectRing.enabled = false;
                return;
            }

            var center = style == WeaponAttackStyle.RangedArea ? targetFlat : new Vector3(transform.position.x, 0.08f, transform.position.z);
            var color = style == WeaponAttackStyle.RangedArea ? effectRadiusColor : cleaveRadiusColor;
            UpdateRing(effectRing, center, effectRadius, color, segments);
            effectRing.enabled = true;
        }

        LineRenderer CreateLine(string name, float width)
        {
            var lineObject = new GameObject(name);
            lineObject.transform.SetParent(visualRoot);

            var line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.loop = true;
            line.widthMultiplier = width;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.enabled = false;
            return line;
        }

        static void UpdateRing(LineRenderer line, Vector3 center, float radius, Color color, int segmentCount)
        {
            line.startColor = color;
            line.endColor = color;
            line.positionCount = segmentCount;

            for (var i = 0; i < segmentCount; i++)
            {
                var angle = i / (float)segmentCount * Mathf.PI * 2f;
                var point = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                line.SetPosition(i, center + point);
            }
        }
    }
}
