# Configuration scène GameWorld — Katana

## Méthode rapide (recommandée)

Dans Unity, menu **Katana → Setup GameWorld Scene**

Cela crée automatiquement :
- Sol + NavMesh
- Joueur avec déplacement clic/ZQSD
- Caméra isométrique Cinemachine
- Managers (`GameBootstrapper`)
- Scène sauvegardée dans `Assets/_Project/Scenes/GameWorld.unity`

Puis appuyez **Play** pour tester.

---

## Méthode manuelle

1. **File → New Scene** (Basic ou Empty)
2. **File → Save As** → `Assets/_Project/Scenes/GameWorld.unity`

---

## 3. Sol + NavMesh

1. **GameObject → 3D Object → Plane** → renommer `Ground`
2. Scale : `(10, 1, 10)`
3. **Window → AI → Navigation**
4. Onglet **Bake** → cliquer **Bake**

---

## 4. Joueur

1. **GameObject → 3D Object → Capsule** → renommer `Player`
2. Tag : **Player** (créer le tag si absent)
3. **Add Component** :
   - `Nav Mesh Agent`
   - `Player Movement Controller`
   - `Player Input Handler`
4. Position : `(0, 1, 0)`

---

## 5. Caméra isométrique (Cinemachine 3)

1. Sélectionner **Main Camera** — la laisser (Cinemachine la pilote)
2. **GameObject → Cinemachine → Cinemachine Camera** → renommer `CM_Isometric`
3. Sur `CM_Isometric` :
   - **Tracking Target** = `Player`
   - Rotation du composant **Rotation Control** ou transform : inclinaison ~**45°** sur X
   - Distance / offset pour vue isométrique (ex. position caméra `(10, 15, -10)` en regardant l'origine)
4. Sur un GameObject vide `--- MANAGERS ---` :
   - Add `Game Bootstrapper`
   - Add `Isometric Camera Controller`
   - Assigner **Cinemachine Camera** = `CM_Isometric`, **Target** = `Player`

---

## 6. Test Play Mode

1. Appuyer **Play**
2. **Clic gauche** sur le sol → le joueur se déplace
3. **ZQSD** → déplacement directionnel (clavier AZERTY)

---

## 7. Build Settings

**File → Build Settings** → **Add Open Scenes** pour inclure `GameWorld`

---

## Dépannage

| Problème | Solution |
|----------|----------|
| Le joueur ne bouge pas | NavMesh baked ? Player a NavMeshAgent ? |
| Clic ignoré | Ground a un Collider ? Layer Ground dans PlayerInputHandler |
| Input System erreur | **Edit → Project Settings → Player → Active Input Handling** = Input System Package |
| Cinemachine introuvable | Package Cinemachine 3.x installé, script utilise `CinemachineCamera` |
