using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class MainMenuUI : MonoBehaviour
    {
        const float ButtonWidth = 300f;
        const float ButtonHeight = 52f;

        GameObject mainPanel;
        GameObject settingsPanel;

        void Awake()
        {
            KatanaUiFactory.EnsureEventSystem();
            BuildUi();
        }

        void Start() => GameSettings.ApplyAudio();

        void BuildUi()
        {
            var canvas = KatanaUiFactory.CreateMenuOverlayCanvas("MainMenuCanvas");
            KatanaUiFactory.CreateFullScreenPanel(canvas.transform, "Backdrop", KatanaUiTheme.MenuBackdrop);

            mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(canvas.transform, false);
            StretchFull(mainPanel.AddComponent<RectTransform>());
            KatanaUiFactory.CreateFullScreenPanel(mainPanel.transform, "MainPanelBg", Color.clear);

            var titlePlate = KatanaUiVisuals.CreateTitlePlate(mainPanel.transform, "KATANA", "ARPG isometrique");
            titlePlate.anchorMin = titlePlate.anchorMax = new Vector2(0.5f, 0.5f);
            titlePlate.pivot = new Vector2(0.5f, 0.5f);
            titlePlate.anchoredPosition = new Vector2(0f, 190f);

            KatanaUiFactory.CreateButton(mainPanel.transform, "Demarrer", new Vector2(0f, 36f), new Vector2(ButtonWidth, ButtonHeight), StartGame);
            KatanaUiFactory.CreateButton(mainPanel.transform, "Parametres", new Vector2(0f, -32f), new Vector2(ButtonWidth, ButtonHeight), ShowSettings);
            KatanaUiFactory.CreateButton(mainPanel.transform, "Quitter", new Vector2(0f, -100f), new Vector2(ButtonWidth, ButtonHeight), QuitGame);

            settingsPanel = new GameObject("SettingsPanelRoot");
            settingsPanel.transform.SetParent(canvas.transform, false);
            StretchFull(settingsPanel.AddComponent<RectTransform>());
            SettingsPanelView.Create(settingsPanel.transform, ShowMain);
            settingsPanel.SetActive(false);
        }

        static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        void ShowMain()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        void ShowSettings()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
            settingsPanel.GetComponentInChildren<SettingsPanelView>()?.RefreshValues();
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
