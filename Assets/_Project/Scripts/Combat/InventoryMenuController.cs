using Katana.CameraSystems;
using Katana.Characters;
using Katana.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Combat
{
    public class InventoryMenuController : MonoBehaviour
    {
        GameObject root;
        InventoryPanelView panel;
        PauseMenuController pauseMenu;
        PlayerInventory inventory;
        WeaponLoadout weaponLoadout;

        public bool IsOpen => root != null && root.activeSelf;

        void Awake()
        {
            InventoryInput.EnsureInitialized();
            pauseMenu = GetComponent<PauseMenuController>();
            GameplayOverlayState.InventoryCloseRequested += CloseInventory;
            GameEventBus.ItemPickedUp += OnItemPickedUp;
            BuildUi();
            CloseInventory();
        }

        void OnDestroy()
        {
            GameplayOverlayState.InventoryCloseRequested -= CloseInventory;
            GameEventBus.ItemPickedUp -= OnItemPickedUp;
        }

        void OnItemPickedUp(ItemPickupEvent pickup)
        {
            if (!IsOpen || pickup.ItemId != "gold")
                return;

            BindPlayerReferences();
            panel?.Refresh(inventory, weaponLoadout);
        }

        void Update()
        {
            if (SceneManager.GetActiveScene().name == GameScenes.MainMenu)
                return;

            if (!InventoryInput.WasPressedThisFrame())
                return;

            if (IsOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        void BindPlayerReferences()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            inventory = player.GetComponent<PlayerInventory>();
            weaponLoadout = player.GetComponent<WeaponLoadout>();
        }

        void BuildUi()
        {
            KatanaUiFactory.EnsureEventSystem();

            var canvas = KatanaUiFactory.CreateMenuOverlayCanvas("InventoryCanvas", 90);
            root = canvas.gameObject;

            var host = KatanaUiFactory.CreateRightMenuHost(canvas.transform);
            panel = InventoryPanelView.Create(host, CloseInventory);
        }

        public void OpenInventory()
        {
            if (pauseMenu != null && pauseMenu.IsPaused)
                return;

            BindPlayerReferences();
            FocusCameraOnPlayer();
            SplitScreenMenuLayout.Enter();
            root.SetActive(true);
            panel?.Refresh(inventory, weaponLoadout);
            Time.timeScale = 0f;
            GameplayOverlayState.SetInventoryOpen(true);
            GameStateManager.Instance?.SetState(GameState.Paused);
        }

        public void CloseInventory()
        {
            if (root == null || !root.activeSelf)
                return;

            root.SetActive(false);
            SplitScreenMenuLayout.Exit();
            GameplayOverlayState.SetInventoryOpen(false);

            if (pauseMenu == null || !pauseMenu.IsPaused)
            {
                Time.timeScale = 1f;
                GameStateManager.Instance?.SetState(GameState.Playing);
            }
        }

        static void FocusCameraOnPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            CameraFollowTarget.EnsureOn(player.transform);
        }

        public void ToggleInventory()
        {
            if (IsOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }
}
