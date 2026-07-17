using Katana.Characters;
using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class CombatHud : MonoBehaviour
    {
        PlayerCombat playerCombat;
        PlayerHealth playerHealth;
        WeaponLoadout weaponLoadout;
        PauseMenuController pauseMenu;
        InventoryMenuController inventoryMenu;
        CombatHudView view;

        void Awake()
        {
            pauseMenu = GetComponent<PauseMenuController>();
            if (pauseMenu == null)
                pauseMenu = gameObject.AddComponent<PauseMenuController>();

            inventoryMenu = GetComponent<InventoryMenuController>();
            if (inventoryMenu == null)
                inventoryMenu = gameObject.AddComponent<InventoryMenuController>();

            BindPlayerReferences();
            BuildUi();
            SubscribeEvents();
            RefreshAll();
        }

        void OnDestroy() => UnsubscribeEvents();

        void Update()
        {
            if ((pauseMenu != null && pauseMenu.IsPaused) || (inventoryMenu != null && inventoryMenu.IsOpen))
            {
                view?.SetVisible(false);
                return;
            }

            view?.SetVisible(true);
            RefreshDynamicValues();
        }

        void BindPlayerReferences()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            playerCombat = player.GetComponent<PlayerCombat>();
            playerHealth = player.GetComponent<PlayerHealth>();
            weaponLoadout = player.GetComponent<WeaponLoadout>();
        }

        void BuildUi()
        {
            KatanaUiFactory.EnsureEventSystem();
            var canvas = KatanaUiFactory.CreateHudOverlayCanvas("CombatHudCanvas", 10);
            view = CombatHudView.Create(canvas.transform);
        }

        void SubscribeEvents()
        {
            if (weaponLoadout != null)
                weaponLoadout.WeaponChanged += OnWeaponChanged;
        }

        void UnsubscribeEvents()
        {
            if (weaponLoadout != null)
                weaponLoadout.WeaponChanged -= OnWeaponChanged;
        }

        void OnWeaponChanged(int index, WeaponProfile weapon) => view?.SetSelectedWeapon(index, weaponLoadout);

        void RefreshAll()
        {
            RefreshDynamicValues();

            if (weaponLoadout != null)
                view?.SetWeapons(weaponLoadout);
        }

        void RefreshDynamicValues()
        {
            if (playerHealth != null)
                view?.SetPlayerHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);

            RefreshTargetFrame();
        }

        void RefreshTargetFrame()
        {
            if (view == null || playerCombat == null)
                return;

            var target = playerCombat.SelectedTarget;
            if (target == null)
            {
                view.ClearTarget();
                return;
            }

            var health = target.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
            {
                view.ClearTarget();
                return;
            }

            view.SetTarget(
                target.name,
                health.CurrentHealth,
                health.MaxHealth,
                playerCombat.IsTargetInRange());
        }
    }
}
