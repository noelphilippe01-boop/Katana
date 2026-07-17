using Katana.Characters;
using Katana.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Combat
{
    public sealed class CombatHudView
    {
        struct WeaponSlotWidgets
        {
            public Image Background;
            public Image Accent;
            public Image IconImage;
            public Text IconLabel;
            public Text KeyLabel;
            public Text NameLabel;
        }

        readonly GameObject root;
        Image playerHealthFill;
        Text playerHealthLabel;
        GameObject targetFrame;
        Text targetNameLabel;
        Text targetStatusLabel;
        Image targetHealthFill;
        Text targetHealthLabel;
        Image targetAccent;
        readonly WeaponSlotWidgets[] weaponSlots = new WeaponSlotWidgets[3];

        CombatHudView(GameObject rootObject) => root = rootObject;

        public static CombatHudView Create(Transform canvasTransform)
        {
            var hudRoot = new GameObject("HudRoot");
            hudRoot.transform.SetParent(canvasTransform, false);
            var hudRect = hudRoot.AddComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;

            var view = new CombatHudView(hudRoot);
            view.Build();
            return view;
        }

        public void SetVisible(bool visible) => root.SetActive(visible);

        void Build()
        {
            BuildPlayerFrame();
            BuildWeaponBar();
            BuildTargetFrame();
        }

        void BuildPlayerFrame()
        {
            var frame = KatanaUiFactory.CreateAnchoredHudFrame(
                root.transform,
                "PlayerFrame",
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(24f, 24f),
                new Vector2(340f, 78f));

            KatanaUiFactory.CreateText(frame, "Title", "JOUEUR", 11, TextAnchor.UpperLeft, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -24f), new Vector2(-12f, -6f));

            var barHost = new GameObject("HealthBarHost");
            barHost.transform.SetParent(frame, false);
            barHost.AddComponent<RectTransform>()
                .SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(14f, 12f), new Vector2(-12f, -28f));

            var (fill, label) = KatanaUiFactory.CreateHealthBar(
                barHost.transform,
                "PlayerHealthBar",
                KatanaUiTheme.HudPlayerHealthFill);
            playerHealthFill = fill;
            playerHealthLabel = label;
        }

        void BuildWeaponBar()
        {
            var bar = KatanaUiFactory.CreateAnchoredHudFrame(
                root.transform,
                "WeaponBar",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 24f),
                new Vector2(520f, 74f));

            KatanaUiFactory.CreateText(bar, "Title", "ARMES", 11, TextAnchor.UpperLeft, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -22f), new Vector2(-12f, -4f));

            const float slotWidth = 156f;
            const float spacing = 8f;
            var startX = -((slotWidth * 3f) + (spacing * 2f)) * 0.5f + slotWidth * 0.5f;

            for (var i = 0; i < weaponSlots.Length; i++)
            {
                var x = startX + i * (slotWidth + spacing);
                weaponSlots[i] = CreateWeaponSlot(bar, i, new Vector2(x, -8f), new Vector2(slotWidth, 44f));
            }
        }

        WeaponSlotWidgets CreateWeaponSlot(Transform parent, int index, Vector2 position, Vector2 size)
        {
            var slotRoot = new GameObject($"WeaponSlot{index}");
            slotRoot.transform.SetParent(parent, false);

            var background = slotRoot.AddComponent<Image>();
            background.color = KatanaUiTheme.WeaponSlotIdle;

            var rect = slotRoot.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            var accent = KatanaUiFactory.CreateAccentStrip(slotRoot.transform, KatanaUiTheme.AccentColor, 3f);

            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(slotRoot.transform, false);
            var iconImage = iconGo.AddComponent<Image>();
            iconImage.raycastTarget = false;
            iconImage.preserveAspect = true;
            iconImage.color = Color.white;
            iconImage.rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(8f, 4f), new Vector2(40f, -4f));
            iconGo.SetActive(false);

            var iconLabel = KatanaUiFactory.CreateText(slotRoot.transform, "Icon", "?", 18, TextAnchor.MiddleLeft, FontStyle.Bold);
            iconLabel.rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(12f, 4f), new Vector2(40f, -4f));

            var keyLabel = KatanaUiFactory.CreateText(slotRoot.transform, "Key", "?", 12, TextAnchor.UpperRight, FontStyle.Bold);
            keyLabel.rectTransform.SetAnchors(new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -22f), new Vector2(-8f, -4f));

            var nameLabel = KatanaUiFactory.CreateText(slotRoot.transform, "Name", "", 12, TextAnchor.MiddleLeft);
            nameLabel.rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(44f, 4f), new Vector2(-8f, -4f));

            return new WeaponSlotWidgets
            {
                Background = background,
                Accent = accent,
                IconImage = iconImage,
                IconLabel = iconLabel,
                KeyLabel = keyLabel,
                NameLabel = nameLabel
            };
        }

        void BuildTargetFrame()
        {
            var frame = KatanaUiFactory.CreateAnchoredHudFrame(
                root.transform,
                "TargetFrame",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -18f),
                new Vector2(380f, 86f));

            targetFrame = frame.gameObject;
            targetAccent = KatanaUiFactory.CreateAccentStrip(frame, KatanaUiTheme.TargetOutOfRange);

            targetNameLabel = KatanaUiFactory.CreateText(frame, "TargetName", "Cible", 15, TextAnchor.UpperLeft, FontStyle.Bold);
            targetNameLabel.rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -28f), new Vector2(-12f, -6f));

            targetStatusLabel = KatanaUiFactory.CreateText(frame, "TargetStatus", "", 12, TextAnchor.UpperRight, FontStyle.Italic);
            targetStatusLabel.rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -28f), new Vector2(-12f, -6f));

            var barHost = new GameObject("TargetHealthHost");
            barHost.transform.SetParent(frame, false);
            barHost.AddComponent<RectTransform>()
                .SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(14f, 12f), new Vector2(-12f, -34f));

            var (fill, label) = KatanaUiFactory.CreateHealthBar(barHost.transform, "TargetHealthBar");
            targetHealthFill = fill;
            targetHealthLabel = label;
            targetFrame.SetActive(false);
        }

        public void SetPlayerHealth(float current, float max)
        {
            var ratio = max > 0f ? current / max : 0f;
            KatanaUiFactory.SetHealthBarFillAmount(playerHealthFill.rectTransform, ratio);
            playerHealthLabel.text = $"{current:0.#} / {max:0}";
        }

        public void SetWeapons(WeaponLoadout loadout)
        {
            if (loadout == null)
                return;

            for (var i = 0; i < weaponSlots.Length; i++)
                BindWeaponSlot(i, loadout, i == loadout.CurrentIndex);
        }

        public void SetSelectedWeapon(int index, WeaponLoadout loadout)
        {
            for (var i = 0; i < weaponSlots.Length; i++)
                BindWeaponSlot(i, loadout, i == index);
        }

        void BindWeaponSlot(int index, WeaponLoadout loadout, bool selected)
        {
            if (index >= weaponSlots.Length || index >= loadout.WeaponCount)
                return;

            var weapon = loadout.GetWeaponAt(index);
            var widgets = weaponSlots[index];
            var accent = KatanaUiTheme.WeaponAccent(weapon.Id);

            widgets.Background.color = selected ? KatanaUiTheme.WeaponSlotActive : KatanaUiTheme.WeaponSlotIdle;
            widgets.Accent.color = accent;

            var icon = KatanaWeaponIcons.Get(weapon.Id);
            if (icon != null)
            {
                widgets.IconImage.sprite = icon;
                widgets.IconImage.gameObject.SetActive(true);
                widgets.IconLabel.gameObject.SetActive(false);
            }
            else
            {
                widgets.IconImage.gameObject.SetActive(false);
                widgets.IconLabel.gameObject.SetActive(true);
                widgets.IconLabel.text = GetWeaponIconFallback(weapon.DisplayName);
                widgets.IconLabel.color = accent;
            }

            widgets.KeyLabel.text = loadout.GetSlotKeyLabel(index);
            widgets.KeyLabel.color = selected ? accent : KatanaUiTheme.HudMutedText;
            widgets.NameLabel.text = weapon.DisplayName;
            widgets.NameLabel.color = selected ? Color.white : KatanaUiTheme.HudMutedText;
        }

        public void ClearTarget() => targetFrame.SetActive(false);

        public void SetTarget(string displayName, float currentHealth, float maxHealth, bool inRange)
        {
            targetFrame.SetActive(true);
            targetNameLabel.text = displayName;

            var ratio = maxHealth > 0f ? currentHealth / maxHealth : 0f;
            KatanaUiFactory.SetHealthBarFillAmount(targetHealthFill.rectTransform, ratio);
            targetHealthFill.color = KatanaUiTheme.HealthColorForRatio(ratio);
            targetHealthLabel.text = $"{currentHealth:0} / {maxHealth:0}";

            var statusColor = inRange ? KatanaUiTheme.TargetInRange : KatanaUiTheme.TargetOutOfRange;
            targetAccent.color = statusColor;
            targetStatusLabel.text = inRange ? "A portee" : "Hors portee";
            targetStatusLabel.color = statusColor;
        }

        static string GetWeaponIconFallback(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                return "?";

            return displayName.Substring(0, 1).ToUpperInvariant();
        }
    }
}
