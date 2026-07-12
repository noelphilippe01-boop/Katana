# Networking Roadmap — Katana

> Phase future. Le solo utilise `LocalSyncAdapter` (no-op).

## Choix envisagé

- **Netcode for GameObjects** (Unity)
- Mode **host/client** coop (2-4 joueurs)

## Ce qui devra être synchronisé

| Donnée | Stratégie |
|--------|-----------|
| Position joueur | Server authoritative, interpolation client |
| Points de vie | Server only, events vers clients |
| Attaques | Commande client → validation serveur → hitbox serveur |
| Loot | Roll serveur, spawn pickup synchronisé |
| Inventaire | Server authoritative |

## Règle d'architecture (déjà en place)

- Toute action gameplay passe par `IGameCommand` (`MoveCommand`, `AttackCommand`)
- `INetworkSyncAdapter` remplacera `LocalSyncAdapter` en ligne
- L'UI ne modifie jamais l'état directement — elle envoie des commandes

## Étapes d'activation (plus tard)

1. Installer Netcode for GameObjects
2. Implémenter `NetcodeSyncAdapter : INetworkSyncAdapter`
3. Déplacer `StatSystem` / `InventorySystem` dans `SimulationLayer`
4. Ajouter `NetworkObject` sur Player et Ennemis
5. Tests host + 1 client en LAN
