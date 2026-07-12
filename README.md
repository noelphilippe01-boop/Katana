# Katana

ARPG isométrique Unity — fondations modulaires, solo d'abord, architecture prête pour le coop en ligne.

## Stack

- Unity 6 LTS (URP)
- Input System, Cinemachine, AI Navigation

## Emplacement

- Local : `C:\Users\Philippe\Documents\Unity Projects\Katana`
- GitHub : `https://github.com/noelphilippe01-boop/Katana`

## Démarrage rapide

1. **Unity Hub** → New project → **3D (URP)** → dossier `Katana`
2. **Package Manager** → Input System, Cinemachine, AI Navigation
3. Suivre le guide complet : **[Docs/UNITY_SETUP.md](Docs/UNITY_SETUP.md)**
4. Dans **Cursor (mode Agent)** : demander la génération automatique des scripts
5. Dans **VS Code** : `git push -u origin main`

## Workflow

| Outil | Usage |
|-------|-------|
| Unity Editor | Scènes, assets, NavMesh, tests Play Mode |
| Cursor | Scripts C# et architecture |
| VS Code | Commandes Git / terminal |

## Structure cible

```
Assets/_Project/
  Scripts/Core/       → GameEventBus, GameStateManager, Bootstrapper
  Scripts/Characters/ → Player input & movement
  Scripts/Camera/     → Caméra isométrique Cinemachine
  Scripts/Combat/     → IDamageable, combat (phase 2)
  Scenes/             → Boot, GameWorld
```
