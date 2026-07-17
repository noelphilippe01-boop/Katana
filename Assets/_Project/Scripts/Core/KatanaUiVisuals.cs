using UnityEngine;
using UnityEngine.UI;

namespace Katana.Core
{
    public enum EquipmentSlotKind
    {
        Helmet,
        Amulet,
        MainHand,
        Chest,
        OffHand,
        Gloves,
        Boots,
        Ring,
        Belt,
        Pants
    }

    public struct EquipmentSlotView
    {
        public RectTransform Root;
        public Image Background;
        public Image Accent;
        public Image IconImage;
        public Text GlyphLabel;
        public Text SlotLabel;
        public Text ItemLabel;
    }

    /// <summary>
    /// Composants visuels réutilisables (fenêtres, slots, cellules, en-têtes).
    /// </summary>
    public static class KatanaUiVisuals
    {
        public static RectTransform CreateWindowPanel(Transform parent, string name, Vector2 size)
        {
            var root = CreateBorderedPanel(
                parent,
                name,
                size,
                KatanaUiTheme.WindowBorder,
                KatanaUiTheme.PanelFill,
                KatanaUiSprites.SpriteId.WindowFrame);

            if (KatanaUiSprites.Get(KatanaUiSprites.SpriteId.WindowFrame) == null)
            {
                CreateTopShine(root);
                CreateAccentStrip(root, KatanaUiTheme.AccentPrimary, 5f);
            }

            return root;
        }

        public static RectTransform CreateHudFrame(Transform parent, string name, Vector2 size)
        {
            var root = CreateBorderedPanel(
                parent,
                name,
                size,
                KatanaUiTheme.HudBorderColor,
                KatanaUiTheme.HudFrameColor,
                KatanaUiSprites.SpriteId.HudFrame);

            if (KatanaUiSprites.Get(KatanaUiSprites.SpriteId.HudFrame) == null)
                CreateAccentStrip(root, KatanaUiTheme.AccentPrimary, 4f);

            return root;
        }

        public static RectTransform CreateInsetPanel(Transform parent, string name, Color fill)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = fill;
            image.raycastTarget = false;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = KatanaUiTheme.WindowBorderInner;
            outline.effectDistance = new Vector2(1f, -1f);

            return go.GetComponent<RectTransform>();
        }

        public static Text CreateSectionTitle(Transform parent, string title, int fontSize = 20)
        {
            var label = KatanaUiFactory.CreateText(parent, "SectionTitle", title, fontSize, TextAnchor.UpperLeft, FontStyle.Bold);
            label.color = KatanaUiTheme.TextPrimary;

            var rule = new GameObject("SectionRule");
            rule.transform.SetParent(parent, false);
            var ruleRect = rule.AddComponent<RectTransform>();
            ruleRect.anchorMin = new Vector2(0f, 1f);
            ruleRect.anchorMax = new Vector2(1f, 1f);
            ruleRect.pivot = new Vector2(0.5f, 1f);
            ruleRect.offsetMin = new Vector2(0f, -fontSize - 10f);
            ruleRect.offsetMax = new Vector2(0f, -fontSize - 6f);
            var ruleImage = rule.AddComponent<Image>();
            ruleImage.color = KatanaUiTheme.PanelDivider;
            ruleImage.raycastTarget = false;

            return label;
        }

        public static EquipmentSlotView CreateEquipmentSlot(
            Transform parent,
            EquipmentSlotKind kind,
            Vector2 position,
            Vector2 size,
            bool fromTopAnchor = true)
        {
            var slotRoot = new GameObject(kind + "Slot");
            slotRoot.transform.SetParent(parent, false);

            var background = slotRoot.AddComponent<Image>();
            background.color = KatanaUiTheme.EquipmentSlotEmpty;

            var rect = slotRoot.GetComponent<RectTransform>();
            if (fromTopAnchor)
            {
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
            }
            else
            {
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
            }

            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            var accent = CreateAccentStrip(slotRoot.transform, KatanaUiTheme.AccentSecondary, 4f);
            var iconImage = CreateSlotIcon(slotRoot.transform);

            var itemLabel = KatanaUiFactory.CreateText(
                slotRoot.transform,
                "ItemLabel",
                string.Empty,
                1,
                TextAnchor.MiddleCenter,
                FontStyle.Bold);
            itemLabel.gameObject.SetActive(false);

            return new EquipmentSlotView
            {
                Root = rect,
                Background = background,
                Accent = accent,
                IconImage = iconImage,
                GlyphLabel = null,
                SlotLabel = null,
                ItemLabel = itemLabel
            };
        }

        public static void SetEquipmentSlotEmpty(EquipmentSlotView slot)
        {
            slot.Background.color = KatanaUiTheme.EquipmentSlotEmpty;
            slot.Accent.color = KatanaUiTheme.AccentSecondary;
            SetSlotIcon(slot.IconImage, null);
            if (slot.GlyphLabel != null)
                slot.GlyphLabel.gameObject.SetActive(false);
            if (slot.ItemLabel != null)
                slot.ItemLabel.gameObject.SetActive(false);
        }

        public static void SetEquipmentSlotEquipped(EquipmentSlotView slot, Color accentColor, Sprite icon = null)
        {
            slot.Background.color = KatanaUiTheme.EquipmentSlotFilled;
            slot.Accent.color = accentColor;
            SetSlotIcon(slot.IconImage, icon);
            if (slot.GlyphLabel != null)
                slot.GlyphLabel.gameObject.SetActive(false);
            if (slot.ItemLabel != null)
                slot.ItemLabel.gameObject.SetActive(false);
        }

        static Image CreateSlotIcon(Transform parent)
        {
            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(parent, false);
            iconGo.transform.SetAsLastSibling();

            var image = iconGo.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.type = Image.Type.Simple;
            image.color = Color.white;

            var rect = iconGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            iconGo.SetActive(false);

            return image;
        }

        static void SetSlotIcon(Image iconImage, Sprite sprite)
        {
            if (iconImage == null)
                return;

            if (sprite == null)
            {
                iconImage.sprite = null;
                iconImage.gameObject.SetActive(false);
                return;
            }

            iconImage.sprite = sprite;
            iconImage.gameObject.SetActive(true);
        }

        public static RectTransform CreateInventoryCell(Transform parent, int index, Vector2 position, float size)
        {
            var slotRoot = new GameObject($"InventoryCell{index}");
            slotRoot.transform.SetParent(parent, false);

            var background = slotRoot.AddComponent<Image>();
            if (KatanaUiSprites.Get(KatanaUiSprites.SpriteId.GridFrame) != null)
            {
                background.color = new Color(1f, 1f, 1f, 0.04f);
            }
            else
            {
                background.color = KatanaUiTheme.InventoryCellFill;
            }
            background.raycastTarget = false;

            var rect = slotRoot.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = position;

            return rect;
        }

        public static RectTransform CreateInventoryGridFrame(Transform parent, Vector2 innerGridSize, float padding)
        {
            var frameWidth = innerGridSize.x + padding * 2f;
            var frameHeight = innerGridSize.y + padding * 2f;

            var frame = new GameObject("InventoryGridFrame");
            frame.transform.SetParent(parent, false);
            var frameRect = frame.AddComponent<RectTransform>();
            frameRect.sizeDelta = new Vector2(frameWidth, frameHeight);

            var frameImage = frame.AddComponent<Image>();
            KatanaUiSprites.TryApplySliced(frameImage, KatanaUiSprites.SpriteId.GridFrame, KatanaUiTheme.InventoryGridFrame);
            frameImage.raycastTarget = false;

            var inner = new GameObject("InventoryGridInner");
            inner.transform.SetParent(frame.transform, false);
            var innerImage = inner.AddComponent<Image>();
            if (KatanaUiSprites.Get(KatanaUiSprites.SpriteId.GridFrame) != null)
            {
                innerImage.color = Color.clear;
            }
            else
            {
                innerImage.color = KatanaUiTheme.InventoryGridInner;
            }

            innerImage.raycastTarget = false;
            var gridInset = KatanaUiSprites.Get(KatanaUiSprites.SpriteId.GridFrame) != null
                ? KatanaUiSprites.ContentInset(KatanaUiSprites.SpriteId.GridFrame)
                : padding;
            StretchInset(inner.GetComponent<RectTransform>(), gridInset);

            return frameRect;
        }

        public static RectTransform CreateTitlePlate(Transform parent, string title, string subtitle)
        {
            var plate = CreateBorderedPanel(
                parent,
                "TitlePlate",
                new Vector2(560f, 150f),
                KatanaUiTheme.WindowBorder,
                KatanaUiTheme.PanelFillRaised,
                KatanaUiSprites.SpriteId.WindowFrame);

            if (KatanaUiSprites.Get(KatanaUiSprites.SpriteId.WindowFrame) == null)
            {
                CreateTopShine(plate);
                CreateAccentStrip(plate, KatanaUiTheme.AccentPrimary, 5f);
            }

            var titleLabel = KatanaUiFactory.CreateText(plate, "Title", title, 56, TextAnchor.UpperCenter, FontStyle.Bold);
            titleLabel.color = KatanaUiTheme.TextPrimary;
            titleLabel.rectTransform.SetAnchors(new Vector2(0f, 0.35f), new Vector2(1f, 1f), new Vector2(16f, 0f), new Vector2(-16f, -12f));

            if (!string.IsNullOrEmpty(subtitle))
            {
                var subtitleLabel = KatanaUiFactory.CreateText(plate, "Subtitle", subtitle, 20, TextAnchor.LowerCenter);
                subtitleLabel.color = KatanaUiTheme.TextSecondary;
                subtitleLabel.rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 0.4f), new Vector2(16f, 8f), new Vector2(-16f, 0f));
            }

            return plate;
        }

        static RectTransform CreateBorderedPanel(
            Transform parent,
            string name,
            Vector2 size,
            Color borderColor,
            Color fillColor)
        {
            return CreateBorderedPanel(
                parent,
                name,
                size,
                borderColor,
                fillColor,
                KatanaUiSprites.SpriteId.WindowFrame);
        }

        static RectTransform CreateBorderedPanel(
            Transform parent,
            string name,
            Vector2 size,
            Color borderColor,
            Color fillColor,
            KatanaUiSprites.SpriteId spriteId)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var border = go.AddComponent<Image>();
            KatanaUiSprites.TryApplySliced(border, spriteId, borderColor);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;

            var inner = new GameObject("Inner");
            inner.transform.SetParent(go.transform, false);
            var innerImage = inner.AddComponent<Image>();
            innerImage.color = KatanaUiSprites.Get(spriteId) != null ? Color.clear : fillColor;
            innerImage.raycastTarget = false;
            StretchInset(inner.GetComponent<RectTransform>(), KatanaUiSprites.ContentInset(spriteId));

            return rect;
        }

        static void CreateTopShine(RectTransform panel)
        {
            var shine = new GameObject("TopShine");
            shine.transform.SetParent(panel, false);
            var image = shine.AddComponent<Image>();
            image.color = KatanaUiTheme.TopShine;
            image.raycastTarget = false;

            var rect = shine.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(3f, -3f);
            rect.offsetMax = new Vector2(-3f, 0f);
        }

        static Image CreateAccentStrip(Transform parent, Color color, float width)
        {
            var strip = new GameObject("AccentStrip");
            strip.transform.SetParent(parent, false);
            var image = strip.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            var rect = strip.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(width, 0f);
            rect.anchoredPosition = Vector2.zero;
            return image;
        }

        static void AddOutline(GameObject target, Color color)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(1f, -1f);
        }

        static void StretchInset(RectTransform rect, float inset)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
