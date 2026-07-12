using UnityEngine;

namespace Katana.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] GameStateManager gameStateManagerPrefab;

        void Awake()
        {
            if (FindAnyObjectByType<GameStateManager>() == null)
            {
                GameStateManager manager;
                if (gameStateManagerPrefab != null)
                    manager = Instantiate(gameStateManagerPrefab);
                else
                    manager = new GameObject("GameStateManager").AddComponent<GameStateManager>();

                DontDestroyOnLoad(manager.gameObject);
            }

            FindAnyObjectByType<GameStateManager>()?.SetState(GameState.Playing);
        }
    }
}
