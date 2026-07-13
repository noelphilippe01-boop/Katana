# Configuration scène GameWorld — Katana

## Méthode rapide (recommandée)

Dans Unity, menu **Katana → Setup GameWorld Scene**

Puis pour une scène existante :

1. **Katana → Setup Combat (current scene)**
2. **Katana → Fix Camera (current scene)**
3. **Katana → Fix Player Setup**
4. **Ctrl+S** puis **Play**

---

## Contenu créé automatiquement

- Sol + repères visuels
- Joueur : `PlayerController`, `PlayerCombat`, `CharacterFacing`, `FacingMarker`
- Caméra isométrique Cinemachine (suivi stable + zoom molette)
- Managers : `GameBootstrapper`, `CombatHud`, `EnemySpawner`, etc.
- Scène : `Assets/_Project/Scenes/GameWorld.unity`

---

## Contrôles (Play Mode)

| Action | Contrôle |
|--------|----------|
| Déplacement | Clic sol ou **ZQSD** (AZERTY) |
| Attaque | Clic sur ennemi rouge |
| Zoom | Molette |
| Loot | Approche la sphère dorée |

---

## Menus Katana utiles

| Menu | Usage |
|------|-------|
| Setup GameWorld Scene | Recréer la scène from scratch |
| Setup Combat | Ennemis, loot, spawn, HUD |
| Fix Camera | Centrage joueur + zoom |
| Fix Player Setup | Composants joueur à jour |
| Cleanup Scene | Retire scripts manquants / obsolètes |
| Bake NavMesh (editor only) | Préparer pathfinding IA futur |

---

## Dépannage

| Problème | Solution |
|----------|----------|
| Le joueur ne bouge pas | **Edit → Project Settings → Player → Active Input Handling** = **Both** ; cliquer dans la vue Game |
| ZQSD ne répond pas | Même réglage Input = Both ; clavier AZERTY |
| Clic ignoré | Le sol `Ground` doit avoir un Collider |
| Menu Katana incomplet | Attendre la fin de la compilation (Console vide) |
| Scripts manquants (rose) | **Katana → Cleanup Scene** |

---

## NavMesh (optionnel)

Le joueur utilise un **mouvement direct** (pas de `NavMeshAgent`).

Le menu **Bake NavMesh** prépare le sol pour une future IA ennemie — non requis pour jouer.

---

## Build Settings

**File → Build Settings** → **Add Open Scenes** pour inclure `GameWorld`
