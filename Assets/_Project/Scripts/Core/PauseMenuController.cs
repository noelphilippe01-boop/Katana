using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class PauseMenuController : MonoBehaviour
    {
        const float ButtonWidth = 300f;
        const float ButtonHeight = 52f;

        GameObject root;
        GameObject rightPane;
        GameObject pausePanel;
        GameObject settingsPanel;

        public bool IsPaused => root != null && root.activeSelf;

        void Awake()
        {
            EscapeInput.EnsureInitialized();
            BuildUi();
            ClosePause();
        }

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
            if (!IsPaused)
                OpenPause();
            else
                ClosePause();
        }

        void HandleEscape()
        {
            if (SceneManager.GetActiveScene().name == GameScenes.MainMenu)
                return;

            if (GameplayOverlayState.IsInventoryOpen)
            {
                GameplayOverlayState.RequestCloseInventory();
                return;
            }

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                ShowPausePanel();
                return;
            }

            if (!IsPaused)
                OpenPause();
            else
                ClosePause();
        }

        void BuildUi()
        {
            KatanaUiFactory.EnsureEventSystem();

            var canvas = KatanaUiFactory.CreateMenuOverlayCanvas("PauseMenuCanvas", 100);
            root = canvas.gameObject;

            rightPane = KatanaUiFactory.CreateRightMenuHost(canvas.transform).gameObject;

            pausePanel = new GameObject("PausePanel");
            pausePanel.transform.SetParent(rightPane.transform, false);
            StretchFull(pausePanel.AddComponent<RectTransform>());

            KatanaUiFactory.CreateText(pausePanel.transform, "Title", "Pause", 44, TextAnchor.MiddleCenter, FontStyle.Bold)
                .rectTransform.SetAnchors(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-220f, 150f), new Vector2(220f, 210f));

            KatanaUiFactory.CreateButton(pausePanel.transform, "Reprendre", new Vector2(0f, 36f), new Vector2(ButtonWidth, ButtonHeight), ClosePause);
            KatanaUiFactory.CreateButton(pausePanel.transform, "Parametres", new Vector2(0f, -32f), new Vector2(ButtonWidth, ButtonHeight), ShowSettings);
            KatanaUiFactory.CreateButton(pausePanel.transform, "Menu principal", new Vector2(0f, -100f), new Vector2(ButtonWidth, ButtonHeight), ReturnToMainMenu);
            KatanaUiFactory.CreateButton(pausePanel.transform, "Quitter", new Vector2(0f, -168f), new Vector2(ButtonWidth, ButtonHeight), GameQuit.Quit);

            settingsPanel = new GameObject("SettingsPanelRoot");
            settingsPanel.transform.SetParent(rightPane.transform, false);
            StretchFull(settingsPanel.AddComponent<RectTransform>());
            SettingsPanelView.Create(settingsPanel.transform, ShowPausePanel);
        }

        static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        void OpenPause()
        {
            if (GameplayOverlayState.IsInventoryOpen)
                GameplayOverlayState.RequestCloseInventory();

            SplitScreenMenuLayout.Enter();
            root.SetActive(true);
            ShowPausePanel();
            Time.timeScale = 0f;
            GameStateManager.Instance?.SetState(GameState.Paused);
        }

        void ClosePause()
        {
            if (root != null)
                root.SetActive(false);

            SplitScreenMenuLayout.Exit();
            Time.timeScale = 1f;
            GameStateManager.Instance?.SetState(GameState.Playing);
        }

        void ShowPausePanel()
        {
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        void ShowSettings()
        {
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
            settingsPanel.GetComponentInChildren<SettingsPanelView>()?.RefreshValues();
        }

        static void ReturnToMainMenu()
        {
            SplitScreenMenuLayout.ForceReset();
            Time.timeScale = 1f;
            GameStateManager.Instance?.SetState(GameState.MainMenu);
            SceneManager.LoadScene(GameScenes.MainMenu);
        }
    }
}
