# 🛡️ TODO Final - Développement OpenDAOC & Amtenaël

Ce document fusionne et priorise les listes TODO_DOL.txt et TODO_OPENDAOC.txt.


---

## 🐞 0. Bugs Constatés (Priorité Correction)

- [X] **Instabilité Serveur** : Crash lors de la commande `/rel` (Release). *Stacktrace: MimicNPC.RemoveFromWorld() / ReaperService*.
- [x] **Factions Thidranki** : Fixé (les gardes n'attaquent plus leur propre royaume). ✅
- [x] **Erreurs au démarrage (SkillBase)** : ✅ Patché via `SkillBase.cs` (mécanisme de repli pour les préfixes `X` et `AtlasOF_`).
- [x] **Sorts/Potions manquants** : ✅ Injectés (IDs 8982, 8983, 31024, 31028, 31032).
- [x] **Scripts manquants** : Restaurer les classes `TextNPCMerchant`, `AmteMob`, etc. ✅ Solutionné par l'implémentation de la classe générique `AvalonExchangerNPC`.
- [ ] **Optimisation Bots** : Population réduite à 5 par royaume (15 total). ✅
- [ ] **[PRIORITÉ] Redémarrage Auto** : Programmer le redémarrage automatique du serveur tous les jours à 04h00.

---

## 🚀 1. Quick-Wins & Prérequis (Actions Immédiates)
*Tâches rapides pour poser les bases (communication, docs, nettoyage).*

- [x] **Système de Bots (Thidranki)** :
  - [x] Initialisation automatique au démarrage.
  - [x] Population maximisée (15 bots total, 5 par royaume) dès le lancement.
  - [ ] Rétablir la population de bots variable comme sur le repo SPB officiel (actuellement fixe pour la beta) -> Difficulté croissante en fonction du nombre de joueurs -> Voir si possible dynamique sans recompiler le serveur
  - [ ] Étudier et implémenter un système de sécurité (Dynamic Scaling) : Surveiller la charge CPU/TPS en temps réel pour brider ou réduire le nombre de bots si le serveur sature.
    - [ ] **Optimisation Mémoire** : Auditer et réduire l'empreinte mémoire du serveur.
      - [ ] Limiter le chargement des mobs aux régions actives (Map 51 et Thidranki) et ne charger le reste que lors de l'extension du contenu.
      - [ ] Nettoyage d'objets et optimisation des structures de données des bots pour maximiser le nombre de Mimics actifs.
    - [ ] **À vérifier** : Amélioration Bots (Siège & Eau) : implémenter le siège des forts (ouverture des portes) et corriger les bugs aquatiques restants (Voir `ProjetsAnnexes/DossierPortage/PLAN_AMELIORATION_BOTS.md`).
  - [x] Fix : Crash NRE (Groupes PvP) et Stats (/mbstats).
  - [x] Fix : Crash au démarrage (accès joueurs non connectés).
- [x] **Documentation Wiki** : 
  - [ ] Recenser ce qui fonctionne en craft/classes sur DOL (par les joueurs)
  - [ ] Recenser monstres et quêtes (et page loot Boss Ma'ati) (par les joueurs)
  - [ ] Expliquer le fonctionnement des armes Ma'ati sur le Wiki (Obtention, paliers, statistiques).
  - [x] Créer `ai_context.md` dans `C:\OpenDAOC_server\ProjetsAnnexes\datas diverses` pour instruire la mise à jour de la liste des factions.
  - [x] Lister les commandes (Admin/GM/Player) et les publier sur le wiki.
- [x] **PNJs & Services** : 
  - [x] Création d'un PNJ spécifique pour l'Instant 50 en artisanat (Légendaire).
  - [x] Mettre à jour le **Héraut des dieux** pour permettre de passer au rang de royaume **13L0**.
  - [x] Mettre en place un **PNJ de création de guilde** (1 personne minimum, tous les emblèmes disponibles pour tous les royaumes). ✅ Implémenté via `GuildRegistrarNPC.cs`.
  - [x] Réparer l'encodage UTF-8 des noms de PNJs (Région 51).
  - [x] Remettre en place les menus et inventaires des marchands (Zone 51). ✅ Validé en jeu (187 marchands chargés en région 51).
  - [x] Retirer la guilde de base automatiquement attribuée aux nouveaux personnages. ✅ Désactivé via ServerProperty `starting_guild`.
  - [x] Virer le portail rouge téléporteur. ✅ ZonePoint 154 supprimé (visuel conservé).
  - [x] Passer tous les gardes dans la faction "Gardes" sur la carte 51. ✅ Faction ID 1000 créée et assignée.
- [ ] **Infrastructure & QA** :
  - [ ] Script PowerShell de sauvegarde/restauration automatisé.
  - [x] Diagnostiquer la lenteur au démarrage de la branche SPB (36s vs OpenDAOC natif). ✅ (Optimisation faite : warnings d'équipement et de zone corrigés).
  - [ ] **À vérifier :**
    - [ ] Optimisation des performances (corriger les warnings `Long TimerService.Tick`).
    - [ ] Indexation régulière de la base de données pour accélérer la recherche des joueurs et objets.
  - [ ] Développer un programme surveillant le dépôt GitHub pour poster un récapitulatif des mises à jour sur un channel Discord.
  - [x] **Audit & Documentation (Basse Priorité)** : 
    - [x] Documenter la structure de la base de données **Breamor** pour faciliter les requêtes IA et éviter les recherches répétitives chronophages.
    - [x] Créer un récapitulatif détaillé de la migration Avalon (PNJs, SkillBase, Fixes).
- [x] **Config Serveur** : Activation du changement de langue (`/language set`), passage par défaut en FR et alignement de 86 propriétés avec le CSV SPB.
  - [ ] Support complet des commandes et dialogues en Anglais, Français et Espagnol (EN/FR/ES).
  - [ ] Supprimer la classe de base (via `ServerProperty` ?) pour permettre de choisir sa classe finale dès le niveau 1.
- [x] **Stabilisation du Build (SPB)** : 
  - [x] Réparation de `House.cs` (propriété `ConsignmentMerchant` décommentée).
  - [x] Bridage temporaire du `MarketService` manquant pour permettre le démarrage.
  - [x] Correction de l'encodage et des erreurs de syntaxe sur les scripts custom (`Aerto.cs`).
  - [ ] **Traduction** : Vérifier en jeu la correspondance des noms d'objets en Français.
- [ ] Tous les chevaux ont le même skin marron, remettre la correspondance entre les items achetés et les skins correspondant
- [ ] **Client & Patches** :
  - [ ] Appliquer le patch pour changer l'écran de chargement en **amtenaTEST**.
  - [ ] **Fin de Beta** : Refaire un patch pour remettre l'écran de chargement **Amtenael** (Logo : "Le temps n'existe plus").

---

## 🛠️ 2. Core Serveur & Migration (Moyen Terme)
*Fonctionnalités nécessaires pour avoir un serveur jouable sur une base saine.*

- [x] **Initialisation OpenDAOC** : Repartir sur un OpenDAOC sain et importer la database copie offi.
- [ ] **Portage des Données Joueurs** : Exporter inventaires, argent, classes depuis l'ancienne base DOL.
- [ ] **Système Predator** : Intégrer les systèmes de type "Predator" disponibles dans le repo `OpenDAoC-Core-master`.
- [ ] **Mapping/Monde** : 
  - [x] Importer le mobilier Lot B (tables `worldobject` et `door`) depuis Breamor.
  - [ ] Importer les scripts de DOL vers Avalon Isle (pas tous) -> Vérifier la doc sinon la liste des scripts et choisir lesquels
  - [x] **Audit & Import Loots Avalon (Map 51)** :
    - [x] Croiser les mobs Map 51 entre Breamor et SPB.
    - [x] Vérifier si les mobs de Breamor ont des loots et évaluer la faisabilité de l'import.
    - [x] Importer les loots validés. ✅ Automatisé via `AvalonExchangerNPC.cs` et `echangeur_final.txt`.
- [x] **Groupage Inter-Royaume (Client 1.127)** :
  - [x] Bypass des filtres clients via `CustomDialog`.
  - [x] Implémentation du `/gjoin` global (pas de limite de distance).
  - [x] **Correction Bug UI** : Corriger la visibilité partielle/incomplète des membres de groupe inter-royaumes dans l'interface.
    - [x] **Problème HP** : Les barres de vie ne descendent pas dans la fenêtre de groupe lors de la prise de dégâts (cross-realm).
    - [x] **Problème Map** : Les groupés n'apparaissent pas sur la carte.
  - [ ] **Améliorations futures** : Ajouter confirmation du joueur invité + popup visuelle (éventuellement remplacer bouton invite dans l'UI).
- [ ] **Guildage Inter-Royaume** : À vérifier in game
- [x] **Bases PvP & GvG** :
  - [x] Activer Thidranki avec bots Niveau 50 (Auto-start au démarrage). ✅ Configuré dans `MimicManager.cs`.
  - [x] Rendre les gardes de Thidranki agressifs envers les joueurs (PvP). ✅ Propriété `PVP_UNCLAIMED_KEEPS_ENEMY` activée.
  - [x] Système de Téléportation "Aerto" : Dialogue et téléportation vers Thidranki (Niveau 50) pour les deux PNJs Aerto (Région 51). ✅ Implémenté via Global Hook.
  - [ ] Porter les colliers de téléportation régionaux (NPC Ansall - Region 51) depuis Breamor/DOL vers OpenDAoC (Attention : compatibilité scripts DOL à vérifier).
  - [x] Mettre un PNJ de sortie pour chacun des royaumes dans Thidranki (Aertis). ✅
- [ ] Amélioration Thidranki : Téléportation de sortie basée sur le Karma (Constructeur/Neutre -> Wearyall Village, Destructeur -> Prios).
  - [ ] Jamtland Mountains avec capture de forts.
  - [ ] Ouvrir PvP H24 (fait) avec bonus de PR en soirée.
  - [x] Réparer les médecins qui ne rendent pas la constitution (healer.cs de mémoire)
  - [ ] Vérifier que les bots (Mimics) dans Thidranki attaquent correctement les joueurs (agressivité PvP).
  - [ ] **Investigation Thidranki** : Identifier l'utilité des PNJs **Void Merchant** et **Pazz** (consulter les clones de `core master` et `database` en sous-dossier).
- [ ] **Économie & RP** :
  - [x] Centraliser le point de spawn de toutes les races sur la map historique.
  - [ ] Terminer le système de /shop (max 25 objets).
  - [ ] Désactiver les gains de PRs liés aux meurtres (PvP) sur la Carte 51 / Avalon Isle (le Rôleplay et les maps dédiées comme Thidranki doivent être les seules sources de PRs). 
  - [x] Implémenter la récompense PR automatique via base de mots-clefs RP (Top Rôlistes).
  - [ ] Bonus RP incrémental : Augmentation progressive du bonus PR en fonction de la durée de la session RP active.
  - [ ] Restaurer le système d'échange des armes de Ma'ati (Essences/Tokens) auprès des marchands dédiés.
- [ ] **Générateur de Quêtes / Animation** : Outil ig pour permettre aux joueurs de créer des quêtes (0 XP, 0 PR) pour leur rôleplay.
- [ ] **Quêtes OpenDAOC ** : Traduction des quêtes de la db OpenDAOC une fois l'essentiel en place.

---

## ✨ 3. Expérience Dynamique & Events (Moyen - Long Terme)
*Transformer le gameplay classique avec des événements vivants.*

- [ ] **Karma Serveur** : Jauge influencée par les joueurs (débloquant accès, PNJ, donjons et buffs Equilibre ou malus).
- [ ] **Système de Réputations Multi-Factions (Style WoW)** :
  - [ ] **Audit Avalon (Map 51)** : Inventorier les mobs et PNJs pour définir 10 à 20 micro-factions cohérentes (ex: Druides de la Forêt vs Champignons Vénéneux, Milice de Wearyall vs Bandits).
  - [ ] **Moteur de Réputation** : Implémenter le gain/perte automatique de points de faction lors du kill d'un mob (ex: Tuer un mob de la faction A donne +X à la faction B et -Y à la faction C).
  - [ ] **Documenter les interactions entre factions sur le wiki**
  - [ ] **Marchands de Prestige** : Créer des PNJs dont l'inventaire se débloque selon le palier de réputation (Amical, Honoré, Exalté) avec des consommables et objets uniques.
- [ ] **Événements Auto (PvE)** :
  - [ ] **Objectif : Atteindre une fréquence de 1 événement par heure.**
  - [ ] Foire de Sombrelune (pop semi-aléatoire).
  - [ ] World Boss (Boss paliers Constructeur/Destructeur attaquant les villes).
  - [ ] Invasions de capitales orientées défense de PNJ/Cristal (ex: Fils Fraenir).
  - [ ] Marchands déclencheurs de Donjons (ex : ramener 100 ongles Guarks pour ouvrir un event temporaire).
  - [ ] Zones corrompues temporairement et Chariots à escorter.
  - [ ] Système de quêtes "Pokémon" : Capturer des monstres pour qu'ils suivent le joueur (s'inspirer du code MimicNPC) / quêtes d'escorte
- [ ] **Événements PvP** : 
  - [ ] Chasse à l'homme (proie avec pactole cumulatif).
  - [ ] Gauntlet (champion tournant) & tournois semi-auto.
  - [ ] Régulation Automatique : malus ou TP Battle Royale si une guilde gagne trop de PR (pour éviter de rouler sur le serveur) / à voir si d'autres idées
  - [ ] Quand on tue un joueur (par exemple un troll) sur la map 51 (avalon) uniquement, on récupère 2 objets : sang de troll + tête de NomDuJoueur (à faire avec toutes les races)

- [ ] **Événements Dynamiques Thidranki** :
  - [ ] Déclenchement de patrouilles de gardes et mini-bosses mobiles en fonction de l'activité (kills/population).
  - [ ] Pop rare d'un **Dragon** (Boss mondial) au centre de Thidranki pour forcer la coopération ou le chaos entre les royaumes.
- [ ] **Sécurité & RP (Modération Dynamique)** :
  - [ ] **Système Anti-Insultes** : Si un joueur profère des insultes (3 fois), il reçoit des avertissements des gardes à proximité. Au-delà, les gardes perdent patience et viennent l'éliminer.
- [ ] **Créateurs de Contenus (CCP)** : Récompenser les joueurs postant sur YouTube/TikTok par des cosmétiques. Workflow Make.com pour post auto et reward in-game.
- [ ] **Équipe Animation Joueur** : Animateurs (ex: Thorkal) avec avantages de statuts (titres, capes) sans farm pour encadrer et créer du jeu.
- [ ] **Nouveautés Gameplay (Long Terme)** :
  - [ ] **Bounty Board** : Panneau de primes quotidiennes (3 cibles aléatoires/jour) avec monnaie dédiée.
  - [ ] **Météo Dynamique** : Impacts sur le gameplay (Brouillard = -portée, Orage = +dégâts foudre).
  - [ ] **Restauration de Hameaux** : Apporter des matériaux de craft pour "réparer" les villages d'Avalon et débloquer des services.
  - [ ] **Mercenaires Solo** : Location d'un bot PNJ d'appoint (Mimic bridé) pour aider au leveling PvE.
  - [ ] **Fragments d'Artéfacts** : Combiner des loots rares pour créer des objets avec pouvoirs actifs.
  - [ ] **GvG Avalon** : Possibilité pour les guildes de revendiquer des camps/tours sur la map 51 pour des bonus passifs.
  - [ ] **Hauts Faits & Auras** : Débloquer des auras visuelles permanentes via des exploits (Kills, Exploration).
  - [ ] **Paris d'Arène** : Système de paris en or sur les vainqueurs des tournois et duels.

---

## 🔮 4. Vision Futuriste & Projets IA (Très Long Terme)
*Outils d'exploitation surpuissants et révolutions graphiques/gameplay.*
  - [ ] Tester le pack de textures 'DAOC Camelot Unfunded' pour la production de contenu vidéo de haute qualité.
- [ ] **Usine à Agents IA (Wiki/Dev/Discord)** :
  - [ ] Un agent écoute Discord (Requêtes joueurs / doutes stats).
  - [ ] Il consulte l'Agent Wiki (nourri de l'offi) et l'Agent Logs pour confronter les stats.
  - [ ] L'Agent Ticket convertit la conclusion en TODO assignée à l'Agent de Développement.
  - [ ] L'Agent Test valide le fix, met à jour le Wiki et notifie le joueur.
- [ ] **Extensions Thématiques Saisonnières (Tous les 3 mois)** :
  - [ ] *Flying Age Of Camelot / LOTR* : Montures volantes, tir du dragon (bolt eldritch), système d'Anneau Unique (Invisibilité, raid 20 ou 40 selon le pouvoir). Modèles Sauron/Nazghuls. Créer un patch client pour les musiques d'ambiance (ex: Thème de l'Isengard qui se déclenche à l'entrée de la map).
  - [ ] *Saison Dune.*
- [ ] **Mécaniques Dynamiques Globales** : Maps débloquées en fonction de la population ou du karma.
- [ ] **Rénovation Technique Ultime** : Etudier la viabilité du portage sous Unreal Engine.

---
*(Note récurrente : Penser éventuellement à un serveur de Test séparé pour implémenter ces feature et faire du "Bug Bounty")*
