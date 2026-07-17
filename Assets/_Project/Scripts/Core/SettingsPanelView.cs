using System;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Core
{
    public class SettingsPanelView : MonoBehaviour
    {
        const float TabBarTop = 58f;
        const float TabBarHeight = 44f;
        const float ContentTop = TabBarTop + TabBarHeight + 14f;
        const float ContentBottom = 82f;
        const float RowHeight = 56f;
        const float RowSpacing = 8f;

        public enum Category
        {
            Audio,
            Gameplay,
            Interface
        }

        Category category = Category.Audio;
        GameObject audioPanel;
        GameObject gameplayPanel;
        GameObject interfacePanel;
        Slider masterSlider;
        Slider musicSlider;
        Slider sfxSlider;
        Slider menuScaleSlider;
        Slider hudScaleSlider;
        Text masterValueLabel;
        Text musicValueLabel;
        Text sfxValueLabel;
        Text menuScaleValueLabel;
        Text hudScaleValueLabel;
        Toggle autoChainToggle;
        Button audioTabButton;
        Button gameplayTabButton;
        Button interfaceTabButton;

        const float DesignPanelHeight = 480f;
        const float DesignPanelWidth = 600f;
        const float DesignPanelAspect = DesignPanelHeight / DesignPanelWidth;

        public static SettingsPanelView Create(Transform parent, Action onBack)
        {
            var canvas = parent.GetComponentInParent<Canvas>();
            var windowWidth = SplitScreenMenuLayout.WindowReferenceWidth;
            var proportionalHeight = windowWidth * DesignPanelAspect;
            var windowHeight = SplitScreenMenuLayout.ResolveWindowHeight(canvas, windowWidth);
            windowHeight = Mathf.Min(windowHeight, proportionalHeight);
            var contentScale = SplitScreenMenuLayout.ResolveContentScale(windowWidth, windowHeight, DesignPanelHeight);

            var root = KatanaUiVisuals.CreateWindowPanel(
                parent,
                "SettingsPanel",
                new Vector2(windowWidth, windowHeight));
            KatanaUiFactory.LayoutRightMenuWindow(root);

            var inner = root.Find("Inner");
            var contentHost = inner != null
                ? KatanaUiFactory.CreateScaledMenuContentHost(inner, windowWidth, DesignPanelHeight, contentScale)
                : KatanaUiFactory.CreateScaledMenuContentHost(root, windowWidth, DesignPanelHeight, contentScale);

            var view = root.gameObject.AddComponent<SettingsPanelView>();
            view.Build(contentHost, onBack);
            return view;
        }

        void Build(RectTransform root, Action onBack)
        {
            KatanaUiFactory.CreateText(root, "Title", "Parametres", 34, TextAnchor.UpperCenter, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(20f, -62f), new Vector2(-20f, -10f));

            var tabBar = CreateTabBar(root);
            audioPanel = CreateAudioPanel(root);
            gameplayPanel = CreateGameplayPanel(root);
            interfacePanel = CreateInterfacePanel(root);
            KatanaUiFactory.CreateFooterButton(root, "Retour", 48f, () => onBack?.Invoke());

            audioTabButton = KatanaUiFactory.CreateToolbarButton(tabBar, "Audio", 0f, 0.32f, 0f, TabBarHeight, () => ShowCategory(Category.Audio));
            gameplayTabButton = KatanaUiFactory.CreateToolbarButton(tabBar, "Gameplay", 0.34f, 0.66f, 0f, TabBarHeight, () => ShowCategory(Category.Gameplay));
            interfaceTabButton = KatanaUiFactory.CreateToolbarButton(tabBar, "Interface", 0.68f, 1f, 0f, TabBarHeight, () => ShowCategory(Category.Interface));

            ShowCategory(Category.Audio);
        }

        static RectTransform CreateTabBar(Transform parent)
        {
            var tabBar = new GameObject("TabBar");
            tabBar.transform.SetParent(parent, false);
            var rect = tabBar.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(20f, -(TabBarTop + TabBarHeight));
            rect.offsetMax = new Vector2(-20f, -TabBarTop);

            var background = tabBar.AddComponent<Image>();
            background.color = KatanaUiTheme.PanelFillInset;
            background.raycastTarget = false;
            return rect;
        }

        GameObject CreateAudioPanel(Transform parent)
        {
            var panel = CreateCategoryPanel(parent, "AudioPanel");
            masterSlider = CreateBoundSlider(
                panel.transform,
                "Volume principal",
                0,
                GameSettings.MasterVolume,
                v => GameSettings.MasterVolume = v,
                FormatAudioLevel,
                out masterValueLabel);
            musicSlider = CreateBoundSlider(
                panel.transform,
                "Volume musique",
                1,
                GameSettings.MusicVolume,
                v => GameSettings.MusicVolume = v,
                FormatAudioLevel,
                out musicValueLabel);
            sfxSlider = CreateBoundSlider(
                panel.transform,
                "Volume effets",
                2,
                GameSettings.SfxVolume,
                v => GameSettings.SfxVolume = v,
                FormatAudioLevel,
                out sfxValueLabel);
            AddResetDefaultsButton(panel.transform, GameSettings.ResetAudioDefaults);
            return panel;
        }

        GameObject CreateGameplayPanel(Transform parent)
        {
            var panel = CreateCategoryPanel(parent, "GameplayPanel");

            var toggleRow = new GameObject("ToggleRow");
            toggleRow.transform.SetParent(panel.transform, false);
            PlaceContentRow(toggleRow.AddComponent<RectTransform>(), 0);

            autoChainToggle = CreateStretchToggle(
                toggleRow.transform,
                "Enchainer les cibles a portee",
                GameSettings.AutoChainTargetsInRange,
                v => GameSettings.AutoChainTargetsInRange = v);

            KatanaUiFactory.CreateText(
                    panel.transform,
                    "Help",
                    "Apres une elimination, attaquer automatiquement le prochain ennemi a portee.",
                    15,
                    TextAnchor.UpperLeft)
                .rectTransform.SetAnchors(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -(RowHeight + 24f)), new Vector2(0f, -(RowHeight * 2f + 8f)));

            AddResetDefaultsButton(panel.transform, GameSettings.ResetGameplayDefaults);
            return panel;
        }

        GameObject CreateInterfacePanel(Transform parent)
        {
            var panel = CreateCategoryPanel(parent, "InterfacePanel");

            menuScaleSlider = CreateBoundSlider(
                panel.transform,
                "Taille des menus",
                0,
                NormalizeMenuScale(GameSettings.MenuUiScale),
                v => GameSettings.MenuUiScale = DenormalizeMenuScale(v),
                v => FormatMenuScaleOffset(DenormalizeMenuScale(v)),
                out menuScaleValueLabel);

            hudScaleSlider = CreateBoundSlider(
                panel.transform,
                "Taille du HUD",
                1,
                NormalizeMenuScale(GameSettings.HudUiScale),
                v => GameSettings.HudUiScale = DenormalizeMenuScale(v),
                v => FormatMenuScaleOffset(DenormalizeMenuScale(v)),
                out hudScaleValueLabel);

            KatanaUiFactory.CreateText(
                    panel.transform,
                    "Help",
                    "Ajuste la taille des menus et du HUD en jeu. 0 correspond a la taille par defaut.",
                    15,
                    TextAnchor.UpperLeft)
                .rectTransform.SetAnchors(
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f),
                    new Vector2(0f, -(RowHeight * 2f + 24f)),
                    new Vector2(0f, -(RowHeight * 3f + 8f)));

            AddResetDefaultsButton(panel.transform, GameSettings.ResetInterfaceDefaults);
            return panel;
        }

        static GameObject CreateCategoryPanel(Transform parent, string name)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.SetAnchors(
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(24f, ContentBottom),
                new Vector2(-24f, -ContentTop));
            return panel;
        }

        static void PlaceContentRow(RectTransform rowRect, int rowIndex)
        {
            var top = 10f + rowIndex * (RowHeight + RowSpacing);
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.offsetMin = new Vector2(0f, -(top + RowHeight));
            rowRect.offsetMax = new Vector2(0f, -top);
        }

        static Slider CreateBoundSlider(
            Transform parent,
            string label,
            int rowIndex,
            float value,
            Action<float> onChanged,
            Func<float, string> formatValue,
            out Text valueLabel)
        {
            var row = new GameObject(label + "Row");
            row.transform.SetParent(parent, false);
            PlaceContentRow(row.AddComponent<RectTransform>(), rowIndex);

            KatanaUiFactory.CreateText(row.transform, "Label", label, 17, TextAnchor.MiddleLeft)
                .rectTransform.SetAnchors(new Vector2(0f, 0.1f), new Vector2(0.34f, 0.9f), Vector2.zero, Vector2.zero);

            var valueText = KatanaUiFactory.CreateText(row.transform, "Value", formatValue(value), 16, TextAnchor.MiddleRight, FontStyle.Bold);
            valueText.rectTransform.SetAnchors(new Vector2(0.82f, 0.1f), new Vector2(1f, 0.9f), Vector2.zero, Vector2.zero);
            valueLabel = valueText;

            var sliderGo = new GameObject(label + "Slider");
            sliderGo.transform.SetParent(row.transform, false);
            var sliderRect = sliderGo.AddComponent<RectTransform>();
            sliderRect.SetAnchors(new Vector2(0.36f, 0.22f), new Vector2(0.8f, 0.78f), Vector2.zero, Vector2.zero);

            var sliderBg = sliderGo.AddComponent<Image>();
            sliderBg.color = KatanaUiTheme.PanelFillInset;

            const float trackMinX = 0.03f;
            const float trackMaxX = 0.97f;
            const float trackMinY = 0.36f;
            const float trackMaxY = 0.64f;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(trackMinX, trackMinY);
            fillAreaRect.anchorMax = new Vector2(trackMaxX, trackMaxY);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = KatanaUiTheme.AccentPrimary;
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderGo.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(trackMinX, trackMinY);
            handleAreaRect.anchorMax = new Vector2(trackMaxX, trackMaxY);
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(0.92f, 0.95f, 1f, 1f);
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(10f, 14f);

            var slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = value;
            slider.onValueChanged.AddListener(v =>
            {
                onChanged?.Invoke(v);
                valueText.text = formatValue(v);
            });
            return slider;
        }

        static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static Toggle CreateStretchToggle(Transform parent, string label, bool value, Action<bool> onChanged)
        {
            var go = new GameObject(label + "Toggle");
            go.transform.SetParent(parent, false);
            StretchFull(go.AddComponent<RectTransform>());

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
            checkRect.sizeDelta = new Vector2(22f, 22f);
            checkRect.anchoredPosition = new Vector2(16f, 0f);
            toggle.graphic = checkImage;

            KatanaUiFactory.CreateText(go.transform, "Label", label, 16, TextAnchor.MiddleLeft)
                .rectTransform.SetAnchors(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(48f, 0f), new Vector2(-8f, 0f));

            return toggle;
        }

        void AddResetDefaultsButton(Transform panel, Action resetAction)
        {
            KatanaUiFactory.CreatePanelActionButton(panel, "Valeurs par defaut", () =>
            {
                resetAction?.Invoke();
                RefreshValues();
            });
        }

        public void RefreshValues()
        {
            RefreshSlider(masterSlider, masterValueLabel, GameSettings.MasterVolume, FormatAudioLevel);
            RefreshSlider(musicSlider, musicValueLabel, GameSettings.MusicVolume, FormatAudioLevel);
            RefreshSlider(sfxSlider, sfxValueLabel, GameSettings.SfxVolume, FormatAudioLevel);
            RefreshSlider(
                menuScaleSlider,
                menuScaleValueLabel,
                NormalizeMenuScale(GameSettings.MenuUiScale),
                v => FormatMenuScaleOffset(DenormalizeMenuScale(v)));
            RefreshSlider(
                hudScaleSlider,
                hudScaleValueLabel,
                NormalizeMenuScale(GameSettings.HudUiScale),
                v => FormatMenuScaleOffset(DenormalizeMenuScale(v)));

            if (autoChainToggle != null)
                autoChainToggle.SetIsOnWithoutNotify(GameSettings.AutoChainTargetsInRange);
        }

        static void RefreshSlider(Slider slider, Text valueLabel, float value, Func<float, string> formatValue)
        {
            if (slider == null)
                return;

            slider.SetValueWithoutNotify(value);
            if (valueLabel != null)
                valueLabel.text = formatValue(value);
        }

        public void ShowCategory(Category next)
        {
            category = next;
            if (audioPanel != null)
                audioPanel.SetActive(category == Category.Audio);
            if (gameplayPanel != null)
                gameplayPanel.SetActive(category == Category.Gameplay);
            if (interfacePanel != null)
                interfacePanel.SetActive(category == Category.Interface);

            UpdateTabVisuals();
        }

        void UpdateTabVisuals()
        {
            HighlightTab(audioTabButton, category == Category.Audio);
            HighlightTab(gameplayTabButton, category == Category.Gameplay);
            HighlightTab(interfaceTabButton, category == Category.Interface);
        }

        static void HighlightTab(Button button, bool active)
        {
            if (button == null)
                return;

            var image = button.GetComponent<Image>();
            if (image != null)
                image.color = active ? KatanaUiTheme.WeaponSlotActive : KatanaUiTheme.ButtonColor;
        }

        static float NormalizeMenuScale(float scale) =>
            Mathf.InverseLerp(GameSettings.MinMenuUiScale, GameSettings.MaxMenuUiScale, scale);

        static float DenormalizeMenuScale(float normalized) =>
            Mathf.Lerp(GameSettings.MinMenuUiScale, GameSettings.MaxMenuUiScale, normalized);

        static string FormatAudioLevel(float normalized) =>
            $"{Mathf.RoundToInt(Mathf.Clamp01(normalized) * 100f)}";

        static string FormatMenuScaleOffset(float scale)
        {
            var offset = Mathf.RoundToInt(scale / GameSettings.MenuUiScaleReference * 100f - 100f);
            if (offset > 0)
                return $"+{offset}";
            if (offset < 0)
                return offset.ToString();
            return "0";
        }
    }
}
