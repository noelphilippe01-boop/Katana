using System;
using Katana.Characters;
using Katana.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Combat
{
    public class InventoryPanelView : MonoBehaviour
    {
        static float PanelWidth => SplitScreenMenuLayout.WindowReferenceWidth;
        const float DesignPanelHeight = 870f;
        const float ContentSidePadding = 16f;
        const float HeaderTop = 58f;
        const float FooterBottom = 16f;
        const float FooterHeight = 40f;
        const float SectionGap = 10f;
        const float InventoryGridVerticalPadding = 12f;

        const float SmallSlot = 80f;
        const float TallSlotWidth = 80f;
        const float TallSlotHeight = 146f;
        const float ArmorSlotHeight = 146f;
        const float PantsSlotHeight = 88f;
        const float AmuletSlot = 54f;
        const float EquipmentColumnGap = 12f;
        const float EquipmentColumnX = SmallSlot * 0.5f + EquipmentColumnGap + SmallSlot * 0.5f;

        const float YHelmet = -12f;
        const float YTallRow = -103f;
        const float YJewelryRow = -260f;
        const float YLowerRow = -351f;

        const int InventoryColumns = 14;
        const int InventoryRows = 6;
        const float InventorySlotGap = 3f;
        const float GridFramePadding = 5f;

        Text goldValueLabel;
        EquipmentSlotView mainWeaponSlot;
        WeaponLoadout boundLoadout;

        static float InventoryContentWidth => PanelWidth - ContentSidePadding * 2f;

        static float ResolveInventorySlotSize()
        {
            return Mathf.Floor(
                (InventoryContentWidth - GridFramePadding * 2f - (InventoryColumns - 1) * InventorySlotGap) / InventoryColumns);
        }

        static float InventoryGridInnerWidth =>
            InventoryColumns * ResolveInventorySlotSize() + (InventoryColumns - 1) * InventorySlotGap;

        static float InventoryGridInnerHeight =>
            InventoryRows * ResolveInventorySlotSize() + (InventoryRows - 1) * InventorySlotGap;

        static float InventoryBlockHeight =>
            InventoryGridInnerHeight + GridFramePadding * 2f + InventoryGridVerticalPadding * 2f;

        static float EquipmentBlockHeight =>
            DesignPanelHeight - HeaderTop - InventoryBlockHeight - SectionGap - InventorySectionBottom;

        static float InventorySectionTop => HeaderTop + EquipmentBlockHeight + SectionGap;

        static float InventorySectionBottom => FooterBottom + FooterHeight + SectionGap;

        static float EquipmentDollHeight => EquipmentBlockHeight - 20f;

        public static InventoryPanelView Create(Transform parent, Action onClose)
        {
            var canvas = parent.GetComponentInParent<Canvas>();
            var windowHeight = SplitScreenMenuLayout.ResolveWindowHeight(canvas, PanelWidth);
            var contentScale = SplitScreenMenuLayout.ResolveContentScale(PanelWidth, windowHeight, DesignPanelHeight);

            var root = KatanaUiVisuals.CreateWindowPanel(
                parent,
                "InventoryPanel",
                new Vector2(PanelWidth, windowHeight));
            KatanaUiFactory.LayoutRightMenuWindow(root);

            var inner = root.Find("Inner");
            var contentHost = inner != null
                ? KatanaUiFactory.CreateScaledMenuContentHost(inner, PanelWidth, DesignPanelHeight, contentScale)
                : KatanaUiFactory.CreateScaledMenuContentHost(root, PanelWidth, DesignPanelHeight, contentScale);

            var view = root.gameObject.AddComponent<InventoryPanelView>();
            view.Build(contentHost, onClose);
            return view;
        }

        void Build(RectTransform root, Action onClose)
        {
            KatanaUiFactory.CreateText(root, "Title", "Inventaire", 34, TextAnchor.UpperCenter, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(20f, -62f), new Vector2(-20f, -10f));

            BuildEquipmentSection(root);
            BuildInventorySection(root);
            BuildFooter(root, onClose);
        }

        void BuildEquipmentSection(RectTransform root)
        {
            var sectionBottom = DesignPanelHeight - InventorySectionTop;
            var section = CreateSectionBetween(root, "EquipmentSection", HeaderTop, sectionBottom);

            var frame = KatanaUiVisuals.CreateInsetPanel(section, "EquipmentFrame", KatanaUiTheme.PanelFillInset);
            StretchSection(frame, 0f, 0f, 0f, 0f);

            var dollHost = new GameObject("EquipmentDoll");
            dollHost.transform.SetParent(frame, false);
            var dollRect = dollHost.AddComponent<RectTransform>();
            dollRect.anchorMin = new Vector2(0.5f, 1f);
            dollRect.anchorMax = new Vector2(0.5f, 1f);
            dollRect.pivot = new Vector2(0.5f, 1f);
            dollRect.sizeDelta = new Vector2(PanelWidth - ContentSidePadding * 2f - 16f, EquipmentDollHeight);
            var dollVerticalInset = Mathf.Max(0f, (EquipmentBlockHeight - EquipmentDollHeight) * 0.5f);
            dollRect.anchoredPosition = new Vector2(0f, -dollVerticalInset);

            KatanaUiVisuals.CreateEquipmentSlot(dollHost.transform, EquipmentSlotKind.Helmet, new Vector2(0f, YHelmet), new Vector2(SmallSlot, SmallSlot));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Amulet,
                new Vector2(SmallSlot * 0.5f + AmuletSlot * 0.5f + 8f, YHelmet + 6f),
                new Vector2(AmuletSlot, AmuletSlot));

            mainWeaponSlot = KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.MainHand,
                new Vector2(-EquipmentColumnX, YTallRow),
                new Vector2(TallSlotWidth, TallSlotHeight));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Chest,
                new Vector2(0f, YTallRow),
                new Vector2(SmallSlot, ArmorSlotHeight));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.OffHand,
                new Vector2(EquipmentColumnX, YTallRow),
                new Vector2(TallSlotWidth, TallSlotHeight));

            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Ring,
                new Vector2(-EquipmentColumnX, YJewelryRow),
                new Vector2(SmallSlot, SmallSlot));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Belt,
                new Vector2(0f, YJewelryRow),
                new Vector2(SmallSlot, SmallSlot));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Ring,
                new Vector2(EquipmentColumnX, YJewelryRow),
                new Vector2(SmallSlot, SmallSlot));

            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Gloves,
                new Vector2(-EquipmentColumnX, YLowerRow),
                new Vector2(SmallSlot, SmallSlot));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Pants,
                new Vector2(0f, YLowerRow),
                new Vector2(SmallSlot, PantsSlotHeight));
            KatanaUiVisuals.CreateEquipmentSlot(
                dollHost.transform,
                EquipmentSlotKind.Boots,
                new Vector2(EquipmentColumnX, YLowerRow),
                new Vector2(SmallSlot, SmallSlot));

            RefreshWeaponSlot();
        }

        void BuildInventorySection(RectTransform root)
        {
            var section = CreateSectionBetween(root, "InventorySection", InventorySectionTop, InventorySectionBottom);

            var slotSize = ResolveInventorySlotSize();
            var gridWidth = InventoryGridInnerWidth;
            var gridHeight = InventoryGridInnerHeight;

            var gridFrame = KatanaUiVisuals.CreateInventoryGridFrame(section, new Vector2(gridWidth, gridHeight), GridFramePadding);
            gridFrame.anchorMin = new Vector2(0.5f, 1f);
            gridFrame.anchorMax = new Vector2(0.5f, 1f);
            gridFrame.pivot = new Vector2(0.5f, 1f);
            gridFrame.anchoredPosition = new Vector2(0f, -InventoryGridVerticalPadding);

            var gridInner = gridFrame.Find("InventoryGridInner");
            var gridHost = new GameObject("InventoryGrid");
            gridHost.transform.SetParent(gridInner, false);
            var gridRect = gridHost.AddComponent<RectTransform>();
            gridRect.anchorMin = gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
            gridRect.anchoredPosition = Vector2.zero;

            for (var row = 0; row < InventoryRows; row++)
            {
                for (var col = 0; col < InventoryColumns; col++)
                {
                    var x = -gridWidth * 0.5f + slotSize * 0.5f + col * (slotSize + InventorySlotGap);
                    var y = gridHeight * 0.5f - slotSize * 0.5f - row * (slotSize + InventorySlotGap);
                    KatanaUiVisuals.CreateInventoryCell(gridHost.transform, row * InventoryColumns + col, new Vector2(x, y), slotSize);
                }
            }
        }

        void BuildFooter(RectTransform root, Action onClose)
        {
            var footer = new GameObject("Footer");
            footer.transform.SetParent(root, false);
            var footerRect = footer.AddComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0f, 0f);
            footerRect.anchorMax = new Vector2(1f, 0f);
            footerRect.pivot = new Vector2(0.5f, 0f);
            footerRect.offsetMin = new Vector2(ContentSidePadding, FooterBottom);
            footerRect.offsetMax = new Vector2(-ContentSidePadding, FooterBottom + FooterHeight);

            var closeButton = new GameObject("CloseButton");
            closeButton.transform.SetParent(footer.transform, false);
            var closeRect = closeButton.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0f, 0f);
            closeRect.anchorMax = new Vector2(0f, 1f);
            closeRect.pivot = new Vector2(0f, 0.5f);
            closeRect.sizeDelta = new Vector2(136f, 0f);
            closeRect.anchoredPosition = Vector2.zero;

            var closeImage = closeButton.AddComponent<Image>();
            closeImage.color = KatanaUiTheme.ButtonNormal;

            var close = closeButton.AddComponent<Button>();
            close.targetGraphic = closeImage;
            KatanaUiTheme.ApplyButtonColors(close, KatanaUiTheme.ButtonNormal, KatanaUiTheme.ButtonHighlight, KatanaUiTheme.ButtonPressed);
            close.onClick.AddListener(() => onClose?.Invoke());

            KatanaUiFactory.CreateText(closeButton.transform, "Label", "Retour", 18, TextAnchor.MiddleCenter);

            var goldBar = KatanaUiVisuals.CreateInsetPanel(footer.transform, "GoldBar", KatanaUiTheme.PanelFillRaised);
            var goldRect = goldBar;
            goldRect.anchorMin = new Vector2(0f, 0f);
            goldRect.anchorMax = new Vector2(1f, 1f);
            goldRect.offsetMin = new Vector2(144f, 0f);
            goldRect.offsetMax = Vector2.zero;

            KatanaUiFactory.CreateAccentStrip(goldBar, KatanaUiTheme.HudGold, 3f);

            KatanaUiFactory.CreateText(goldBar, "GoldTitle", "Or", 15, TextAnchor.MiddleLeft, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(0.22f, 1f), new Vector2(14f, 0f), new Vector2(0f, 0f));

            goldValueLabel = KatanaUiFactory.CreateText(goldBar, "GoldValue", "0", 22, TextAnchor.MiddleRight, FontStyle.Bold);
            goldValueLabel.color = KatanaUiTheme.HudGold;
            goldValueLabel.rectTransform.SetAnchors(new Vector2(0.22f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(-12f, 0f));
        }

        static RectTransform CreateSectionBetween(RectTransform root, string name, float top, float bottom)
        {
            var section = new GameObject(name);
            section.transform.SetParent(root, false);
            var rect = section.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(ContentSidePadding, bottom);
            rect.offsetMax = new Vector2(-ContentSidePadding, -top);
            return rect;
        }

        static void StretchSection(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(right, top);
        }

        public void BindLoadout(WeaponLoadout loadout)
        {
            boundLoadout = loadout;
            if (boundLoadout != null)
                boundLoadout.WeaponChanged += OnWeaponChanged;

            RefreshWeaponSlot();
        }

        void OnDestroy()
        {
            if (boundLoadout != null)
                boundLoadout.WeaponChanged -= OnWeaponChanged;
        }

        void OnWeaponChanged(int index, WeaponProfile weapon) => RefreshWeaponSlot();

        public void Refresh(PlayerInventory inventory, WeaponLoadout loadout)
        {
            if (loadout != boundLoadout)
            {
                if (boundLoadout != null)
                    boundLoadout.WeaponChanged -= OnWeaponChanged;

                BindLoadout(loadout);
            }
            else
            {
                RefreshWeaponSlot();
            }

            if (goldValueLabel != null && inventory != null)
                goldValueLabel.text = inventory.Gold.ToString();
        }

        void RefreshWeaponSlot()
        {
            var weapon = boundLoadout != null && boundLoadout.WeaponCount > 0
                ? boundLoadout.GetWeaponAt(0)
                : WeaponProfile.Katana;

            KatanaUiVisuals.SetEquipmentSlotEquipped(
                mainWeaponSlot,
                KatanaUiTheme.WeaponAccent(weapon.Id),
                KatanaWeaponIcons.Get(weapon.Id));
        }
    }
}
