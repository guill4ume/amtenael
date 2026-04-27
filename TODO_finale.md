# 🛡️ TODO Final - Développement OpenDAOC & Amtenaël

Ce document fusionne et priorise les listes TODO_DOL.txt et TODO_OPENDAOC.txt.

> [!IMPORTANT]
> **🌟 Priorité Absolue : Étude de faisabilité du portage DOL vers OpenDAOC**
> *Estimer le temps de portage vs corriger l'ancienne base DOL. (Travaillé en parallèle).*

---

## 🐞 0. Bugs Constatés (Priorité Correction)

- [ ] **Instabilité Serveur** : Crash lors de la commande `/rel` (Release). *Stacktrace: MimicNPC.RemoveFromWorld() / ReaperService*.
- [x] **Factions Thidranki** : Fixé (les gardes n'attaquent plus leur propre royaume). ✅
- [ ] **Sorts/Potions manquants** : "spell ID not found" pour Ciboulette (31028), Abat (31032), Soupe (31024). Spells identifiés dans Breamor, à injecter.
- [ ] **Scripts manquants** : Restaurer les classes `TextNPCMerchant`, `AmteMob`, etc. (Investigation GitHub).
- [x] **Optimisation Bots** : Population réduite à 5 par royaume (15 total). ✅

---

## 🚀 1. Quick-Wins & Prérequis (Actions Immédiates)
*Tâches rapides pour poser les bases (communication, docs, nettoyage).*

- [x] **Setup Vidéo/Com** : Filmer l'étude de portage via OBS (scinder en eps. 20 min) et configurer la publication automatique sur YouTube.
- [x] **Système de Bots (Thidranki)** :
  - [x] Initialisation automatique au démarrage.
  - [x] Population maximisée (60 bots total, 20 par royaume) dès le lancement.
  - [ ] Rétablir la population de bots variable (proportionnelle à la population réelle) comme sur le repo SPB officiel (actuellement fixe pour la beta).
  - [ ] Étudier et implémenter un système de sécurité (Dynamic Scaling) : Surveiller la charge CPU/TPS en temps réel pour brider ou réduire le nombre de bots si le serveur sature.
  - [x] Fix : Crash NRE (Groupes PvP) et Stats (/mbstats).
  - [x] Fix : Crash au démarrage (accès joueurs non connectés).
- [x] **Documentation Wiki** : 
  - [ ] Recenser ce qui fonctionne en craft/classes sur DOL (par les joueurs)
  - [ ] Recenser monstres et quêtes (et page loot Boss Ma'ati) (par les joueurs)
  - [x] Lister les commandes (Admin/GM/Player) et les publier sur le wiki.
- [x] **PNJs & Services** : 
  - [x] Mettre en place un PNJ "Instant 50" (Héraut des dieux : Niveau 50, Richesse, Artisanat).
  - [x] Réparer l'encodage UTF-8 des noms de PNJs (Région 51).
  - [x] Remettre en place les menus et inventaires des marchands (Zone 51). ✅ Validé en jeu (187 marchands chargés en région 51).
  - [x] Retirer la guilde de base automatiquement attribuée aux nouveaux personnages. ✅ Désactivé via ServerProperty `starting_guild`.
  - [x] Virer le portail rouge téléporteur. ✅ ZonePoint 154 supprimé (visuel conservé).
  - [x] Passer tous les gardes dans la faction "Gardes" sur la carte 51. ✅ Faction ID 1000 créée et assignée.
- [ ] **Infrastructure & QA** :
  - [x] Script PowerShell de sauvegarde/restauration automatisé.
  - [ ] Diagnostiquer la lenteur au démarrage de la branche SPB (36s vs OpenDAOC natif).
  - [ ] Programmer le redémarrage automatique du serveur tous les jours à 04h00 (via `ServerProperty` ?).
- [x] **Config Serveur** : Activation du changement de langue (`/language set`), passage par défaut en FR et alignement de 86 propriétés avec le CSV SPB.
  - [ ] Supprimer la classe de base (via `ServerProperty` ?) pour permettre de choisir sa classe finale dès le niveau 1.
- [x] **Stabilisation du Build (SPB)** : 
  - [x] Réparation de `House.cs` (propriété `ConsignmentMerchant` décommentée).
  - [x] Bridage temporaire du `MarketService` manquant pour permettre le démarrage.
  - [x] Correction de l'encodage et des erreurs de syntaxe sur les scripts custom (`Aerto.cs`).
  - [ ] **Traduction** : Vérifier en jeu la correspondance des noms d'objets en Français.
- [ ] Tous les chevaux ont le même skin marron, remettre la correspondance entre les items achetés et les skins correspondant

---

## 🛠️ 2. Core Serveur & Migration (Moyen Terme)
*Fonctionnalités nécessaires pour avoir un serveur jouable sur une base saine.*

- [x] **Initialisation OpenDAOC** : Repartir sur un OpenDAOC sain et importer la database copie offi.
- [ ] **Portage des Données Joueurs** : Exporter inventaires, argent, classes depuis l'ancienne base DOL.
- [ ] **Mapping/Monde** : 
  - [x] Importer le mobilier Lot B (tables `worldobject` et `door`) depuis Breamor.
  - [ ] Importer les scripts de DOL vers Avalon Isle (serveur Beta).
- [x] **Groupage Inter-Royaume (Client 1.127)** :
  - [x] Bypass des filtres clients via `CustomDialog`.
  - [x] Implémentation du `/gjoin` global (pas de limite de distance).
  - [ ] **Améliorations futures** : Ajouter confirmation du joueur invité + popup visuelle (éventuellement remplacer bouton invite dans l'UI).
- [ ] **Guildage Inter-Royaume** : À vérifier in game
- [x] **Bases PvP & GvG** :
  - [x] Activer Thidranki avec bots Niveau 50 (Auto-start au démarrage). ✅ Configuré dans `MimicManager.cs`.
  - [x] Rendre les gardes de Thidranki agressifs envers les joueurs (PvP). ✅ Propriété `PVP_UNCLAIMED_KEEPS_ENEMY` activée.
  - [x] Système de Téléportation "Aerto" : Dialogue et téléportation vers Thidranki (Niveau 50) pour les deux PNJs Aerto (Région 51). ✅ Implémenté via Global Hook.
  - [ ] Porter les colliers de téléportation régionaux (NPC Ansall - Region 51) depuis Breamor/DOL vers OpenDAoC (Attention : compatibilité scripts DOL à vérifier).
  - [ ] Mettre un PNJ de sortie pour chacun des royaumes dans Thidranki.
  - [ ] Jamtland Mountains avec capture de forts.
  - [ ] Ouvrir PvP H24 (fait) avec bonus de PR en soirée.
  - [ ] Réparer les médecins qui ne rendent pas la constitution (healer.cs de mémoire)
- [ ] **Économie & RP** :
  - [x] Centraliser le point de spawn de toutes les races sur la map historique.
  - [ ] Terminer le système de /shop (max 25 objets).
  - [ ] Désactiver les gains de PRs sur la carte historique (le rôleplay et les maps GvG/PvP priment). 
  - [x] Implémenter la récompense PR automatique via base de mots-clefs RP (Top Rôlistes).
- [ ] **Générateur de Quêtes / Animation** : Outil ig pour permettre aux joueurs de créer des quêtes (0 XP, 0 PR) pour leur RP.
- [ ] **Quêtes OpenDAOC ** : Traduction des quêtes de la db OpenDAOC une fois l'essentiel en place.

---

## ✨ 3. Expérience Dynamique & Events (Moyen - Long Terme)
*Transformer le gameplay classique avec des événements vivants.*

- [ ] **Karma Serveur** : Jauge influencée par les joueurs (débloquant accès, PNJ, donjons et buffs Equilibre ou malus).
- [ ] **Factions & Réputations** : Ordre vs Chaos. Farming de mobs procurant objets RP et **montures de prestige**.
- [ ] **Événements Auto (PvE)** :
  - [ ] Foire de Sombrelune (pop semi-aléatoire).
  - [ ] World Boss (Boss paliers Constructeur/Destructeur attaquant les villes).
  - [ ] Invasions de capitales orientées défense de PNJ/Cristal (ex: Fils Fraenir).
  - [ ] Marchands déclencheurs de Donjons (ex : ramener 100 ongles Guarks pour ouvrir un event temporaire).
  - [ ] Zones corrompues temporairement et Chariots à escorter.
- [ ] **Événements PvP** : 
  - [ ] Chasse à l'homme (proie avec pactole cumulatif).
  - [ ] Gauntlet (champion tournant) & tournois semi-auto.
  - [ ] Régulation Automatique : malus ou TP Battle Royale si une guilde gagne trop de PR (pour éviter de rouler sur le serveur).
- [ ] **Créateurs de Contenus (CCP)** : Récompenser les joueurs postant sur YouTube/TikTok par des cosmétiques. Workflow Make.com pour post auto et reward in-game.
- [ ] **Équipe Animation Joueur** : Animateurs (ex: Thorkal) avec avantages de statuts (titres, capes) sans farm pour encadrer et créer du jeu.

---

## 🔮 4. Vision Futuriste & Projets IA (Très Long Terme)
*Outils d'exploitation surpuissants et révolutions graphiques/gameplay.*

- [ ] **Usine à Agents IA (Wiki/Dev/Discord)** :
  - [ ] Un agent écoute Discord (Requêtes joueurs / doutes stats).
  - [ ] Il consulte l'Agent Wiki (nourri de l'offi) et l'Agent Logs pour confronter les stats.
  - [ ] L'Agent Ticket convertit la conclusion en TODO assignée à l'Agent de Développement.
  - [ ] L'Agent Test valide le fix, met à jour le Wiki et notifie le joueur.
- [ ] **Extensions Thématiques Saisonnières (Tous les 3 mois)** :
  - [ ] *Flying Age Of Camelot / LOTR* : Montures volantes, Bolt, système d'Anneau Unique (Invisibilité, raid 20 ou 40 selon le pouvoir). Modèles Sauron/Nazghuls.
  - [ ] *Saison Dune.*
- [ ] **Mécaniques Dynamiques Globales** : Maps débloquées en fonction de la population ou du karma.
- [ ] **Rénovation Technique Ultime** : Etudier la viabilité du portage sous Unreal Engine.

---
*(Note récurrente : Penser éventuellement à un serveur de Test séparé pour implémenter ces feature et faire du "Bug Bounty")*
