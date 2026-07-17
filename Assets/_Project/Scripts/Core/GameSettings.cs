using UnityEngine;

namespace Katana.Core
{
    public static class GameSettings
    {
        const string MasterVolumeKey = "katana.master_volume";
        const string MusicVolumeKey = "katana.music_volume";
        const string SfxVolumeKey = "katana.sfx_volume";
        const string AutoChainTargetsKey = "katana.auto_chain_targets";

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(MasterVolumeKey, 0.85f);
            set
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
                ApplyAudio();
            }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
            set => PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SfxVolumeKey, 0.85f);
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

        public static void ApplyAudio() => AudioListener.volume = MasterVolume;
    }
}
