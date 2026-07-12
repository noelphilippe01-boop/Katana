using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class SceneFlowController : MonoBehaviour
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public void ReloadCurrent() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
