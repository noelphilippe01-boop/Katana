# Post-traitement sprites UI Katana

Pipeline esthétique standard pour tous les PNG dans `Assets/_Project/Resources/UI/` :

| Type | Dossier | Traitement |
|------|---------|------------|
| **Icônes** (armes, objets) | `Weapons/` | Fond sombre retiré + crop serré |
| **Cadres** (fenêtres, grilles, HUD) | `Sprites/` | Letterbox noir retiré (flood-fill bords) + crop + 9-slice |

## Depuis Unity

Menu **Katana → Process UI Sprites (transparency + crop)** après avoir ajouté ou remplacé un PNG.

## En ligne de commande

```bash
cd Tools/ui-sprite-postprocess
npm install
npm run process
```

Ajouter un nouveau fichier dans `process-ui-sprites.cjs` (tableau `jobs`).
