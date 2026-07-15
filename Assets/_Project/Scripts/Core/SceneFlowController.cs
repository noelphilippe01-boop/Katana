using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class SceneFlowController : MonoBehaviour
    {
        public void LoadMainMenu() => SceneManager.LoadScene(GameScenes.MainMenu);

        public void LoadGameWorld() => SceneManager.LoadScene(GameScenes.GameWorld);

        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public void ReloadCurrent() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
