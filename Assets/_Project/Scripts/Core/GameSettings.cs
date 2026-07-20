using UnityEngine;

namespace Katana.Core
{
    public static class GameSettings
    {
        const string MasterVolumeKey = "katana.master_volume";
        const string MusicVolumeKey = "katana.music_volume";
        const string SfxVolumeKey = "katana.sfx_volume";
        const string AutoChainTargetsKey = "katana.auto_chain_targets";
        const string MenuUiScaleKey = "katana.menu_ui_scale";
        const string HudUiScaleKey = "katana.hud_ui_scale";

        public const float MenuUiScaleReference = 1.25f;
        public const float DefaultMenuUiScale = MenuUiScaleReference;
        public const float MinMenuUiScale = MenuUiScaleReference * 0.7f;
        public const float MaxMenuUiScale = MenuUiScaleReference * 1.3f;
        public const float DefaultHudUiScale = MenuUiScaleReference;
        public const float MinHudUiScale = MinMenuUiScale;
        public const float MaxHudUiScale = MaxMenuUiScale;
        public const float DefaultMasterVolume = 0.85f;
        public const float DefaultMusicVolume = 0.75f;
        public const float DefaultSfxVolume = 0.85f;
        public const bool DefaultAutoChainTargetsInRange = true;

        public static float GetGameplayWindowScale(string windowId) =>
            Mathf.Clamp(
                PlayerPrefs.GetFloat(GameplayWindowScalePrefix + windowId, DefaultGameplayWindowScale),
                MinGameplayWindowScale,
                MaxGameplayWindowScale);

        public static void SetGameplayWindowScale(string windowId, float scale)
        {
            PlayerPrefs.SetFloat(
                GameplayWindowScalePrefix + windowId,
                Mathf.Clamp(scale, MinGameplayWindowScale, MaxGameplayWindowScale));
            PlayerPrefs.Save();
        }

        public static void ResetGameplayWindowScales()
        {
            PlayerPrefs.DeleteKey(GameplayWindowScalePrefix + "inventory");
            PlayerPrefs.DeleteKey(GameplayWindowScalePrefix + "settings");
            PlayerPrefs.DeleteKey(GameplayWindowScalePrefix + "stats");
            PlayerPrefs.DeleteKey(GameplayWindowScalePrefix + "skills");
            PlayerPrefs.Save();
        }

        const string GameplayWindowScalePrefix = "katana.gameplay_window_scale.";
        public const float DefaultGameplayWindowScale = 1f;
        public const float MinGameplayWindowScale = 0.72f;
        public const float MaxGameplayWindowScale = 1.38f;

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
            set
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
                ApplyAudio();
            }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            set => PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
            set => PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
        }

        public static bool AutoChainTargetsInRange
        {
            get => PlayerPrefs.GetInt(AutoChainTargetsKey, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(AutoChainTargetsKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static float MenuUiScale
        {
            get => Mathf.Clamp(
                PlayerPrefs.GetFloat(MenuUiScaleKey, DefaultMenuUiScale),
                MinMenuUiScale,
                MaxMenuUiScale);
            set
            {
                PlayerPrefs.SetFloat(MenuUiScaleKey, Mathf.Clamp(value, MinMenuUiScale, MaxMenuUiScale));
                PlayerPrefs.Save();
                MenuUiScaleRegistry.ApplyAll();
            }
        }

        public static float HudUiScale
        {
            get => Mathf.Clamp(
                PlayerPrefs.GetFloat(HudUiScaleKey, DefaultHudUiScale),
                MinHudUiScale,
                MaxHudUiScale);
            set
            {
                PlayerPrefs.SetFloat(HudUiScaleKey, Mathf.Clamp(value, MinHudUiScale, MaxHudUiScale));
                PlayerPrefs.Save();
                HudUiScaleRegistry.ApplyAll();
            }
        }

        public static void ApplyAudio() => AudioListener.volume = MasterVolume;

        public static void ResetAudioDefaults()
        {
            MasterVolume = DefaultMasterVolume;
            MusicVolume = DefaultMusicVolume;
            SfxVolume = DefaultSfxVolume;
        }

        public static void ResetGameplayDefaults() =>
            AutoChainTargetsInRange = DefaultAutoChainTargetsInRange;

        public static void ResetInterfaceDefaults()
        {
            MenuUiScale = DefaultMenuUiScale;
            HudUiScale = DefaultHudUiScale;
            ResetGameplayWindowScales();
        }
    }
}
