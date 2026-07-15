using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class MainMenuUI : MonoBehaviour
    {
        enum MenuScreen
        {
            Main,
            Settings
        }

        MenuScreen screen = MenuScreen.Main;

        void Start() => GameSettings.ApplyAudio();

        void OnGUI()
        {
            DrawBackdrop();

            switch (screen)
            {
                case MenuScreen.Main:
                    DrawMainScreen();
                    break;
                case MenuScreen.Settings:
                    DrawSettingsScreen();
                    break;
            }
        }

        void DrawBackdrop()
        {
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), GUIContent.none);
        }

        void DrawMainScreen()
        {
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            var subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };

            var centerX = Screen.width * 0.5f;
            GUI.Label(new Rect(centerX - 220f, Screen.height * 0.18f, 440f, 60f), "KATANA", titleStyle);
            GUI.Label(new Rect(centerX - 220f, Screen.height * 0.28f, 440f, 28f), "ARPG isometrique", subtitleStyle);

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.42f, 240f, 44f, "Demarrer"))
                StartGame();

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.52f, 240f, 44f, "Parametres"))
                screen = MenuScreen.Settings;

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.62f, 240f, 44f, "Quitter"))
                QuitGame();
        }

        void DrawSettingsScreen()
        {
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            var centerX = Screen.width * 0.5f;
            var panelX = centerX - 180f;
            var panelY = Screen.height * 0.22f;

            GUI.Box(new Rect(panelX - 16f, panelY - 16f, 392f, 260f), GUIContent.none);
            GUI.Label(new Rect(panelX, panelY, 360f, 36f), "Parametres", titleStyle);

            var y = panelY + 52f;
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

            y += 56f;
            if (DrawMenuButton(panelX + 60f, y, 240f, 40f, "Retour"))
                screen = MenuScreen.Main;
        }

        static bool DrawMenuButton(float x, float y, float width, float height, string label)
        {
            return GUI.Button(new Rect(x, y, width, height), label);
        }

        static void StartGame()
        {
            Time.timeScale = 1f;
            GameStateManager.Instance?.SetState(GameState.Playing);
            SceneManager.LoadScene(GameScenes.GameWorld);
        }

        static void QuitGame() => GameQuit.Quit();
    }
}
