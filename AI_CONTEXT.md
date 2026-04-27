# CONTEXTE ARCHITECTURE OPENDAOC - POUR IA

**Note à l'IA traitant ce fichier :** Lis ce fichier en priorité absolue avant de modifier des fichiers ou de proposer des commandes.

## 1. Environnements
Le projet repose désormais sur un serveur unique :
- **SERVEUR UNIQUE / SPB** (`C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB`) : Serveur principal basé sur SinglePlayerBots, utilisé à la fois pour le test et la production. Il est configuré en `GAME_TYPE: "PvP"`.
> **Note sur le mode PvP :** Ce mode autorise par défaut de **grouper avec n'importe qui (cross-realm)**. Il permet aussi d'**attaquer n'importe quel joueur** de n'importe quel royaume, *sauf* s'il fait partie de votre propre groupe ou guilde.
- **Base de Données** : Clone de OpenDAoC-Database. Une interface phpMyAdmin est disponible localement.

## 2. Architecture de Compilation (Critique)
- **Le problème historique** : Le paramètre d'origine `ENABLE_COMPILATION: "True"` (compilation des scripts C# au démarrage par le conteneur Linux .NET 8) **NE FONCTIONNE PAS FIABLEMENT**. Il cause des crashs `Bad IL Format` en production à cause de dépendances système introuvables.
- **La solution implémentée** : 
  - La Prod a été modifiée dans `docker-compose.yml` : l'instruction `image:` a été remplacée par `build: .`
  - Le paramètre `ENABLE_COMPILATION` a été passé à `"False"`.
  - **Conséquence** : Les scripts C# custom sont compilés par le SDK Microsoft local lors du `docker compose build`. C'est robuste et ça empêche le lancement du serveur si le code C# contient la moindre faute de syntaxe.

## 3. Règles C# pour les Scripts Personnalisés
- **Emplacement** : Tous les scripts maison DOIVENT être placés dans le dossier `GameServer/custom/` ou `GameServer/scripts/customnpc/`.
- **Librairies obsolètes** : NE PAS UTILISER `log4net` (ex: `ILog log = ...`). Le moteur moderne tourne sous `NLog`. Utiliser `private static readonly Logger log = LogManager.GetCurrentClassLogger();` ou `Console.WriteLine`.
- **Système TextNPC (Marchands avec Menus)** : Le système de marchands avec dialogue textuel (`TextNPCMerchant`) est disponible dans `GameServer/custom/TextNPC/`. Il est composé de 6 fichiers : `ITextNPC.cs`, `TextNPCBrain.cs`, `TextNPCCondition.cs`, `TextNPCMerchant.cs`, `DBTextNPC.cs`, `DBEchangeur.cs`. Les données textuelles sont stockées dans les tables `textnpc` et `echangeur` en base de données. **Le `ClassType` du mob en BDD doit correspondre exactement au nom de classe du script** (ex: `DOL.GS.Scripts.TextNPCMerchant`).
- **Requêtes Joueurs** : 
  - En Prod, pour itérer sur tous les joueurs connectés, utiliser : `foreach (GamePlayer player in ClientService.Instance.GetPlayers())`.

## 4. Stratégie de Mise à jour (DevOps) & GitHub Fork
Le serveur est sur un dépôt "Fork" GitHub unique (ex: `https://github.com/guill4ume/amtenael`).
- La branche `spb` pour le serveur unique (SPB).
- La branche `global` pour la racine (`C:\OpenDAOC_server`) : Documentation, TODO et scripts de pilotage.
- **Mises à jour Officielles** : Le serveur peut synchroniser le code officiel via un `git pull upstream master` (OpenDAoC).
- **Mise à jour en Production** : Historiquement géré via un script .bat, la mise à jour se fait maintenant principalement via les branches Git.

## 5. Workflow de Développement et Sauvegardes
- **Développement** : Les scripts maison sont créés, testés et déployés sur le serveur SPB.
- **Sauvegarde SPB** : Se fait via un simple `git commit` et `git push origin spb`.

## 6. Précautions et Dépannage (Restauration SPB)
- **Fichiers Dupliqués C#** : Ne **JAMAIS** laisser de fichiers copies Windows (ex : `MonScript(1).cs` ou `MonScript - Copie.cs`) dans le dossier projet. Le compilateur `.NET` va lire tous les `.cs` et plantera au démarrage avec une erreur `CS0111` (Duplication de classe).
- **Encodage de la Base de Données** : Attention aux sauvegardes générées via PowerShell avec `>`. Elles sont souvent encodées en UTF-16, ce qui empêchera l'import dans MariaDB (Erreur `ASCII '\0'`). Il faut forcer l'export en UTF-8 (ex: `| Out-File -Encoding utf8`).