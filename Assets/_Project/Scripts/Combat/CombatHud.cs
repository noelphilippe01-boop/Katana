using Katana.Characters;
using UnityEngine;

namespace Katana.Combat
{
    public class CombatHud : MonoBehaviour
    {
        PlayerCombat playerCombat;
        PlayerInventory inventory;

        void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            playerCombat = player.GetComponent<PlayerCombat>();
            inventory = player.GetComponent<PlayerInventory>();
        }

        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            GUI.Label(new Rect(12f, 12f, 520f, 120f),
                "Katana — Combat\nClic ennemi: attaquer | Clic sol: deplacer | ZQSD: deplacer | Molette: zoom",
                style);

            if (inventory != null)
                GUI.Label(new Rect(12f, 56f, 320f, 24f), $"Or: {inventory.Gold}", style);

            if (playerCombat == null)
                return;

            var target = playerCombat.SelectedTarget;
            if (target == null)
            {
                GUI.Label(new Rect(12f, 84f, 360f, 24f), "Cible: aucune", style);
                return;
            }

            var health = target.GetComponent<EnemyHealth>();
            if (health == null || !health.IsAlive)
            {
                GUI.Label(new Rect(12f, 84f, 360f, 24f), "Cible: aucune", style);
                return;
            }

            GUI.Label(new Rect(12f, 84f, 360f, 24f),
                $"Cible: {target.name}  PV: {health.CurrentHealth:0}/{health.MaxHealth:0}",
                style);
        }
    }
}
