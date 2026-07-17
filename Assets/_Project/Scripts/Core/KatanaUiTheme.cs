using UnityEngine;

namespace Katana.Core
{
    /// <summary>
    /// Palette et tokens visuels partagés par toutes les interfaces Katana.
    /// Direction : fantasy sombre, accents froides (acier/bleu) + or pour la monnaie.
    /// </summary>
    public static class KatanaUiTheme
    {
        // Fonds
        public static readonly Color MenuBackdrop = new(0.03f, 0.05f, 0.09f, 0.94f);
        public static readonly Color PauseBackdrop = new(0.02f, 0.04f, 0.08f, 0.78f);
        public static readonly Color GameplayPauseDim = new(0.02f, 0.04f, 0.08f, 0.42f);
        public static readonly Color MenuPaneClear = new(0f, 0f, 0f, 0f);
        public static readonly Color PanelFill = new(0.06f, 0.09f, 0.14f, 0.97f);
        public static readonly Color PanelFillRaised = new(0.08f, 0.12f, 0.18f, 0.98f);
        public static readonly Color PanelFillInset = new(0.04f, 0.06f, 0.1f, 0.98f);

        // Bordures & lignes
        public static readonly Color WindowBorder = new(0.22f, 0.34f, 0.5f, 0.95f);
        public static readonly Color WindowBorderInner = new(0.1f, 0.15f, 0.22f, 0.9f);
        public static readonly Color PanelDivider = new(0.16f, 0.24f, 0.34f, 0.75f);
        public static readonly Color TopShine = new(0.45f, 0.58f, 0.72f, 0.22f);

        // Accents
        public static readonly Color AccentPrimary = new(0.38f, 0.64f, 0.96f, 1f);
        public static readonly Color AccentSecondary = new(0.28f, 0.48f, 0.72f, 1f);
        public static readonly Color AccentColor = AccentPrimary;

        // Boutons
        public static readonly Color ButtonNormal = new(0.14f, 0.22f, 0.36f, 1f);
        public static readonly Color ButtonHighlight = new(0.2f, 0.32f, 0.5f, 1f);
        public static readonly Color ButtonPressed = new(0.1f, 0.16f, 0.28f, 1f);
        public static readonly Color ButtonColor = ButtonNormal;
        public static readonly Color ButtonGhost = new(0.1f, 0.14f, 0.22f, 0.85f);

        // Texte
        public static readonly Color TextPrimary = new(0.94f, 0.96f, 1f, 1f);
        public static readonly Color TextSecondary = new(0.72f, 0.78f, 0.86f, 0.92f);
        public static readonly Color TextMuted = new(0.52f, 0.58f, 0.66f, 0.85f);
        public static readonly Color HudMutedText = TextSecondary;

        // HUD combat
        public static readonly Color HudPanelColor = new(0.02f, 0.04f, 0.08f, 0.55f);
        public static readonly Color HudFrameColor = PanelFillRaised;
        public static readonly Color HudBorderColor = WindowBorder;
        public static readonly Color HudHealthFill = new(0.24f, 0.78f, 0.38f, 1f);
        public static readonly Color HudPlayerHealthFill = new(0.86f, 0.12f, 0.12f, 1f);
        public static readonly Color HudHealthLow = new(0.88f, 0.24f, 0.2f, 1f);
        public static readonly Color HudHealthBackground = new(0.06f, 0.06f, 0.08f, 0.95f);
        public static readonly Color HudGold = new(1f, 0.82f, 0.28f, 1f);
        public static readonly Color TargetInRange = new(0.34f, 0.82f, 0.46f, 1f);
        public static readonly Color TargetOutOfRange = new(0.95f, 0.55f, 0.24f, 1f);
        public static readonly Color WeaponSlotActive = new(0.14f, 0.24f, 0.4f, 0.98f);
        public static readonly Color WeaponSlotIdle = new(0.07f, 0.1f, 0.15f, 0.9f);

        // Équipement & inventaire
        public static readonly Color EquipmentSlotEmpty = new(0.07f, 0.1f, 0.15f, 0.94f);
        public static readonly Color EquipmentSlotFilled = new(0.11f, 0.17f, 0.28f, 0.98f);
        public static readonly Color EquipmentGlyphEmpty = new(0.28f, 0.34f, 0.42f, 0.45f);
        public static readonly Color InventoryGridFrame = new(0.2f, 0.3f, 0.44f, 0.96f);
        public static readonly Color InventoryGridInner = new(0.1f, 0.14f, 0.21f, 1f);
        public static readonly Color InventoryCellFill = new(0.13f, 0.17f, 0.24f, 1f);

        // Compatibilité anciens noms
        public static readonly Color PanelColor = PanelFill;

        public static Color HealthColorForRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return ratio <= 0.25f
                ? HudHealthLow
                : Color.Lerp(HudHealthLow, HudHealthFill, (ratio - 0.25f) / 0.75f);
        }

        public static Color WeaponAccent(string weaponId) => weaponId switch
        {
            "katana" => new Color(0.42f, 0.68f, 0.98f, 1f),
            "arc" => new Color(0.48f, 0.86f, 0.52f, 1f),
            "baton_mage" => new Color(0.72f, 0.48f, 0.98f, 1f),
            _ => AccentPrimary
        };

        public static void ApplyButtonColors(UnityEngine.UI.Button button, Color normal, Color highlight, Color pressed)
        {
            if (button == null)
                return;

            var colors = button.colors;
            colors.normalColor = normal;
            colors.highlightedColor = highlight;
            colors.pressedColor = pressed;
            colors.selectedColor = highlight;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }
    }
}
