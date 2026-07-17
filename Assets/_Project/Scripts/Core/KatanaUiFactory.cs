using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Katana.Core
{
    public static class KatanaUiFactory
    {
        static Font uiFont;

        public static Font UiFont => uiFont ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            go.AddComponent<InputSystemUIInputModule>();
#else
            go.AddComponent<StandaloneInputModule>();
#endif
        }

        public static Canvas CreateOverlayCanvas(string name, int sortOrder = 0)
        {
            var canvasGo = new GameObject(name);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static Canvas CreateMenuOverlayCanvas(string name, int sortOrder = 0)
        {
            var canvas = CreateOverlayCanvas(name, sortOrder);
            MenuUiScaleRegistry.Register(canvas.GetComponent<CanvasScaler>());
            return canvas;
        }

        public static Canvas CreateHudOverlayCanvas(string name, int sortOrder = 0)
        {
            var canvas = CreateOverlayCanvas(name, sortOrder);
            HudUiScaleRegistry.Register(canvas.GetComponent<CanvasScaler>());
            return canvas;
        }

        public static void ApplyMenuScale(CanvasScaler scaler, float scale)
        {
            if (scaler == null)
                return;

            scale = Mathf.Max(0.5f, scale);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f / scale, 1080f / scale);
            scaler.matchWidthOrHeight = 0.5f;
        }

        public static RectTransform CreateFullScreenPanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;

            var rect = go.GetComponent<RectTransform>();
            StretchFull(rect);
            return rect;
        }

        public static RectTransform CreateScreenHalfPanel(Transform parent, ScreenHalf half, Color color, bool raycastTarget = true)
        {
            return half == ScreenHalf.Right
                ? CreateFixedRightPane(parent, color, raycastTarget)
                : CreateGameplayDimPanel(parent, color, raycastTarget);
        }

        public static RectTransform CreateGameplayDimPanel(Transform parent, Color color, bool raycastTarget = false)
        {
            var panel = CreateFullScreenPanel(parent, "GameplayDimPanel", color);
            panel.GetComponent<Image>().raycastTarget = raycastTarget;
            return panel;
        }

        public static RectTransform CreateFixedRightPane(Transform parent, Color color, bool raycastTarget = false)
        {
            var go = new GameObject("RightMenuPane");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(SplitScreenMenuLayout.RightPaneReferenceWidth, 0f);
            rect.anchoredPosition = Vector2.zero;

            if (color.a > 0.01f)
            {
                var image = go.AddComponent<Image>();
                image.color = color;
                image.raycastTarget = raycastTarget;
            }

            return rect;
        }

        public static RectTransform CreateRightMenuHost(Transform parent) =>
            CreateFixedRightPane(parent, KatanaUiTheme.MenuPaneClear);

        /// <summary>
        /// Ancre une fenêtre menu au centre vertical, alignée à droite du parent (inset standard).
        /// </summary>
        public static void LayoutRightMenuWindow(RectTransform window)
        {
            window.anchorMin = window.anchorMax = new Vector2(1f, 0.5f);
            window.pivot = new Vector2(1f, 0.5f);
            window.anchoredPosition = new Vector2(-SplitScreenMenuLayout.WindowPaneInset, 0f);
        }

        public static RectTransform CreateScaledMenuContentHost(
            Transform parent,
            float designWidth,
            float designHeight,
            float contentScale)
        {
            var host = new GameObject("ScaledContent");
            host.transform.SetParent(parent, false);

            var rect = host.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(designWidth, designHeight);
            host.transform.localScale = Vector3.one * contentScale;
            return rect;
        }

        public static RectTransform CreatePanel(Transform parent, string name, Vector2 size, Color color)
        {
            var rect = KatanaUiVisuals.CreateWindowPanel(parent, name, size);
            var inner = rect.Find("Inner");
            if (inner != null && inner.TryGetComponent<Image>(out var innerImage))
                innerImage.color = color;
            return rect;
        }

        public static RectTransform CreateAnchoredPanel(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            Color color)
        {
            var rect = CreatePanel(parent, name, sizeDelta, color);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        public static RectTransform CreateAnchoredHudFrame(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var rect = KatanaUiVisuals.CreateHudFrame(parent, name, sizeDelta);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        public static Image CreateAccentStrip(Transform parent, Color color, float width = 4f)
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

        public static (Image fill, Text label) CreateHealthBar(
            Transform parent,
            string name,
            Color fillColor)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            var rowRect = row.AddComponent<RectTransform>();
            StretchFull(rowRect);

            var background = new GameObject("Background");
            background.transform.SetParent(row.transform, false);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = KatanaUiTheme.HudHealthBackground;
            bgImage.raycastTarget = false;
            StretchFull(background.GetComponent<RectTransform>());

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(background.transform, false);
            var fill = fillGo.AddComponent<Image>();
            fill.color = fillColor;
            fill.type = Image.Type.Simple;
            fill.raycastTarget = false;
            var fillRect = fillGo.GetComponent<RectTransform>();
            SetHealthBarFillAmount(fillRect, 1f);

            var label = CreateText(row.transform, "Label", "", 13, TextAnchor.MiddleCenter, FontStyle.Bold);
            label.color = Color.white;
            return (fill, label);
        }

        public static void SetHealthBarFillAmount(RectTransform fillRect, float ratio)
        {
            if (fillRect == null)
                return;

            ratio = Mathf.Clamp01(ratio);
            const float pad = 2f;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(ratio, 1f);
            fillRect.offsetMin = new Vector2(pad, pad);
            fillRect.offsetMax = ratio >= 1f ? new Vector2(-pad, -pad) : Vector2.zero;
        }

        public static (Image fill, Text label) CreateHealthBar(Transform parent, string name) =>
            CreateHealthBar(parent, name, KatanaUiTheme.HudHealthFill);

        public static Text CreateText(
            Transform parent,
            string name,
            string content,
            int fontSize,
            TextAnchor alignment,
            FontStyle fontStyle = FontStyle.Normal)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = UiFont;
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.fontStyle = fontStyle;
            text.color = KatanaUiTheme.TextPrimary;
            text.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            StretchFull(rect);
            return text;
        }

        public static Button CreateButton(
            Transform parent,
            string label,
            Vector2 anchoredPosition,
            Vector2 size,
            Action onClick,
            int fontSize = 20)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = KatanaUiTheme.ButtonNormal;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            KatanaUiTheme.ApplyButtonColors(button, KatanaUiTheme.ButtonNormal, KatanaUiTheme.ButtonHighlight, KatanaUiTheme.ButtonPressed);
            button.onClick.AddListener(() => onClick?.Invoke());

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            CreateText(go.transform, "Label", label, fontSize, TextAnchor.MiddleCenter);
            return button;
        }

        public static Button CreateToolbarButton(
            Transform parent,
            string label,
            float anchorMinX,
            float anchorMaxX,
            float top,
            float height,
            Action onClick,
            int fontSize = 18)
        {
            var go = new GameObject(label + "TabButton");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = KatanaUiTheme.ButtonNormal;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            KatanaUiTheme.ApplyButtonColors(button, KatanaUiTheme.ButtonNormal, KatanaUiTheme.ButtonHighlight, KatanaUiTheme.ButtonPressed);
            button.onClick.AddListener(() => onClick?.Invoke());

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(anchorMinX, 1f);
            rect.anchorMax = new Vector2(anchorMaxX, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(6f, -(top + height));
            rect.offsetMax = new Vector2(-6f, -top);

            CreateText(go.transform, "Label", label, fontSize, TextAnchor.MiddleCenter);
            return button;
        }

        public static Button CreateFooterButton(
            Transform parent,
            string label,
            float height,
            Action onClick,
            int fontSize = 20)
        {
            var go = new GameObject(label + "FooterButton");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = KatanaUiTheme.ButtonNormal;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            KatanaUiTheme.ApplyButtonColors(button, KatanaUiTheme.ButtonNormal, KatanaUiTheme.ButtonHighlight, KatanaUiTheme.ButtonPressed);
            button.onClick.AddListener(() => onClick?.Invoke());

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, 0f);
            rect.anchorMax = new Vector2(0.8f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(0f, 18f);
            rect.offsetMax = new Vector2(0f, 18f + height);

            CreateText(go.transform, "Label", label, fontSize, TextAnchor.MiddleCenter);
            return button;
        }

        public static Button CreatePanelActionButton(
            Transform parent,
            string label,
            Action onClick,
            int fontSize = 16)
        {
            var go = new GameObject(label + "ActionButton");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = KatanaUiTheme.ButtonGhost;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            KatanaUiTheme.ApplyButtonColors(button, KatanaUiTheme.ButtonGhost, KatanaUiTheme.ButtonHighlight, KatanaUiTheme.ButtonPressed);
            button.onClick.AddListener(() => onClick?.Invoke());

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(280f, 40f);
            rect.anchoredPosition = new Vector2(0f, 6f);

            CreateText(go.transform, "Label", label, fontSize, TextAnchor.MiddleCenter);
            return button;
        }

        public static Toggle CreateToggle(
            Transform parent,
            string label,
            Vector2 anchoredPosition,
            Vector2 size,
            bool value,
            Action<bool> onChanged)
        {
            var go = new GameObject(label + "Toggle");
            go.transform.SetParent(parent, false);

            var background = go.AddComponent<Image>();
            background.color = KatanaUiTheme.PanelFillInset;

            var toggle = go.AddComponent<Toggle>();
            toggle.isOn = value;
            toggle.targetGraphic = background;
            toggle.onValueChanged.AddListener(v => onChanged?.Invoke(v));

            var checkGo = new GameObject("Checkmark");
            checkGo.transform.SetParent(go.transform, false);
            var checkImage = checkGo.AddComponent<Image>();
            checkImage.color = KatanaUiTheme.AccentColor;
            var checkRect = checkGo.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0f, 0.5f);
            checkRect.anchorMax = new Vector2(0f, 0.5f);
            checkRect.sizeDelta = new Vector2(24f, 24f);
            checkRect.anchoredPosition = new Vector2(14f, 0f);

            toggle.graphic = checkImage;

            CreateText(go.transform, "Label", label, 15, TextAnchor.MiddleLeft)
                .rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(40f, 0f), new Vector2(-8f, 0f));

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            return toggle;
        }

        static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    public static class RectTransformUiExtensions
    {
        public static void SetAnchors(
            this RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
    }
}
