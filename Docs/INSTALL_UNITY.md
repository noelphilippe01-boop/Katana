# Installation Unity — Katana

## Statut (session en cours)

| Étape | Statut |
|-------|--------|
| Unity Hub 3.19.4 | **Installé** |
| Unity Editor 6.5 (6000.5.3f1) | **En cours sur D:** |
| Unity CLI (beta) | Installé (redémarrer le terminal pour `unity`) |

---

## Finaliser l'installation (Unity Hub — interface graphique)

Unity Hub devrait être ouvert. Si ce n'est pas le cas :

```powershell
& "C:\Program Files\Unity Hub\Unity Hub.exe"
```

### Dans Unity Hub :

1. **Se connecter** avec votre compte Unity (gratuit) — [id.unity.com](https://id.unity.com)
2. Onglet **Installs** → vérifier si `6000.3.19f1` est en téléchargement
   - Si absent : **Install Editor** → **Unity 6** → version **6000.3.19f1 (LTS)**
3. Modules recommandés pour Katana :
   - **Microsoft Visual Studio Community** (ou votre IDE C#)
   - **Documentation** (optionnel)
   - Pas obligatoire pour débuter : Android, WebGL, etc.
4. Attendre la fin du téléchargement (~2-4 Go, 10-30 min selon connexion)
5. Quand l'install est **Done**, passer à l'étape « Créer le projet »

---

## Vérifier depuis le terminal (VS Code)

```powershell
& "C:\Program Files\Unity Hub\Unity Hub.exe" -- --headless editors -i
```

Quand l'éditeur est installé, vous verrez :

```
6000.3.19f1,C:\Program Files\Unity\Hub\Editor\6000.3.19f1\Editor\Unity.exe
```

---

## Créer le projet Katana (après install Editor)

1. Unity Hub → **Projects** → **New project**
2. Template : **3D (URP)** — *Universal Render Pipeline*
3. Nom : `Katana`
4. Emplacement : `C:\Users\Philippe\Documents\Unity Projects`
5. **Create project**

Puis suivre [UNITY_SETUP.md](UNITY_SETUP.md) étape 2 (packages).

---

## Dépannage

### Espace insuffisant sur C: (votre cas)

Unity 6.5 demande **~14 Go** sur C:, vous n'avez que **9,2 Go** libres.
Votre disque **D:** a **~373 Go** libres — installez Unity dessus :

1. Unity Hub → **icône engrenage** (Preferences / Préférences)
2. **Installs** → **Editor location** / **Emplacement des éditeurs**
3. Changer vers : `D:\Unity\Hub\Editor` (ou `D:\Program Files\Unity\Hub\Editor`)
4. Optionnel : **Downloads location** → `D:\Unity\Hub\Downloads`
5. Relancer l'installation

> Pendant l'install, Unity utilise aussi des fichiers temporaires. Prévoyez **~20 Go libres** sur le disque cible pour être tranquille — D: est largement suffisant.

### Version plus légère (alternative)

Au lieu de **Unity 6.5** (13,98 Go), installez **Unity 6.3 LTS** (`6000.3.19f1`) :
- Hub → Installs → **Install Editor** → onglet **Archives** ou version LTS 6.3
- Souvent un peu plus léger, et c'est la version LTS recommandée pour Katana

**Téléchargement bloqué** : Installs → ⋮ sur la version → Retry

**Dossier non vide** : Unity peut refuser de créer le projet si `README.md` / `Docs/` existent. Solutions :
- Créer dans un sous-dossier temporaire puis fusionner
- Ou : Hub → Add → sélectionner le dossier après avoir généré `Assets/` via un projet vide

**Licence** : Personal (gratuit) si revenus < 200k USD/an
