# REFERENCE TECHNIQUE - Portage DOL → OpenDAoC-SPB

> [!CAUTION]
> **VALIDATION OBLIGATOIRE :** Une compilation sans erreur et des logs verts sont une condition nécessaire, mais **insuffisante**. Rien n'est validé hors test IN-GAME.

Dernière mise à jour : 07 Avril 2026
Objectif : Guider les développeurs sur les correspondances de code entre l'ancienne base Dawn Of Light et l'architecture ECS.

---

## 🧠 Paradigmes & Correspondances Rapides

| Ancien DOL | Nouveau SPB | Note |
|---|---|---|
| `RegionTimer(owner, callback)` | `ECSGameTimer(o, cb, ms)` | Dans `DOL.GS` |
| `.getProperty<T>()` | `.GetProperty<T>()` | Majuscule obligatoire |
| `.setProperty(k, v)` | `.SetProperty(k, v)` | Majuscule obligatoire |
| `m_aggroTable` | `GetBaseAggroAmount(living) > 0` | ECS helper |
| `BringFriends(npc, list)` | `BringFriends(npc)` | Surcharge simplifiée |
| `NPCEquipment` | `DbNpcEquipment` | Table DB Master |

---

## 🛠 Inventaire Technique des Systèmes

### 1. Systèmes de Règles (`scripts/custom/Managers/AmtenaelRules.cs`)
- **Activation** : Substitue le Ruleset PvP par défaut. Tagged avec `[ServerRules(EGameServerType.GST_PvP)]`.
- **Conflit Résolu** : L'attribut `ServerRules` dans `PvPServerRules.cs` a été commenté pour garantir la priorité de `AmtenaelRules`.
- **Diagnostic** : Utilise `log4net` (ILog) pour confirmer l'activation au démarrage (`Ruleset [GST_PvP] successfully LOADED`).
- **Anonymat (Leaks)** : 
    - `GamePlayer.cs` : Force le nom RP (`GetName(target)` / Race) dans les messages d'examen.
    - `PacketLib168.cs` (`SendObjectGuildID`) : Force l'ID de guilde à **0** pour les joueurs non-alliés afin de masquer l'identité de guilde en PvP.
- **Visibilité des Groupes (Realm Lie)** : Surcharge de `GetLivingRealm` pour synchroniser les barres de groupe inter-royaumes.
- **Actions Inter-Royaumes** : Autorise le groupement, l'échange et les soins/buffs entre royaumes différents hors zones RvR.

### 2. Infrastructure Docker (`Dockerfile`)
- **Configuration persistante** : Le dossier `/app/config` est créé au build et le fichier `invalidnames.txt` est initialisé pour éviter les erreurs bloquantes au démarrage.
- **Build Linux** : Compilation via `dotnet build` au sein de l'image Alpine multi-stage.
- **Import** (vers le Docker SPB) :
    ```powershell
    Get-Content breamor_export.sql | docker exec -i openbots-db mysql -u root -pmy-secret-pw opendaoc
    ```

### 3. Jail & Prison (ECS)
- **Component** : `JailComponent.cs`
- **Service** : `JailService.cs`
- **Commandes** : `/jail <player> <duration>`, `/unjail <player>`.

### 4. Système Roleplay (Reward)
- Analyse le chat (`/say`) pour les mots clés médiévaux.
- Utilise un `ScheduledTask` via `PvPBonusManager` et `RoleplayReward`.

### 5. Systèmes en Sommeil (Neutralisés pour Phase 1)
- **Marché ECS** : `MarketService.cs`. Actuellement exclu du build pour stabilité.
- **Montures Volantes** : `FlyingMountItem.cs`. Exclu du build.
- **Stubs GamePlayer** : Les propriétés `IsOnFlyingMount`, `IsAllowedToFly` et `FlyingMountType` retournent des valeurs par défaut.

---

## ⚙️ Paramétrage Spécifique (ServerProperties)

Certains comportements globaux ont été modifiés pour aligner le serveur sur les besoins d'Amtenaël :

- **Gardes de Forts (PvP)** : `PVP_UNCLAIMED_KEEPS_ENEMY` est à `true`. Cela rend les gardes des forts non-capturés (ex: Thidranki) agressifs envers tous les royaumes.
- **Auto-Start Bots** : L'initialisation du système Mimic est déclenchée par `[GameServerStartedEvent]` dans `MimicManager.cs`.

---

## 🔧 Points Critiques & Hooks ECS

- **ItemEquipped** : L'event `PlayerInventoryEvent.ItemEquipped` n'existe plus dans cette version de SPB.
- **Droit Admin** : PrivLevel 3 (`ePrivLevel.Admin`) requis pour les commandes MimicNPC.
- **Slashes de compilation** : Toujours utiliser les forward slashes `/` dans le fichier `.csproj` pour le build Linux/Docker.
