using UnityEngine;

namespace Katana.Core
{
    public static class SettingsMenuDrawer
    {
        public enum Category
        {
            Audio,
            Gameplay
        }

        public static void Draw(ref Category category, float panelX, float panelY)
        {
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            var tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 13 };

            GUI.Box(new Rect(panelX - 16f, panelY - 16f, 392f, 300f), GUIContent.none);
            GUI.Label(new Rect(panelX, panelY, 360f, 36f), "Parametres", titleStyle);

            var tabY = panelY + 44f;
            if (GUI.Button(new Rect(panelX, tabY, 170f, 28f), "Audio", tabStyle))
                category = Category.Audio;

            if (GUI.Button(new Rect(panelX + 180f, tabY, 170f, 28f), "Gameplay", tabStyle))
                category = Category.Gameplay;

            var y = tabY + 40f;
            switch (category)
            {
                case Category.Audio:
                    DrawAudioSettings(panelX, ref y, labelStyle);
                    break;
                case Category.Gameplay:
                    DrawGameplaySettings(panelX, ref y, labelStyle);
                    break;
            }
        }

        static void DrawAudioSettings(float panelX, ref float y, GUIStyle labelStyle)
        {
            GUI.Label(new Rect(panelX, y, 160f, 24f), "Volume principal", labelStyle);
            GameSettings.MasterVolume = GUI.HorizontalSlider(
                new Rect(panelX + 170f, y + 4f, 180f, 20f),
                GameSettings.MasterVolume,
                0f,
                1f);

            y += 40f;
            GUI.Label(new Rect(panelX, y, 160f, 24f), "Volume musique", labelStyle);
            GameSettings.MusicVolume = GUI.HorizontalSlider(
                new Rect(panelX + 170f, y + 4f, 180f, 20f),
                GameSettings.MusicVolume,
                0f,
                1f);

            y += 40f;
            GUI.Label(new Rect(panelX, y, 160f, 24f), "Volume effets", labelStyle);
            GameSettings.SfxVolume = GUI.HorizontalSlider(
                new Rect(panelX + 170f, y + 4f, 180f, 20f),
                GameSettings.SfxVolume,
                0f,
                1f);
        }

        static void DrawGameplaySettings(float panelX, ref float y, GUIStyle labelStyle)
        {
            GameSettings.AutoChainTargetsInRange = GUI.Toggle(
                new Rect(panelX, y, 360f, 24f),
                GameSettings.AutoChainTargetsInRange,
                "Enchainer les cibles a portee");

            y += 28f;
            GUI.Label(
                new Rect(panelX, y, 360f, 48f),
                "Apres une elimination, attaquer automatiquement le prochain ennemi a portee.",
                labelStyle);
        }
    }
}
