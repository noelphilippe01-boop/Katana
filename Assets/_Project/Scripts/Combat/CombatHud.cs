using Katana.Characters;
using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class CombatHud : MonoBehaviour
    {
        PlayerCombat playerCombat;
        PlayerInventory inventory;
        PlayerHealth playerHealth;
        PauseMenuController pauseMenu;

        void Awake()
        {
            pauseMenu = GetComponent<PauseMenuController>();
            if (pauseMenu == null)
                pauseMenu = gameObject.AddComponent<PauseMenuController>();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            playerCombat = player.GetComponent<PlayerCombat>();
            inventory = player.GetComponent<PlayerInventory>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        void OnGUI()
        {
            if (pauseMenu != null && pauseMenu.IsPaused)
                return;

            var style = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            GUI.Label(new Rect(12f, 12f, 560f, 120f),
                "Katana — Combat\nClic ennemi: attaquer | ZQSD: deplacer | & e \": armes | Echap: pause",
                style);

            if (playerHealth != null)
                GUI.Label(new Rect(12f, 56f, 360f, 24f),
                    $"PV: {playerHealth.CurrentHealth:0}/{playerHealth.MaxHealth:0}",
                    style);

            if (inventory != null)
                GUI.Label(new Rect(12f, 80f, 320f, 24f), $"Or: {inventory.Gold}", style);

            if (playerCombat == null)
                return;

            var target = playerCombat.SelectedTarget;
            if (target == null)
            {
                GUI.Label(new Rect(12f, 108f, 360f, 24f), "Cible: aucune", style);
                return;
            }

            var health = target.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
            {
                GUI.Label(new Rect(12f, 108f, 360f, 24f), "Cible: aucune", style);
                return;
            }

            var rangeLabel = playerCombat.IsTargetInRange() ? "a portee" : "hors portee";
            GUI.Label(new Rect(12f, 108f, 420f, 24f),
                $"Cible: {target.name}  PV: {health.CurrentHealth:0}/{health.MaxHealth:0}  ({rangeLabel})",
                style);
        }
    }
}
