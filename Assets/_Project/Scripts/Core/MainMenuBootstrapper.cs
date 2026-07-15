using UnityEngine;

namespace Katana.Core
{
    public class MainMenuBootstrapper : MonoBehaviour
    {
        void Awake()
        {
            EnsureGameStateManager();
            GameSettings.ApplyAudio();
            GameStateManager.Instance?.SetState(GameState.MainMenu);
        }

        static void EnsureGameStateManager()
        {
            if (FindAnyObjectByType<GameStateManager>() != null)
                return;

            var managerObject = new GameObject("GameStateManager");
            managerObject.AddComponent<GameStateManager>();
        }
    }
}
