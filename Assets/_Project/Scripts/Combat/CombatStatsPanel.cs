using Katana.Characters;
using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class CombatStatsPanel : MonoBehaviour
    {
        [SerializeField] float panelWidth = 280f;
        [SerializeField] float margin = 16f;

        PlayerStats stats;
        WeaponLoadout loadout;
        PlayerHealth health;

        void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            stats = player.GetComponent<PlayerStats>();
            loadout = player.GetComponent<WeaponLoadout>();
            health = player.GetComponent<PlayerHealth>();
        }

        void OnGUI()
        {
            if (stats == null || loadout == null)
                return;

            var x = Screen.width - panelWidth - margin;
            var y = margin;

            GUI.Box(new Rect(x, y, panelWidth, 420f), GUIContent.none);

            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            var activeStyle = new GUIStyle(labelStyle) { fontStyle = FontStyle.Bold };

            var lineY = y + 10f;
            GUI.Label(new Rect(x + 12f, lineY, panelWidth - 24f, 24f), "Statistiques", titleStyle);
            lineY += 28f;

            DrawWeaponsList(x, ref lineY, labelStyle, activeStyle);
            lineY += 8f;

            GUI.Label(new Rect(x + 12f, lineY, panelWidth - 24f, 22f), "— Combat —", labelStyle);
            lineY += 22f;

            DrawStatLine(x, ref lineY, "Degats", $"{stats.AttackDamage:0.#}", labelStyle);
            DrawStatLine(x, ref lineY, "Taux crit", $"{stats.CriticalChance * 100f:0.#} %", labelStyle);
            DrawStatLine(x, ref lineY, "Degats crit", $"{stats.CriticalHitDamage:0.#} (x{stats.CriticalMultiplier:0.#})", labelStyle);
            DrawStatLine(x, ref lineY, "Portee", $"{stats.AttackRange:0.#}", labelStyle);
            if (stats.EffectRadius > 0f)
                DrawStatLine(x, ref lineY, FormatEffectLabel(stats.AttackStyle), $"{stats.EffectRadius:0.#}", labelStyle);
            DrawStatLine(x, ref lineY, "Style", FormatAttackStyle(stats.AttackStyle), labelStyle);
            DrawStatLine(x, ref lineY, "Vitesse att.", $"{stats.AttacksPerSecond:0.#}/s", labelStyle);
            DrawStatLine(x, ref lineY, "Type", FormatDamageType(stats.DamageType), labelStyle);

            lineY += 6f;
            GUI.Label(new Rect(x + 12f, lineY, panelWidth - 24f, 22f), "— Defense —", labelStyle);
            lineY += 22f;

            DrawStatLine(x, ref lineY, "Defense", $"{stats.Defense:0.#}", labelStyle);
            if (health != null)
                DrawStatLine(x, ref lineY, "PV", $"{health.CurrentHealth:0}/{health.MaxHealth:0}", labelStyle);

            lineY += 6f;
            GUI.Label(new Rect(x + 12f, lineY, panelWidth - 24f, 22f), "— Bonus —", labelStyle);
            lineY += 22f;

            DrawStatLine(x, ref lineY, "Vitesse mv", $"{stats.MoveSpeed:0.#}", labelStyle);
            DrawStatLine(x, ref lineY, "Vol de vie", $"{stats.LifeStealPercent:0.#} %", labelStyle);
        }

        void DrawWeaponsList(float x, ref float lineY, GUIStyle normal, GUIStyle active)
        {
            for (var i = 0; i < loadout.WeaponCount; i++)
            {
                var key = loadout.GetSlotKeyLabel(i);
                var isActive = i == loadout.CurrentIndex;
                var weaponName = loadout.GetWeaponAt(i).DisplayName;
                var prefix = isActive ? "> " : "  ";
                var style = isActive ? active : normal;
                GUI.Label(new Rect(x + 12f, lineY, panelWidth - 24f, 20f), $"{prefix}[{key}] {weaponName}", style);
                lineY += 20f;
            }
        }

        static void DrawStatLine(float x, ref float y, string label, string value, GUIStyle style)
        {
            GUI.Label(new Rect(x + 12f, y, 130f, 20f), label, style);
            GUI.Label(new Rect(x + 142f, y, 120f, 20f), value, style);
            y += 20f;
        }

        static string FormatDamageType(DamageType type) => type switch
        {
            DamageType.Magic => "Magique",
            DamageType.Fire => "Feu",
            DamageType.Cold => "Froid",
            _ => "Physique"
        };

        static string FormatAttackStyle(WeaponAttackStyle style) => style switch
        {
            WeaponAttackStyle.MeleeCleave => "Melee (cleave)",
            WeaponAttackStyle.RangedSingle => "Distance (cible)",
            WeaponAttackStyle.RangedArea => "Distance (zone)",
            _ => style.ToString()
        };

        static string FormatEffectLabel(WeaponAttackStyle style) =>
            style == WeaponAttackStyle.RangedArea ? "Zone AoE" : "Cleave";
    }
}
