# Katana — Guide de démarrage (Session 1)

Ce guide couvre tout ce que vous devez faire pour la **première session** : Unity Editor, VS Code (commandes Git), et Cursor (code).

## Prérequis

- [Unity Hub](https://unity.com/download) avec **Unity 6 LTS** (ou 2022.3 LTS)
- Module **Windows Build Support**
- Ce dossier : `C:\Users\Philippe\Documents\Unity Projects\Katana`
- Repo GitHub : `https://github.com/noelphilippe01-boop/Katana`

---

## Étape 1 — Créer le projet Unity (Unity Hub / site Unity)

1. Ouvrir **Unity Hub**
2. **Projects → New project**
3. Template : **3D (URP)** — Universal Render Pipeline
4. Nom : `Katana`
5. Emplacement : `C:\Users\Philippe\Documents\Unity Projects`
6. Si le dossier `Katana` existe déjà avec `README.md`, Unity ajoutera `Assets`, `ProjectSettings`, `Packages` à côté

> Si Unity refuse un dossier non vide : renommez temporairement `Katana` en `Katana-temp`, créez le projet, puis copiez `README.md` et `Docs/` dedans.

---

## Étape 2 — Packages Unity (Unity Editor)

Dans Unity : **Window → Package Manager**, installer :

| Package | Usage |
|---------|-------|
| Input System | Contrôles clic + ZQSD (AZERTY) |
| Cinemachine | Caméra isométrique |
| AI Navigation | NavMesh ennemis / joueur |

Quand Unity demande de **remplacer l'ancien Input Manager** → cliquez **Yes**.

---

## Étape 3 — Lier GitHub (VS Code ou terminal)

Dans VS Code, terminal intégré :

```powershell
cd "C:\Users\Philippe\Documents\Unity Projects\Katana"
git remote add origin https://github.com/noelphilippe01-boop/Katana.git
git fetch origin
git checkout -b main
git branch -u origin/main
```

Si `remote origin` existe déjà :

```powershell
git remote set-url origin https://github.com/noelphilippe01-boop/Katana.git
git fetch origin
git checkout -b main origin/main
```

> Utilisez **`main`**, pas `master`.

---

## Étape 4 — Structure de dossiers (Cursor / VS Code)

Créer sous `Assets/_Project/` :

```
Assets/_Project/
  Art/Models/
  Art/Textures/
  Art/Animations/
  Art/VFX/
  Audio/Music/
  Audio/SFX/
  Data/Items/
  Data/Abilities/
  Data/Enemies/
  Data/LootTables/
  Data/Quests/
  Prefabs/Characters/
  Prefabs/Enemies/
  Prefabs/UI/
  Prefabs/Environment/
  Scenes/
  Scripts/Core/
  Scripts/Characters/
  Scripts/Combat/
  Scripts/Camera/
  Scripts/AI/
  Scripts/Inventory/
  Scripts/Loot/
  Scripts/Quests/
  Scripts/UI/
  Scripts/Save/
  Scripts/Audio/
  Scripts/Networking/
  Settings/URP/
  Settings/Input/
```

Demandez à Cursor en **mode Agent** : *« Crée la structure et les scripts de la session 1 selon Docs/UNITY_SETUP.md »*

---

## Étape 5 — Assembly Definitions

### `Assets/_Project/Scripts/Core/Project.Core.asmdef`

```json
{
    "name": "Project.Core",
    "rootNamespace": "Katana.Core",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

### `Assets/_Project/Scripts/Project.Gameplay.asmdef`

```json
{
    "name": "Project.Gameplay",
    "rootNamespace": "Katana",
    "references": [
        "Project.Core",
        "Unity.InputSystem",
        "Cinemachine"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

---

## Étape 6 — Scripts Core (Cursor)

### `GameEventBus.cs` — `Scripts/Core/`

```csharp
using System;
using UnityEngine;

namespace Katana.Core
{
    public static class GameEventBus
    {
        public static event Action<Vector3> PlayerMoveRequested;
        public static event Action<GameObject> TargetSelected;
        public static event Action<DamageInfo> DamageDealt;
        public static event Action<GameObject> EnemyKilled;
        public static event Action<ItemPickupEvent> ItemPickedUp;

        public static void RaisePlayerMoveRequested(Vector3 destination) =>
            PlayerMoveRequested?.Invoke(destination);

        public static void RaiseTargetSelected(GameObject target) =>
            TargetSelected?.Invoke(target);

        public static void RaiseDamageDealt(DamageInfo info) =>
            DamageDealt?.Invoke(info);

        public static void RaiseEnemyKilled(GameObject enemy) =>
            EnemyKilled?.Invoke(enemy);

        public static void RaiseItemPickedUp(ItemPickupEvent pickup) =>
            ItemPickedUp?.Invoke(pickup);
    }

    public struct DamageInfo
    {
        public GameObject Source;
        public GameObject Target;
        public float Amount;
        public bool IsCritical;
    }

    public struct ItemPickupEvent
    {
        public string ItemId;
        public int Quantity;
    }
}
```

### `GameState.cs` + `GameStateManager.cs`

```csharp
namespace Katana.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}
```

```csharp
using System;
using UnityEngine;

namespace Katana.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Boot;
        public event Action<GameState> StateChanged;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            StateChanged?.Invoke(newState);
        }
    }
}
```

### `GameBootstrapper.cs`

```csharp
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
                var manager = gameStateManagerPrefab != null
                    ? Instantiate(gameStateManagerPrefab)
                    : new GameObject("GameStateManager").AddComponent<GameStateManager>();
                DontDestroyOnLoad(manager.gameObject);
            }

            FindAnyObjectByType<GameStateManager>()?.SetState(GameState.Playing);
        }
    }
}
```

### `SceneFlowController.cs`

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katana.Core
{
    public class SceneFlowController : MonoBehaviour
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
        public void ReloadCurrent() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

---

## Étape 7 — Interfaces combat + réseau (stubs)

### `IDamageable.cs` — `Scripts/Combat/`

```csharp
using Katana.Core;

namespace Katana.Combat
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(DamageInfo damage);
    }
}
```

### `ICombatant.cs`

```csharp
namespace Katana.Combat
{
    public interface ICombatant
    {
        void RequestAttack();
        void RequestAbility(int slotIndex);
    }
}
```

### `INetworkSyncAdapter.cs` — `Scripts/Networking/`

```csharp
namespace Katana.Networking
{
    public interface INetworkSyncAdapter
    {
        void SendCommand(IGameCommand command);
        bool IsOnline { get; }
    }

    public interface IGameCommand { }

    public struct MoveCommand : IGameCommand
    {
        public UnityEngine.Vector3 Destination;
    }

    public struct AttackCommand : IGameCommand
    {
        public UnityEngine.GameObject Target;
    }
}
```

### `LocalSyncAdapter.cs`

```csharp
namespace Katana.Networking
{
    public class LocalSyncAdapter : INetworkSyncAdapter
    {
        public bool IsOnline => false;
        public void SendCommand(IGameCommand command) { /* solo : no-op */ }
    }
}
```

---

## Étape 8 — Joueur et caméra

### `PlayerInputHandler.cs` — `Scripts/Characters/`

```csharp
using Katana.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] LayerMask groundLayer = ~0;
        [SerializeField] LayerMask targetLayer = ~0;
        Camera mainCamera;

        void Awake() => mainCamera = Camera.main;

        void Update()
        {
            if (Mouse.current == null || Keyboard.current == null) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleClick();

            if (Keyboard.current.wKey.isPressed) MoveDirection(Vector3.forward);
            if (Keyboard.current.sKey.isPressed) MoveDirection(Vector3.back);
            if (Keyboard.current.aKey.isPressed) MoveDirection(Vector3.left);
            if (Keyboard.current.dKey.isPressed) MoveDirection(Vector3.right);
        }

        void HandleClick()
        {
            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out var hit, 200f, targetLayer) && hit.collider.CompareTag("Enemy"))
            {
                GameEventBus.RaiseTargetSelected(hit.collider.gameObject);
                return;
            }

            if (Physics.Raycast(ray, out hit, 200f, groundLayer))
                GameEventBus.RaisePlayerMoveRequested(hit.point);
        }

        void MoveDirection(Vector3 localDir)
        {
            var camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            var camRight = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
            var worldDir = camForward * localDir.z + camRight * localDir.x;
            GameEventBus.RaisePlayerMoveRequested(transform.position + worldDir * 3f);
        }
    }
}
```

### `PlayerMovementController.cs`

```csharp
using Katana.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Katana.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovementController : MonoBehaviour
    {
        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            GameEventBus.PlayerMoveRequested += OnMoveRequested;
        }

        void OnDestroy() => GameEventBus.PlayerMoveRequested -= OnMoveRequested;

        void OnMoveRequested(Vector3 destination)
        {
            if (NavMesh.SamplePosition(destination, out var hit, 2f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }
}
```

### `IsometricCameraController.cs` — `Scripts/Camera/`

```csharp
using Cinemachine;
using UnityEngine;

namespace Katana.CameraSystems
{
    public class IsometricCameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [SerializeField] Transform target;

        void LateUpdate()
        {
            if (virtualCamera != null && target != null)
                virtualCamera.Follow = target;
        }
    }
}
```

---

## Étape 9 — Scène GameWorld (Unity Editor)

1. **File → New Scene** → sauvegarder `Assets/_Project/Scenes/GameWorld.unity`
2. Créer un **Plane** (sol) → scale (10, 1, 10)
3. **Window → AI → Navigation** → Bake NavMesh
4. Créer **Player** :
   - Capsule + `NavMeshAgent` + `PlayerMovementController` + `PlayerInputHandler`
   - Tag : `Player`
5. Créer **CM vcam** (Cinemachine Virtual Camera) :
   - Follow = Player
   - Body = Framing Transposer, angle ~45° sur l'axe X
6. Ajouter `GameBootstrapper` sur un GameObject `--- MANAGERS ---`
7. **File → Build Settings** → ajouter `GameWorld` à la liste

---

## Étape 10 — Premier commit (VS Code)

```powershell
cd "C:\Users\Philippe\Documents\Unity Projects\Katana"
git add .
git commit -m "feat: fondations Katana — structure, core scripts, mouvement isométrique"
git push -u origin main
```

---

## Workflow récapitulatif

| Outil | Rôle |
|-------|------|
| **Unity Hub / Editor** | Projet URP, scènes, NavMesh, prefabs, Play Mode |
| **Cursor** | Écrire et générer les scripts C# (mode Agent) |
| **VS Code** | Terminal Git, commandes `git push` |
| **GitHub** | Sauvegarde du code |

---

## Prochaine session (après validation Play Mode)

- Combat : `CombatController`, `StatSystem`, `AbilityData` ScriptableObject
- Ennemi dummy avec tag `Enemy` + barre de vie
- Highlight de cible au clic

Demandez dans Cursor : **« exécute la phase 2 du plan Katana »**
