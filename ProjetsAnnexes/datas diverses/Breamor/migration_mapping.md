# Mapping de Migration Amtenaël -> OpenDAOC-SPB

Ce document répertorie les tables identifiées dans le dump `BDDamte030326.sql` et leur destination cible dans l'architecture OpenDAOC-SPB.

## 1. Tables de Données de Base (Priorité Haute)

| Table Amtenaël (Legacy) | Table SPB (Cible) | Statut | Commentaires |
| :--- | :--- | :--- | :--- |
| `mob` | `mob` | À mapper | Contient tous les PNJs et monstres. Attention aux scripts custom. |
| `itemtemplate` | `itemtemplate` | À mapper | Tous les objets du jeu. Vérifier les bonus ECS. |
| `merchantitem` | `merchantitem` | À mapper | Inventaire des marchands. |
| `npcequipment` | `npcequipment` | À mapper | Apparence et équipement des PNJs. |
| `amtebrainsparam` | `amtebrainsparam` | À conserver | Paramètres d'IA spécifiques à Amtenaël. |

## 2. Système de Loot

| Table Amtenaël (Legacy) | Table SPB (Cible) | Statut | Commentaires |
| :--- | :--- | :--- | :--- |
| `loottemplate` | `loottemplate` | À mapper | |
| `mobdroptemplate` | `mobdroptemplate` | À mapper | |
| `lootchangertemplate`| `lootchangertemplate`| À mapper | |

## 3. Données de Monde & Objets

| Table Amtenaël (Legacy) | Table SPB (Cible) | Statut | Commentaires |
| :--- | :--- | :--- | :--- |
| `worldobject` | `worldobject` | À mapper | Portes, coffres, objets interactifs. |
| `door` | `door` | À mapper | |

## 4. Factions & Réputation

| Table Amtenaël (Legacy) | Table SPB (Cible) | Statut | Commentaires |
| :--- | :--- | :--- | :--- |
| `faction` | `faction` | À mapper | |
| `linkedfaction` | `linkedfaction` | À mapper | |

## Notes Techniques
- **Encodage** : Le dump semble être en `Latin1` ou `UTF-8` selon les outils (vérifier les caractères accentués).
- **Scripts Custom** : Les colonnes `ClassType` dans `mob` doivent être vérifiées pour correspondre aux classes disponibles dans le `GameServer` SPB.
