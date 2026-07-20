using System;
using Katana.Characters;
using Katana.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Combat
{
    public class InventoryPanelView : MonoBehaviour
    {
        const float DesignPanelWidth = GameplayWindowProfile.InventoryDesignWidth;
        const float DesignPanelHeight = GameplayWindowProfile.InventoryDesignHeight;
        const float ContentSidePadding = 4f;
        const float TitleTopPadding = 16f;
        const float TitleBandHeight = 40f;
        const float TitleBottomPadding = 16f;
        const float HeaderTop = TitleTopPadding + TitleBandHeight + TitleBottomPadding;
        const float FooterBottom = 20f;
        const float FooterHeight = 40f;
        const float SectionGap = 2f;
        const float InventoryGridVerticalPadding = 4f;
        const float EquipmentSectionPaddingY = 4f;

        const float EquipmentUnit = 38f;
        const float EquipmentGridGap = 6f;
        const float EquipmentFramePad = 6f;
        const int EquipmentGridColumns = 6;
        const int EquipmentGridRows = 9;

        const int InventoryColumns = 14;
        const int InventoryRows = 6;
        const float InventorySlotGap = 3f;
        const float GridFramePadding = 5f;

        Text goldValueLabel;
        EquipmentSlotView mainWeaponSlot;
        WeaponLoadout boundLoadout;

        static float InventoryContentWidth => DesignPanelWidth - ContentSidePadding * 2f;

        static float CellStep => EquipmentUnit + EquipmentGridGap;

        static float GridSpan(int cells) =>
            cells * EquipmentUnit + (cells - 1) * EquipmentGridGap;

        static float EquipmentDollWidth => GridSpan(EquipmentGridColumns) + EquipmentFramePad * 2f;

        static float EquipmentContentHeight => GridSpan(EquipmentGridRows) + EquipmentFramePad * 2f;

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

        static float FooterReserved => FooterBottom + FooterHeight + SectionGap;

        static float EquipmentBlockHeight => EquipmentContentHeight + EquipmentSectionPaddingY * 2f;

        static float EquipmentDollHeight => EquipmentContentHeight;

        static float InventorySectionTop => HeaderTop + EquipmentBlockHeight + SectionGap;

        static Vector2 GridSlotTopPosition(int col, int row, int horizontalCells)
        {
            var x = -EquipmentDollWidth * 0.5f
                + EquipmentFramePad
                + col * CellStep
                + GridSpan(horizontalCells) * 0.5f;
            var y = -(EquipmentFramePad + row * CellStep);
            return new Vector2(x, y);
        }

        static Vector2 GridSlotSize(int verticalCells, int horizontalCells) =>
            new(GridSpan(horizontalCells), GridSpan(verticalCells));

        public static InventoryPanelView Create(Transform parent, Action onClose)
        {
            var canvas = parent.GetComponentInParent<Canvas>();
            var profile = GameplayWindowProfile.Inventory(canvas);
            var scale = GameSettings.GetGameplayWindowScale(profile.WindowId);
            var windowSize = profile.ResolveWindowSize(scale);
            var innerSize = profile.InnerSize(windowSize);

            var root = KatanaUiVisuals.CreateWindowPanel(
                parent,
                "InventoryPanel",
                windowSize);

            var inner = root.Find("Inner");
            var contentHost = inner != null
                ? KatanaUiFactory.CreateDesignLayoutHost(inner, profile.DesignWidth, profile.DesignHeight, innerSize.x, innerSize.y)
                : KatanaUiFactory.CreateDesignLayoutHost(root, profile.DesignWidth, profile.DesignHeight, innerSize.x, innerSize.y);

            ResizableGameplayWindow.Attach(root, profile, resizable: true);

            var view = root.gameObject.AddComponent<InventoryPanelView>();
            view.Build(contentHost, onClose);
            return view;
        }

        void Build(RectTransform root, Action onClose)
        {
            BuildEquipmentSection(root);
            BuildInventorySection(root);
            BuildFooter(root, onClose);

            var title = KatanaUiFactory.CreateText(root, "Title", "Inventaire", 34, TextAnchor.UpperCenter, FontStyle.Bold);
            title.color = KatanaUiTheme.TextPrimary;
            title.rectTransform.SetAsLastSibling();
            title.rectTransform.SetAnchors(
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(ContentSidePadding, -(TitleTopPadding + TitleBandHeight)),
                new Vector2(-ContentSidePadding, -TitleTopPadding));
        }

        void BuildEquipmentSection(RectTransform root)
        {
            var equipmentSectionBottom = DesignPanelHeight - HeaderTop - EquipmentBlockHeight;
            var section = CreateSectionBetween(root, "EquipmentSection", HeaderTop, equipmentSectionBottom);

            var frame = KatanaUiVisuals.CreateInsetPanel(section, "EquipmentFrame", KatanaUiTheme.PanelFillInset);
            frame.anchorMin = frame.anchorMax = new Vector2(0.5f, 1f);
            frame.pivot = new Vector2(0.5f, 1f);
            frame.sizeDelta = new Vector2(EquipmentDollWidth, EquipmentBlockHeight);
            frame.anchoredPosition = Vector2.zero;

            var dollHost = new GameObject("EquipmentDoll");
            dollHost.transform.SetParent(frame, false);
            var dollRect = dollHost.AddComponent<RectTransform>();
            dollRect.anchorMin = new Vector2(0.5f, 1f);
            dollRect.anchorMax = new Vector2(0.5f, 1f);
            dollRect.pivot = new Vector2(0.5f, 1f);
            dollRect.sizeDelta = new Vector2(EquipmentDollWidth, EquipmentDollHeight);
            dollRect.anchoredPosition = new Vector2(0f, -EquipmentSectionPaddingY);

            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Helmet, 2, 0, 2, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Amulet, 4, 1, 1, 1);

            mainWeaponSlot = PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.MainHand, 0, 2, 3, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Chest, 2, 2, 3, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.OffHand, 4, 2, 3, 2);

            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Ring, 1, 5, 1, 1);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Belt, 2, 5, 1, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Ring, 4, 5, 1, 1);

            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Pants, 2, 6, 3, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Gloves, 0, 7, 2, 2);
            PlaceGridEquipmentSlot(dollHost.transform, EquipmentSlotKind.Boots, 4, 7, 2, 2);

            RefreshWeaponSlot();
        }

        static EquipmentSlotView PlaceGridEquipmentSlot(
            Transform parent,
            EquipmentSlotKind kind,
            int col,
            int row,
            int verticalCells,
            int horizontalCells)
        {
            return KatanaUiVisuals.CreateEquipmentSlot(
                parent,
                kind,
                GridSlotTopPosition(col, row, horizontalCells),
                GridSlotSize(verticalCells, horizontalCells));
        }

        void BuildInventorySection(RectTransform root)
        {
            var section = CreateSectionBetween(root, "InventorySection", InventorySectionTop, FooterReserved);

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
