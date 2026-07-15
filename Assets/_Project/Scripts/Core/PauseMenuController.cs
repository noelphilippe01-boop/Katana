using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class PauseMenuController : MonoBehaviour
    {
        enum PauseScreen
        {
            Hidden,
            Pause,
            Settings
        }

        PauseScreen screen = PauseScreen.Hidden;

        public bool IsPaused => screen != PauseScreen.Hidden;

        void Awake() => EscapeInput.EnsureInitialized();

        void Update()
        {
            if (SceneManager.GetActiveScene().name == GameScenes.MainMenu)
                return;

            if (!EscapeInput.WasPressedThisFrame())
                return;

            HandleEscape();
        }

        public void TogglePauseMenu()
        {
            if (screen == PauseScreen.Hidden)
                OpenPause();
            else
                ClosePause();
        }

        void HandleEscape()
        {
            if (SceneManager.GetActiveScene().name == GameScenes.MainMenu)
                return;

            if (screen == PauseScreen.Settings)
            {
                screen = PauseScreen.Pause;
                return;
            }

            if (screen == PauseScreen.Hidden)
                OpenPause();
            else
                ClosePause();
        }

        void OnGUI()
        {
            if (screen == PauseScreen.Hidden)
                return;

            DrawBackdrop();

            switch (screen)
            {
                case PauseScreen.Pause:
                    DrawPauseScreen();
                    break;
                case PauseScreen.Settings:
                    DrawSettingsScreen();
                    break;
            }
        }

        void OpenPause()
        {
            screen = PauseScreen.Pause;
            Time.timeScale = 0f;
            GameStateManager.Instance?.SetState(GameState.Paused);
        }

        void ClosePause()
        {
            screen = PauseScreen.Hidden;
            Time.timeScale = 1f;
            GameStateManager.Instance?.SetState(GameState.Playing);
        }

        void DrawBackdrop()
        {
            var previous = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.02f, 0.04f, 0.08f, 0.72f);
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), GUIContent.none);
            GUI.backgroundColor = previous;
        }

        void DrawPauseScreen()
        {
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            var centerX = Screen.width * 0.5f;
            GUI.Label(new Rect(centerX - 180f, Screen.height * 0.24f, 360f, 44f), "Pause", titleStyle);

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.38f, 240f, 44f, "Reprendre"))
                ClosePause();

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.48f, 240f, 44f, "Parametres"))
                screen = PauseScreen.Settings;

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.58f, 240f, 44f, "Menu principal"))
                ReturnToMainMenu();

            if (DrawMenuButton(centerX - 120f, Screen.height * 0.68f, 240f, 44f, "Quitter"))
                GameQuit.Quit();
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
                screen = PauseScreen.Pause;
        }

        static bool DrawMenuButton(float x, float y, float width, float height, string label) =>
            GUI.Button(new Rect(x, y, width, height), label);

        static void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            GameStateManager.Instance?.SetState(GameState.MainMenu);
            SceneManager.LoadScene(GameScenes.MainMenu);
        }
    }
}
