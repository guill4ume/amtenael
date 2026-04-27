# DASHBOARD - Portage Amtenaël → OpenDAoC-SPB

> [!IMPORTANT]
> **RÈGLE D'OR :** Toute modification de code ou de configuration (même validée par les logs) n'est considérée comme **DÉFINITIVE** qu'après une validation **IN-GAME** (connexion client, combat, groupement).

Dernière mise à jour : 07 Avril 2026
Statut Global : 🟢 **OPÉRATIONNEL (Mode PvP)**

---

## 🚀 État des Lieux (Serveur Unique SPB)

Le projet est désormais unifié sur le serveur **OpenDAoC-SPB**. La version Core-master a été abandonnée pour simplifier la maintenance et éviter les conflits de configuration.

| Système | Statut | Note |
|---|---|---|
| Core Server (SPB) | 🟢 OK | Build Docker Réussi |
| MariaDB | 🟢 OK | Schéma OpenDAoC + PNJ Avalon |
| Anonymat RP | 🟢 OK | Filtrage PacketLib + ExamineMessages |
| Inter-Royaume | 🟢 OK | Ruleset Custom Amtenael (GST_PvP) |
| RP Rewards | 🟢 OK | RoleplayReward & PvPBonusManager actifs |
| Maintenance | 🟢 UNIQUE | SPB remplace définitivement la Prod |

---

## ✅ Accomplissements (Historique)

### Phase 0 : Règles & Identité (Aujourd'hui)
- [x] Compilation Docker réussie (Exclusion FlyingMountItem/MarketExplorer).
- [x] Activation des Scripts `RoleplayReward.cs` & `PvPBonusManager.cs`.
- [x] Neutralisation des stubs `IsOnFlyingMount` dans GamePlayer.

### Phase 0 : Environnement & Startup
- [x] Zone de Démarrage (Startup Location) : Validée (Avalon).
- [x] Sécurité MimicNPC : Commandes bots réservées aux Admins.
- [x] Déblocage /level 50 : ✅ TESTÉ & VALIDÉ.
- [x] Config Classe : `start_as_base_class` = False (Classe finale immédiate).
- [x] Config Level : `starting_level` = 1 (Désactivation Instant 50 Auto).

### Phase 2 : NPCs Avalon (Map 51)
- [x] Injection de 4 028 PNJs (Import direct Breamor).
- [ ] Traduction partielle des noms (Anglais -> Français).
- [ ] Assigner le royaume de base aux NPCs selon leur faction.
- [x] **Aerto Teleportation** : Téléportation vers Thidranki via dialogue pour tous les PNJs nommés "Aerto" (Map 51). ✅ VALIDÉ.

### Phase 2.1 : Ruleset Custom "PvP" (Priorité Amtenael) - ✅ VALIDÉ
*But : Remplacer les règles standards PvP par Amtenael (Anonymat + Groupement).*
- [x] **Configuration** : `GAME_TYPE: "PvP"` dans `docker-compose.yml`.
- [x] **Ruleset** : `AmtenaelRules.cs` (Confirmé par logs serveur).
- [x] **Anonymat** : Filtrage de l'ID de guilde (0) dans `PacketLib168.cs`.
- [x] **Groupement** : Surcharge de `GetLivingRealm` (Realm Lie) et `IsSameRealm`.
- [x] **Diagnostic** : Intégration `log4net` pour suivi du chargement.
- [x] **Zones de Combat (Thidranki)** :
    - [x] Passage au niveau 50 pour tous les bots.
    - [x] Fix de l'auto-spawn au démarrage du serveur.
    - [x] Population stabilisée à 5 bots/royaume (15 total) pour performance et stabilité.
    - [x] Activation de l'agressivité des gardes (PVP). ✅ FIXÉ : Les gardes n'attaquent plus leurs alliés.
    - [x] **Fix de Stabilité** : Correction du crash NRE lors du groupement Mimic en mode PvP. ✅ VALIDÉ.

### Tâches Secondaires
- [ ] Traduction des races en Français (Basse priorité).
- [ ] Traduction partielle des noms des PNJ Avalon.

---

## 📋 Checklist Globale (Prochaines Étapes)

- [ ] **Phase 3 : Tests in-game** (Groupement Inter-Royaume, Interactions).
- [ ] **Phase 6 : Factions Jormtland** (Attente coordonnées SQL Map 163).
- [ ] **Lot B : Économie** (Items, Loots, Mobilier via XML).
- [ ] **Lot C : Quêtes Historiques**
- [ ] **Phase 7 : Anneau des Guarks** (Recherche Hook ECS pour équipement).

---

## 🛠 Derniers Changements (Walkthrough Express)
### Phase 1 : Stabilisation & PvP ✅
- [x] Activation du mode PvP dans `docker-compose.yml`
- [x] Priorisation de `AmtenaelRules.cs` (Attribut `PvP` commenté dans `PvPServerRules.cs`)
- [x] Diagnostic de chargement via ILog log4net
- [x] Correction de l'anonymat (Guild ID 0) dans `PacketLib168.cs`
- [x] Correction dossier `/app/config` et `invalidnames.txt` dans le `Dockerfile`
- [x] Nettoyage des erreurs de loot orphelins via `fix_missing_loot.sql`
