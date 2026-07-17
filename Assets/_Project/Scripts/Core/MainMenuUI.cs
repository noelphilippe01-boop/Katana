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
        SettingsMenuDrawer.Category settingsCategory = SettingsMenuDrawer.Category.Audio;

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
            var centerX = Screen.width * 0.5f;
            var panelX = centerX - 180f;
            var panelY = Screen.height * 0.22f;

            SettingsMenuDrawer.Draw(ref settingsCategory, panelX, panelY);

            if (DrawMenuButton(panelX + 60f, panelY + 248f, 240f, 40f, "Retour"))
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
